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
            if (client.GetChannel(392829581192855554) is IMessageChannel generalNAEU)
            {
                await CheckQueueTimeoutAsync(generalNAEU, "NA", 1);
                await CheckQueueTimeoutAsync(generalNAEU, "NA", 2);
                await CheckQueueTimeoutAsync(generalNAEU, "EU", 1);
                await CheckQueueTimeoutAsync(generalNAEU, "EU", 2);

                if (Globals.timerCount % 4 == 0)
                {
                    await CheckRoomAsync(generalNAEU, "NA", 1);
                    await CheckRoomAsync(generalNAEU, "NA", 2);
                    await CheckRoomAsync(generalNAEU, "EU", 1);
                    await CheckRoomAsync(generalNAEU, "EU", 2);
                }
                
            }

            if (client.GetChannel(422045385612328973) is IMessageChannel generalAUSSEA)
            {
                await CheckQueueTimeoutAsync(generalAUSSEA, "AUS", 1);
                await CheckQueueTimeoutAsync(generalAUSSEA, "SEA", 1);
                await CheckQueueTimeoutAsync(generalAUSSEA, "AUS", 2);
                await CheckQueueTimeoutAsync(generalAUSSEA, "SEA", 2);
            }

            if (client.GetChannel(429366707656589312) is IMessageChannel decayNAEU)
            {
                if (Globals.timerCount % 120 == 0)
                {
                    await EloDecayAsync(decayNAEU, "NA", 1);
                    await EloDecayAsync(decayNAEU, "NA", 2);
                    await EloDecayAsync(decayNAEU, "EU", 1);
                    await EloDecayAsync(decayNAEU, "EU", 2);
                }
            }

            if (client.GetChannel(422768563800244234) is IMessageChannel decayAUSSEA)
            {
                if (Globals.timerCount % 120 == 0)
                {
                    await EloDecayAsync(decayAUSSEA, "AUS", 1);
                    await EloDecayAsync(decayAUSSEA, "AUS", 2);
                    await EloDecayAsync(decayAUSSEA, "SEA", 1);
                    await EloDecayAsync(decayAUSSEA, "SEA", 2);
                }
            }

            if (client.GetChannel(401167888762929153) is IMessageChannel queue1InfoNAEU)
            {
                IEnumerable<IMessage> messageList = await queue1InfoNAEU.GetMessagesAsync(4).Flatten();

                IUserMessage leaderboardNAm = messageList.ToList()[3] as IUserMessage;
                IUserMessage leaderboardEUm = messageList.ToList()[2] as IUserMessage;
                IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                if (leaderboardNAm != null) await UpdateLeaderboardAsync(leaderboardNAm, "NA", 1);
                if (leaderboardEUm != null) await UpdateLeaderboardAsync(leaderboardEUm, "EU", 1);
                if (matchListm != null) await UpdateMatchesAsync(matchListm, "NA", "EU", 1);
                if (queueListm != null) await UpdateQueueAsync(queueListm, "NA", "EU", 1);
                
            }

            if (client.GetChannel(404558855771521024) is IMessageChannel queue2InfoNAEU)
            {
                IEnumerable<IMessage> messageList = await queue2InfoNAEU.GetMessagesAsync(4).Flatten();

                IUserMessage leaderboardNAm = messageList.ToList()[3] as IUserMessage;
                IUserMessage leaderboardEUm = messageList.ToList()[2] as IUserMessage;
                IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                if (leaderboardNAm != null) await UpdateLeaderboardAsync(leaderboardNAm, "NA", 2);
                if (leaderboardEUm != null) await UpdateLeaderboardAsync(leaderboardEUm, "EU", 2);
                if (matchListm != null) await UpdateMatchesAsync(matchListm, "NA", "EU", 2);
                if (queueListm != null) await UpdateQueueAsync(queueListm, "NA", "EU", 2);

            }

            if (client.GetChannel(423372016922525697) is IMessageChannel queue1InfoAUSSEA)
            {
                IEnumerable<IMessage> messageList = await queue1InfoAUSSEA.GetMessagesAsync(4).Flatten();

                IUserMessage leaderboardAUSm = messageList.ToList()[3] as IUserMessage;
                IUserMessage leaderboardSEAm = messageList.ToList()[2] as IUserMessage;
                IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                if (leaderboardAUSm != null) await UpdateLeaderboardAsync(leaderboardAUSm, "AUS", 1);
                if (leaderboardSEAm != null) await UpdateLeaderboardAsync(leaderboardSEAm, "SEA", 1);
                if (matchListm != null) await UpdateMatchesAsync(matchListm, "AUS", "SEA", 1);
                if (queueListm != null) await UpdateQueueAsync(queueListm, "AUS", "SEA", 1);

            }

            if (client.GetChannel(437510787951493121) is IMessageChannel queue2InfoAUSSEA)
            {
                IEnumerable<IMessage> messageList = await queue2InfoAUSSEA.GetMessagesAsync(4).Flatten();

                IUserMessage leaderboardAUSm = messageList.ToList()[3] as IUserMessage;
                IUserMessage leaderboardSEAm = messageList.ToList()[2] as IUserMessage;
                IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                if (leaderboardAUSm != null) await UpdateLeaderboardAsync(leaderboardAUSm, "AUS", 2);
                if (leaderboardSEAm != null) await UpdateLeaderboardAsync(leaderboardSEAm, "SEA", 2);
                if (matchListm != null) await UpdateMatchesAsync(matchListm, "AUS", "SEA", 2);
                if (queueListm != null) await UpdateQueueAsync(queueListm, "AUS", "SEA", 2);
            }

            Globals.timerCount++;
        },
        null,
        TimeSpan.FromMinutes(0),  // 4) Time that message should fire after the timer is created
        TimeSpan.FromSeconds(30)); // 5) Time after which message should repeat (use `Timeout.Infinite` for no repeat)
    }

    public async Task UpdateLeaderboardAsync(IUserMessage thisMessage, string region, int gameMode)
    {
        var embed = new EmbedBuilder
        {
            Title = $"{region} Leaderboard",
            Color = HelperFunctions.GetRegionColor(region)
        };

        int i = 1;
        string query = $"SELECT username, elo, wins, loss FROM leaderboard{region}{gameMode} ORDER BY elo DESC;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (i > 25) break;
                embed.AddField(x =>
                {
                    x.Name = $"{i}: {reader.GetString(0)}";
                    x.Value = $"{reader.GetInt16(1)} elo\n{reader.GetInt16(2)} - {reader.GetInt16(3)}";
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

    public async Task UpdateMatchesAsync(IUserMessage thisMessage, string region1, string region2, int gameMode)
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

    public async Task UpdateQueueAsync(IUserMessage thisMessage, string region1, string region2, int gameMode)
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

    public async Task CheckQueueTimeoutAsync(IMessageChannel thisChannel, string region, int gameMode)
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

    public async Task CheckRoomAsync(IMessageChannel thisChannel, string region, int gameMode)
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

    public async Task EloDecayAsync(IMessageChannel thisChannel, string region, int gameMode)
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
            Console.WriteLine($"{user.Username} has lost {decayDays[i] + 2} {gameMode}v{gameMode} elo to decay");

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