using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Web.Http;


using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System.Web.Http.ExceptionHandling;
using System.Threading;
using System.Web.Http.Results;

namespace BPR
{
    public static class Globals
    {
        public static MySqlConnection conn = new MySqlConnection("");
        public static int timerCount = 0;
    }

    class Program
    {

        private CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private TimerService _timer;

        static void Main(string[] args)
                => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            //GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler),
            //    new OopsExceptionHandler(GlobalConfiguration.Configuration.Services.GetExceptionHandler()));

            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _timer = new TimerService(_client);

            _client.Log += Log;

            string token = "";

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if message comes from bot, to prevent rereading bot responses
            if (message.Author.IsBot) return;
            // Determine if the message is a command, based on where it's located or a mention prefix
            if (message.Channel.Id != 429366707656589312) return;
            // Create a Command Context
            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                await message.DeleteAsync();
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        internal class OopsExceptionHandler : IExceptionHandler
        {
            private readonly IExceptionHandler _innerHandler;

            public OopsExceptionHandler(IExceptionHandler innerHandler)
            {
                if (innerHandler == null)
                    throw new ArgumentNullException(nameof(innerHandler));

                _innerHandler = innerHandler;
            }

            public IExceptionHandler InnerHandler
            {
                get { return _innerHandler; }
            }

            public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
            {
                Handle(context);

                return Task.FromResult<object>(null);
            }

            public void Handle(ExceptionHandlerContext context)
            {
                // Create your own custom result here...
                // In dev, you might want to null out the result
                // to display the YSOD.
                // context.Result = null;
                context.Result = new InternalServerErrorResult(context.Request);
            }
        }
    }
}
