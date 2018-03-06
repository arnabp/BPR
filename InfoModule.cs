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
    public class HelperFunctions
    {
        public static async Task ExecuteSQLQueryAsync(string query)
        {
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

        public static string GetRoleRegion(ulong roleID)
        {
            if (roleID == 419355178680975370) return "NA";
            else if (roleID == 419355321061081088) return "NA";
            else if (roleID == 419355374529937408) return "EU";
            else if (roleID == 419355453550624768) return "EU";
            else return "";
        }
    }

    public class InfoModule : ModuleBase<SocketCommandContext>
    {
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

        [Command("GetUser")]
        [Summary("Gets username from ID")]
        public async Task GetUser([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            if (user != null) await Context.Channel.SendMessageAsync($"This user is {user.Username}");
            else await Context.Channel.SendMessageAsync($"This user does not exist");

        }

        [Command("ChangeName")]
        [Summary("Changes the bot's name")]
        public async Task ChangeName([Remainder] string newName)
        {
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = newName);
        }
    }

    [Group("queue1")]
    [Alias("q1")]
    public class Queue1Module : ModuleBase<SocketCommandContext>
    {

        [Command("join")]
        [Alias("j")]
        [Summary("Joins the 1v1 queue")]
        public async Task JoinAsync()
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            Console.WriteLine($"{userInfo.Username} is attempting to join 1v1 queue");

            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);
                
            bool isInQueue = false, isInMatch = false;
            int queueCount = 0, inLeaderboard = 0;

            string query = $"SELECT count(*) FROM leaderboard{region}1 WHERE id = {userInfo.Id};";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    inLeaderboard = reader.GetInt16(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (inLeaderboard == 0)
            {
                await Context.Channel.SendMessageAsync("You are not qualified to play this game mode.");
                return;
            }

            query = $"SELECT count(*) FROM queue{region}1;";
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

            if (queueCount > 0)
            {
                query = $"SELECT id FROM queue{region}1;";
                await Globals.conn.OpenAsync();
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
                    await Globals.conn.CloseAsync();
                    throw;
                }
                await Globals.conn.CloseAsync();
            }

            query = $"SELECT id1, id2 FROM matches{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == userInfo.Id || reader.GetUInt64(1) == userInfo.Id) isInMatch = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (isInQueue)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 1v1 queue has now refreshed their queue timer");

                query = $"UPDATE queue{region}1 SET time = {DateTime.Now.ToBinary()} WHERE id = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                Console.WriteLine($"{userInfo.Username} refreshed their queue timer");
            }
            else if (isInMatch)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 1v1 match tried to queue");
                Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
            }
            else
            {
                query = $"INSERT INTO queue{region}1(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                await Context.Channel.SendMessageAsync($"A player has been added to {region} 1v1 queue");

                if (queueCount == 1)
                {
                    if (region == "NA") await NewMatchNA(0, 1); // This should be modified when anti-smurfing mechanism is introduced
                    else if (region == "EU") await NewMatchEU(0, 1);
                    else Console.WriteLine("Wrong region detected, role error");

                    query = $"TRUNCATE TABLE queue{region}1;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
            }
        }

        [Command("leave")]
        [Alias("l")]
        [Summary("Leaves the 1v1 queue")]
        public async Task QueueLeaveAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave 1v1 queue");
            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);;

            bool isInQueue = false;
            int queueCount = 0;
            string query = $"SELECT count(*) FROM queue{region}1;";
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

            if (queueCount > 0)
            {
                query = $"SELECT id FROM queue{region}1;";
                await Globals.conn.OpenAsync();
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
                    await Globals.conn.CloseAsync();
                    throw;
                }
                await Globals.conn.CloseAsync();
            }

            if (isInQueue)
            {
                query = $"DELETE FROM queue{region}1 WHERE id = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Player not in {region} 1v1 queue tried to leave queue");
                Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
            }
            await Context.Message.DeleteAsync();
        }

        private async Task NewMatchNA(int p1, int p2)
        {
            long p1time = 0, p2time = 0;
            string p1name = "", p2name = "";
            ulong p1id = 0, p2id = 0;
            bool is1InQueue = false, is2InQueue = false;
            string query = $"SELECT time, username, id FROM queueNA1;";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            query = $"SELECT id FROM queueNA2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == p1id) is1InQueue = true;
                    if (reader.GetUInt64(0) == p2id) is2InQueue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            if (is1InQueue)
            {
                query = $"DELETE FROM queueNA2 WHERE id = {p1id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the NA 2v2 queue");
            }

            if (is2InQueue)
            {
                query = $"DELETE FROM queueNA2 WHERE id = {p2id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the NA 2v2 queue");
            }

            query = $"INSERT INTO matchesNA1(id1, id2, username1, username2, time, reverted) VALUES({p1id}, {p2id}, '{p1name}', '{p2name}', {DateTime.Now.ToBinary()}, 0);";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            int matchCount = 0;
            query = $"SELECT count(*) FROM matchesNA1;";
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

            await Context.Channel.SendMessageAsync($"New match has started between <@{p1id}> and <@{p2id}>");
            Console.WriteLine($"NA 1v1 Match #{matchCount} has started.");

            await Context.Channel.SendMessageAsync($"Please remember to add your room number with !match1 room 00000");
        }

        private async Task NewMatchEU(int p1, int p2)
        {
            long p1time = 0, p2time = 0;
            string p1name = "", p2name = "";
            ulong p1id = 0, p2id = 0;
            bool is1InQueue = false, is2InQueue = false;
            string query = $"SELECT time, username, id FROM queueEU1;";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            query = $"SELECT id FROM queueEU2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == p1id) is1InQueue = true;
                    if (reader.GetUInt64(0) == p2id) is2InQueue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            if (is1InQueue)
            {
                query = $"DELETE FROM queueEU2 WHERE id = {p1id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the EU 2v2 queue");
            }

            if (is2InQueue)
            {
                query = $"DELETE FROM queueEU2 WHERE id = {p2id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the EU 2v2 queue");
            }

            query = $"INSERT INTO matchesEU1(id1, id2, username1, username2, time, reverted) VALUES({p1id}, {p2id}, '{p1name}', '{p2name}', {DateTime.Now.ToBinary()}, 0);";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            int matchCount = 0;
            query = $"SELECT count(*) FROM matchesEU1;";
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

            await Context.Channel.SendMessageAsync($"New 1v1 match has started between <@{p1id}> and <@{p2id}>");
            Console.WriteLine($"EU 1v1 Match #{matchCount} has started.");

            await Context.Channel.SendMessageAsync($"Please remember to add your room number with !match1 room 00000");
        }
    }

    [Group("queue2")]
    [Alias("q2")]
    public class Queue2Module : ModuleBase<SocketCommandContext>
    {

        [Command("join")]
        [Alias("j")]
        [Summary("Joins the 2v2 queue")]
        public async Task JoinAsync()
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            Console.WriteLine($"{userInfo.Username} is attempting to join 2v2 queue");

            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);

            bool isInQueue = false, isInMatch = false;
            int queueCount = 0, inLeaderboard = 0;

            string query = $"SELECT count(*) FROM leaderboard{region}2 WHERE id = {userInfo.Id};";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    inLeaderboard = reader.GetInt16(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (inLeaderboard == 0)
            {
                await Context.Channel.SendMessageAsync("You are not qualified to play this game mode.");
                return;
            }

            query = $"SELECT count(*) FROM queue{region}2;";
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

            if (queueCount > 0)
            {
                query = $"SELECT id FROM queue{region}2;";
                await Globals.conn.OpenAsync();
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
                    await Globals.conn.CloseAsync();
                    throw;
                }
                await Globals.conn.CloseAsync();
            }

            query = $"SELECT id1, id2, id3, id4 FROM matches{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == userInfo.Id
                        || reader.GetUInt64(1) == userInfo.Id
                        || reader.GetUInt64(2) == userInfo.Id
                        || reader.GetUInt64(3) == userInfo.Id) isInMatch = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (isInQueue)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 2v2 queue has now refreshed their queue timer");

                query = $"UPDATE queue{region}2 SET time = {DateTime.Now.ToBinary()} WHERE id = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                Console.WriteLine($"{userInfo.Username} refreshed their queue timer");
            }
            else if (isInMatch)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 2v2 match tried to queue");
                Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
            }
            else
            {
                query = $"INSERT INTO queue{region}2(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                await Context.Channel.SendMessageAsync($"A player has been added to {region} 2v2 queue");

                if (queueCount == 3)
                {
                    if (region == "NA") await NewMatchNA(0, 1, 2, 3); // This should be modified when anti-smurfing mechanism is introduced
                    else if (region == "EU") await NewMatchEU(0, 1, 2, 3);
                    else Console.WriteLine("Wrong region detected, role error");
                    query = $"TRUNCATE TABLE queue{region}2;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
            }
        }

        [Command("leave")]
        [Alias("l")]
        [Summary("Leaves the 2v2 queue")]
        public async Task QueueLeaveAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave 2v2 queue");

            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);
            bool isInQueue = false;
            int queueCount = 0;
            string query = $"SELECT count(*) FROM queue{region}2;";
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

            if (queueCount > 0)
            {
                query = $"SELECT id FROM queue{region}2;";
                await Globals.conn.OpenAsync();
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
                    await Globals.conn.CloseAsync();
                    throw;
                }
                await Globals.conn.CloseAsync();
            }

            if (isInQueue)
            {
                query = $"DELETE FROM queue{region}2 WHERE id = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 2v2 queue");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Player not in {region} 2v2 queue tried to leave queue");
                Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
            }
            
            await Context.Message.DeleteAsync();
        }

        private async Task NewMatchNA(int p1, int p2, int p3, int p4)
        {
            int[] playerPositions = new int[4] { p1, p2, p3, p4 };
            Random rnd = new Random();
            int[] newplayerPositions = playerPositions.OrderBy(x => rnd.Next()).ToArray();
            p1 = newplayerPositions[0];
            p2 = newplayerPositions[1];
            p3 = newplayerPositions[2];
            p4 = newplayerPositions[3];

            long p1time = 0, p2time = 0, p3time = 0, p4time = 0;
            string p1name = "", p2name = "", p3name = "", p4name = "";
            ulong p1id = 0, p2id = 0, p3id = 0, p4id = 0;
            bool is1InQueue = false, is2InQueue = false, is3InQueue = false, is4InQueue = false;
            string query = $"SELECT time, username, id FROM queueNA2;";
            await Globals.conn.OpenAsync();
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
                    if (i == p3)
                    {
                        p3time = reader.GetInt64(0);
                        p3name = reader.GetString(1);
                        p3id = reader.GetUInt64(2);
                    }
                    if (i == p4)
                    {
                        p4time = reader.GetInt64(0);
                        p4name = reader.GetString(1);
                        p4id = reader.GetUInt64(2);
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

            query = $"SELECT id FROM queueNA1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == p1id) is1InQueue = true;
                    if (reader.GetUInt64(0) == p2id) is2InQueue = true;
                    if (reader.GetUInt64(0) == p3id) is3InQueue = true;
                    if (reader.GetUInt64(0) == p4id) is4InQueue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            if (is1InQueue)
            {
                query = $"DELETE FROM queueNA1 WHERE id = {p1id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the NA 1v1 queue");
            }
            if (is2InQueue)
            {
                query = $"DELETE FROM queueNA1 WHERE id = {p2id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the NA 1v1 queue");
            }
            if (is3InQueue)
            {
                query = $"DELETE FROM queueNA1 WHERE id = {p3id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the NA 1v1 queue");
            }
            if (is4InQueue)
            {
                query = $"DELETE FROM queueNA1 WHERE id = {p4id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the NA 1v1 queue");
            }

            query = $"INSERT INTO matchesNA2(id1, id2, id3, id4, username1, username2, username3, username4, time, reverted) " +
                $"VALUES({p1id}, {p2id}, {p3id}, {p4id}, '{p1name}', '{p2name}', '{p3name}', '{p4name}', {DateTime.Now.ToBinary()}, 0);";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            int matchCount = 0;
            query = $"SELECT count(*) FROM matchesNA2;";
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

            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;

            query = $"SELECT id, elo FROM leaderboardNA2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if(reader.GetUInt64(0) == p1id)
                    {
                        p1elo = reader.GetDouble(1);
                    }
                    else if(reader.GetUInt64(0) == p2id)
                    {
                        p2elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == p3id)
                    {
                        p3elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == p4id)
                    {
                        p4elo = reader.GetDouble(1);
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

            if(p1elo + p2elo > p3elo + p4elo)
            {
                await Context.Channel.SendMessageAsync($"New match has started between <@{p1id}>, <@{p2id}> and <@{p3id}>, <@{p4id}>");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"New match has started between <@{p3id}>, <@{p4id}> and <@{p1id}>, <@{p2id}>");
            }
            
            Console.WriteLine($"NA 2v2 Match #{matchCount} has started.");

            await Context.Channel.SendMessageAsync($"Please remember to add your room number with !match2 room 00000");
        }

        private async Task NewMatchEU(int p1, int p2, int p3, int p4)
        {
            int[] playerPositions = new int[4] { p1, p2, p3, p4 };
            Random rnd = new Random();
            int[] newplayerPositions = playerPositions.OrderBy(x => rnd.Next()).ToArray();
            p1 = newplayerPositions[0];
            p2 = newplayerPositions[1];
            p3 = newplayerPositions[2];
            p4 = newplayerPositions[3];

            long p1time = 0, p2time = 0, p3time = 0, p4time = 0;
            string p1name = "", p2name = "", p3name = "", p4name = "";
            ulong p1id = 0, p2id = 0, p3id = 0, p4id = 0;
            bool is1InQueue = false, is2InQueue = false, is3InQueue = false, is4InQueue = false;
            string query = $"SELECT time, username, id FROM queueEU2;";
            await Globals.conn.OpenAsync();
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
                    if (i == p3)
                    {
                        p3time = reader.GetInt64(0);
                        p3name = reader.GetString(1);
                        p3id = reader.GetUInt64(2);
                    }
                    if (i == p4)
                    {
                        p4time = reader.GetInt64(0);
                        p4name = reader.GetString(1);
                        p4id = reader.GetUInt64(2);
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

            query = $"SELECT id FROM queueEU1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == p1id) is1InQueue = true;
                    if (reader.GetUInt64(0) == p2id) is2InQueue = true;
                    if (reader.GetUInt64(0) == p3id) is3InQueue = true;
                    if (reader.GetUInt64(0) == p4id) is4InQueue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            if (is1InQueue)
            {
                query = $"DELETE FROM queueEU1 WHERE id = {p1id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the EU 1v1 queue");
            }
            if (is2InQueue)
            {
                query = $"DELETE FROM queueEU1 WHERE id = {p2id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the EU 1v1 queue");
            }
            if (is3InQueue)
            {
                query = $"DELETE FROM queueEU1 WHERE id = {p3id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the EU 1v1 queue");
            }
            if (is4InQueue)
            {
                query = $"DELETE FROM queueEU1 WHERE id = {p4id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the EU 1v1 queue");
            }

            query = $"INSERT INTO matchesEU2(id1, id2, id3, id4, username1, username2, username3, username4, time, reverted) " +
                $"VALUES({p1id}, {p2id}, {p3id}, {p4id}, '{p1name}', '{p2name}', '{p3name}', '{p4name}', {DateTime.Now.ToBinary()}, 0);";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            int matchCount = 0;
            query = $"SELECT count(*) FROM matchesEU2;";
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

            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;

            query = $"SELECT id, elo FROM leaderboardEU2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == p1id)
                    {
                        p1elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == p2id)
                    {
                        p2elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == p3id)
                    {
                        p3elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == p4id)
                    {
                        p4elo = reader.GetDouble(1);
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

            if (p1elo + p2elo > p3elo + p4elo)
            {
                await Context.Channel.SendMessageAsync($"New match has started between <@{p1id}>, <@{p2id}> and <@{p3id}>, <@{p4id}>");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"New match has started between <@{p3id}>, <@{p4id}> and <@{p1id}>, <@{p2id}>");
            }

            Console.WriteLine($"EU 2v2 Match #{matchCount} has started.");

            await Context.Channel.SendMessageAsync($"Please remember to add your room number with !match2 room 00000");
        }
    }

    [Group("match1")]
    [Alias("m1")]
    public class Match1Module : ModuleBase<SocketCommandContext>
    {
        [Command("report")]
        [Summary("Allows user to report current match")]
        [Alias("score", "result", "r")]
        public async Task MatchReportAsync([Remainder] [Summary("The winner, \"Y\" or \"N\"")] string winner)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool? isP1 = null;
            ulong p1ID = 0, p2ID = 0, reverter = 0;
            string p1Username = "";
            string p2Username = "";
            int thisMatchNum = 1;

            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);
            string query = $"SELECT id1, id2, username1, username2, reverted FROM matches{region}1;";
            await Globals.conn.OpenAsync();
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
                        reverter = reader.GetUInt64(4);
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(1))
                    {
                        isP1 = false;
                        p1ID = reader.GetUInt64(0);
                        p2ID = reader.GetUInt64(1);
                        p1Username = reader.GetString(2);
                        p2Username = reader.GetString(3);
                        reverter = reader.GetUInt64(4);
                        break;
                    }
                    thisMatchNum++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
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
            query = $"SELECT elo FROM leaderboard{region}1 WHERE id = {p1ID};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"SELECT elo FROM leaderboard{region}1 WHERE id = {p2ID};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            query = $"DELETE FROM matchesHistory1 WHERE id1 = {p1ID} OR id2 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory1 WHERE id1 = {p2ID} OR id2 = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"INSERT INTO matchesHistory1(id1, id2, oldElo1, oldElo2, isP1, region, username1, username2, reporter) " +
                $"VALUES({p1ID}, {p2ID}, {p1elo}, {p2elo}, {isP1}, '{region}', '{p1Username}', '{p2Username}', {userInfo.Id});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            results = EloConvert(p1elo, p2elo, (bool)isP1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;

            Color embedColor = Color.Blue;
            if (region == "NA") embedColor = Color.Blue;
            else embedColor = Color.Green;

            var embed = new EmbedBuilder
            {
                Title = "Match Result",
                Color = embedColor
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

            string p1ResultString = "wins";
            string p2ResultString = "loss";
            if (!(bool)isP1)
            {
                p1ResultString = "loss";
                p2ResultString = "wins";
            }

            Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
            query = $"UPDATE leaderboard{region}1 SET elo = {new1} WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}1 SET {p1ResultString} = {p1ResultString} + 1 WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            Console.WriteLine($"Giving {p2Username} {results.Item2} elo, resulting in {new2}");
            query = $"UPDATE leaderboard{region}1 SET elo = {new2} WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}1 SET {p2ResultString} = {p2ResultString} + 1 WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if(reverter != 0)
            {
                query = $"UPDATE leaderboard{region}1 SET elo = elo - 20 WHERE id = {reverter};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
            }
                
            query = $"DELETE FROM matches{region}1 WHERE id1 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            Console.WriteLine($"{region} 1v1 Match #{thisMatchNum} has ended.");
        }

        [Command("room")]
        [Summary("Adds room number to match info")]
        public async Task AddRoomNumberAsync([Remainder] int room)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");
            int thisMatchNum = 1, idnum = 0;
            bool isInMatch = false;

            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);
            string query = $"SELECT id1, id2, username1, username2 FROM matches{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (userInfo.Id == reader.GetUInt64(0))
                    {
                        isInMatch = true;
                        idnum = 1;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(1))
                    {
                        isInMatch = true;
                        idnum = 2;
                        break;
                    }
                    thisMatchNum++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (isInMatch)
            {
                query = $"UPDATE matches{region}1 SET room = {room} WHERE id{idnum} = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                await Context.Channel.SendMessageAsync($"{region} 1v1 Match #{thisMatchNum} is in room #{room}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"You are not currently in an {region} 1v1 match.");
            }
            await Context.Message.DeleteAsync();
        }

        [Command("revert")]
        [Summary("Reverts the last reported match for a player")]
        public async Task RevertMatchAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");
            int thisMatchNum = 1, revertRequests = 0, thisPlayerNum = 0;
            bool hasAlreadyReverted = false;

            string query = $"SELECT id1, id2, revert1, revert2 FROM matchesHistory1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (userInfo.Id == reader.GetUInt64(0))
                    {
                        revertRequests += reader.GetInt16(2);
                        revertRequests += reader.GetInt16(3);
                        if (reader.GetInt16(2) == 1) hasAlreadyReverted = true;
                        thisPlayerNum = 1;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(1))
                    {
                        revertRequests += reader.GetInt16(2);
                        revertRequests += reader.GetInt16(3);
                        if (reader.GetInt16(3) == 1) hasAlreadyReverted = true;
                        thisPlayerNum = 2;
                        break;
                    }
                    thisMatchNum++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (hasAlreadyReverted)
            {
                await Context.Channel.SendMessageAsync("A player tried to revert the match twice.");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{revertRequests + 1}/2 players have requested the last 1v1 match to be reverted");

                if (revertRequests < 1)
                {
                    query = $"UPDATE matchesHistory1 SET revert{thisPlayerNum} = 1 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
                else
                {
                    ulong p1ID = 0, p2ID = 0, reporter = 0;
                    double p1elo = 0, p2elo = 0;
                    int isP1 = 0;
                    string region = "", p1Username = "", p2Username = "";

                    query = $"SELECT id1, id2, oldElo1, oldElo2, isP1, region, username1, username2, reporter FROM matchesHistory1 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await Globals.conn.OpenAsync();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            p1ID = reader.GetUInt64(0);
                            p2ID = reader.GetUInt64(1);
                            p1elo = reader.GetDouble(2);
                            p2elo = reader.GetDouble(3);
                            isP1 = reader.GetInt16(4);
                            region = reader.GetString(5);
                            p1Username = reader.GetString(6);
                            p2Username = reader.GetString(7);
                            reporter = reader.GetUInt64(8);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        await Globals.conn.CloseAsync();
                        throw;
                    }
                    await Globals.conn.CloseAsync();

                    query = $"UPDATE leaderboard{region}1 SET elo = {p1elo} WHERE id = {p1ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}1 SET elo = {p2elo} WHERE id = {p2ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"INSERT INTO matches{region}1(id1, id2, username1, username2, time, reverted) VALUES({p1ID}, {p2ID}, '{p1Username}', '{p2Username}', {DateTime.Now.ToBinary()}, {reporter});";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    string p1ResultString = "wins";
                    string p2ResultString = "loss";
                    if (isP1 == 0)
                    {
                        p1ResultString = "loss";
                        p2ResultString = "wins";
                    }

                    query = $"UPDATE leaderboard{region}1 SET {p1ResultString} = {p1ResultString} - 1 WHERE id = {p1ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}1 SET {p2ResultString} = {p2ResultString} - 1 WHERE id = {p2ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    await Context.Channel.SendMessageAsync("The last 1v1 match has been reverted. Please report the match correctly now.");
                }
            }
        }

        [Command("clearhistory")]
        [Summary("Truncates the matchesHistory1 table")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ClearHistoryAsync()
        {
            string query = $"TRUNCATE TABLE matchesHistory1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("Match history cleared.");
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
            string query = $"SELECT elo FROM leaderboardNA1 WHERE id = {p1ID};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"SELECT elo FROM leaderboardNA1 WHERE id = {p2ID};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            results = EloConvert(p1elo, p2elo, isP1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;

            query = $"UPDATE leaderboardNA1 SET elo = {new1} WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            
            query = $"UPDATE leaderboardNA1 SET elo = {new2} WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

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
            string query = $"SELECT elo FROM leaderboardEU1 WHERE id = {p1ID};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"SELECT elo FROM leaderboardEU1 WHERE id = {p2ID};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            results = EloConvert(p1elo, p2elo, isP1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;

            query = $"UPDATE leaderboardEU1 SET elo = {new1} WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            
            query = $"UPDATE leaderboardEU1 SET elo = {new2} WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync($"Match hard updated");
        }

        private Tuple<double, double> EloConvert(double p1, double p2, bool isP1)
        {
            double expected1 = 1 / (1 + System.Math.Pow(10, ((p2 - p1) / 400)));
            double expected2 = 1 / (1 + System.Math.Pow(10, ((p1 - p2) / 400)));

            double change1 = (32 * (Convert.ToDouble(isP1) - expected1));
            double change2 = (32 * (Convert.ToDouble(!isP1) - expected2));

            var allChange = Tuple.Create(change1, change2);

            return allChange;
        }
        
    }

    [Group("match2")]
    [Alias("m2")]
    public class Match2Module : ModuleBase<SocketCommandContext>
    {
        [Command("report")]
        [Summary("Allows user to report current match")]
        [Alias("score", "result", "r")]
        public async Task MatchReportAsync([Remainder] [Summary("The winner, \"Y\" or \"N\"")] string winner)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool? isT1 = null;
            ulong p1ID = 0, p2ID = 0, p3ID = 0, p4ID = 0, reverter = 0;
            string p1Username = "", p2Username = "", p3Username = "", p4Username = "";
            int thisMatchNum = 1;

            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);
            string query = $"SELECT id1, id2, id3, id4, username1, username2, username3, username4, reverted FROM matches{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (userInfo.Id == reader.GetUInt64(0) || userInfo.Id == reader.GetUInt64(1))
                    {
                        isT1 = true;
                        p1ID = reader.GetUInt64(0);
                        p2ID = reader.GetUInt64(1);
                        p3ID = reader.GetUInt64(2);
                        p4ID = reader.GetUInt64(3);
                        p1Username = reader.GetString(4);
                        p2Username = reader.GetString(5);
                        p3Username = reader.GetString(6);
                        p4Username = reader.GetString(7);
                        reverter = reader.GetUInt64(8);
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(2) || userInfo.Id == reader.GetUInt64(3))
                    {
                        isT1 = false;
                        p1ID = reader.GetUInt64(0);
                        p2ID = reader.GetUInt64(1);
                        p3ID = reader.GetUInt64(2);
                        p4ID = reader.GetUInt64(3);
                        p1Username = reader.GetString(4);
                        p2Username = reader.GetString(5);
                        p3Username = reader.GetString(6);
                        p4Username = reader.GetString(7);
                        reverter = reader.GetUInt64(8);
                        break;
                    }
                    thisMatchNum++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            if (isT1 == null)
            {
                await Context.Channel.SendMessageAsync($"You are currently not in a match against anyone");
                return;
            }

            if (winner == "W" || winner == "w" || winner == "Y" || winner == "y") { }
            else if (winner == "L" || winner == "l" || winner == "N" || winner == "n")
            {
                isT1 = !isT1;
            }
            else
            {
                Console.WriteLine($"{userInfo.Username} entered the wrong result type");
                await Context.Channel.SendMessageAsync("Invalid results entered");
                return;
            }

            Console.WriteLine($"isT1: {isT1}");

            var results = new Tuple<double, double, double, double>(0, 0, 0, 0);
            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;
            query = $"SELECT id, elo FROM leaderboard{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetDouble(0) == p1ID) p1elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p2ID) p2elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p3ID) p3elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p4ID) p4elo = reader.GetDouble(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p1ID} OR id2 = {p1ID} OR id3 = {p1ID} OR id4 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p2ID} OR id2 = {p2ID} OR id3 = {p2ID} OR id4 = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p3ID} OR id2 = {p3ID} OR id3 = {p3ID} OR id4 = {p3ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p4ID} OR id2 = {p4ID} OR id3 = {p4ID} OR id4 = {p4ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"INSERT INTO matchesHistory2(id1, id2, id3, id4, oldElo1, oldElo2, oldElo3, oldElo4, isT1, region, username1, username2, username3, username4, reporter) " +
                $"VALUES({p1ID}, {p2ID}, {p3ID}, {p4ID}, {p1elo}, {p2elo}, {p3elo}, {p4elo}, {isT1}, '{region}', '{p1Username}', '{p2Username}', '{p3Username}', '{p4Username}', {userInfo.Id});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            results = EloConvert(p1elo, p2elo, p3elo, p4elo, (bool)isT1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;
            double new3 = p3elo + results.Item3;
            double new4 = p4elo + results.Item4;

            Color embedColor = Color.Blue;
            if (region == "NA") embedColor = Color.Blue;
            else embedColor = Color.Green;

            var embed = new EmbedBuilder
            {
                Title = "Match Result",
                Color = embedColor
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
            embed.AddField(x =>
            {
                x.Name = $"{p3Username}: {Convert.ToInt32(results.Item3)} elo";
                x.Value = $"{p3Username} now has {Convert.ToInt32(new3)} elo";
            });
            embed.AddField(x =>
            {
                x.Name = $"{p4Username}: {Convert.ToInt32(results.Item4)} elo";
                x.Value = $"{p4Username} now has {Convert.ToInt32(new4)} elo";
            });

            await Context.Channel.SendMessageAsync("", embed: embed);

            string t1ResultString = "wins";
            string t2ResultString = "loss";
            if (!(bool)isT1)
            {
                t1ResultString = "loss";
                t2ResultString = "wins";
            }

            Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
            query = $"UPDATE leaderboard{region}2 SET elo = {new1} WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}2 SET {t1ResultString} = {t1ResultString} + 1 WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            Console.WriteLine($"Giving {p2Username} {results.Item2} elo, resulting in {new2}");
            query = $"UPDATE leaderboard{region}2 SET elo = {new2} WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}2 SET {t1ResultString} = {t1ResultString} + 1 WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            Console.WriteLine($"Giving {p3Username} {results.Item3} elo, resulting in {new3}");
            query = $"UPDATE leaderboard{region}2 SET elo = {new3} WHERE id = {p3ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}2 SET {t2ResultString} = {t2ResultString} + 1 WHERE id = {p3ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            Console.WriteLine($"Giving {p4Username} {results.Item4} elo, resulting in {new4}");
            query = $"UPDATE leaderboard{region}2 SET elo = {new4} WHERE id = {p4ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}2 SET {t2ResultString} = {t2ResultString} + 1 WHERE id = {p4ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if(reverter != 0)
            {
                query = $"UPDATE leaderboard{region}2 SET elo = elo - 20 WHERE id = {reverter};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
            }
                
            query = $"DELETE FROM matches{region}2 WHERE id1 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            Console.WriteLine($"Match #{thisMatchNum} has ended.");
        }

        [Command("revert")]
        [Summary("Reverts the last reported match for a player")]
        public async Task RevertMatchAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");
            int thisMatchNum = 1, revertRequests = 0, thisPlayerNum = 0;
            bool hasAlreadyReverted = false;

            string query = $"SELECT id1, id2, id3, id4, revert1, revert2, revert3, revert4 FROM matchesHistory2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (userInfo.Id == reader.GetUInt64(0))
                    {
                        revertRequests += reader.GetInt16(4);
                        revertRequests += reader.GetInt16(5);
                        revertRequests += reader.GetInt16(6);
                        revertRequests += reader.GetInt16(7);
                        if (reader.GetInt16(4) == 1) hasAlreadyReverted = true;
                        thisPlayerNum = 1;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(1))
                    {
                        revertRequests += reader.GetInt16(4);
                        revertRequests += reader.GetInt16(5);
                        revertRequests += reader.GetInt16(6);
                        revertRequests += reader.GetInt16(7);
                        if (reader.GetInt16(5) == 1) hasAlreadyReverted = true;
                        thisPlayerNum = 2;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(2))
                    {
                        revertRequests += reader.GetInt16(4);
                        revertRequests += reader.GetInt16(5);
                        revertRequests += reader.GetInt16(6);
                        revertRequests += reader.GetInt16(7);
                        if (reader.GetInt16(6) == 1) hasAlreadyReverted = true;
                        thisPlayerNum = 3;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(3))
                    {
                        revertRequests += reader.GetInt16(4);
                        revertRequests += reader.GetInt16(5);
                        revertRequests += reader.GetInt16(6);
                        revertRequests += reader.GetInt16(7);
                        if (reader.GetInt16(7) == 1) hasAlreadyReverted = true;
                        thisPlayerNum = 4;
                        break;
                    }
                    thisMatchNum++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (hasAlreadyReverted)
            {
                await Context.Channel.SendMessageAsync("A player tried to revert the match twice.");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{revertRequests + 1}/3 players have requested the last 1v1 match to be reverted");

                if (revertRequests < 2)
                {
                    query = $"UPDATE matchesHistory2 SET revert{thisPlayerNum} = 1 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
                else
                {
                    ulong p1ID = 0, p2ID = 0, p3ID = 0, p4ID = 0;
                    double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0, reporter = 0;
                    int isT1 = 0;
                    string region = "", p1Username = "", p2Username = "", p3Username = "", p4Username = "";

                    query = $"SELECT id1, id2, id3, id4, oldElo1, oldElo2, oldElo3, oldElo4, isT1, region, username1, username2, username3, username4, reporter " +
                        $"FROM matchesHistory2 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await Globals.conn.OpenAsync();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            p1ID = reader.GetUInt64(0);
                            p2ID = reader.GetUInt64(1);
                            p3ID = reader.GetUInt64(2);
                            p4ID = reader.GetUInt64(3);
                            p1elo = reader.GetDouble(4);
                            p2elo = reader.GetDouble(5);
                            p3elo = reader.GetDouble(6);
                            p4elo = reader.GetDouble(7);
                            isT1 = reader.GetInt16(8);
                            region = reader.GetString(9);
                            p1Username = reader.GetString(10);
                            p2Username = reader.GetString(11);
                            p3Username = reader.GetString(12);
                            p4Username = reader.GetString(13);
                            reporter = reader.GetUInt64(14);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        await Globals.conn.CloseAsync();
                        throw;
                    }
                    await Globals.conn.CloseAsync();

                    query = $"UPDATE leaderboard{region}2 SET elo = {p1elo} WHERE id = {p1ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}2 SET elo = {p2elo} WHERE id = {p2ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}2 SET elo = {p3elo} WHERE id = {p3ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}2 SET elo = {p4elo} WHERE id = {p4ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"INSERT INTO matches{region}2(id1, id2, id3, id4, username1, username2, username3, username4, time, reverted) " +
                        $"VALUES({p1ID}, {p2ID}, {p3ID}, {p4ID}, '{p1Username}', '{p2Username}', '{p3Username}', '{p4Username}', {DateTime.Now.ToBinary()}, {reporter});";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    string t1ResultString = "wins";
                    string t2ResultString = "loss";
                    if (isT1 == 0)
                    {
                        t1ResultString = "loss";
                        t2ResultString = "wins";
                    }

                    query = $"UPDATE leaderboard{region}2 SET {t1ResultString} = {t1ResultString} - 1 WHERE id = {p1ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}2 SET {t1ResultString} = {t1ResultString} - 1 WHERE id = {p2ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}2 SET {t2ResultString} = {t2ResultString} - 1 WHERE id = {p3ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}2 SET {t2ResultString} = {t2ResultString} - 1 WHERE id = {p4ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    await Context.Channel.SendMessageAsync("The last 2v2 match has been reverted. Please report the match correctly now.");
                }
            }
        }

        [Command("clearhistory")]
        [Summary("Truncates the matchesHistory2 table")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ClearHistoryAsync()
        {
            string query = $"TRUNCATE TABLE matchesHistory2;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("Match history cleared.");
        }

        [Command("room")]
        [Summary("Adds room number to match info")]
        public async Task AddRoomNumberAsync([Remainder] int room)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");
            int thisMatchNum = 1, idnum = 0;
            bool isInMatch = false;

            string region = HelperFunctions.GetRoleRegion(Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id);
            string query = $"SELECT id1, id2, id3, id4 username1, username2 FROM matches{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (userInfo.Id == reader.GetUInt64(0))
                    {
                        isInMatch = true;
                        idnum = 1;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(1))
                    {
                        isInMatch = true;
                        idnum = 2;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(2))
                    {
                        isInMatch = true;
                        idnum = 3;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(3))
                    {
                        isInMatch = true;
                        idnum = 4;
                        break;
                    }
                    thisMatchNum++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            if (isInMatch)
            {
                query = $"UPDATE matches{region}2 SET room = {room} WHERE id{idnum} = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                await Context.Channel.SendMessageAsync($"{region} 2v2 Match #{thisMatchNum} is in room #{room}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"You are not currently in an {region} 2v2 match.");
            }
            await Context.Message.DeleteAsync();
        }

        [Command("superNA")]
        [Summary("Allows admin to report any match")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminMatchReportNAAsync(ulong p1ID, ulong p2ID, ulong p3ID, ulong p4ID)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool isT1 = true;

            var results = new Tuple<double, double, double, double>(0, 0, 0, 0);
            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;
            string query = $"SELECT id, elo FROM leaderboardNA2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetDouble(0) == p1ID) p1elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p2ID) p2elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p3ID) p3elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p4ID) p4elo = reader.GetDouble(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            results = EloConvert(p1elo, p2elo, p3elo, p4elo, isT1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;
            double new3 = p3elo + results.Item3;
            double new4 = p4elo + results.Item4;

            query = $"UPDATE leaderboardNA2 SET elo = {new1} WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            
            query = $"UPDATE leaderboardNA2 SET elo = {new2} WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboardNA2 SET elo = {new3} WHERE id = {p3ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            
            query = $"UPDATE leaderboardNA2 SET elo = {new4} WHERE id = {p4ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync($"Match hard updated");
        }

        [Command("superEU")]
        [Summary("Allows admin to report any match")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminMatchReportEUAsync(ulong p1ID, ulong p2ID, ulong p3ID, ulong p4ID)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool isT1 = true;

            var results = new Tuple<double, double, double, double>(0, 0, 0, 0);
            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;
            string query = $"SELECT id, elo FROM leaderboardEU2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetDouble(0) == p1ID) p1elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p2ID) p2elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p3ID) p3elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p4ID) p4elo = reader.GetDouble(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            results = EloConvert(p1elo, p2elo, p3elo, p4elo, isT1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;
            double new3 = p3elo + results.Item3;
            double new4 = p4elo + results.Item4;

            query = $"UPDATE leaderboardEU2 SET elo = {new1} WHERE id = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            
            query = $"UPDATE leaderboardEU2 SET elo = {new2} WHERE id = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboardEU2 SET elo = {new3} WHERE id = {p3ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            
            query = $"UPDATE leaderboardEU2 SET elo = {new4} WHERE id = {p4ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync($"Match hard updated");
        }

        private Tuple<double, double, double, double> EloConvert(double p1, double p2, double p3, double p4, bool isT1)
        {
            double t1avg = (p1 + p2) / 2;
            double t2avg = (p3 + p4) / 2;

            p1 = (p1 + t1avg) / 2;
            p2 = (p2 + t1avg) / 2;
            p3 = (p3 + t2avg) / 2;
            p4 = (p4 + t2avg) / 2;

            double expected1 = 1 / (1 + System.Math.Pow(10, ((t2avg - p1) / 400)));
            double expected2 = 1 / (1 + System.Math.Pow(10, ((t2avg - p2) / 400)));
            double expected3 = 1 / (1 + System.Math.Pow(10, ((t1avg - p3) / 400)));
            double expected4 = 1 / (1 + System.Math.Pow(10, ((t1avg - p4) / 400)));

            double change1 = (32 * (Convert.ToDouble(isT1) - expected1));
            double change2 = (32 * (Convert.ToDouble(isT1) - expected2));
            double change3 = (32 * (Convert.ToDouble(!isT1) - expected3));
            double change4 = (32 * (Convert.ToDouble(!isT1) - expected4));

            var allChange = Tuple.Create(change1, change2, change3, change4);

            return allChange;
        }
    }

    [Group("leaderboardNA1")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardNA1Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardNA1(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.Now.ToBinary()});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(419355178680975370));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the NA 1v1 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardNA1 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"DELETE FROM leaderboardNA1 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(419355178680975370));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the 1v1 leaderboards");
        }

        [Command("wl")]
        [Summary("Sets the win/loss ratio of a player")]
        public async Task SetWinLossAsync(ulong id, int wins, int losses)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardNA1 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"UPDATE leaderboardNA1 SET wins1 = {wins}, loss1 = {losses} WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            await Context.Channel.SendMessageAsync($"{username} win/loss in 1v1 has been updated");
        }
        
        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardNA1 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderbaordNA1 SET decaytimer = {DateTime.Now.ToBinary()};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for NA 1v1 leaderboard");
        }

    }

    [Group("leaderboardNA2")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardNA2Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardNA2(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.Now.ToBinary()});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(419355321061081088));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the NA 2v2 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardNA2 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"DELETE FROM leaderboardNA2 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(419355321061081088));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the NA 2v2 leaderboards");
        }

        [Command("wl")]
        [Summary("Sets the win/loss ratio of a player")]
        public async Task SetWinLossAsync(ulong id, int wins, int losses)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardNA2 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"UPDATE leaderboardNA2 SET wins2 = {wins}, loss2 = {losses} WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            await Context.Channel.SendMessageAsync($"{username} win/loss in 2v2 has been updated");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardNA2 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderbaordNA2 SET decaytimer = {DateTime.Now.ToBinary()};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for NA 2v2 leaderboard");
        }
    }

    [Group("leaderboardEU1")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardEU1Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardEU1(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.Now.ToBinary()});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(419355374529937408));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the EU 1v1 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();
            string username = "";
            string query = $"SELECT username FROM leaderboardEU1 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"DELETE FROM leaderboardEU1 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(419355374529937408));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the EU 1v1 leaderboards");
        }

        [Command("wl")]
        [Summary("Sets the win/loss ratio of a player")]
        public async Task SetWinLossAsync(ulong id, int wins, int losses, [Remainder] string gamemode)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardEU1 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"UPDATE leaderboardEU1 SET wins1 = {wins}, loss1 = {losses} WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            await Context.Channel.SendMessageAsync($"{username} win/loss in 1v1 has been updated");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardEU1 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderbaordEU1 SET decaytimer = {DateTime.Now.ToBinary()};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for EU 1v1 leaderboard");
        }
    }

    [Group("leaderboardEU2")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardEU2Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardEU2(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.Now.ToBinary()});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(419355453550624768));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the EU 2v2 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();
            string username = "";
            string query = $"SELECT username FROM leaderboardEU2 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"DELETE FROM leaderboardEU2 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(419355453550624768));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the EU 2v2 leaderboards");
        }

        [Command("wl")]
        [Summary("Sets the win/loss ratio of a player")]
        public async Task SetWinLossAsync(ulong id, int wins, int losses, [Remainder] string gamemode)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardEU2 WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();
            query = $"UPDATE leaderboardEU2 SET wins2 = {wins}, loss2 = {losses} WHERE id = {id};";
            await Globals.conn.OpenAsync();
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
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            await Context.Channel.SendMessageAsync($"{username} win/loss in 2v2 has been updated");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardEU2 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderbaordEU2 SET decaytimer = {DateTime.Now.ToBinary()};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for EU 2v2 leaderboard");
        }
    }
}
