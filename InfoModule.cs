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

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                bool isInQueue = false, isInMatch = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueNA1;";
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
                    query = $"SELECT id FROM queueNA1;";
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

                query = $"SELECT id1, id2 FROM matchesNA1;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (isInQueue)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 1v1 queue has now refreshed their queue timer");

                    query = $"UPDATE queueNA1 SET time = {DateTime.Now.ToBinary()} WHERE id = {userInfo.Id};";
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
                    Console.WriteLine($"{userInfo.Username} refreshed their queue timer");
                }
                else if (isInMatch)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 1v1 match tried to queue");
                    Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
                }
                else
                {
                    query = $"INSERT INTO queueNA1(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
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

                    await Context.Channel.SendMessageAsync($"A player has been added to NA 1v1 queue");

                    if (queueCount == 1)
                    {
                        await NewMatchNA(0, 1); // This should be modified when anti-smurfing mechanism is introduced
                        Globals.conn.Open();
                        query = $"TRUNCATE TABLE queueNA1;";
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
                    }
                }
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                bool isInQueue = false, isInMatch = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueEU1;";
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
                    query = $"SELECT id FROM queueEU1;";
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

                query = $"SELECT id1, id2 FROM matchesEU1;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (isInQueue)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 1v1 queue has now refreshed their queue timer");
                    
                    query = $"UPDATE queueEU1 SET time = {DateTime.Now.ToBinary()} WHERE id = {userInfo.Id};";
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
                    Console.WriteLine($"{userInfo.Username} refreshed their queue timer");
                }
                else if (isInMatch)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 1v1 match tried to queue");
                    Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
                }
                else
                {
                    query = $"INSERT INTO queueEU1(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
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

                    await Context.Channel.SendMessageAsync($"A player has been added to EU 1v1 queue");
                    Console.WriteLine($"{userInfo.Username} has joined queue");

                    if (queueCount == 1)
                    {
                        await NewMatchEU(0, 1); // This should be modified when anti-smurfing mechanism is introduced
                        Globals.conn.Open();
                        query = $"TRUNCATE TABLE queueEU1;";
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
                    }
                }
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
        }

        [Command("leave")]
        [Alias("l")]
        [Summary("Leaves the 1v1 queue")]
        public async Task QueueLeaveAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave 1v1 queue");

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueNA1;";
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
                    query = $"SELECT id FROM queueNA1;";
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
                    query = $"DELETE FROM queueNA1 WHERE id = {userInfo.Id};";
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
                    await Context.Channel.SendMessageAsync($"A player has left the NA 1v1 queue");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Player not in 1v1 queue tried to leave queue");
                    Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
                }
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueEU1;";
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
                    query = $"SELECT id FROM queueEU1;";
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
                    query = $"DELETE FROM queueEU1 WHERE id = {userInfo.Id};";
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
                    await Context.Channel.SendMessageAsync($"A player has left the EU 1v1 queue");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Player not in 1v1 queue tried to leave queue");
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
            string query = $"SELECT time, username, id FROM queueNA1;";
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

            query = $"INSERT INTO matchesNA1(id1, id2, username1, username2, time) VALUES({p1id}, {p2id}, '{p1name}', '{p2name}', {DateTime.Now.ToBinary()});";
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
            query = $"SELECT count(*) FROM matchesNA1;";
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
            Console.WriteLine($"NA 1v1 Match #{matchCount} has started.");

            await Context.Channel.SendMessageAsync($"Please remember to add your room number with !match1 room 00000");
        }

        private async Task NewMatchEU(int p1, int p2)
        {
            long p1time = 0, p2time = 0;
            string p1name = "", p2name = "";
            ulong p1id = 0, p2id = 0;
            string query = $"SELECT time, username, id FROM queueEU1;";
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

            query = $"INSERT INTO matchesEU1(id1, id2, username1, username2, time) VALUES({p1id}, {p2id}, '{p1name}', '{p2name}', {DateTime.Now.ToBinary()});";
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
            query = $"SELECT count(*) FROM matchesEU1;";
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

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                bool isInQueue = false, isInMatch = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueNA2;";
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
                    query = $"SELECT id FROM queueNA2;";
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

                query = $"SELECT id1, id2, id3, id4 FROM matchesNA2;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (isInQueue)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 2v2 queue has now refreshed their queue timer");

                    query = $"UPDATE queueNA2 SET time = {DateTime.Now.ToBinary()} WHERE id = {userInfo.Id};";
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
                    Console.WriteLine($"{userInfo.Username} refreshed their queue timer");
                }
                else if (isInMatch)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 2v2 match tried to queue");
                    Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
                }
                else
                {
                    query = $"INSERT INTO queueNA2(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
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

                    await Context.Channel.SendMessageAsync($"A player has been added to NA 2v2 queue");

                    if (queueCount == 3)
                    {
                        await NewMatchNA(0, 1, 2, 3); // This should be modified when anti-smurfing mechanism is introduced
                        Globals.conn.Open();
                        query = $"TRUNCATE TABLE queueNA2;";
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
                    }
                }
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                bool isInQueue = false, isInMatch = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueEU2;";
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
                    query = $"SELECT id FROM queueEU2;";
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

                query = $"SELECT id1, id2, id3, id4 FROM matchesEU2;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                if (isInQueue)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 2v2 queue has now refreshed their queue timer");

                    query = $"UPDATE queueEU2 SET time = {DateTime.Now.ToBinary()} WHERE id = {userInfo.Id};";
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
                    Console.WriteLine($"{userInfo.Username} refreshed their queue timer");
                }
                else if (isInMatch)
                {
                    await Context.Channel.SendMessageAsync($"Player already in 2v2 match tried to queue");
                    Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
                }
                else
                {
                    query = $"INSERT INTO queueEU2(time, username, id) VALUES({DateTime.Now.ToBinary()}, '{userInfo.Username}', {userInfo.Id});";
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

                    await Context.Channel.SendMessageAsync($"A player has been added to EU 2v2 queue");
                    Console.WriteLine($"{userInfo.Username} has joined queue");

                    if (queueCount == 3)
                    {
                        await NewMatchEU(0, 1, 2, 3); // This should be modified when anti-smurfing mechanism is introduced
                        Globals.conn.Open();
                        query = $"TRUNCATE TABLE queueEU2;";
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
                    }
                }
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
        }

        [Command("leave")]
        [Alias("l")]
        [Summary("Leaves the 2v2 queue")]
        public async Task QueueLeaveAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave 2v2 queue");

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueNA2;";
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
                    query = $"SELECT id FROM queueNA2;";
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
                    query = $"DELETE FROM queueNA2 WHERE id = {userInfo.Id};";
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
                    await Context.Channel.SendMessageAsync($"A player has left the NA 2v2 queue");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Player not in 2v2 queue tried to leave queue");
                    Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
                }
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                bool isInQueue = false;
                int queueCount = 0;
                string query = $"SELECT count(*) FROM queueEU2;";
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
                    query = $"SELECT id FROM queueEU2;";
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
                    query = $"DELETE FROM queueEU2 WHERE id = {userInfo.Id};";
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
                    await Context.Channel.SendMessageAsync($"A player has left the EU 2v2 queue");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Player not in 2v2 queue tried to leave queue");
                    Console.WriteLine($"{userInfo.Username} tried to leave an empty queue");
                }
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
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
            string query = $"SELECT time, username, id FROM queueNA2;";
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            query = $"INSERT INTO matchesNA2(id1, id2, id3, id4, username1, username2, username3, username4, time) " +
                $"VALUES({p1id}, {p2id}, {p3id}, {p4id}, '{p1name}', '{p2name}', '{p3name}', '{p4name}', {DateTime.Now.ToBinary()});";
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
            query = $"SELECT count(*) FROM matchesNA2;";
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

            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;

            query = $"SELECT id, elo2 FROM leaderboardNA;";
            Globals.conn.Open();
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

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
            string query = $"SELECT time, username, id FROM queueEU2;";
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            query = $"INSERT INTO matchesEU2(id1, id2, id3, id4, username1, username2, username3, username4, time) " +
                $"VALUES({p1id}, {p2id}, {p3id}, {p4id}, '{p1name}', '{p2name}', '{p3name}', '{p4name}', {DateTime.Now.ToBinary()});";
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
            query = $"SELECT count(*) FROM matchesEU2;";
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

            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;

            query = $"SELECT id, elo2 FROM leaderboardEU;";
            Globals.conn.Open();
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

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
            ulong p1ID = 0;
            ulong p2ID = 0;
            string p1Username = "";
            string p2Username = "";
            int thisMatchNum = 1;

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                string query = $"SELECT id1, id2, username1, username2 FROM matchesNA1;";
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
                query = $"SELECT elo1 FROM leaderboardNA WHERE id = {p1ID};";
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
                query = $"SELECT elo1 FROM leaderboardNA WHERE id = {p2ID};";
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

                string p1ResultString = "wins1";
                string p2ResultString = "loss1";
                if (!(bool)isP1)
                {
                    p1ResultString = "loss1";
                    p2ResultString = "wins1";
                }

                Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
                query = $"UPDATE leaderboardNA SET elo1 = {new1} WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardNA SET {p1ResultString} = {p1ResultString} + 1 WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardNA SET elo1 = {new2} WHERE id = {p2ID};";
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
                query = $"UPDATE leaderboardNA SET {p2ResultString} = {p2ResultString} + 1 WHERE id = {p2ID};";
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
                query = $"DELETE FROM matchesNA1 WHERE id1 = {p1ID};";
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
                string query = $"SELECT id1, id2, username1, username2 FROM matchesEU1;";
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
                query = $"SELECT elo1 FROM leaderboardEU WHERE id = {p1ID};";
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
                query = $"SELECT elo1 FROM leaderboardEU WHERE id = {p2ID};";
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

                string p1ResultString = "wins1";
                string p2ResultString = "loss1";
                if (!(bool)isP1)
                {
                    p1ResultString = "loss1";
                    p2ResultString = "wins1";
                }

                Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
                query = $"UPDATE leaderboardEU SET elo1 = {new1} WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardEU SET {p1ResultString} = {p1ResultString} + 1 WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardEU SET elo1 = {new2} WHERE id = {p2ID};";
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
                query = $"UPDATE leaderboardEU SET {p2ResultString} = {p2ResultString} + 1 WHERE id = {p2ID};";
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
                query = $"DELETE FROM matchesEU1 WHERE id1 = {p1ID};";
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

        [Command("room")]
        [Summary("Adds room number to match info")]
        public async Task AddRoomNumberAsync([Remainder] int room)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");
            int thisMatchNum = 1, idnum = 0;
            bool isInMatch = false;

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                string query = $"SELECT id1, id2, username1, username2 FROM matchesNA1;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                
                query = $"UPDATE matchesNA1 SET room = {room} WHERE id{idnum} = {userInfo.Id};";
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

                await Context.Channel.SendMessageAsync($"NA 1v1 Match #{thisMatchNum} is in room #{room}");
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                string query = $"SELECT id1, id2, username1, username2 FROM matchesEU1;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                query = $"UPDATE matchesEU1 SET room = {room} WHERE id{idnum} = {userInfo.Id};";
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

                await Context.Channel.SendMessageAsync($"EU 1v1 Match #{thisMatchNum} is in room #{room}");
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
            await Context.Message.DeleteAsync();
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
            string query = $"SELECT elo1 FROM leaderboardNA WHERE id = {p1ID};";
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
            query = $"SELECT elo1 FROM leaderboardNA WHERE id = {p2ID};";
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

            query = $"UPDATE leaderboardNA SET elo1 = {new1} WHERE id = {p1ID};";
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
            query = $"UPDATE leaderboardNA SET elo1 = {new2} WHERE id = {p2ID};";
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
            string query = $"SELECT elo1 FROM leaderboardEU WHERE id = {p1ID};";
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
            query = $"SELECT elo1 FROM leaderboardEU WHERE id = {p2ID};";
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

            query = $"UPDATE leaderboardEU SET elo1 = {new1} WHERE id = {p1ID};";
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
            query = $"UPDATE leaderboardEU SET elo1 = {new2} WHERE id = {p2ID};";
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
            ulong p1ID = 0, p2ID = 0, p3ID = 0, p4ID = 0;
            string p1Username = "", p2Username = "", p3Username = "", p4Username = "";
            int thisMatchNum = 1;

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                string query = $"SELECT id1, id2, id3, id4, username1, username2, username3, username4 FROM matchesNA2;";
                Globals.conn.Open();
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
                query = $"SELECT id, elo2 FROM leaderboardNA;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                results = EloConvert(p1elo, p2elo, p3elo, p4elo, (bool)isT1);

                double new1 = p1elo + results.Item1;
                double new2 = p2elo + results.Item2;
                double new3 = p3elo + results.Item3;
                double new4 = p4elo + results.Item4;

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

                string t1ResultString = "wins2";
                string t2ResultString = "loss2";
                if (!(bool)isT1)
                {
                    t1ResultString = "loss2";
                    t2ResultString = "wins2";
                }

                Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
                query = $"UPDATE leaderboardNA SET elo2 = {new1} WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardNA SET {t1ResultString} = {t1ResultString} + 1 WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardNA SET elo2 = {new2} WHERE id = {p2ID};";
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
                query = $"UPDATE leaderboardNA SET {t1ResultString} = {t1ResultString} + 1 WHERE id = {p2ID};";
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
                Console.WriteLine($"Giving {p3Username} {results.Item3} elo, resulting in {new3}");
                query = $"UPDATE leaderboardNA SET elo2 = {new3} WHERE id = {p3ID};";
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
                query = $"UPDATE leaderboardNA SET {t2ResultString} = {t2ResultString} + 1 WHERE id = {p3ID};";
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
                Console.WriteLine($"Giving {p4Username} {results.Item4} elo, resulting in {new4}");
                Globals.conn.Open();
                query = $"UPDATE leaderboardNA SET elo2 = {new4} WHERE id = {p4ID};";
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
                query = $"UPDATE leaderboardNA SET {t2ResultString} = {t2ResultString} + 1 WHERE id = {p4ID};";
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
                query = $"DELETE FROM matchesNA2 WHERE id1 = {p1ID};";
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
                string query = $"SELECT id1, id2, id3, id4, username1, username2, username3, username4 FROM matchesEU2;";
                Globals.conn.Open();
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
                query = $"SELECT id, elo2 FROM leaderboardEU;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();
                results = EloConvert(p1elo, p2elo, p3elo, p4elo, (bool)isT1);

                double new1 = p1elo + results.Item1;
                double new2 = p2elo + results.Item2;
                double new3 = p3elo + results.Item3;
                double new4 = p4elo + results.Item4;

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

                string t1ResultString = "wins2";
                string t2ResultString = "loss2";
                if (!(bool)isT1)
                {
                    t1ResultString = "loss2";
                    t2ResultString = "wins2";
                }

                Console.WriteLine($"Giving {p1Username} {results.Item1} elo, resulting in {new1}");
                query = $"UPDATE leaderboardEU SET elo2 = {new1} WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardEU SET {t1ResultString} = {t1ResultString} + 1 WHERE id = {p1ID};";
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
                query = $"UPDATE leaderboardEU SET elo2 = {new2} WHERE id = {p2ID};";
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
                query = $"UPDATE leaderboardEU SET {t1ResultString} = {t1ResultString} + 1 WHERE id = {p2ID};";
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
                Console.WriteLine($"Giving {p3Username} {results.Item3} elo, resulting in {new3}");
                query = $"UPDATE leaderboardEU SET elo2 = {new3} WHERE id = {p3ID};";
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
                query = $"UPDATE leaderboardEU SET {t2ResultString} = {t2ResultString} + 1 WHERE id = {p3ID};";
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
                Console.WriteLine($"Giving {p4Username} {results.Item4} elo, resulting in {new4}");
                Globals.conn.Open();
                query = $"UPDATE leaderboardEU SET elo2 = {new4} WHERE id = {p4ID};";
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
                query = $"UPDATE leaderboardEU SET {t2ResultString} = {t2ResultString} + 1 WHERE id = {p4ID};";
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
                query = $"DELETE FROM matchesEU2 WHERE id1 = {p1ID};";
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

        [Command("room")]
        [Summary("Adds room number to match info")]
        public async Task AddRoomNumberAsync([Remainder] int room)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");
            int thisMatchNum = 1, idnum = 0;
            bool isInMatch = false;

            if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442734271004672)
            {
                string query = $"SELECT id1, id2, id3, id4 username1, username2 FROM matchesNA2;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                query = $"UPDATE matchesNA2 SET room = {room} WHERE id{idnum} = {userInfo.Id};";
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

                await Context.Channel.SendMessageAsync($"NA 2v2 Match #{thisMatchNum} is in room #{room}");
            }
            else if (Context.Guild.GetUser(userInfo.Id).Roles.ElementAt(1).Id == 396442764298158081)
            {
                string query = $"SELECT id1, id2, id3, id4 username1, username2 FROM matchesEU2;";
                Globals.conn.Open();
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
                    Globals.conn.Close();
                    throw;
                }
                Globals.conn.Close();

                query = $"UPDATE matchesEU2 SET room = {room} WHERE id{idnum} = {userInfo.Id};";
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

                await Context.Channel.SendMessageAsync($"EU 2v2 Match #{thisMatchNum} is in room #{room}");
            }
            else await Context.Channel.SendMessageAsync($"Incorrect role order or roles has not been added.");
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
            string query = $"SELECT id, elo2 FROM leaderboardNA;";
            Globals.conn.Open();
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            results = EloConvert(p1elo, p2elo, p3elo, p4elo, isT1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;
            double new3 = p3elo + results.Item3;
            double new4 = p4elo + results.Item4;

            query = $"UPDATE leaderboardNA SET elo2 = {new1} WHERE id = {p1ID};";
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
            query = $"UPDATE leaderboardNA SET elo2 = {new2} WHERE id = {p2ID};";
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
            query = $"UPDATE leaderboardNA SET elo3 = {new3} WHERE id = {p3ID};";
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
            query = $"UPDATE leaderboardNA SET elo4 = {new4} WHERE id = {p4ID};";
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
        public async Task AdminMatchReportEUAsync(ulong p1ID, ulong p2ID, ulong p3ID, ulong p4ID)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            bool isT1 = true;

            var results = new Tuple<double, double, double, double>(0, 0, 0, 0);
            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;
            string query = $"SELECT id, elo2 FROM leaderboardEU;";
            Globals.conn.Open();
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
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();
            results = EloConvert(p1elo, p2elo, p3elo, p4elo, isT1);

            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;
            double new3 = p3elo + results.Item3;
            double new4 = p4elo + results.Item4;

            query = $"UPDATE leaderboardEU SET elo2 = {new1} WHERE id = {p1ID};";
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
            query = $"UPDATE leaderboardEU SET elo2 = {new2} WHERE id = {p2ID};";
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
            query = $"UPDATE leaderboardEU SET elo3 = {new3} WHERE id = {p3ID};";
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
            query = $"UPDATE leaderboardEU SET elo4 = {new4} WHERE id = {p4ID};";
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

            double change1 = (32 * (Convert.ToDouble(isT1) - expected1)) + 2;
            double change2 = (32 * (Convert.ToDouble(isT1) - expected2)) + 2;
            double change3 = (32 * (Convert.ToDouble(!isT1) - expected3)) + 2;
            double change4 = (32 * (Convert.ToDouble(!isT1) - expected4)) + 2;

            var allChange = Tuple.Create(change1, change2, change3, change4);

            return allChange;
        }
    }

    [Group("leaderboardNA")]
    public class LeaderboardNAModule : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        [Alias("register")]
        [Summary("Join the leaderboard")]
        public async Task JoinDBAsync()
        {
            await Context.Message.DeleteAsync();
            var userInfo = Context.User;
            var user = Context.Guild.GetUser(userInfo.Id);

            int count = 0;
            string query = $"SELECT count(*) FROM leaderboardNA;";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    count = reader.GetInt16(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            if (count < 30)
            {
                query = $"INSERT INTO leaderboardNA(id, username) VALUES({userInfo.Id}, '{userInfo.Username}');";
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

                await Context.Channel.SendMessageAsync($"{userInfo.Username} has been succesfully registered to the NA leaderboard! You have 2500 elo.");
                Console.WriteLine($"{userInfo.Username} has been registered");
            }
            else
            {
                await Context.Channel.SendMessageAsync("The NA leaderboard has been filled up. Please try again next season.");
                Console.WriteLine($"{userInfo.Username} tried to join a filled up leaderboard.");
            }
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

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(396442734271004672));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the leaderboards");
        }

        [Command("wl")]
        [Summary("Sets the win/loss ratio of a player")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetWinLossAsync(ulong id, int wins, int losses, [Remainder] string gamemode)
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
            query = $"UPDATE leaderboardNA SET wins{gamemode} = {wins}, loss{gamemode} = {losses} WHERE id = {id};";
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

            await Context.Channel.SendMessageAsync($"{username} win/loss in {gamemode}s has been updated");
        }
    }

    [Group("leaderboardEU")]
    public class LeaderboardEUModule : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        [Alias("register")]
        [Summary("Join the leaderboard")]
        public async Task JoinDBAsync()
        {
            await Context.Message.DeleteAsync();
            var userInfo = Context.User;
            var user = Context.Guild.GetUser(userInfo.Id);

            int count = 0;
            string query = $"SELECT count(*) FROM leaderboardEU;";
            Globals.conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    count = reader.GetInt16(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.conn.Close();
                throw;
            }
            Globals.conn.Close();

            if (count < 30)
            {
                query = $"INSERT INTO leaderboardEU(id, username) VALUES({userInfo.Id}, '{userInfo.Username}');";
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

                await Context.Channel.SendMessageAsync($"{userInfo.Username} has been succesfully registered to the EU leaderboard! You have 2500 elo.");
                Console.WriteLine($"{userInfo.Username} has been registered");
            }
            else
            {
                await Context.Channel.SendMessageAsync("The EU leaderboard has been filled up. Please try again next season.");
                Console.WriteLine($"{userInfo.Username} tried to join a filled up leaderboard.");
            }
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

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(396442764298158081));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the leaderboards");
        }

        [Command("wl")]
        [Summary("Sets the win/loss ratio of a player")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetWinLossAsync(ulong id, int wins, int losses, [Remainder] string gamemode)
        {
            var user = Context.Guild.GetUser(id);
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
            query = $"UPDATE leaderboardEU SET wins{gamemode} = {wins}, loss{gamemode} = {losses} WHERE id = {id};";
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

            await Context.Channel.SendMessageAsync($"{username} win/loss in {gamemode}s has been updated");
        }
    }
}
