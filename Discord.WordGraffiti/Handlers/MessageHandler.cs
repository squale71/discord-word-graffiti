using Discord.Commands;
using Discord.WebSocket;
using Discord.WordGraffiti.DAL.Models;
using Discord.WordGraffiti.DAL.Repositories;
using Discord.WordGraffiti.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IUserRepository _userRepository;
        private IWordRepository _wordRepository;
        private IConfigRepository _configRepository;
        private IUserWordMappingRepository _userWordMappingRepository;

        private IMessageChannel _channel;

        public MessageHandler(DiscordSocketClient client, CommandService commands, IUserRepository userRepository, IWordRepository wordRepository, IUserWordMappingRepository userWordMappingRepository, IConfigRepository configRepository)
        {
            _client = client;
            _commands = commands;
            _userRepository = userRepository;
            _wordRepository = wordRepository;
            _userWordMappingRepository = userWordMappingRepository;
            _configRepository = configRepository;
        }

        public async Task HandleUserMessage(SocketUserMessage message)
        {
            _channel = _client.GetChannel(message.Channel.Id) as IMessageChannel;

            var user = await _userRepository.GetById(message.Author.Id);

            if (user == null)
            {
                user = new User
                {
                    Id = message.Author.Id,
                    Username = message.Author.Username
                };

                await _userRepository.Insert(user);
            }

            var configValue = await GetConfigInfo("maxWords");
           
            if (configValue == null || !int.TryParse(configValue.Value, out int maximum))
            {
                return;
            }

            if (WordHelper.TryGetMaximumWordsFromString(message.Content, maximum, out List<string> words))
            {
                return;
            }

            var wordColl = await _wordRepository.GetByWords(words);

            if (wordColl.Any())
            {
                await SetUserWordMappings(user, wordColl);

                await _channel.SendMessageAsync("That message was worth " + wordColl.Sum(x => x.Value) + " points!");
            }
        }


        private async Task<Config> GetConfigInfo(string configName)
        {
            Config config = await _configRepository.GetByName(configName);
            if(config != null)
            {
                Console.WriteLine("Config " + configName + " found. Value is: " + config.Value);
                return config;
            }
            else
            {
                Console.WriteLine("No such config found");
                return null;
            }
        }

        private async Task SetUserWordMappings(User user, IEnumerable<Word> words)
        {
            try
            {
                var wordCollection = words.Select(x => x.Name);

                // Of the words, get words claimed by other users
                var claimedWords = await _userWordMappingRepository.GetWordsOwnedByOtherUsers(user.Id, wordCollection);

                if (claimedWords.Any())
                {
                    // Update claimed words to new user ID. (including new value)
                    await _userWordMappingRepository.UpdateUserWordOwnership(user.Id, claimedWords);
                }

                // All word mappings the user currently has.
                var currentMappings = await _userWordMappingRepository.GetUserOwnedWords(user.Id, wordCollection);

                // Of the new words, get only the words not already owned by the user.
                var unclaimedWords = words.Where(x => !claimedWords.Select(y => y.WordId).Contains(x.Id) && !currentMappings.Select(y => y.WordId).Contains(x.Id));

                // Create new mappings for the words not yet owned by the user.
                var mappingCollection = unclaimedWords.Select(x => new UserWordMapping
                {
                    UserId = user.Id,
                    WordId = x.Id,
                    Value = x.Value
                });

                if (mappingCollection.Any())
                {
                    // Insert new unclaimed words into DB.
                    await _userWordMappingRepository.InsertWordMappings(mappingCollection);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }                 
        }
    }

    public interface IMessageHandler
    {
        Task HandleUserMessage(SocketUserMessage message);
    }
}
