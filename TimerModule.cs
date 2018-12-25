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
            if (Globals.timerCount % 2 == 0)
            {
                if (client.GetChannel(392829581192855554) is IMessageChannel generalTimeoutNAEU)
                {
                    await CheckQueueTimeoutAsync(generalTimeoutNAEU, "NA", 1);
                    await CheckQueueTimeoutAsync(generalTimeoutNAEU, "NA", 2);
                    await CheckQueueTimeoutAsync(generalTimeoutNAEU, "EU", 1);
                    await CheckQueueTimeoutAsync(generalTimeoutNAEU, "EU", 2);
                }

                if (client.GetChannel(422045385612328973) is IMessageChannel generalTimeoutAUSSEA)
                {
                    await CheckQueueTimeoutAsync(generalTimeoutAUSSEA, "AUS", 1);
                    await CheckQueueTimeoutAsync(generalTimeoutAUSSEA, "SEA", 1);
                    await CheckQueueTimeoutAsync(generalTimeoutAUSSEA, "AUS", 2);
                    await CheckQueueTimeoutAsync(generalTimeoutAUSSEA, "SEA", 2);
                }

                if (client.GetChannel(410988141491519488) is IMessageChannel generalTimeoutBRZ)
                {
                    await CheckQueueTimeoutAsync(generalTimeoutBRZ, "BRZ", 1);
                    await CheckQueueTimeoutAsync(generalTimeoutBRZ, "BRZ", 2);
                }

                if (client.GetChannel(422768563800244234) is IMessageChannel decayAUSSEA)
                {
                    if (false)
                    {
                        await EloDecayAsync(decayAUSSEA, "AUS", 1);
                        await EloDecayAsync(decayAUSSEA, "AUS", 2);
                        await EloDecayAsync(decayAUSSEA, "SEA", 1);
                        await EloDecayAsync(decayAUSSEA, "SEA", 2);
                    }
                }

                if (client.GetChannel(438991295310987264) is IMessageChannel decayBRZ)
                {
                    if (false)
                    {
                        await EloDecayAsync(decayBRZ, "BRZ", 1);
                        await EloDecayAsync(decayBRZ, "BRZ", 2);
                    }
                }

                if (client.GetChannel(401167888762929153) is IMessageChannel queue1InfoNA)
                {
                    IEnumerable<IMessage> messageList = await queue1InfoNA.GetMessagesAsync(5).Flatten();

                    IUserMessage leaderboardT1m = messageList.ToList()[4] as IUserMessage;
                    IUserMessage leaderboardT2m = messageList.ToList()[3] as IUserMessage;
                    IUserMessage leaderboardT3m = messageList.ToList()[2] as IUserMessage;
                    IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                    IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                    if (leaderboardT1m != null) await UpdateLeaderboardAsync(leaderboardT1m, "NA", 1, 1);
                    if (leaderboardT2m != null) await UpdateLeaderboardAsync(leaderboardT2m, "NA", 1, 2);
                    if (leaderboardT3m != null) await UpdateLeaderboardAsync(leaderboardT3m, "NA", 1, 3);
                    if (matchListm != null) await UpdateMatchesAsync(matchListm, "NA", 1);
                    if (queueListm != null) await UpdateQueueAsync(queueListm, "NA", 1);

                }

                if (client.GetChannel(525425865262104576) is IMessageChannel queue1InfoEU)
                {
                    IEnumerable<IMessage> messageList = await queue1InfoEU.GetMessagesAsync(5).Flatten();

                    IUserMessage leaderboardT1m = messageList.ToList()[4] as IUserMessage;
                    IUserMessage leaderboardT2m = messageList.ToList()[3] as IUserMessage;
                    IUserMessage leaderboardT3m = messageList.ToList()[2] as IUserMessage;
                    IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                    IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                    if (leaderboardT1m != null) await UpdateLeaderboardAsync(leaderboardT1m, "EU", 1, 1);
                    if (leaderboardT2m != null) await UpdateLeaderboardAsync(leaderboardT2m, "EU", 1, 2);
                    if (leaderboardT3m != null) await UpdateLeaderboardAsync(leaderboardT3m, "EU", 1, 3);
                    if (matchListm != null) await UpdateMatchesAsync(matchListm, "EU", 1);
                    if (queueListm != null) await UpdateQueueAsync(queueListm, "EU", 1);

                }

                if (client.GetChannel(404558855771521024) is IMessageChannel queue2InfoNA)
                {
                    IEnumerable<IMessage> messageList = await queue2InfoNA.GetMessagesAsync(5).Flatten();

                    IUserMessage leaderboardT1m = messageList.ToList()[4] as IUserMessage;
                    IUserMessage leaderboardT2m = messageList.ToList()[3] as IUserMessage;
                    IUserMessage leaderboardT3m = messageList.ToList()[2] as IUserMessage;
                    IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                    IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                    if (leaderboardT1m != null) await UpdateLeaderboardAsync(leaderboardT1m, "NA", 2, 1);
                    if (leaderboardT2m != null) await UpdateLeaderboardAsync(leaderboardT2m, "NA", 2, 2);
                    if (leaderboardT3m != null) await UpdateLeaderboardAsync(leaderboardT3m, "NA", 2, 3);
                    if (matchListm != null) await UpdateMatchesAsync(matchListm, "NA", 2);
                    if (queueListm != null) await UpdateQueueAsync(queueListm, "NA", 2);

                }

                if (client.GetChannel(525425886195875861) is IMessageChannel queue2InfoEU)
                {
                    IEnumerable<IMessage> messageList = await queue2InfoEU.GetMessagesAsync(5).Flatten();

                    IUserMessage leaderboardT1m = messageList.ToList()[4] as IUserMessage;
                    IUserMessage leaderboardT2m = messageList.ToList()[3] as IUserMessage;
                    IUserMessage leaderboardT3m = messageList.ToList()[2] as IUserMessage;
                    IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                    IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                    if (leaderboardT1m != null) await UpdateLeaderboardAsync(leaderboardT1m, "EU", 2, 1);
                    if (leaderboardT2m != null) await UpdateLeaderboardAsync(leaderboardT2m, "EU", 2, 2);
                    if (leaderboardT3m != null) await UpdateLeaderboardAsync(leaderboardT3m, "EU", 2, 3);
                    if (matchListm != null) await UpdateMatchesAsync(matchListm, "EU", 2);
                    if (queueListm != null) await UpdateQueueAsync(queueListm, "EU", 2);

                }

                if (false && client.GetChannel(423372016922525697) is IMessageChannel queue1InfoAUSSEA)
                {
                    IEnumerable<IMessage> messageList = await queue1InfoAUSSEA.GetMessagesAsync(4).Flatten();

                    IUserMessage leaderboardAUSm = messageList.ToList()[3] as IUserMessage;
                    IUserMessage leaderboardSEAm = messageList.ToList()[2] as IUserMessage;
                    IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                    IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                    if (leaderboardAUSm != null) await UpdateLeaderboardAsync(leaderboardAUSm, "AUS", 1, 1);
                    if (leaderboardSEAm != null) await UpdateLeaderboardAsync(leaderboardSEAm, "SEA", 1, 1);
                    if (matchListm != null) await UpdateMatchesAsync(matchListm, "AUS", "SEA", 1);
                    if (queueListm != null) await UpdateQueueAsync(queueListm, "AUS", "SEA", 1);

                }

                if (false && client.GetChannel(437510787951493121) is IMessageChannel queue2InfoAUSSEA)
                {
                    IEnumerable<IMessage> messageList = await queue2InfoAUSSEA.GetMessagesAsync(4).Flatten();

                    IUserMessage leaderboardAUSm = messageList.ToList()[3] as IUserMessage;
                    IUserMessage leaderboardSEAm = messageList.ToList()[2] as IUserMessage;
                    IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                    IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                    if (leaderboardAUSm != null) await UpdateLeaderboardAsync(leaderboardAUSm, "AUS", 2, 1);
                    if (leaderboardSEAm != null) await UpdateLeaderboardAsync(leaderboardSEAm, "SEA", 2, 1);
                    if (matchListm != null) await UpdateMatchesAsync(matchListm, "AUS", "SEA", 2);
                    if (queueListm != null) await UpdateQueueAsync(queueListm, "AUS", "SEA", 2);
                }

                if (client.GetChannel(525950148581523466) is IMessageChannel bank1InfoNAEU)
                {
                    IEnumerable<IMessage> messageList = await bank1InfoNAEU.GetMessagesAsync(4).Flatten();

                    IUserMessage bankInfoNA1m = messageList.ToList()[3] as IUserMessage;
                    IUserMessage bankInfoNA2m = messageList.ToList()[2] as IUserMessage;
                    IUserMessage bankInfoEU1m = messageList.ToList()[1] as IUserMessage;
                    IUserMessage bankInfoEU2m = messageList.ToList()[0] as IUserMessage;

                    if (bankInfoNA1m != null && bankInfoNA2m != null) await UpdateBankInfoAsync(bankInfoNA1m, bankInfoNA2m, "NA", 1);
                    if (bankInfoEU1m != null && bankInfoEU2m != null) await UpdateBankInfoAsync(bankInfoEU1m, bankInfoEU2m, "EU", 1);
                }

                if (client.GetChannel(525950398297669639) is IMessageChannel bank2InfoNAEU)
                {
                    IEnumerable<IMessage> messageList = await bank2InfoNAEU.GetMessagesAsync(4).Flatten();

                    IUserMessage bankInfoNA1m = messageList.ToList()[3] as IUserMessage;
                    IUserMessage bankInfoNA2m = messageList.ToList()[2] as IUserMessage;
                    IUserMessage bankInfoEU1m = messageList.ToList()[1] as IUserMessage;
                    IUserMessage bankInfoEU2m = messageList.ToList()[0] as IUserMessage;

                    if (bankInfoNA1m != null && bankInfoNA2m != null) await UpdateBankInfoAsync(bankInfoNA1m, bankInfoNA2m, "NA", 2);
                    if (bankInfoEU1m != null && bankInfoEU2m != null) await UpdateBankInfoAsync(bankInfoEU1m, bankInfoEU2m, "EU", 2);
                }
            }

            if (Globals.timerCount % 20 == 0)
            {
                if (client.GetChannel(392829581192855554) is IMessageChannel generalDecayNAEU)
                {
                    await MidnightBankDecrease(generalDecayNAEU, "NA", 1);
                    await MidnightBankDecrease(generalDecayNAEU, "NA", 2);
                    await MidnightBankDecrease(generalDecayNAEU, "EU", 1);
                    await MidnightBankDecrease(generalDecayNAEU, "EU", 2);
                }
            }

            if (client.GetChannel(392829581192855554) is IMessageChannel generalNAEU)
            {
                if (Globals.regionList["NA"].inQueue1)
                    await CheckQueueMatchCreate1sAsync(generalNAEU, "NA");
                if (Globals.regionList["NA"].inQueue2)
                    await CheckQueueMatchCreate2sAsync(generalNAEU, "NA");
                if (Globals.regionList["EU"].inQueue1)
                    await CheckQueueMatchCreate1sAsync(generalNAEU, "EU");
                if (Globals.regionList["EU"].inQueue2)
                    await CheckQueueMatchCreate2sAsync(generalNAEU, "EU");
            }

            if (client.GetChannel(422045385612328973) is IMessageChannel generalAUSSEA)
            {
                if (Globals.regionList["AUS"].inQueue1)
                    await CheckQueueMatchCreate1sAsync(generalAUSSEA, "AUS");
                if (Globals.regionList["AUS"].inQueue2)
                    await CheckQueueMatchCreate2sAsync(generalAUSSEA, "AUS");
                if (Globals.regionList["SEA"].inQueue1)
                    await CheckQueueMatchCreate1sAsync(generalAUSSEA, "SEA");
                if (Globals.regionList["SEA"].inQueue2)
                    await CheckQueueMatchCreate2sAsync(generalAUSSEA, "SEA");
            }

            Globals.timerCount++;
        },
        null,
        TimeSpan.FromMinutes(0),  // 4) Time that message should fire after the timer is created
        TimeSpan.FromSeconds(15)); // 5) Time after which message should repeat (use `Timeout.Infinite` for no repeat)
    }

    private async Task CheckQueueMatchCreate1sAsync(IMessageChannel thisChannel, string region)
    {
        List<Player> playersInQueue = new List<Player>(10);
        string query = $"SELECT id, username, tier FROM queue{region}1 ORDER BY time;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Player thisPlayer = new Player
                {
                    id = reader.GetUInt64(0),
                    username = reader.GetString(1),
                    tier = reader.GetInt16(2)
                };

                playersInQueue.Add(thisPlayer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        while (playersInQueue.Count > 0)
        {
            Player thisPlayer = playersInQueue[0];
            playersInQueue.RemoveAt(0);
            Player checkPlayer;
            for (int i = 0; i < playersInQueue.Count; i++)
            {
                checkPlayer = playersInQueue[i];
                if ((thisPlayer.tier & checkPlayer.tier) > 0)
                {
                    await MatchModule.CreateMatch1Async(thisChannel, thisPlayer, checkPlayer, region);
                    playersInQueue.Remove(checkPlayer);
                    i--;
                    break;
                }
            }
        }
    }

    private async Task CheckQueueMatchCreate2sAsync(IMessageChannel thisChannel, string region)
    {
        List<Player> playersInQueue = new List<Player>(20);
        List<Team> teamsInQueue = new List<Team>(10);
        string query = $"SELECT id, username, tier, tierTeammate FROM queue{region}2 ORDER BY time;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Player thisPlayer = new Player
                {
                    id = reader.GetUInt64(0),
                    username = reader.GetString(1),
                    tier = reader.GetInt16(2),
                    tierTeammate = reader.GetInt16(3)
                };

                playersInQueue.Add(thisPlayer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        while (playersInQueue.Count > 0)
        {
            Player thisPlayer = playersInQueue[0];
            playersInQueue.RemoveAt(0);
            List<Player> potentialTeammates = new List<Player>(playersInQueue.Count);
            foreach (var checkPlayer in playersInQueue)
            {
                if ((thisPlayer.tierTeammate & checkPlayer.tierTeammate) > 0)
                {
                    potentialTeammates.Add(checkPlayer);
                }
            }

            if (potentialTeammates.Count > 0)
            {
                Player teammate = potentialTeammates[Globals.rnd.Next(potentialTeammates.Count)];
                playersInQueue.Remove(teammate);
                teamsInQueue.Add(new Team { p1 = thisPlayer, p2 = teammate, tier = thisPlayer.tier & teammate.tier });
            }
        }

        while (teamsInQueue.Count > 0)
        {
            Team thisTeam = teamsInQueue[0];
            teamsInQueue.RemoveAt(0);
            Team checkTeam;
            for (int i = 0; i < teamsInQueue.Count; i++)
            {
                checkTeam = teamsInQueue[i];
                if ((thisTeam.tier & checkTeam.tier) > 0)
                {
                    await MatchModule.CreateMatch2Async(thisChannel, thisTeam, checkTeam, region);
                    teamsInQueue.Remove(checkTeam);
                    i--;
                    break;
                }
            }
        }
    }

    private async Task UpdateLeaderboardAsync(IUserMessage thisMessage, string region, int gameMode, int tier)
    {
        var embed = new EmbedBuilder
        {
            Title = $"Tier {tier} Leaderboard",
            Color = HelperFunctions.GetTierColor(tier)
        };

        int i = 1;
        string query = $"SELECT id, username, elo, wins, loss FROM leaderboard{region}{gameMode} ORDER BY elo DESC;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (i > 25) break;
                if (TierModule.GetTierModule(region, gameMode).getPlayerTier(reader.GetUInt64(0)) != tier)
                    continue;
                embed.AddField(x =>
                {
                    x.Name = $"{i}: {reader.GetString(1)}";
                    x.Value = $"{reader.GetInt16(2)} elo\n{reader.GetInt16(3)} - {reader.GetInt16(4)}";
                });
                i++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    private async Task UpdateBankInfoAsync(IUserMessage thisMessage1, IUserMessage thisMessage2, string region, int gameMode)
    {
        var embed1 = new EmbedBuilder
        {
            Title = $"{region} Banks 1/2",
            Color = HelperFunctions.GetRegionColor(region)
        };

        var embed2 = new EmbedBuilder
        {
            Title = $"{region} Banks 2/2",
            Color = HelperFunctions.GetRegionColor(region)
        };

        int i = 1;
        string query = $"SELECT username, bank FROM leaderboard{region}{gameMode} ORDER BY username;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (i <= 25)
                {
                    embed1.AddField(x =>
                    {
                        x.Name = $"{reader.GetString(0)}: {reader.GetInt16(1)}";
                        x.Value = "​";
                        x.IsInline = true;
                    });
                }
                else
                {
                    embed2.AddField(x =>
                    {
                        x.Name = $"{reader.GetString(0)}: {reader.GetInt16(1)}";
                        x.Value = "​";
                        x.IsInline = true;
                    });
                }
                
                i++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        await thisMessage1.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed1.Build();
        });

        await thisMessage2.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed2.Build();
        });
    }

    private async Task UpdateMatchesAsync(IUserMessage thisMessage, string region1, string region2, int gameMode)
    {
        string pluralizer;
        int matchCount1 = 0, matchCount2 = 0;
        string query = $"SELECT count(*) FROM matches{region1}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                matchCount1 = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();
        query = $"SELECT count(*) FROM matches{region2}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                matchCount2 = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();
        if (matchCount1 + matchCount2 != 1) pluralizer = "es";
        else pluralizer = "";
        var embed = new EmbedBuilder
        {
            Title = "Ongoing Matches",
            Description = $"{matchCount1 + matchCount2} match{pluralizer} ongoing"
        };
        if (gameMode == 1) query = $"SELECT username1, username2, room FROM matches{region1}{gameMode};";
        else query = $"SELECT username1, username2, username3, username4, room FROM matches{region1}{gameMode};";

        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            int k = 1;
            while (reader.Read())
            {
                embed.AddField(x =>
                {
                    x.Name = $"{region1} Match #{k}";
                    if (gameMode == 1) x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}\nRoom Number: #{reader.GetInt32(2)}";
                    else x.Value = $"{reader.GetString(0)} and {reader.GetString(1)} vs {reader.GetString(2)} and {reader.GetString(3)}\nRoom Number: #{reader.GetInt32(4)}";
                });
                k++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        if (gameMode == 1) query = $"SELECT username1, username2, room FROM matches{region2}{gameMode};";
        else query = $"SELECT username1, username2, username3, username4, room FROM matches{region2}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            int k = 1;
            while (reader.Read())
            {
                embed.AddField(x =>
                {
                    x.Name = $"{region2} Match #{k}";
                    if (gameMode == 1) x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}\nRoom Number: #{reader.GetInt32(2)}";
                    else x.Value = $"{reader.GetString(0)} and {reader.GetString(1)} vs {reader.GetString(2)} and {reader.GetString(3)}\nRoom Number: #{reader.GetInt32(4)}";
                });
                k++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        if (matchCount1 + matchCount2 != 0)
        {
            embed.Color = Color.Red;
        }

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    private async Task UpdateQueueAsync(IUserMessage thisMessage, string region1, string region2, int gameMode)
    {
        int queueCount1 = 0, queueCount2 = 0;
        string query = $"SELECT count(*) FROM queue{region1}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                queueCount1 = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        query = $"SELECT count(*) FROM queue{region2}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                queueCount2 = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        string pluralizer, pluralizer1, pluralizer2;
        int totalCount = queueCount1 + queueCount2;
        if (totalCount == 1) pluralizer = "person is";
        else pluralizer = "people are";

        if (queueCount1 == 1) pluralizer1 = "person is";
        else pluralizer1 = "people are";

        if (queueCount2 == 1) pluralizer2 = "person is";
        else pluralizer2 = "people are";

        var embed = new EmbedBuilder
        {
            Title = "Queue List",
            Description = $"{totalCount} {pluralizer} in queue"
        };
        embed.AddField(x =>
        {
            x.Name = $"{region1} Queue";
            x.Value = $"{queueCount1} {pluralizer1} in queue";
        });
        embed.AddField(x =>
        {
            x.Name = $"{region2} Queue";
            x.Value = $"{queueCount2} {pluralizer2} in queue";
        });

        if(totalCount != 0)
        {
            embed.Color = Color.Red;
        }

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    private async Task UpdateMatchesAsync(IUserMessage thisMessage, string region, int gameMode)
    {
        string pluralizer;
        int matchCount = 0;
        string query = $"SELECT count(*) FROM matches{region}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                matchCount = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();
        
        if (matchCount != 1) pluralizer = "es";
        else pluralizer = "";
        var embed = new EmbedBuilder
        {
            Title = "Ongoing Matches",
            Description = $"{matchCount} match{pluralizer} ongoing"
        };
        if (gameMode == 1) query = $"SELECT username1, username2, room FROM matches{region}{gameMode};";
        else query = $"SELECT username1, username2, username3, username4, room FROM matches{region}{gameMode};";

        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            int k = 1;
            while (reader.Read())
            {
                embed.AddField(x =>
                {
                    x.Name = $"{region} Match #{k}";
                    if (gameMode == 1) x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}\nRoom Number: #{reader.GetInt32(2)}";
                    else x.Value = $"{reader.GetString(0)} and {reader.GetString(1)} vs {reader.GetString(2)} and {reader.GetString(3)}\nRoom Number: #{reader.GetInt32(4)}";
                });
                k++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        if (matchCount != 0)
        {
            embed.Color = Color.Red;
        }

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    private async Task UpdateQueueAsync(IUserMessage thisMessage, string region, int gameMode)
    {
        int queueCount = 0;
        string query = $"SELECT count(*) FROM queue{region}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                queueCount = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        string pluralizer;
        if (queueCount == 1) pluralizer = "person is";
        else pluralizer = "people are";

        var embed = new EmbedBuilder
        {
            Title = "Queue List",
            Description = $"{queueCount} {pluralizer} in queue"
        };

        if (queueCount != 0)
        {
            embed.Color = Color.Red;
        }

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    private async Task CheckQueueTimeoutAsync(IMessageChannel thisChannel, string region, int gameMode)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> timeOuts = new List<ulong>(5);
        string query = $"SELECT time, id FROM queue{region}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DateTime oldTime = new DateTime(reader.GetInt64(0));
                TimeSpan timeDif = nowTime - oldTime;
                if (timeDif.TotalMinutes > 10)
                {
                    timeOuts.Add(reader.GetUInt64(1));
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        foreach (var id in timeOuts)
        {
            await Globals.conn.OpenAsync();
            query = $"DELETE FROM queue{region}{gameMode} WHERE id = {id};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
                await thisChannel.SendMessageAsync($"A player has timed out of {region} {gameMode}v{gameMode} queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
        }
    }

    private async Task MidnightBankDecrease(IMessageChannel thisChannel, string region, int gameMode)
    {
        int hour = DateTime.Now.Hour;
        int day = DateTime.Now.Day;
        bool checkNeeded = false;
        if (hour != 7)
            return;

        Dictionary<ulong, bool> midnightChecks = new Dictionary<ulong, bool>();
        string query = $"SELECT id, midnightCheck FROM leaderboard{region}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (midnightChecks[reader.GetUInt64(0)] = (reader.GetInt16(1) != hour))
                    checkNeeded = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        if (!checkNeeded)
            return;

        Dictionary<ulong, int> banks = new Dictionary<ulong, int>();
        query = $"SELECT id, bank FROM leaderboard{region}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                banks[reader.GetUInt64(0)] = reader.GetInt16(1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        foreach(var dictChecker in midnightChecks)
        {
            if (dictChecker.Value)
            {
                if (banks[dictChecker.Key] > 0)
                {
                    query = $"UPDATE leaderboard{region}{gameMode} SET bank = bank - 1 WHERE id = {dictChecker.Key};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
                else
                {
                    query = $"UPDATE leaderboard{region}{gameMode} SET elo = elo - 50 WHERE id = {dictChecker.Key};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    await thisChannel.SendMessageAsync($"<@{dictChecker.Key}> has lost 50 elo from having an empty bank");
                }
                
                query = $"UPDATE leaderboard{region}{gameMode} SET midnightCheck = {day} WHERE id = {dictChecker.Key};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
            }
        }
    }

    private async Task CheckRoomAsync(IMessageChannel thisChannel, string region, int gameMode)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> nullRooms = new List<ulong>(5);
        string query = $"SELECT room, id1 FROM matches{region}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DateTime oldTime = new DateTime(reader.GetInt64(0));
                TimeSpan timeDif = nowTime - oldTime;
                if (reader.GetInt32(0) == 0)
                {
                    nullRooms.Add(reader.GetUInt64(1));
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        foreach (var id in nullRooms)
        {
            if(gameMode == 1) query = $"SELECT id1, id2 FROM matches{region}{gameMode} WHERE id1 = {id};";
            else query = $"SELECT id1, id2, id3, id4 FROM matches{region}{gameMode} WHERE id1 = {id};";

            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if(gameMode == 1) await thisChannel.SendMessageAsync($"Hey <@{reader.GetInt64(0)}> and <@{reader.GetInt64(1)}>, please add your room number so it can be streamed on ProBrawlhalla");
                    else await thisChannel.SendMessageAsync($"Hey <@{reader.GetInt64(0)}>, <@{reader.GetInt64(1)}>, <@{reader.GetInt64(2)}>, and <@{reader.GetInt64(3)}>, please add your room number so it can be streamed on ProBrawlhalla");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
        }
    }

    private async Task EloDecayAsync(IMessageChannel thisChannel, string region, int gameMode)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> decayWarning = new List<ulong>(30);
        List<ulong> decayIDs = new List<ulong>(30);
        List<int> decayDays = new List<int>(30);
        string query = $"SELECT decaytimer, decayed, id FROM leaderboard{region}{gameMode};";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DateTime oldTime = new DateTime(reader.GetInt64(0));
                TimeSpan timeDifBinary = nowTime - oldTime;
                int timeDif = (int)timeDifBinary.TotalDays;
                int decayed = reader.GetInt16(1);
                ulong id = reader.GetUInt64(2);
                if (timeDif == 2 && decayed == -1)
                {
                    decayWarning.Add(id);
                }

                if (timeDif - decayed == 3 && decayed > -1)
                {
                    decayIDs.Add(id);
                    decayDays.Add((int)timeDif);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        foreach (var id in decayWarning)
        {
            await Task.Delay(5000);
            await Globals.conn.OpenAsync();
            query = $"UPDATE leaderboard{region}{gameMode} SET decayed = 0 WHERE id = {id};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
                await thisChannel.SendMessageAsync($"Hey <@{id}> your elo decay will be starting tomorrow. Play a {gameMode}v{gameMode} game in the next 24 hours to prevent this from happening.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
        }

        for (int i = 0; i < decayIDs.Count; i++)
        {
            query = $"UPDATE leaderboard{region}{gameMode} SET elo = elo - {decayDays[i] + 2} WHERE id = {decayIDs[i]};";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            var user = await thisChannel.GetUserAsync(decayIDs[i]);
            Console.WriteLine($"{user.Username} has lost {decayDays[i] + 2} {gameMode}v{gameMode} elo to decay in {region}");

            query = $"UPDATE leaderboard{region}{gameMode} SET decayed = decayed + 1 WHERE id = {decayIDs[i]};";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
        }
    }
}