using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPR
{
    public class LeaderboardUser : IComparable<LeaderboardUser>
    {
        public ulong id;
        public ulong teammateId;
        public string username;
        public int points;
        public int streak;
        public int wins;
        public int loss;

        /**
         * Note: This CompareTo compares backwards to sort order is descending
         */
        public int CompareTo(LeaderboardUser other)
        {
            if (other == null)
            {
                return 0;
            }
            if (streak != other.streak)
            {
                return other.streak.CompareTo(streak);
            }
            if (points != other.points)
            {
                return other.points.CompareTo(points);
            }
            else return Convert.ToInt32(Globals.rnd.NextDouble() >= 0.5);
        }
    }

    public struct Match
    {
        public ulong id1;
        public ulong id2;
        public ulong? id3;
        public ulong? id4;
        public uint room;

        public int GetNum(ulong id)
        {
            if (id == id1) return 1;
            else if (id == id2) return 2;
            else if (id == id3) return 3;
            else if (id == id4) return 4;
            else return 0;
        }
    }

    public struct MatchHistory
    {
        public ulong id1;
        public ulong id2;
        public ulong? id3;
        public ulong? id4;
        public bool revert1;
        public bool revert2;
        public bool? revert3;
        public bool? revert4;
        public int oldScore1;
        public int oldScore2;
        public int? oldScore3;
        public int? oldScore4;
        public int oldStreak1;
        public int oldStreak2;
        public int? oldStreak3;
        public int? oldStreak4;
        public bool isReporterWinner;
        public ulong reporter;

        public int GetTotalReverts()
        {
            return
                (revert1 ? 1 : 0) +
                (revert2 ? 1 : 0) +
                (revert3.GetValueOrDefault() ? 1 : 0) +
                (revert4.GetValueOrDefault() ? 1 : 0);
        }

        public bool HasAlreadyReverted(ulong id)
        {
            if (id == id1) return revert1;
            else if (id == id2) return revert2;
            else if (id == id3) return revert3.GetValueOrDefault();
            else if (id == id4) return revert4.GetValueOrDefault();
            else return false;
        }

        public int GetPlayerNum(ulong id)
        {
            if (id == id1) return 1;
            else if (id == id2) return 2;
            else if (id == id3) return 3;
            else if (id == id4) return 4;
            else return 0;
        }

        public int GetPlayerScore(ulong id)
        {
            if (id == id1) return oldScore1;
            else if (id == id2) return oldScore2;
            else if (id == id3) return oldScore3.GetValueOrDefault();
            else if (id == id4) return oldScore4.GetValueOrDefault();
            else return 0;
        }

        public int GetPlayerStreak(ulong id)
        {
            if (id == id1) return oldStreak1;
            else if (id == id2) return oldStreak2;
            else if (id == id3) return oldStreak3.GetValueOrDefault();
            else if (id == id4) return oldStreak4.GetValueOrDefault();
            else return 0;
        }

        public ulong GetIdFromNum(int num)
        {
            switch (num)
            {
                case 1: return id1;
                case 2: return id2;
                case 3: return id3.Value;
                case 4: return id4.Value;
                default: return 0;
            }
        }
    }

    public struct GameConfig
    {
        public ulong serverId;
        public int gameMode;
        public long startTime;
        public long checkinTime;
        public long endTime;
        public int state;
    }

    public static class BHP
    {
        private static async Task CheckSQLStateAsync()
        {
            while (Globals.conn.State != System.Data.ConnectionState.Closed)
            {
                await Task.Delay(100);
            }
        }

        private static async Task ExecuteSQLQueryAsync(string query)
        {
            await CheckSQLStateAsync();
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }

        }

        private static async Task<int> CountQuery(string query)
        {
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return reader.GetInt16(0);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            } 
            finally
            {
                await Globals.conn.CloseAsync();
            }
        }

        public static async Task BackupLeaderboard()
        {
            GameConfig lastConfig = Globals.config.Value;
            string date = new DateTime(lastConfig.startTime).ToString("yyyy'_'MM'_'dd");
            int existingTableCount = await CountQuery($"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'leaderboard_{lastConfig.gameMode}_{date}';");
            
            if (existingTableCount == 1)
            {
                await ExecuteSQLQueryAsync($"DROP TABLE leaderboard_{lastConfig.gameMode}_{date}");
            }

            await ExecuteSQLQueryAsync(
                $"CREATE TABLE leaderboard_{lastConfig.gameMode}_{date} LIKE leaderboard;\n" +
                $"ALTER TABLE leaderboard_{lastConfig.gameMode}_{date} DISABLE KEYS;\n" + 
                $"INSERT INTO leaderboard_{lastConfig.gameMode}_{date} SELECT * FROM leaderboard;\n" +
                $"TRUNCATE TABLE leaderboard;"
            );
        }

        public static async Task<int> GetMatchCount()
        {
            return await CountQuery($"SELECT count(*) FROM matches;");
        }

        public static async Task<int> GetLeaderboardCount()
        {
            return await CountQuery($"SELECT count(*) FROM leaderboard;");
        }

        public static async Task<LeaderboardUser> GetLeaderboardUser(ulong id)
        {
            string query = $"SELECT * FROM leaderboard;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == id)
                    {
                        LeaderboardUser leaderboardUser = new LeaderboardUser
                        {
                            id = reader.GetUInt64(0),
                            teammateId = reader.GetUInt64(1),
                            username = reader.GetString(2),
                            points = reader.GetInt16(3),
                            streak = reader.GetInt16(4),
                            wins = reader.GetInt16(5),
                            loss = reader.GetInt16(6)
                        };
                        return leaderboardUser;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }
        }

        public static async Task<Dictionary<ulong, LeaderboardUser>> GetLeaderboardUsers(List<ulong> ids)
        {
            Dictionary<ulong, LeaderboardUser> leaderboardUsers = new Dictionary<ulong, LeaderboardUser>(4);
            string query = $"SELECT * FROM leaderboard;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    foreach (ulong id in ids)
                    {
                        if (reader.GetUInt64(0) == id)
                        {
                            LeaderboardUser leaderboardUser = new LeaderboardUser
                            {
                                id = reader.GetUInt64(0),
                                teammateId = reader.GetUInt64(1),
                                username = reader.GetString(2),
                                points = reader.GetInt16(3),
                                streak = reader.GetInt16(4),
                                wins = reader.GetInt16(5),
                                loss = reader.GetInt16(6)
                            };
                            leaderboardUsers.Add(leaderboardUser.id, leaderboardUser);
                        }
                    }
                }

                if (leaderboardUsers.Count != ids.Count)
                {
                    string message = $"GetLeaderboardUsers could not find all {ids.Count} ids";
                    Console.WriteLine(message);
                    throw new DataMisalignedException(message);
                }
                return leaderboardUsers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }
        }

        public static async Task<List<LeaderboardUser>> GetLeaderboard()
        {
            List<LeaderboardUser> leaderboardUsers = new List<LeaderboardUser>();
            string query = $"SELECT * FROM leaderboard ORDER BY points DESC;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LeaderboardUser leaderboardUser = new LeaderboardUser
                    {
                        id = reader.GetUInt64(0),
                        teammateId = reader.GetUInt64(1),
                        username = reader.GetString(2),
                        points = reader.GetInt16(3),
                        streak = reader.GetInt16(4),
                        wins = reader.GetInt16(5),
                        loss = reader.GetInt16(6)
                    };
                    leaderboardUsers.Add(leaderboardUser);
                }
                return leaderboardUsers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }
        }

        public static async Task<Match?> GetMatch(ulong id)
        {
            string query = $"SELECT * FROM matches;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == id ||
                        reader.GetUInt64(1) == id ||
                        reader.GetUInt64(2) == id ||
                        reader.GetUInt64(3) == id)
                    {
                        Match match = new Match
                        {
                            id1 = reader.GetUInt64(0),
                            id2 = reader.GetUInt64(1),
                            id3 = reader.GetUInt64(2),
                            id4 = reader.GetUInt64(3)
                        };
                        return match;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }
        }

        public static async Task<List<Match>> GetMatches()
        {
            List<Match> matches = new List<Match>();
            string query = $"SELECT * FROM matches;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Match match = new Match
                    {
                        id1 = reader.GetUInt64(0),
                        id2 = reader.GetUInt64(1),
                        id3 = reader.GetUInt64(2),
                        id4 = reader.GetUInt64(3),
                        room = reader.GetUInt32(4)
                    };
                    matches.Add(match);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }

            return matches;
        }

        public static async Task<MatchHistory?> GetMatchHistory(ulong id)
        {
            string query = $"SELECT * FROM matchesHistory;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == id ||
                        reader.GetUInt64(1) == id ||
                        reader.GetUInt64(2) == id ||
                        reader.GetUInt64(3) == id)
                    {
                        MatchHistory matchHistory = new MatchHistory
                        {
                            id1 = reader.GetUInt64(0),
                            id2 = reader.GetUInt64(1),
                            id3 = reader.GetUInt64(2),
                            id4 = reader.GetUInt64(3),
                            revert1 = reader.GetBoolean(4),
                            revert2 = reader.GetBoolean(5),
                            revert3 = reader.GetBoolean(6),
                            revert4 = reader.GetBoolean(7),
                            oldScore1 = reader.GetInt16(8),
                            oldScore2 = reader.GetInt16(9),
                            oldScore3 = reader.GetInt16(10),
                            oldScore4 = reader.GetInt16(11),
                            oldStreak1 = reader.GetInt16(12),
                            oldStreak2 = reader.GetInt16(13),
                            oldStreak3 = reader.GetInt16(14),
                            oldStreak4 = reader.GetInt16(15),
                            isReporterWinner = reader.GetBoolean(16),
                            reporter = reader.GetUInt64(17)
                        };
                        return matchHistory;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }
        }

        public static async Task<bool> GetLeavePlayer(ulong id)
        {
            return (await CountQuery($"SELECT count(*) FROM leaving WHERE id={id};") == 1);
        }

        public static async Task UpdateLocalConfig()
        {
            string query = $"SELECT * FROM config;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    GameConfig config = new GameConfig
                    {
                        serverId = reader.GetUInt64(0),
                        gameMode = reader.GetInt16(1),
                        startTime = reader.GetInt64(2),
                        checkinTime = reader.GetInt64(3),
                        endTime = reader.GetInt64(4),
                        state = reader.GetInt16(5)
                    };

                    Globals.config = config;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                await Globals.conn.CloseAsync();
            }
        }

        public static async Task PutLeaderboardUser(ulong id, string username, ulong teammateId = 0)
        {
            await ExecuteSQLQueryAsync($"INSERT INTO leaderboard(id, teammateId, username) VALUES({id}, {teammateId}, '{username}') ON DUPLICATE KEY UPDATE teammateId = {teammateId};");
        }

        public static async Task PutMatch(Match match)
        {
            string query;
            if (Globals.config.Value.gameMode == 1)
            {
                query = $"INSERT INTO matches(id1, id2) " +
                $"VALUES({match.id1}, {match.id2});";
            }
            else if (Globals.config.Value.gameMode == 2)
            {
                query = $"INSERT INTO matches(id1, id2, id3, id4) " +
                $"VALUES({match.id1}, {match.id2}, {match.id3}, {match.id4});";
            }
            else
            {
                string message = "Invalid gameMode for PutMatch";
                Console.WriteLine(message);
                throw new DataMisalignedException(message);
            }
            await ExecuteSQLQueryAsync(query);
        }

        public static async Task PutLeave(ulong id)
        {
            await ExecuteSQLQueryAsync($"INSERT INTO leaving(id) VALUES({id});");
        }

        public static async Task PutMatchFromHistory(MatchHistory matchHistory)
        {
            string query;
            if (Globals.config.Value.gameMode == 1)
            {
                query = $"INSERT INTO matches(id1, id2) " +
                $"VALUES({matchHistory.id1}, {matchHistory.id2});";
            }
            else if (Globals.config.Value.gameMode == 2)
            {
                query = $"INSERT INTO matches(id1, id2, id3, id4) " +
                $"VALUES({matchHistory.id1}, {matchHistory.id2}, {matchHistory.id3}, {matchHistory.id4});";
            }
            else
            {
                string message = "Invalid gameMode for PutMatch";
                Console.WriteLine(message);
                throw new DataMisalignedException(message);
            }
            await ExecuteSQLQueryAsync(query);
        }

        public static async Task PutMatchRoom(ulong id, int idNum, uint roomNumber)
        {
            await ExecuteSQLQueryAsync($"UPDATE matches SET room = {roomNumber} WHERE id{idNum} = {id}");
        }

        public static async Task DeleteMatchHistory(ulong id)
        {
            await ExecuteSQLQueryAsync($"DELETE FROM matchesHistory WHERE id1 = {id} OR id2 = {id} OR id3 = {id} OR id4 = {id};");
        }

        public static async Task DeleteLeaderboardUsers(List<ulong> ids)
        {
            string allIds = "";
            foreach (ulong id in ids)
            {
                allIds += $"{id},";
            }

            
            await ExecuteSQLQueryAsync($"DELETE FROM leaderboard WHERE id IN ({allIds.Remove(allIds.Length - 1, 1)});");
        }

        public static async Task PutMatchHistory(List<ulong> playerIds, Dictionary<ulong, LeaderboardUser> leaderboardUsers, ulong userId, bool winner)
        {
            string query;
            if (leaderboardUsers.Count == 2)
            {
                LeaderboardUser p1 = leaderboardUsers[playerIds[0]], p2 = leaderboardUsers[playerIds[1]];
                query = $"INSERT INTO matchesHistory(id1, id2, oldScore1, oldScore2, oldStreak1, oldStreak2, isReporterWinner, reporter) " +
                $"VALUES({p1.id}, {p2.id}, {p1.points}, {p2.points}, {p1.streak}, {p2.streak}, {winner}, {userId});";
            }
            else if (leaderboardUsers.Count == 4)
            {
                LeaderboardUser p1 = leaderboardUsers[playerIds[0]], p2 = leaderboardUsers[playerIds[1]], p3 = leaderboardUsers[playerIds[2]], p4 = leaderboardUsers[playerIds[3]];
                query = $"INSERT INTO matchesHistory(id1, id2, id3, id4, oldScore1, oldScore2, oldScore3, oldScore4, oldStreak1, oldStreak2, oldStreak3, oldStreak4, isReporterWinner, reporter) " +
                $"VALUES({p1.id}, {p2.id}, {p3.id}, {p4.id}, {p1.points}, {p2.points}, {p3.points}, {p4.points}, {p1.streak}, {p2.streak}, {p3.streak}, {p4.streak}, {winner}, {userId});";
            }
            else
            {
                string message = "Invalid number of users for PutMatchHistory";
                Console.WriteLine(message);
                throw new DataMisalignedException(message);
            }
            await ExecuteSQLQueryAsync(query);
        }

        public static async Task UpdateLeaderboardUserScore(ulong id, int score)
        {
            string recordString = score == 0 ? "loss" : "wins";
            string streakString = score == 0 ? "0" : "streak + 1";
            await ExecuteSQLQueryAsync($"UPDATE leaderboard SET points = points + {score}, {recordString} = {recordString} + 1, streak = {streakString} WHERE id = {id};");
        }

        public static async Task UpdateLeaderboardRevert(ulong id, int points, int streak, bool winner)
        {
            string recordString = winner ? "wins" : "loss";
            await ExecuteSQLQueryAsync($"UPDATE leaderboard SET points = {points}, {recordString} = {recordString} - 1, streak = {streak} WHERE id = {id};");
        }

        public static async Task UpdateMatchHistoryRevert(ulong id, int num)
        {
            await ExecuteSQLQueryAsync($"UPDATE matchesHistory SET revert{num} = 1 WHERE id{num} = {id};");
        }

        public static async Task DeleteMatch(ulong player1Id)
        {
            await ExecuteSQLQueryAsync($"DELETE FROM matches WHERE id1 = {player1Id};");
        }

        public static async Task DeleteLeave(ulong id)
        {
            await ExecuteSQLQueryAsync($"DELETE FROM leaving WHERE id = {id};");
        }

        public static async Task PutConfig(GameConfig config)
        {
            await ExecuteSQLQueryAsync($"INSERT INTO config(serverId, gameMode, startTime, checkinTime, endTime, state) " +
                $"VALUES({config.serverId}, {config.gameMode}, {config.startTime}, {config.checkinTime}, {config.endTime}, {config.state})");
        }

        public static async Task UpdateConfigState(int state)
        {
            await ExecuteSQLQueryAsync($"UPDATE config SET state = {state};");
        }

        public static async Task ClearConfig()
        {
            await ExecuteSQLQueryAsync("TRUNCATE TABLE config");
        }

        public static async Task ClearLeaderboard()
        {
            await ExecuteSQLQueryAsync("TRUNCATE TABLE leaderboard");
        }
    }
}