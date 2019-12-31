using System;
using System.Threading.Tasks;
using System.Reflection;


using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System.Web.Http.ExceptionHandling;
using System.Threading;
using System.Web.Http.Results;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BPR
{
    public struct Region
    {
        public ulong id;
        public bool status;
        public bool status1;
        public bool status2;
        public bool inQueue1;
        public bool inQueue2;
        public int tiers;
    }
    public static class Globals
    {
        public static MySqlConnection conn;
        public static int timerCount = 0;

        public static Dictionary<ulong, Role> roleList;
        public static Dictionary<string, Region> regionList;

        public static TierModule tiersNA1 = new TierModule("NA", 1);
        public static TierModule tiersNA2 = new TierModule("NA", 2);
        public static TierModule tiersEU1 = new TierModule("EU", 1);
        public static TierModule tiersEU2 = new TierModule("EU", 2);
        public static TierModule tiersTEST1 = new TierModule("TEST", 1);
        public static TierModule tiersTEST2 = new TierModule("TEST", 2);

        public static Random rnd = new Random();

        public static async Task InitRegions()
        {
            regionList = new Dictionary<string, Region>();
            string query = $"SELECT region, status, serverID, tiers, status1s, status2s FROM regionStatus;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    regionList[reader.GetString(0)] = new Region {
                        id = reader.GetUInt64(2),
                        status = reader.GetBoolean(1),
                        status1 = reader.GetBoolean(4),
                        status2 = reader.GetBoolean(5),
                        tiers = reader.GetInt16(3)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            var listOfRegions = regionList.ToList();

            foreach(var thisValue in listOfRegions)
            {
                Region thisRegion = regionList[thisValue.Key];

                query = $"SELECT count(*) FROM queue{thisValue.Key}1;";
                await Globals.conn.OpenAsync();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        thisRegion.inQueue1 = reader.GetInt16(0) > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await Globals.conn.CloseAsync();
                    throw;
                }
                await Globals.conn.CloseAsync();

                query = $"SELECT count(*) FROM queue{thisValue.Key}2;";
                await Globals.conn.OpenAsync();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        thisRegion.inQueue2 = reader.GetInt16(0) > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await Globals.conn.CloseAsync();
                    throw;
                }
                await Globals.conn.CloseAsync();

                regionList[thisValue.Key] = thisRegion;

                Console.WriteLine($"{thisValue.Key} - 1v1: {thisRegion.inQueue1}, 2v2: {thisRegion.inQueue2}");
            }
        }
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
            string token;
            using (TextReader reader = File.OpenText(@"/root/BPR/secrets.csv"))
            {
                var csv = new CsvHelper.CsvReader(reader);
                csv.Read();
                token = csv.GetField<String>(0);

                csv.Read();
                Globals.conn = new MySqlConnection(csv.GetField<String>(0));
                reader.Close();
            }

            await Globals.InitRegions();

            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _timer = new TimerService(_client);

            _client.Log += Log;
            
            await Globals.tiersNA1.InitTierList();
            await Globals.tiersNA2.InitTierList();
            await Globals.tiersEU1.InitTierList();
            await Globals.tiersEU2.InitTierList();

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
            SocketUserMessage message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if message comes from bot, to prevent rereading bot responses
            if (message.Author.IsBot) return;
            // Determine if the message is a command, based on where it's located or a mention prefix
            if (message.Channel.Id != 392829581192855554 && message.Channel.Id != 422045385612328973) return;
            // Create a Command Context
            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                if(result.ErrorReason != "Unknown command.")
                {
                    await message.DeleteAsync();
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
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
