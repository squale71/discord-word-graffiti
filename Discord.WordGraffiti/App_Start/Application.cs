using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql;
using Discord.WordGraffiti.DAL.Database;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Discord.WordGraffiti.DAL.Models;
using Discord.WordGraffiti.DAL.Repositories;

namespace Discord.WordGraffiti.App_Start
{
    public class Application
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private Application() { }

        public static async Task Initialize()
        {
            var app = new Application();

            await app.RunBotAsync();
        }

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                // Example of injecting a repository here
                // .AddSingleton<IRepository<Model>, ModelRepository>()
                .BuildServiceProvider();

            string botToken = Configuration.Instance.Get("DiscordApiKey");

            _client.Log += Log;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleMessageAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
           
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            using (var db = new PostgresDBProvider())
            {
                long testval = 0;
                using (var cmd = new NpgsqlCommand("SELECT id FROM user WHERE EXISTS (SELECT id FROM user WHERE id ='" + message.Author.Id + "');", db.Connection))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        //testval = reader.GetBoolean
                Console.WriteLine(testval);      
                    //var user = new User();

                //Console.WriteLine(message.Author.Id);

                //user.Id = message.Author.Id;
                //user.Username = message.Author.Username;

                //Console.WriteLine("DB User ID: " + user.Id);
                //Console.WriteLine("DB Username: " + user.Username);


                //using (var cmd = new NpgsqlCommand("SELECT value from word WHERE name='" + word + "'", db.Connection))
                //using (var reader = cmd.ExecuteReader())
                //    while (reader.Read())
                //        val = reader.GetInt32(0);                

            }

            int argPos = 0;

            if (message.HasStringPrefix("/word ", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var res = await _commands.ExecuteAsync(context, argPos, _services);

                if (!res.IsSuccess)
                {
                    Console.WriteLine(res.ErrorReason);
                }
            }
            else //This is where we're parsing all chat messages to do point assignment stuff. Probably should break this out somewhere (into multiple pieces, really)so consider this proof of concept.
            {
                var uniqueWords = await GetWordsFromMessage(message);
                //public async Task GetValueAsync(int id)
                {
                 
                }
                int wordVals = 0;
                using (var db = new PostgresDBProvider())
                {
                    foreach (var word in uniqueWords)
                    {
                        Console.WriteLine(word);
                        int val = 0;
                        using (var cmd = new NpgsqlCommand("SELECT value from word WHERE name='"+word+"'", db.Connection))
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                val = reader.GetInt32(0);
                        wordVals += val;
                    }
                }
                var chnl = _client.GetChannel(message.Channel.Id) as IMessageChannel;
                await chnl.SendMessageAsync("That message was worth " + wordVals + " points!");
            }
        }
        
        private async Task<HashSet<string>> GetWordsFromMessage(SocketUserMessage message)
        {
            string[] msgWords = Regex.Replace(message.Content, @"[^\w]", " ").Split(' '); // splits messages into an array of words - also strips out non-letter characters and replaces with a space.
            var uniqueWords = new HashSet<string>(msgWords); //reduces list to unique words only
            return uniqueWords;
        }

        public async Task GetValueAsync(int id)
        {

        }

    }
}
