using System;
using System.Threading; // 1) Add this namespace
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using BPR;
using System.Collections.Generic;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;

public class TimerService
{
    private readonly int MIN_ENTRANTS_IN_QUEUE = 6;
    private readonly int START_HOUR_1 = 23;
    private readonly int START_HOUR_2 = 22;

    private readonly Timer _timer; // 2) Add a field like this
    // This example only concerns a single timer.
    // If you would like to have multiple independant timers,
    // you could use a collection such as List<Timer>,
    // or even a Dictionary<string, Timer> to quickly get
    // a specific Timer instance by name.

    public TimerService(DiscordSocketClient client)
    {
        _timer = new Timer(async _ =>
        {
            // 3) Any code you want to periodically run goes here:
            if (Globals.config.HasValue && Globals.config.Value.state != -1)
            {
                if (client.GetChannel(HelperFunctions.GetChannelId(Globals.config.Value.gameMode)) is IMessageChannel generalChannel)
                {
                    if (Globals.config.Value.state == 0)
                    {
                        if (DateTime.Now.Ticks > Globals.config.Value.checkinTime)
                        {
                            GameConfig config = Globals.config.Value;
                            config.state = 1;
                            Globals.config = config;

                            await BHP.UpdateConfigState(1);
                            await generalChannel.SendMessageAsync($"Checkin has ended. Generating matches.");
                            await GenerateMatchesAsync(generalChannel);
                        }
                    }
                    else
                    {
                        if (client.GetChannel(HelperFunctions.GetChannelId(0)) is IMessageChannel leaderboardChannel)
                        {
                            IUserMessage message = await GetMessageFromChannel(leaderboardChannel);
                            if (message != null) await UpdateLeaderboardAsync(message);
                        }

                        if (DateTime.Now.Ticks > Globals.config.Value.endTime)
                        {
                            GameConfig config = Globals.config.Value;
                            config.state = -1;
                            Globals.config = config;
                            await BHP.UpdateConfigState(-1);
                            await generalChannel.SendMessageAsync($"@everyone The session has now ended, thanks for playing! Check the leaderboard channel to see your result");
                        }
                        else
                        {
                            await GenerateMatchesAsync(generalChannel);
                        }
                    }
                }
            }
            else
            {
                DateTime now = DateTime.Now.ToUniversalTime();
                if (now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek!= DayOfWeek.Sunday)
                {
                    if (now.Hour == START_HOUR_1 && now.Minute >= 40)
                    {
                        if (client.GetChannel(HelperFunctions.GetChannelId(1)) is IMessageChannel generalChannel1)
                        {
                            await StartSessionAsync(generalChannel1, 392829581192855552, 1, 20, 120);
                        }
                    }
                    //else if (now.Hour == START_HOUR_2 && now.Minute >= 40)
                    //{
                    //    if (client.GetChannel(HelperFunctions.GetChannelId(2)) is IMessageChannel generalChannel2)
                    //    {
                    //        await StartSessionAsync(generalChannel2, 392829581192855552, 2, 20, 120);
                    //    }
                    //}
                }
            }
            Globals.timerCount++;
        },
        null,
        TimeSpan.FromMinutes(0),  // 4) Time that message should fire after the timer is created
        TimeSpan.FromSeconds(15)); // 5) Time after which message should repeat (use `Timeout.Infinite` for no repeat)
    }

    public static async Task<IUserMessage> GetMessageFromChannel(IMessageChannel channel)
    {
        IEnumerable<IMessage> messageList = await channel.GetMessagesAsync(1).Flatten();

        if (messageList.ToList()[0] is IUserMessage leaderboard) return leaderboard;
        else return null;
    }

    public static async Task UpdateLeaderboardAsync(IUserMessage thisMessage)
    {
        string leaderboardString = "**LEADERBOARD**\n";
        List<LeaderboardUser> leaderboard = await BHP.GetLeaderboard();

        foreach (LeaderboardUser leaderboardUser in leaderboard)
        {
            leaderboardString += $"`{leaderboardUser.username}`: {leaderboardUser.points}\n";
        }

        await thisMessage.ModifyAsync(x => {
            x.Content = leaderboardString;
        });
    }

    private async Task GenerateMatchesAsync(IMessageChannel thisChannel)
    {
        List<LeaderboardUser> leaderboard = await BHP.GetLeaderboard();
        List<Match> matches = await BHP.GetMatches();

        List<LeaderboardUser> queue = new List<LeaderboardUser>(leaderboard.Count);

        foreach (LeaderboardUser leaderboardUser in leaderboard)
        {
            bool userFound = false;
            foreach (Match match in matches)
            {
                if (leaderboardUser.id == match.id1 ||
                    leaderboardUser.id == match.id2 ||
                    leaderboardUser.id == match.id3 ||
                    leaderboardUser.id == match.id4)
                {
                    userFound = true;
                    break;
                }
            }

            if (!userFound && leaderboardUser.active)
            {
                if (Globals.config.Value.gameMode == 2)
                {
                    LeaderboardUser teammateUser = leaderboard.Find(user => leaderboardUser.teammateId == user.id);
                    if (teammateUser != null && teammateUser.teammateId == leaderboardUser.id)
                    {
                        queue.Add(leaderboardUser);
                    }
                }
                else
                {
                    queue.Add(leaderboardUser);
                }
            }
        }

        int n = queue.Count;

        // Sort by streak in descending order
        queue.Sort();

        int gameSize = Globals.config.Value.gameMode * 2;
        if (queue.Count < (MIN_ENTRANTS_IN_QUEUE * Globals.config.Value.gameMode)) return;

        while (queue.Count > gameSize - 1)
        {
            if (Globals.config.Value.gameMode == 1)
            {
                LeaderboardUser p1 = queue[0];
                LeaderboardUser p2 = queue[1];
                queue.RemoveRange(0, 2);

                await BHP.PutMatch(new Match()
                {
                    id1 = p1.id,
                    id2 = p2.id
                });

                if (p1.points > p2.points)
                {
                    await thisChannel.SendMessageAsync($"New match has started between <@{p1.id}> and <@{p2.id}>");
                }
                else
                {
                    await thisChannel.SendMessageAsync($"New match has started between <@{p2.id}> and <@{p1.id}>");
                }
            }
            else
            {
                LeaderboardUser p1 = queue[0];
                queue.RemoveAt(0);
                int teammateIndex = queue.FindIndex(u => u.id == p1.teammateId);
                LeaderboardUser p2 = queue[teammateIndex];
                queue.RemoveAt(teammateIndex);


                LeaderboardUser p3 = queue[0];
                queue.RemoveAt(0);
                teammateIndex = queue.FindIndex(u => u.id == p3.teammateId);
                LeaderboardUser p4 = queue[teammateIndex];
                queue.RemoveAt(teammateIndex);

                await BHP.PutMatch(new Match()
                {
                    id1 = p1.id,
                    id2 = p2.id,
                    id3 = p3.id,
                    id4 = p4.id
                });

                if (p1.points > p3.points)
                {
                    await thisChannel.SendMessageAsync($"New match has started between <@{p1.id}>, <@{p2.id}> and <@{p3.id}>, <@{p4.id}>");
                }
                else
                {
                    await thisChannel.SendMessageAsync($"New match has started between <@{p1.id}>, <@{p2.id}> and <@{p3.id}>, <@{p4.id}>");
                }
            }
        }

        await BHP.UpdateLeaderboardSkipped(queue);

        await thisChannel.SendMessageAsync($"Please remember to add your room number with `match room 00000`");
    }

    public static async Task StartSessionAsync(IMessageChannel channel, ulong guildId, int gameMode, int checkinMinutes, int totalMinutes)
    {
        Console.WriteLine($"Starting a {gameMode}v{gameMode} session for {totalMinutes} minutes with checkin {checkinMinutes} minutes");
        await BHP.BackupLeaderboard();
        await BHP.ClearConfig();

        GameConfig config = new GameConfig()
        {
            serverId = guildId,
            gameMode = gameMode,
            startTime = DateTime.Now.Ticks,
            checkinTime = DateTime.Now.AddMinutes(checkinMinutes).Ticks,
            endTime = DateTime.Now.AddMinutes(checkinMinutes + totalMinutes).Ticks,
            state = 0
        };

        Globals.config = config;
        await BHP.PutConfig(config);

        string atTeammate = gameMode == 2 ? " @teammate" : "";
        await channel.SendMessageAsync($"@everyone Starting a {gameMode}v{gameMode} session! Please use command `session join{atTeammate}` in the next {checkinMinutes} minutes to check in to the tournament.");
    }
}