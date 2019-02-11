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

        public MessageHandler(DiscordSocketClient client, CommandService commands, IUserRepository userRepository, IWordRepository wordRepository)
        {
            _client = client;
            _commands = commands;
            _userRepository = userRepository;
            _wordRepository = wordRepository;
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
                //do point assignment shtuff here.
                Console.WriteLine("User already exists");
            }

            var uniqueWords = await GetWordsFromMessage(message);
            var wordColl = await _wordRepository.GetByWords(uniqueWords);

            var chnl = _client.GetChannel(message.Channel.Id) as IMessageChannel;
            await chnl.SendMessageAsync("That message was worth " + wordColl.Sum(x => x.Value) + " points!");
        }

        private async Task<HashSet<string>> GetWordsFromMessage(SocketUserMessage message)
        {
            string[] msgWords = Regex.Replace(message.Content, @"[^\w]", " ").Split(' '); // splits messages into an array of words - also strips out non-letter characters and replaces with a space.
            var uniqueWords = new HashSet<string>(msgWords); //reduces list to unique words only
            return uniqueWords;
        }

    }

    public interface IMessageHandler
    {
        Task HandleUserMessage(SocketUserMessage message);
    }
}
