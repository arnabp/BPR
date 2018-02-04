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
                await CheckQueueTimeoutNA1(general);
                await CheckQueueTimeoutEU1(general);
                await CheckQueueTimeoutNA2(general);
                await CheckQueueTimeoutEU2(general);
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
        },
        null,
        TimeSpan.FromMinutes(0),  // 4) Time that message should fire after the timer is created
        TimeSpan.FromSeconds(30)); // 5) Time after which message should repeat (use `Timeout.Infinite` for no repeat)
    }

    public async Task UpdateLeaderboardNA1(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "NA Leaderboard"
        };

        int i = 1;
        string query = $"SELECT username, elo1, wins1, loss1 FROM leaderboardNA ORDER BY elo1 DESC;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task UpdateLeaderboardEU1(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "EU Leaderboard"
        };

        int i = 1;
        string query = $"SELECT username, elo1, wins1, loss1 FROM leaderboardEU ORDER BY elo1 DESC;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

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
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();
        query = $"SELECT count(*) FROM matchesEU1;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();
        if (matchCountNA + matchCountEU != 1) pluralizer = "es";
        else pluralizer = "";
        var embed = new EmbedBuilder
        {
            Title = "Ongoing Matches",
            Description = $"{matchCountNA + matchCountEU} match{pluralizer} ongoing"
        };
        query = $"SELECT username1, username2, room FROM matchesNA1;";
        Globals.conn.Open();
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
                    x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}\nRoom Number: #{reader.GetInt16(2)}";
                });
                k++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        query = $"SELECT username1, username2, room FROM matchesEU1;";
        Globals.conn.Open();
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
                    x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}\nRoom Number: #{reader.GetInt16(2)}";
                });
                k++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task UpdateQueue1(IUserMessage thisMessage)
    {
        int queueCountNA = 0, queueCountEU = 0;
        string query = $"SELECT count(*) FROM queueNA1;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        query = $"SELECT count(*) FROM queueEU1;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

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

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task UpdateLeaderboardNA2(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "NA Leaderboard"
        };

        int i = 1;
        string query = $"SELECT username, elo2, wins2, loss2 FROM leaderboardNA ORDER BY elo2 DESC;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task UpdateLeaderboardEU2(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "EU Leaderboard"
        };

        int i = 1;
        string query = $"SELECT username, elo2, wins2, loss2 FROM leaderboardEU ORDER BY elo2 DESC;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

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
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();
        query = $"SELECT count(*) FROM matchesEU2;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();
        if (matchCountNA + matchCountEU != 1) pluralizer = "es";
        else pluralizer = "";
        var embed = new EmbedBuilder
        {
            Title = "Ongoing Matches",
            Description = $"{matchCountNA + matchCountEU} match{pluralizer} ongoing"
        };
        query = $"SELECT username1, username2, username3, username4, room FROM matchesNA2;";
        Globals.conn.Open();
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
                    x.Value = $"{reader.GetString(0)} and {reader.GetString(1)} vs {reader.GetString(2)} and {reader.GetString(3)}\nRoom Number: #{reader.GetInt16(4)}";
                });
                k++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        query = $"SELECT username1, username2, username3, username4, room FROM matchesEU2;";
        Globals.conn.Open();
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
                    x.Value = $"{reader.GetString(0)} and {reader.GetString(1)} vs {reader.GetString(2)} and {reader.GetString(3)}\nRoom Number: #{reader.GetInt16(4)}";
                });
                k++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task UpdateQueue2(IUserMessage thisMessage)
    {
        int queueCountNA = 0, queueCountEU = 0;
        string query = $"SELECT count(*) FROM queueNA2;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        query = $"SELECT count(*) FROM queueEU2;";
        Globals.conn.Open();
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
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

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

        await thisMessage.ModifyAsync(x => {
            x.Content = "";
            x.Embed = embed.Build();
        });
    }

    public async Task CheckQueueTimeoutNA1(IMessageChannel thisChannel)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> timeOuts = new List<ulong>(5);
        string query = $"SELECT time, id FROM queueNA1;";
        Globals.conn.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TimeSpan timeDif = nowTime - DateTime.FromBinary(reader.GetInt64(0));
                if (timeDif.TotalMinutes > 10)
                {
                    timeOuts.Add(reader.GetUInt64(1));
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        foreach (var id in timeOuts)
        {
            Globals.conn.Open();
            query = $"DELETE FROM queueNA1 WHERE id = {id};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
                await thisChannel.SendMessageAsync($"A player has timed out of NA 1v1 queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
        }
    }

    public async Task CheckQueueTimeoutEU1(IMessageChannel thisChannel)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> timeOuts = new List<ulong>(5);
        string query = $"SELECT time, id FROM queueEU1;";
        Globals.conn.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TimeSpan timeDif = nowTime - DateTime.FromBinary(reader.GetInt64(0));
                if (timeDif.TotalMinutes > 10)
                {
                    timeOuts.Add(reader.GetUInt64(1));
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        foreach (var id in timeOuts)
        {
            Globals.conn.Open();
            query = $"DELETE FROM queueEU1 WHERE id = {id};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
                await thisChannel.SendMessageAsync($"A player has timed out of EU 1v1 queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
        }
    }

    public async Task CheckQueueTimeoutNA2(IMessageChannel thisChannel)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> timeOuts = new List<ulong>(5);
        string query = $"SELECT time, id FROM queueNA2;";
        Globals.conn.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TimeSpan timeDif = nowTime - DateTime.FromBinary(reader.GetInt64(0));
                if (timeDif.TotalMinutes > 10)
                {
                    timeOuts.Add(reader.GetUInt64(1));
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        foreach (var id in timeOuts)
        {
            Globals.conn.Open();
            query = $"DELETE FROM queueNA2 WHERE id = {id};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
                await thisChannel.SendMessageAsync($"A player has timed out of NA 2v2 queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
        }
    }

    public async Task CheckQueueTimeoutEU2(IMessageChannel thisChannel)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> timeOuts = new List<ulong>(5);
        string query = $"SELECT time, id FROM queueEU2;";
        Globals.conn.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TimeSpan timeDif = nowTime - DateTime.FromBinary(reader.GetInt64(0));
                if (timeDif.TotalMinutes > 10)
                {
                    timeOuts.Add(reader.GetUInt64(1));
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        foreach (var id in timeOuts)
        {
            Globals.conn.Open();
            query = $"DELETE FROM queueEU2 WHERE id = {id};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
                await thisChannel.SendMessageAsync($"A player has timed out of EU 2v2 queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
        }
    }

    public async Task CheckQueueTimeoutTest(IMessageChannel thisChannel)
    {
        DateTime nowTime = DateTime.Now;

        List<ulong> timeOuts = new List<ulong>(5);
        string query = $"SELECT time, id, username FROM queuetest;";
        Globals.conn.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TimeSpan timeDif = nowTime - DateTime.FromBinary(reader.GetInt64(0));
                Console.WriteLine($"{reader.GetString(2)} has been in queue for {timeDif.TotalSeconds} seconds.");
                if (timeDif.TotalMinutes > 10)
                {
                    Console.WriteLine($"{reader.GetString(2)} is being removed from queue");
                    timeOuts.Add(reader.GetUInt64(1));
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Globals.conn.Close();
            throw;
        }
        Globals.conn.Close();

        foreach(var id in timeOuts)
        {
            Globals.conn.Open();
            query = $"DELETE FROM queuetest WHERE id = {id};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
                await thisChannel.SendMessageAsync($"A player has timed out of test queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
        }
    }
}