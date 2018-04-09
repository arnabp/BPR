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
            if (client.GetChannel(392829581192855554) is IMessageChannel general)
            {
                await CheckQueueTimeoutAsync(general, "NA", 1);
                await CheckQueueTimeoutAsync(general, "NA", 2);
                await CheckQueueTimeoutAsync(general, "EU", 1);
                await CheckQueueTimeoutAsync(general, "EU", 2);

                if (Globals.timerCount % 4 == 0)
                {
                    await CheckRoomAsync(general, "NA", 1);
                    await CheckRoomAsync(general, "NA", 2);
                    await CheckRoomAsync(general, "EU", 1);
                    await CheckRoomAsync(general, "EU", 2);
                }
                
            }

            if (client.GetChannel(429366707656589312) is IMessageChannel rank_s)
            {
                if (Globals.timerCount % 120 == 0)
                {
                    await EloDecayAsync(rank_s, "NA", 1);
                    await EloDecayAsync(rank_s, "NA", 2);
                    await EloDecayAsync(rank_s, "EU", 1);
                    await EloDecayAsync(rank_s, "EU", 2);
                }
            }

            if (client.GetChannel(401167888762929153) is IMessageChannel queue1Info)
            {
                IEnumerable<IMessage> messageList = await queue1Info.GetMessagesAsync(4).Flatten();

                IUserMessage leaderboardNAm = messageList.ToList()[3] as IUserMessage;
                IUserMessage leaderboardEUm = messageList.ToList()[2] as IUserMessage;
                IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                if (leaderboardNAm != null) await UpdateLeaderboardNA1(leaderboardNAm);
                if (leaderboardEUm != null) await UpdateLeaderboardEU1(leaderboardEUm);
                if (matchListm != null) await UpdateMatches1(matchListm);
                if (queueListm != null) await UpdateQueue1(queueListm);
                
            }

            if (client.GetChannel(404558855771521024) is IMessageChannel queue2Info)
            {
                IEnumerable<IMessage> messageList = await queue2Info.GetMessagesAsync(4).Flatten();

                IUserMessage leaderboardNAm = messageList.ToList()[3] as IUserMessage;
                IUserMessage leaderboardEUm = messageList.ToList()[2] as IUserMessage;
                IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                if (leaderboardNAm != null) await UpdateLeaderboardNA2(leaderboardNAm);
                if (leaderboardEUm != null) await UpdateLeaderboardEU2(leaderboardEUm);
                if (matchListm != null) await UpdateMatches2(matchListm);
                if (queueListm != null) await UpdateQueue2(queueListm);

            }

            Globals.timerCount++;
        },
        null,
        TimeSpan.FromMinutes(0),  // 4) Time that message should fire after the timer is created
        TimeSpan.FromSeconds(30)); // 5) Time after which message should repeat (use `Timeout.Infinite` for no repeat)
    }

    public async Task UpdateLeaderboardNA1(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "NA Leaderboard",
            Color = Color.Blue
        };

        int i = 1;
        string query = $"SELECT username, elo, wins, loss FROM leaderboardNA1 ORDER BY elo DESC;";
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

    public async Task UpdateLeaderboardEU1(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "EU Leaderboard",
            Color = Color.Green
        };

        int i = 1;
        string query = $"SELECT username, elo, wins, loss FROM leaderboardEU1 ORDER BY elo DESC;";
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

    public async Task UpdateMatches1(IUserMessage thisMessage)
    {
        string pluralizer;
        int matchCountNA = 0, matchCountEU = 0;
        string query = $"SELECT count(*) FROM matchesNA1;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                matchCountNA = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();
        query = $"SELECT count(*) FROM matchesEU1;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                matchCountEU = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();
        if (matchCountNA + matchCountEU != 1) pluralizer = "es";
        else pluralizer = "";
        var embed = new EmbedBuilder
        {
            Title = "Ongoing Matches",
            Description = $"{matchCountNA + matchCountEU} match{pluralizer} ongoing"
        };
        query = $"SELECT username1, username2, room FROM matchesNA1;";
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
                    x.Name = $"NA Match #{k}";
                    x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}\nRoom Number: #{reader.GetInt32(2)}";
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

        query = $"SELECT username1, username2, room FROM matchesEU1;";
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
                    x.Name = $"EU Match #{k}";
                    x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}\nRoom Number: #{reader.GetInt32(2)}";
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

        if (matchCountNA + matchCountEU != 0)
        {
            embed.Color = Color.Red;
        }

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task UpdateQueue1(IUserMessage thisMessage)
    {
        int queueCountNA = 0, queueCountEU = 0;
        string query = $"SELECT count(*) FROM queueNA1;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                queueCountNA = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        query = $"SELECT count(*) FROM queueEU1;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                queueCountEU = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        string pluralizer, pluralizerNA, pluralizerEU;
        int totalCount = queueCountNA + queueCountEU;
        if (totalCount == 1) pluralizer = "person is";
        else pluralizer = "people are";

        if (queueCountNA == 1) pluralizerNA = "person is";
        else pluralizerNA = "people are";

        if (queueCountEU == 1) pluralizerEU = "person is";
        else pluralizerEU = "people are";

        var embed = new EmbedBuilder
        {
            Title = "Queue List",
            Description = $"{totalCount} {pluralizer} in queue"
        };
        embed.AddField(x =>
        {
            x.Name = "NA Queue";
            x.Value = $"{queueCountNA} {pluralizerNA} in queue";
        });
        embed.AddField(x =>
        {
            x.Name = "EU Queue";
            x.Value = $"{queueCountEU} {pluralizerEU} in queue";
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

    public async Task UpdateLeaderboardNA2(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "NA Leaderboard",
            Color = Color.Blue
        };

        int i = 1;
        string query = $"SELECT username, elo, wins, loss FROM leaderboardNA2 ORDER BY elo DESC;";
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

    public async Task UpdateLeaderboardEU2(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "EU Leaderboard",
            Color = Color.Green
        };

        int i = 1;
        string query = $"SELECT username, elo, wins, loss FROM leaderboardEU2 ORDER BY elo DESC;";
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

    public async Task UpdateMatches2(IUserMessage thisMessage)
    {
        string pluralizer;
        int matchCountNA = 0, matchCountEU = 0;
        string query = $"SELECT count(*) FROM matchesNA2;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                matchCountNA = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();
        query = $"SELECT count(*) FROM matchesEU2;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                matchCountEU = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();
        if (matchCountNA + matchCountEU != 1) pluralizer = "es";
        else pluralizer = "";
        var embed = new EmbedBuilder
        {
            Title = "Ongoing Matches",
            Description = $"{matchCountNA + matchCountEU} match{pluralizer} ongoing"
        };
        query = $"SELECT username1, username2, username3, username4, room FROM matchesNA2;";
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
                    x.Name = $"NA Match #{k}";
                    x.Value = $"{reader.GetString(0)} and {reader.GetString(1)} vs {reader.GetString(2)} and {reader.GetString(3)}\nRoom Number: #{reader.GetInt32(4)}";
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

        query = $"SELECT username1, username2, username3, username4, room FROM matchesEU2;";
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
                    x.Name = $"EU Match #{k}";
                    x.Value = $"{reader.GetString(0)} and {reader.GetString(1)} vs {reader.GetString(2)} and {reader.GetString(3)}\nRoom Number: #{reader.GetInt32(4)}";
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

        if (matchCountNA + matchCountEU != 0)
        {
            embed.Color = Color.Red;
        }

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task UpdateQueue2(IUserMessage thisMessage)
    {
        int queueCountNA = 0, queueCountEU = 0;
        string query = $"SELECT count(*) FROM queueNA2;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                queueCountNA = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        query = $"SELECT count(*) FROM queueEU2;";
        await Globals.conn.OpenAsync();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                queueCountEU = reader.GetInt16(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Globals.conn.CloseAsync();
            throw;
        }
        await Globals.conn.CloseAsync();

        string pluralizer, pluralizerNA, pluralizerEU;
        int totalCount = queueCountNA + queueCountEU;
        if (totalCount == 1) pluralizer = "person is";
        else pluralizer = "people are";

        if (queueCountNA == 1) pluralizerNA = "person is";
        else pluralizerNA = "people are";

        if (queueCountEU == 1) pluralizerEU = "person is";
        else pluralizerEU = "people are";

        var embed = new EmbedBuilder
        {
            Title = "Queue List",
            Description = $"{totalCount} {pluralizer} in queue"
        };
        embed.AddField(x =>
        {
            x.Name = "NA Queue";
            x.Value = $"{queueCountNA} {pluralizerNA} in queue";
        });
        embed.AddField(x =>
        {
            x.Name = "EU Queue";
            x.Value = $"{queueCountEU} {pluralizerEU} in queue";
        });

        if (totalCount != 0)
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
                if (timeDif == 6 && decayed == -1)
                {
                    decayWarning.Add(id);
                }

                if (timeDif - decayed == 7 && decayed > -1)
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
            query = $"UPDATE leaderboard{region}{gameMode} SET elo = elo - {decayDays[i] - 2} WHERE id = {decayIDs[i]};";
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
            Console.WriteLine($"{user.Username} has lost {decayDays[i] - 2} {gameMode}v{gameMode} elo to decay");

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