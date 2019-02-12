using Discord.Commands;
using Discord.WebSocket;
using Discord.WordGraffiti.DAL.Models;
using Discord.WordGraffiti.DAL.Repositories;
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
            var user = await _userRepository.GetById(message.Author.Id);

            if (user == null)
            {
                user = new User();
                user.Id = message.Author.Id;
                user.Username = message.Author.Username;

                Console.WriteLine("adding user");
                await _userRepository.Insert(user);
            }
            else
            {
                
                Console.WriteLine("User already exists");
            }

            var uniqueWords = GetWordsFromMessage(message);
            if(!uniqueWords.Any())
            {
                Console.WriteLine("No unique words or word limit exceeded");
            }
            var wordColl = await _wordRepository.GetByWords(uniqueWords);

            await SetUserWordMappings(user, wordColl);

            var chnl = _client.GetChannel(message.Channel.Id) as IMessageChannel;
            await chnl.SendMessageAsync("That message was worth " + wordColl.Sum(x => x.Value) + " points!");
        }

        private HashSet<string> GetWordsFromMessage(SocketUserMessage message)
        {
            string[] msgWords = Regex.Replace(message.Content, @"[^\w]", " ").Split(' '); // splits messages into an array of words - also strips out non-letter characters and replaces with a space.
            var uniqueWords = new HashSet<string>(msgWords.Select(x => x.ToLower())); //reduces list to unique words only
            var maxWords = GetConfigInfo("maxWords").Result.Value; //Looks up a config with the name "maxWords" and stores its value
            if (uniqueWords.Count > Convert.ToInt32(maxWords)) //If the number of unique words(uniqueWords) exceeds the maxWords value, we reject the message return no words
            {
                Console.WriteLine("Too many words!");
                var chnl = _client.GetChannel(message.Channel.Id) as IMessageChannel;
                chnl.SendMessageAsync("I didn't count your last message - it exceeded the limit of " + maxWords + " words");
                return null;
            }
            else
            {
                return uniqueWords;
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
