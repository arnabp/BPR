﻿using System;
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
    public IMessageChannel channelOverride;

    private readonly int MIN_PLAYERS_IN_QUEUE = 16;

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
            if (Globals.config.HasValue)
            {
                if (client.GetChannel(HelperFunctions.GetChannelId("NA", 0)) is IMessageChannel generalChannel)
                {
                    if (Globals.config.Value.state == 0)
                    {
                        if (DateTime.Now.AddMinutes(-5).Ticks > Globals.config.Value.startTime)
                        {
                            GameConfig config = Globals.config.Value;
                            config.state = 1;
                            Globals.config = config;

                            await BHP.UpdateConfigState();
                            await generalChannel.SendMessageAsync($"Checkin has ended. Generating matches.");
                            if (Globals.config.Value.gameMode == 2) await CleanLeaderboardAsync();
                            await GenerateMatchesAsync(generalChannel);
                        }
                    }
                    else
                    {
                        if (client.GetChannel(HelperFunctions.GetChannelId("NA", 1)) is IMessageChannel leaderboardChannel)
                        {
                            IEnumerable<IMessage> messageList = await leaderboardChannel.GetMessagesAsync(1).Flatten();

                            if (messageList.ToList()[0] is IUserMessage leaderboard) await UpdateLeaderboardAsync(leaderboard);
                        }

                        if (DateTime.Now.Ticks > Globals.config.Value.endTime)
                        {
                            Globals.config = null;
                            await BHP.ClearConfig();
                            await generalChannel.SendMessageAsync($"@everyone The session has now ended, thanks for playing! Check the leaderboard channel to see your result");
                        }
                        else
                        {
                            await GenerateMatchesAsync(generalChannel);
                        }
                    }
                }
            }
            Globals.timerCount++;
        },
        null,
        TimeSpan.FromMinutes(0),  // 4) Time that message should fire after the timer is created
        TimeSpan.FromSeconds(15)); // 5) Time after which message should repeat (use `Timeout.Infinite` for no repeat)
    }

    private async Task UpdateLeaderboardAsync(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Color = Color.Blue
        };

        string leaderboardString = "";
        List<LeaderboardUser> leaderboard = await BHP.GetLeaderboard();

        foreach (LeaderboardUser leaderboardUser in leaderboard)
        {
            leaderboardString += $"`{leaderboardUser.username}`: {leaderboardUser.points}\n";
        }

        embed.AddField(x =>
        {
            x.Name = $"Leaderboard";
            x.Value = leaderboardString;
        });

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    private async Task CleanLeaderboardAsync()
    {
        List<LeaderboardUser> leaderboard = await BHP.GetLeaderboard();
        List<ulong> singles = new List<ulong>(leaderboard.Count);

        foreach (LeaderboardUser leaderboardUser in leaderboard)
        {
            bool found = false;
            foreach (LeaderboardUser teammateUser in leaderboard)
            {
                if (leaderboardUser.teammateId == teammateUser.id && teammateUser.teammateId == leaderboardUser.id)
                {
                    found = true;
                    break;
                }
            }

            if (!found) singles.Add(leaderboardUser.id);
        }

        if (singles.Count > 0) await BHP.DeleteLeaderboardUsers(singles);
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

            if (!userFound) queue.Add(leaderboardUser);
        }

        int n = queue.Count;

        while (n > 1)
        {
            n--;
            int k = Globals.rnd.Next(n + 1);
            LeaderboardUser temp = queue[k];
            queue[k] = queue[n];
            queue[n] = temp;
        }

        int gameSize = Globals.config.Value.gameMode * 2;
        if (queue.Count <= MIN_PLAYERS_IN_QUEUE) return;

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

        await thisChannel.SendMessageAsync($"Please remember to add your room number with `match room 00000`");
    }
}