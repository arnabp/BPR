using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using MySql.Data.MySqlClient;
using System.Linq;

namespace BPR
{
    public struct Player
    {
        public ulong id;
        public string username;
        public int tier;
        public int tierTeammate;
    }

    public struct Team
    {
        public Player p1;
        public Player p2;
        public int tier;
    }

    public class MatchModule
    {
        public static async Task CreateMatch1Async(IMessageChannel thisChannel, Player p1, Player p2, string region)
        {
            bool is1InQueue = false, is2InQueue = false;

            string query = $"SELECT id FROM queue{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == p1.id) is1InQueue = true;
                    if (reader.GetUInt64(0) == p2.id) is2InQueue = true;
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
                query = $"DELETE FROM queue{region}2 WHERE id = {p1.id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await thisChannel.SendMessageAsync($"A player has left the {region} 2v2 queue");
            }

            if (is2InQueue)
            {
                query = $"DELETE FROM queue{region}2 WHERE id = {p2.id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await thisChannel.SendMessageAsync($"A player has left the {region} 2v2 queue");
            }

            query = $"INSERT INTO matches{region}1(id1, id2, username1, username2, time, reverted) VALUES({p1.id}, {p2.id}, '{p1.username}', '{p2.username}', {DateTime.UtcNow.Ticks}, 0);";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            int matchCount = 0;
            query = $"SELECT count(*) FROM matches{region}1;";
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

            double p1elo = 0, p2elo = 0;

            query = $"SELECT id, elo FROM leaderboard{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == p1.id)
                    {
                        p1elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == p2.id)
                    {
                        p2elo = reader.GetDouble(1);
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

            if (p1elo > p2elo)
            {
                await thisChannel.SendMessageAsync($"New match has started between <@{p1.id}> and <@{p2.id}>");
            }
            else
            {
                await thisChannel.SendMessageAsync($"New match has started between <@{p2.id}> and <@{p1.id}>");
            }

            Console.WriteLine($"{region} 1v1 Match #{matchCount} has started.");

            query = $"DELETE FROM queue{region}1 WHERE id = {p1.id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}1 WHERE id = {p2.id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await thisChannel.SendMessageAsync($"Please remember to add your room number with match1 room 00000");

            query = $"SELECT count(*) FROM queue{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetInt16(0) == 0)
                    {
                        Region thisRegion = Globals.regionList[region];
                        thisRegion.inQueue1 = false;
                        Globals.regionList[region] = thisRegion;
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
        }

        public static async Task CreateMatch2Async(IMessageChannel thisChannel, Team t1, Team t2, string region)
        {
            bool is1InQueue = false, is2InQueue = false, is3InQueue = false, is4InQueue = false;

            string query = $"SELECT id FROM queue{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == t1.p1.id) is1InQueue = true;
                    if (reader.GetUInt64(0) == t1.p2.id) is2InQueue = true;
                    if (reader.GetUInt64(0) == t2.p1.id) is3InQueue = true;
                    if (reader.GetUInt64(0) == t2.p2.id) is4InQueue = true;
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
                query = $"DELETE FROM queue{region}1 WHERE id = {t1.p1.id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await thisChannel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }
            if (is2InQueue)
            {
                query = $"DELETE FROM queue{region}1 WHERE id = {t1.p2.id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await thisChannel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }
            if (is3InQueue)
            {
                query = $"DELETE FROM queue{region}1 WHERE id = {t2.p1.id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await thisChannel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }
            if (is4InQueue)
            {
                query = $"DELETE FROM queue{region}1 WHERE id = {t2.p2.id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await thisChannel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }

            query = $"INSERT INTO matches{region}2(id1, id2, id3, id4, username1, username2, username3, username4, time, reverted) " +
                $"VALUES({t1.p1.id}, {t1.p2.id}, {t2.p1.id}, {t2.p2.id}, '{t1.p1.username}', '{t1.p2.username}', '{t2.p1.username}', '{t2.p2.username}', {DateTime.UtcNow.Ticks}, 0);";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            int matchCount = 0;
            query = $"SELECT count(*) FROM matches{region}2;";
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

            query = $"SELECT id, elo FROM leaderboard{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == t1.p1.id)
                    {
                        p1elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == t1.p2.id)
                    {
                        p2elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == t2.p1.id)
                    {
                        p3elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == t2.p2.id)
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
                await thisChannel.SendMessageAsync($"New match has started between <@{t1.p1.id}>, <@{t1.p2.id}> and <@{t2.p1.id}>, <@{t2.p2.id}>");
            }
            else
            {
                await thisChannel.SendMessageAsync($"New match has started between <@{t2.p1.id}>, <@{t2.p2.id}> and <@{t1.p1.id}>, <@{t1.p2.id}>");
            }

            Console.WriteLine($"{region} 2v2 Match #{matchCount} has started.");

            query = $"DELETE FROM queue{region}2 WHERE id = {t1.p1.id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}2 WHERE id = {t1.p2.id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}2 WHERE id = {t2.p1.id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}2 WHERE id = {t2.p2.id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await thisChannel.SendMessageAsync($"Please remember to add your room number with match2 room 00000");

            query = $"SELECT count(*) FROM queue{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetInt16(0) == 0)
                    {
                        Region thisRegion = Globals.regionList[region];
                        thisRegion.inQueue2 = false;
                        Globals.regionList[region] = thisRegion;
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
        }
    }
}
