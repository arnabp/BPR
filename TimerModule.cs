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
            // 3) Any code you want to periodically run goes here, for example:
            Console.WriteLine("Tick");

            if (client.GetChannel(401167888762929153) is IMessageChannel chan)
            {
                IEnumerable<IMessage> messageList = await chan.GetMessagesAsync(4).Flatten();

                IUserMessage leaderboardNAm = messageList.ToList()[3] as IUserMessage;
                IUserMessage leaderboardEUm = messageList.ToList()[2] as IUserMessage;
                IUserMessage matchListm = messageList.ToList()[1] as IUserMessage;
                IUserMessage queueListm = messageList.ToList()[0] as IUserMessage;

                if (leaderboardNAm != null) await UpdateLeaderboardNA(leaderboardNAm);
                if (leaderboardEUm != null) await UpdateLeaderboardEU(leaderboardEUm);
                if (matchListm != null) await UpdateMatches(matchListm);
                if (queueListm != null) await UpdateQueue(queueListm);
            }
        },
        null,
        TimeSpan.FromMinutes(0),  // 4) Time that message should fire after the timer is created
        TimeSpan.FromSeconds(30)); // 5) Time after which message should repeat (use `Timeout.Infinite` for no repeat)
    }

    public async Task UpdateLeaderboardNA(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "NA Leaderboard"
        };

        int i = 1;
        string query = $"SELECT username, elo FROM leaderboardNA ORDER BY elo DESC;";
        Globals.conn.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                embed.AddField(x =>
                {
                    x.Name = $"{i}: {reader.GetString(0)}";
                    x.Value = $"{reader.GetInt16(1)} elo";
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

    public async Task UpdateLeaderboardEU(IUserMessage thisMessage)
    {
        var embed = new EmbedBuilder
        {
            Title = "EU Leaderboard"
        };

        int i = 1;
        string query = $"SELECT username, elo FROM leaderboardEU ORDER BY elo DESC;";
        Globals.conn.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                embed.AddField(x =>
                {
                    x.Name = $"{i}: {reader.GetString(0)}";
                    x.Value = $"{reader.GetInt16(1)} elo";
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

    public async Task UpdateMatches(IUserMessage thisMessage)
    {
        string pluralizer;
        int matchCountNA = 0, matchCountEU = 0;
        string query = $"SELECT count(*) FROM matchesNA;";
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
        query = $"SELECT count(*) FROM matchesEU;";
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
        Console.WriteLine($"Pluralization checked");
        var embed = new EmbedBuilder
        {
            Title = "Ongoing Matches",
            Description = $"{matchCountNA + matchCountEU} match{pluralizer} ongoing"
        };
        query = $"SELECT username1, username2 FROM matchesNA;";
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
                    x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}";
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

        query = $"SELECT username1, username2 FROM matchesEU;";
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
                    x.Value = $"{reader.GetString(0)} vs {reader.GetString(1)}";
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

    public async Task UpdateQueue(IUserMessage thisMessage)
    {
        int queueCountNA = 0, queueCountEU = 0;
        string query = $"SELECT count(*) FROM queueNA;";
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

        query = $"SELECT count(*) FROM queueEU;";
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
}