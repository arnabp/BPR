using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;
using Discord;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace BPR
{
    public struct Player
    {
        public ulong id;
        public string username;
        public string discriminator;
        public bool inMatch;
        public Player(ulong ID, string USERNAME, string DISCRIMINATOR)
        {
            id = ID;
            username = USERNAME;
            discriminator = DISCRIMINATOR;
            inMatch = false;
        }
    }

    public struct Match
    {
        public Player p1;
        public Player p2;
        public Match(Player P1, Player P2)
        {
            p1 = P1;
            p2 = P2;
        }
    }

    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes a message.")]
        public async Task EchoAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            await Context.Channel.SendMessageAsync(echo);
            Console.WriteLine($"{echo} has been echoed");
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
            Console.WriteLine($"{userInfo.Username} is attempting to join queue");

            bool isInQueue = false;
            if(Globals.liveQueue.Count > 0)
            {
                if (Globals.liveQueue.Peek().username == userInfo.Username) isInQueue = true;
            }
            if (isInQueue)
            {
                await Context.Channel.SendMessageAsync($"You are already in the queue");
                Console.WriteLine($"{userInfo.Username} tried to join the queue twice");
            }
            else
            {
                Globals.liveQueue.Enqueue(new Player(userInfo.Id, userInfo.Username, userInfo.Discriminator));
                await Context.Channel.SendMessageAsync($"{userInfo.Username}#{userInfo.Discriminator} added to queue");
                Console.WriteLine($"{userInfo} has joined queue");
                if (Globals.liveQueue.Count > 1)
                {
                    await NewMatch(Globals.liveQueue.Dequeue(), Globals.liveQueue.Dequeue());
                }
            }

        }
        
        [Command("list")]
        [Summary("List current people in queue")]
        public async Task QueueListAsync()
        {
            Console.WriteLine($"Matches are being listed");
            string pluralizer;
            if (Globals.liveQueue.Count == 1) pluralizer = "person is";
            else pluralizer = "people are";
            var embed = new EmbedBuilder
            {
                Title = "Queue List",
                Description = $"{Globals.liveQueue.Count} {pluralizer} in queue"
            };
            if(Globals.liveQueue.Count == 1)
            {
                embed.AddField(x => {
                    x.Name = $"{Globals.liveQueue.Peek().username}";
                });
            }

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("leave")]
        [Summary("Leaves the current queue")]
        public async Task QueueLeaveAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave queue");
            bool isInQueue = false;
            if (Globals.liveQueue.Count > 0)
            {
                if (Globals.liveQueue.Peek().username == userInfo.Username) isInQueue = true;
            }
            if (isInQueue)
            {
                Globals.liveQueue.Dequeue();
            }
            else
            {
                await Context.Channel.SendMessageAsync($"You are not in queue");
                Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
            }
        }

        private async Task NewMatch(Player p1, Player p2)
        {
            Match thisMatch = new Match(p1, p2);
            Globals.matches.Add(thisMatch);
            Globals.matchCount++;

            await Context.Channel.SendMessageAsync($"New match has started between <@{p1.id}> and <@{p2.id}>");
            Console.WriteLine($"Match #{Globals.matchCount} has started.");
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
            if (Globals.matchCount == 1) pluralizer = "es";
            else pluralizer = "";
            var embed = new EmbedBuilder
            {
                Title = "Ongoing Matches",
                Description = $"{Globals.matchCount} match{pluralizer} ongoing"
            };
            for (int i = 0; i < Globals.matchCount; i++)
            {
                embed.AddField(x =>
                {
                    x.Name = $"{Globals.matches[i].p1.username} vs {Globals.matches[i].p2.username}";
                });
            }

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("info")]
        [Summary("Gives info about user's current match")]
        [Alias("mine","about")]
        public async Task MatchInfoAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is requesting match info");
            for (int i = 0; i < Globals.matchCount; i++)
            {
                if(userInfo.Id == Globals.matches[i].p1.id)
                {
                    await Context.Channel.SendMessageAsync($"You are currently in a match against {Globals.matches[i].p2.username}");
                    return;
                }
                else if (userInfo.Id == Globals.matches[i].p2.id)
                {
                    await Context.Channel.SendMessageAsync($"You are currently in a match against {Globals.matches[i].p2.username}");
                    return;
                }
            }
            await Context.Channel.SendMessageAsync($"You are currently not in a match against anyone");
            Console.WriteLine($"{userInfo.Username} has requested bad info");
        }

        [Command("report")]
        [Summary("Allows user to report current match")]
        [Alias("score", "result")]
        public async Task MatchReportAsync([Remainder] [Summary("The winner, \"Y\" or \"N\"")] string winner)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool? isP1 = null;
            int i;
            for (i = 0; i < Globals.matchCount; i++)
            {
                if (userInfo.Id == Globals.matches[i].p1.id)
                {
                    isP1 = true;
                    break;
                }
                else if (userInfo.Id == Globals.matches[i].p2.id)
                {
                    isP1 = false;
                    break;
                }
            }
            if (isP1 == null)
            {
                await Context.Channel.SendMessageAsync($"You are currently not in a match against anyone");
                return;
            }

            if(winner == "Y" || winner == "y") { }
            else if (winner == "N" || winner == "n")
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

            var results = new Tuple<double, double>(0,0);
            double p1elo = 0, p2elo = 0;
            string query = $"SELECT elo FROM leaderboard WHERE id = {Globals.matches[i].p1.id};";
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
            query = $"SELECT elo FROM leaderboard WHERE id = {Globals.matches[i].p2.id};";
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
                x.Name = $"{Globals.matches[i].p1.username}: {Convert.ToInt32(results.Item1)} elo";
                x.Value = $"{Globals.matches[i].p1.username} now has {Convert.ToInt32(new1)} elo";
            });
            embed.AddField(x =>
            {
                x.Name = $"{Globals.matches[i].p2.username}: {Convert.ToInt32(results.Item2)} elo";
                x.Value = $"{Globals.matches[i].p2.username} now has {Convert.ToInt32(new2)} elo";
            });

            await Context.Channel.SendMessageAsync("", embed: embed);

            Console.WriteLine($"Giving {Globals.matches[i].p1.username} {results.Item1} elo, resulting in {new1}");
            query = $"UPDATE leaderboard SET elo = {new1} WHERE id = {Globals.matches[i].p1.id};";
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
            Console.WriteLine($"Giving {Globals.matches[i].p2.username} {results.Item2} elo, resulting in {new2}");
            Globals.conn.Open();
            query = $"UPDATE leaderboard SET elo = {new2} WHERE id = {Globals.matches[i].p2.id};";
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

            
            for (int j = i + 1; j < Globals.matchCount; j++)
            {
                Globals.matches[j - 1] = Globals.matches[j];
            }
            Console.WriteLine($"Match #{Globals.matchCount} has ended.");
            Globals.matchCount--;
        }

        private Tuple<double, double> EloConvert(double p1, double p2, bool isP1)
        {
            double expected1 = 1 / (1 + System.Math.Pow(10, ((p2 - p1) / 400)));
            double expected2 = 1 / (1 + System.Math.Pow(10, ((p1 - p2) / 400)));

            double change1 = 16 * (Convert.ToDouble(isP1) - expected1);
            double change2 = 16 * (Convert.ToDouble(!isP1) - expected2);

            var allChange = Tuple.Create(change1, change2);

            return allChange;
        }

    }

    [Group("leaderboard")]
    public class LeaderboardModule : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        [Summary("Join the leaderboard")]
        public async Task JoinDBAsync()
        {
            var userInfo = Context.User;
            string query = $"INSERT INTO leaderboard(id, username) VALUES({userInfo.Id}, '{userInfo.Username}');";
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

            await Context.Channel.SendMessageAsync($"You've been succesfully registered! You have 2500 elo.");
            Console.WriteLine($"{userInfo.Username} has been registered");
        }

        [Command("list")]
        [Summary("Lists the current status of the leaderboard")]
        [Alias("view","show")]
        public async Task ListLeaderboardAsync()
        {
            Console.WriteLine($"Leaderboard is being requested");
            var embed = new EmbedBuilder
            {
                Title = "Leaderboard"
            };
            
            int i = 1;
            string query = $"SELECT username, elo FROM leaderboard ORDER BY elo DESC;";
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
                        x.Value = $"{reader.GetDouble(1)} elo";
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
    }
}
