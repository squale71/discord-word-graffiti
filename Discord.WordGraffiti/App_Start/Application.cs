using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WordGraffiti.DAL.Repositories;
using Discord.WordGraffiti.Handlers;

namespace Discord.WordGraffiti.App_Start
{
    public class Application
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private IMessageHandler _messageHandler;

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
                .AddSingleton<IWordRepository, WordRepository>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IMessageHandler, MessageHandler>()
                .BuildServiceProvider();

            _messageHandler = new MessageHandler(_client, _commands, _services.GetService<IUserRepository>(), _services.GetService<IWordRepository>());

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
            else 
            {
                await _messageHandler.HandleUserMessage(message);
            }
        }
    }
}
