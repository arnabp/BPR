using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;
using Discord;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace BPR
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes a message.")]
        public async Task EchoAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            await Context.Channel.SendMessageAsync(echo);
            Console.WriteLine($"{echo} has been echoed");
        }
        
        [Command("CheckRoleId")]
        [Summary("Gets name of role from given id.")]
        public async Task CheckRoleIdAsync([Remainder] ulong id)
        {
            await Context.Channel.SendMessageAsync(Context.Guild.GetRole(id).Name);
        }

        [Command("CheckGuildUser")]
        [Summary("Gets the username of the person using the command (through Guild)")]
        public async Task CheckGuildUserAsync()
        {
            var userinfo = Context.User;
            var user = Context.Guild.GetUser(userinfo.Id);
            await Context.Channel.SendMessageAsync(user.Username);
        }

        [Command("CheckFirstRole")]
        [Summary("Gets the first role attributed to the user")]
        public async Task CheckFirstRole()
        {
            var userinfo = Context.User;
            var user = Context.Guild.GetUser(userinfo.Id);
            await Context.Channel.SendMessageAsync($"{user.Roles.ElementAt(1).Id}: {user.Roles.ElementAt(1).Name}");
        }

        [Command("CheckMessageId")]
        [Summary("Gets the id of the sent message")]
        public async Task CheckMessageId()
        {
            await Context.Channel.SendMessageAsync($"Id is {Context.Message.Id}");
        }

        [Command("Silent")]
        [Summary("Deletes the message")]
        public async Task Silent()
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync($"Message from {userInfo.Username} has been deleted.");
        }
    }

    [Group("queue")]
    public class QueueModule : ModuleBase<SocketCommandContext>
    {

        [Command("join")]
        [Summary("Joins the current queue")]
        public async Task JoinAsync()
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            Console.WriteLine($"{userInfo.Username} is attempting to join queue");

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueNA;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (queueCount > 0)
                {
                    query = $"SELECT id FROM queueNA;";
                    Globals.conn.Open();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            if (reader.GetUInt64(0) == userInfo.Id) isInQueue = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();
                }

                if (isInQueue)
                {
                    await Context.Channel.SendMessageAsync($"Player already in queue tried to rejoin queue");
                    Console.WriteLine($"{userInfo.Username} tried to join the queue twice");
                }
                else
                {
                    query = $"INSERT INTO queueNA(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
                    Globals.conn.Open();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();

                    await Context.Channel.SendMessageAsync($"A player has been added to NA queue");
                    Console.WriteLine($"{userInfo.Username} has joined queue");
                    
                    if (queueCount > 1)
                    {
                        await NewMatchNA(0, 1); // This should be modified when anti-smurfing mechanism is introduced
                    }
                }
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueEU;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (queueCount > 0)
                {
                    query = $"SELECT id FROM queueEU;";
                    Globals.conn.Open();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader.GetUInt64(0) == userInfo.Id) isInQueue = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();
                }

                if (isInQueue)
                {
                    await Context.Channel.SendMessageAsync($"Player already in queue tried to rejoin queue");
                    Console.WriteLine($"{userInfo.Username} tried to join the queue twice");
                }
                else
                {
                    query = $"INSERT INTO queueEU(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
                    Globals.conn.Open();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();

                    await Context.Channel.SendMessageAsync($"A player has been added to NA queue");
                    Console.WriteLine($"{userInfo.Username} has joined queue");

                    if (queueCount > 1)
                    {
                        await NewMatchNA(0, 1); // This should be modified when anti-smurfing mechanism is introduced
                    }
                }
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
        }

        [Command("list")]
        [Summary("List current people in queue")]
        public async Task QueueListAsync()
        {
            Console.WriteLine($"Queue is being listed");

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

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("leave")]
        [Summary("Leaves the current queue")]
        public async Task QueueLeaveAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave queue");

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueNA;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (queueCount > 0)
                {
                    query = $"SELECT id FROM queueNA;";
                    Globals.conn.Open();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader.GetUInt64(0) == userInfo.Id) isInQueue = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();
                }
                
                if (isInQueue)
                {
                    Globals.conn.Open();
                    query = $"DELETE FROM queueNA WHERE id = {userInfo.Id};";
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();
                    await Context.Channel.SendMessageAsync($"A player has left the NA queue");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Player not in queue tried to leave queue");
                    Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
                }
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueEU;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (queueCount > 0)
                {
                    query = $"SELECT id FROM queueEU;";
                    Globals.conn.Open();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader.GetUInt64(0) == userInfo.Id) isInQueue = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();
                }

                if (isInQueue)
                {
                    Globals.conn.Open();
                    query = $"DELETE FROM queueEU WHERE id = {userInfo.Id};";
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Globals.conn.Close();
                        throw;
                    }
                    Globals.conn.Close();
                    await Context.Channel.SendMessageAsync($"A player has left the EU queue");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Player not in queue tried to leave queue");
                    Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
                }
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
            await Context.Message.DeleteAsync();
        }

        private async Task NewMatchNA(int p1, int p2)
        {
            long p1time = 0, p2time = 0;
            string p1name = "", p2name = "";
            ulong p1id = 0, p2id = 0;
            string query = $"SELECT time, username, id FROM queueNA;";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int i = 0;
                while (reader.Read())
                {
                    if (i == p1)
                    {
                        p1time = reader.GetInt64(0);
                        p1name = reader.GetString(1);
                        p1id = reader.GetUInt64(2);
                    }
                    if(i == p2)
                    {
                        p2time = reader.GetInt64(0);
                        p2name = reader.GetString(1);
                        p2id = reader.GetUInt64(2);
                    }
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

            query = $"INSERT INTO matchesNA(id1, id2, username1, username2) VALUES({p1id}, {p2id}, '{p1name}', '{p2name}');";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            int matchCount = 0;
            query = $"SELECT count(*) FROM matchesNA;";
            Globals.conn.Open();
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            await Context.Channel.SendMessageAsync($"New match has started between <@{p1id}> and <@{p2id}>");
            Console.WriteLine($"NA Match #{matchCount} has started.");

        }

        private async Task NewMatchEU(int p1, int p2)
        {
            long p1time = 0, p2time = 0;
            string p1name = "", p2name = "";
            ulong p1id = 0, p2id = 0;
            string query = $"SELECT time, username, id FROM queueEU;";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int i = 0;
                while (reader.Read())
                {
                    if (i == p1)
                    {
                        p1time = reader.GetInt64(0);
                        p1name = reader.GetString(1);
                        p1id = reader.GetUInt64(2);
                    }
                    if (i == p2)
                    {
                        p2time = reader.GetInt64(0);
                        p2name = reader.GetString(1);
                        p2id = reader.GetUInt64(2);
                    }
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

            query = $"INSERT INTO matchesEU(id1, id2, username1, username2) VALUES({p1id}, {p2id}, '{p1name}', '{p2name}');";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            int matchCount = 0;
            query = $"SELECT count(*) FROM matchesEU;";
            Globals.conn.Open();
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            await Context.Channel.SendMessageAsync($"New match has started between <@{p1id}> and <@{p2id}>");
            Console.WriteLine($"EU Match #{matchCount} has started.");

        }
    }

    [Group("match")]
    public class MatchModule : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        [Summary("Lists all ongoing matches")]
        [Alias("all","ongoing","show")]
        public async Task ListMatchAsync()
        {
            Console.WriteLine($"Matches are being listed");
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

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("report")]
        [Summary("Allows user to report current match")]
        [Alias("score", "result")]
        public async Task MatchReportAsync([Remainder] [Summary("The winner, \"Y\" or \"N\"")] string winner)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool? isP1 = null;
            ulong p1ID = 0;
            ulong p2ID = 0;
            string p1Username = "";
            string p2Username = "";
            int thisMatchNum = 1;

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                string query = $"SELECT id1, id2, username1, username2 FROM matchesNA;";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (userInfo.Id == reader.GetUInt64(0))
                        {
                            isP1 = true;
                            p1ID = reader.GetUInt64(0);
                            p2ID = reader.GetUInt64(1);
                            p1Username = reader.GetString(2);
                            p2Username = reader.GetString(3);
                            break;
                        }
                        else if (userInfo.Id == reader.GetUInt64(1))
                        {
                            isP1 = false;
                            p1ID = reader.GetUInt64(0);
                            p2ID = reader.GetUInt64(1);
                            p1Username = reader.GetString(2);
                            p2Username = reader.GetString(3);
                            break;
                        }
                        thisMatchNum++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                if (isP1 == null)
                {
                    await Context.Channel.SendMessageAsync($"You are currently not in a match against anyone");
                    return;
                }

                if (winner == "W" || winner == "w" || winner == "Y" || winner == "y") { }
                else if (winner == "L" || winner == "l" || winner == "N" || winner == "n")
                {
                    isP1 = !isP1;
                }
                else
                {
                    Console.WriteLine($"{userInfo.Username} entered the wrong result type");
                    await Context.Channel.SendMessageAsync("Invalid results entered");
                    return;
                }

                Console.WriteLine($"isP1: {isP1}");

                var results = new Tuple<double, double>(0, 0);
                double p1elo = 0, p2elo = 0;
                query = $"SELECT elo FROM leaderboardNA WHERE id = {p1ID};";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        p1elo = reader.GetDouble(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                query = $"SELECT elo FROM leaderboardNA WHERE id = {p2ID};";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        p2elo = reader.GetDouble(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                results = EloConvert(p1elo, p2elo, (bool)isP1);

                double new1 = p1elo + results.Item1;
                double new2 = p2elo + results.Item2;

                var embed = new EmbedBuilder
                {
                    Title = "Match Result"
                };
                embed.AddField(x =>
                {
                    x.Name = $"{p1Username}: {Convert.ToInt32(results.Item1)} elo";
                    x.Value = $"{p1Username} now has {Convert.ToInt32(new1)} elo";
                });
                embed.AddField(x =>
                {
                    x.Name = $"{p2Username}: {Convert.ToInt32(results.Item2)} elo";
                    x.Value = $"{p2Username} now has {Convert.ToInt32(new2)} elo";
                });

                await Context.Channel.SendMessageAsync("", embed: embed);

                Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
                query = $"UPDATE leaderboardNA SET elo = {new1} WHERE id = {p1ID};";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                Console.WriteLine($"Giving {p2Username} {results.Item2} elo, resulting in {new2}");
                Globals.conn.Open();
                query = $"UPDATE leaderboardNA SET elo = {new2} WHERE id = {p2ID};";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                Globals.conn.Open();
                query = $"DELETE FROM matchesNA WHERE id1 = {p1ID};";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                Console.WriteLine($"Match #{thisMatchNum} has ended.");
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                string query = $"SELECT id1, id2, username1, username2 FROM matchesEU;";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        if (userInfo.Id == reader.GetUInt64(0))
                        {
                            isP1 = true;
                            p1ID = reader.GetUInt64(0);
                            p2ID = reader.GetUInt64(1);
                            p1Username = reader.GetString(2);
                            p2Username = reader.GetString(3);
                            break;
                        }
                        else if (userInfo.Id == reader.GetUInt64(1))
                        {
                            isP1 = false;
                            p1ID = reader.GetUInt64(0);
                            p2ID = reader.GetUInt64(1);
                            p1Username = reader.GetString(2);
                            p2Username = reader.GetString(3);
                            break;
                        }
                        thisMatchNum++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                if (isP1 == null)
                {
                    await Context.Channel.SendMessageAsync($"You are currently not in a match against anyone");
                    return;
                }

                if (winner == "W" || winner == "w" || winner == "Y" || winner == "y") { }
                else if (winner == "L" || winner == "l" || winner == "N" || winner == "n")
                {
                    isP1 = !isP1;
                }
                else
                {
                    Console.WriteLine($"{userInfo.Username} entered the wrong result type");
                    await Context.Channel.SendMessageAsync("Invalid results entered");
                    return;
                }

                Console.WriteLine($"isP1: {isP1}");

                var results = new Tuple<double, double>(0, 0);
                double p1elo = 0, p2elo = 0;
                query = $"SELECT elo FROM leaderboardEU WHERE id = {p1ID};";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        p1elo = reader.GetDouble(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                query = $"SELECT elo FROM leaderboardEU WHERE id = {p2ID};";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        p2elo = reader.GetDouble(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                results = EloConvert(p1elo, p2elo, (bool)isP1);

                double new1 = p1elo + results.Item1;
                double new2 = p2elo + results.Item2;

                var embed = new EmbedBuilder
                {
                    Title = "Match Result"
                };
                embed.AddField(x =>
                {
                    x.Name = $"{p1Username}: {Convert.ToInt32(results.Item1)} elo";
                    x.Value = $"{p1Username} now has {Convert.ToInt32(new1)} elo";
                });
                embed.AddField(x =>
                {
                    x.Name = $"{p2Username}: {Convert.ToInt32(results.Item2)} elo";
                    x.Value = $"{p2Username} now has {Convert.ToInt32(new2)} elo";
                });

                await Context.Channel.SendMessageAsync("", embed: embed);

                Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
                query = $"UPDATE leaderboardEU SET elo = {new1} WHERE id = {p1ID};";
                Globals.conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                Console.WriteLine($"Giving {p2Username} {results.Item2} elo, resulting in {new2}");
                Globals.conn.Open();
                query = $"UPDATE leaderboardEU SET elo = {new2} WHERE id = {p2ID};";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                Globals.conn.Open();
                query = $"DELETE FROM matchesEU WHERE id1 = {p1ID};";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                Console.WriteLine($"Match #{thisMatchNum} has ended.");
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
        }

        [Command("superNA")]
        [Summary("Allows admin to report any match")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminMatchReportNAAsync(ulong p1ID, ulong p2ID)
        {


            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool isP1 = true;
           

            var results = new Tuple<double, double>(0, 0);
            double p1elo = 0, p2elo = 0;
            string query = $"SELECT elo FROM leaderboardNA WHERE id = {p1ID};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    p1elo = reader.GetDouble(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            query = $"SELECT elo FROM leaderboardNA WHERE id = {p2ID};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    p2elo = reader.GetDouble(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            results = EloConvert(p1elo, p2elo, isP1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;
            
            query = $"UPDATE leaderboardNA SET elo = {new1} WHERE id = {p1ID};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            
            Globals.conn.Open();
            query = $"UPDATE leaderboardNA SET elo = {new2} WHERE id = {p2ID};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            await Context.Channel.SendMessageAsync($"Match hard updated");
        }

        [Command("superEU")]
        [Summary("Allows admin to report any match")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminMatchReportEUAsync(ulong p1ID, ulong p2ID)
        {


            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool isP1 = true;


            var results = new Tuple<double, double>(0, 0);
            double p1elo = 0, p2elo = 0;
            string query = $"SELECT elo FROM leaderboardEU WHERE id = {p1ID};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    p1elo = reader.GetDouble(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            query = $"SELECT elo FROM leaderboardEU WHERE id = {p2ID};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    p2elo = reader.GetDouble(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            results = EloConvert(p1elo, p2elo, isP1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;

            query = $"UPDATE leaderboardEU SET elo = {new1} WHERE id = {p1ID};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            Globals.conn.Open();
            query = $"UPDATE leaderboardEU SET elo = {new2} WHERE id = {p2ID};";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            await Context.Channel.SendMessageAsync($"Match hard updated");
        }

        private Tuple<double, double> EloConvert(double p1, double p2, bool isP1)
        {
            double expected1 = 1 / (1 + System.Math.Pow(10, ((p2 - p1) / 400)));
            double expected2 = 1 / (1 + System.Math.Pow(10, ((p1 - p2) / 400)));

            double change1 = (32 * (Convert.ToDouble(isP1) - expected1)) + 2;
            double change2 = (32 * (Convert.ToDouble(!isP1) - expected2)) + 2;

            var allChange = Tuple.Create(change1, change2);

            return allChange;
        }

    }

    [Group("leaderboardNA")]
    public class LeaderboardNAModule : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        [Summary("Join the leaderboard")]
        public async Task JoinDBAsync()
        {
            var userInfo = Context.User;
            var user = Context.Guild.GetUser(userInfo.Id);
            string query = $"INSERT INTO leaderboardNA(id, username) VALUES({userInfo.Id}, '{userInfo.Username}');";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            await user.AddRoleAsync(Context.Guild.GetRole(396442734271004672));

            await Context.Channel.SendMessageAsync($"{userInfo.Username} has been succesfully registered! You have 2500 elo.");
            Console.WriteLine($"{userInfo.Username} has been registered");
        }

        [Command("list")]
        [Summary("Lists the current status of the leaderboard")]
        [Alias("view","show")]
        public async Task ListLeaderboardAsync()
        {
            Console.WriteLine($"NA Leaderboard is being requested");
            var embed = new EmbedBuilder
            {
                Title = "Leaderboard"
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

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardNA WHERE id = {id};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    username = reader.GetString(0);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            query = $"DELETE FROM leaderboardNA WHERE id = {id};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            await user.RemoveRoleAsync(Context.Guild.GetRole(396442734271004672));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the leaderboards");
        }
    }

    [Group("leaderboardEU")]
    public class LeaderboardEUModule : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        [Summary("Join the leaderboard")]
        public async Task JoinDBAsync()
        {
            await Context.Message.DeleteAsync();

            var userInfo = Context.User;
            var user = Context.Guild.GetUser(userInfo.Id);
            string query = $"INSERT INTO leaderboardEU(id, username) VALUES({userInfo.Id}, '{userInfo.Username}');";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            
            await user.AddRoleAsync(Context.Guild.GetRole(396442764298158081));

            await Context.Channel.SendMessageAsync($"{userInfo.Username} has been succesfully registered! You have 2500 elo.");
            Console.WriteLine($"{userInfo.Username} has been registered");
        }

        [Command("list")]
        [Summary("Lists the current status of the leaderboard")]
        [Alias("view", "show")]
        public async Task ListLeaderboardAsync()
        {
            Console.WriteLine($"EU Leaderboard is being requested");
            var embed = new EmbedBuilder
            {
                Title = "Leaderboard"
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

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();
            string username = "";
            string query = $"SELECT username FROM leaderboardEU WHERE id = {id};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    username = reader.GetString(0);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            query = $"DELETE FROM leaderboardEU WHERE id = {id};";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            await user.RemoveRoleAsync(Context.Guild.GetRole(396442764298158081));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the leaderboards");
        }
    }
}
