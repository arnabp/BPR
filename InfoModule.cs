using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;
using Discord;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using Discord.WebSocket;

namespace BPR
{
    public struct Role
    {
        public string region;
        public int gameMode;
        public int tier;
    }

    public static class HelperFunctions
    {
        public static Role GetRoleRegion(ulong roleID)
        {
            if (Globals.roleList == null)
            {
                Globals.roleList = new Dictionary<ulong, Role>();
                Globals.roleList.Add(GetRoleId("NA", 1, 1), new Role { region = "NA", gameMode = 1, tier = 1 });
                Globals.roleList.Add(GetRoleId("NA", 1, 2), new Role { region = "NA", gameMode = 1, tier = 2 });
                Globals.roleList.Add(GetRoleId("NA", 1, 3), new Role { region = "NA", gameMode = 1, tier = 3 });
                Globals.roleList.Add(GetRoleId("NA", 1, 4), new Role { region = "NA", gameMode = 1, tier = 4 });
                Globals.roleList.Add(GetRoleId("NA", 2, 1), new Role { region = "NA", gameMode = 2, tier = 1 });
                Globals.roleList.Add(GetRoleId("NA", 2, 2), new Role { region = "NA", gameMode = 2, tier = 2 });
                Globals.roleList.Add(GetRoleId("NA", 2, 3), new Role { region = "NA", gameMode = 2, tier = 3 });
                Globals.roleList.Add(GetRoleId("NA", 2, 4), new Role { region = "NA", gameMode = 2, tier = 4 });
                Globals.roleList.Add(GetRoleId("EU", 1, 1), new Role { region = "EU", gameMode = 1, tier = 1 });
                Globals.roleList.Add(GetRoleId("EU", 1, 2), new Role { region = "EU", gameMode = 1, tier = 2 });
                Globals.roleList.Add(GetRoleId("EU", 1, 3), new Role { region = "EU", gameMode = 1, tier = 3 });
                Globals.roleList.Add(GetRoleId("EU", 2, 1), new Role { region = "EU", gameMode = 2, tier = 1 });
                Globals.roleList.Add(GetRoleId("EU", 2, 2), new Role { region = "EU", gameMode = 2, tier = 2 });
                Globals.roleList.Add(GetRoleId("EU", 2, 3), new Role { region = "EU", gameMode = 2, tier = 3 });
                Globals.roleList.Add(GetRoleId("AUS", 1, 3), new Role { region = "AUS", gameMode = 1, tier = 3 });
                Globals.roleList.Add(GetRoleId("AUS", 2, 3), new Role { region = "AUS", gameMode = 2, tier = 3 });
                Globals.roleList.Add(GetRoleId("SEA", 1, 3), new Role { region = "SEA", gameMode = 1, tier = 3 });
                Globals.roleList.Add(GetRoleId("SEA", 2, 3), new Role { region = "SEA", gameMode = 2, tier = 3 });
                Globals.roleList.Add(GetRoleId("TEST", 1, 1), new Role { region = "TEST", gameMode = 1, tier = 1 });
                Globals.roleList.Add(GetRoleId("TEST", 1, 2), new Role { region = "TEST", gameMode = 1, tier = 2 });
                Globals.roleList.Add(GetRoleId("TEST", 1, 3), new Role { region = "TEST", gameMode = 1, tier = 3 });
                Globals.roleList.Add(GetRoleId("TEST", 1, 4), new Role { region = "TEST", gameMode = 1, tier = 4 });
                Globals.roleList.Add(GetRoleId("TEST", 2, 1), new Role { region = "TEST", gameMode = 2, tier = 1 });
                Globals.roleList.Add(GetRoleId("TEST", 2, 2), new Role { region = "TEST", gameMode = 2, tier = 2 });
                Globals.roleList.Add(GetRoleId("TEST", 2, 3), new Role { region = "TEST", gameMode = 2, tier = 3 });
                Globals.roleList.Add(GetRoleId("TEST", 2, 4), new Role { region = "TEST", gameMode = 2, tier = 4 });
            }

            return Globals.roleList[roleID];
        }

        public static ulong GetRoleId(string region, int gameMode, int tier)
        {
            if (region == "NA")
            {
                if (tier == 1)
                {
                    if (gameMode == 1)
                        return 419355178680975370;
                    else if (gameMode == 2)
                        return 419355321061081088;
                }
                else if (tier == 2)
                {
                    if (gameMode == 1)
                        return 522951562667098115;
                    else if (gameMode == 2)
                        return 522951571839909898;
                }
                else if (tier == 3)
                {
                    if (gameMode == 1)
                        return 522951569994547200;
                    else if (gameMode == 2)
                        return 522951573366767637;
                }
                else if (tier == 4)
                {
                    if (gameMode == 1)
                        return 663122214211682325;
                    else if (gameMode == 2)
                        return 663122136587698206;
                }
            }
            else if (region == "EU")
            {
                if (tier == 1)
                {
                    if (gameMode == 1)
                        return 419355374529937408;
                    else if (gameMode == 2)
                        return 419355453550624768;
                }
                else if (tier == 2)
                {
                    if (gameMode == 1)
                        return 522951575195615271;
                    else if (gameMode == 2)
                        return 522951577955205123;
                }
                else if (tier == 3)
                {
                    if (gameMode == 1)
                        return 522951576625610762;
                    else if (gameMode == 2)
                        return 522951579515748354;
                }
            }
            else if (region == "AUS")
            {
                if (tier == 1)
                    return 0;
                if (tier == 2)
                    return 0;
                if (tier == 3)
                {
                    if (gameMode == 1)
                        return 423095293039607809;
                    else if (gameMode == 2)
                        return 529047745487437824;
                }
            }
            else if (region == "SEA")
            {
                if (tier == 1)
                    return 0;
                if (tier == 2)
                    return 0;
                if (tier == 3)
                {
                    if (gameMode == 1)
                        return 423095346131107853;
                    else if (gameMode == 2)
                        return 529047311188492288;
                }
            }
            else if (region == "TEST")
            {
                if (tier == 1)
                {
                    if (gameMode == 1)
                        return 123456789000007181;
                    else if (gameMode == 2)
                        return 123456789000007182;
                }
                else if (tier == 2)
                {
                    if (gameMode == 1)
                        return 123456789000007281;
                    else if (gameMode == 2)
                        return 123456789000007282;
                }
                else if (tier == 3)
                {
                    if (gameMode == 1)
                        return 123456789000007381;
                    else if (gameMode == 2)
                        return 123456789000007382;
                }
                else if (tier == 4)
                {
                    if (gameMode == 1)
                        return 123456789000007481;
                    else if (gameMode == 2)
                        return 123456789000007482;
                }
            }

            return 0;
        }

        public static ulong GetChannelId(string region, int channelType)
        {
            // General
            if (channelType == 0)
            {
                if (region == "NA" || region == "EU")
                {
                    return 392829581192855554;
                }
                if (region == "AUS" || region == "SEA")
                {
                    return 422045385612328973;
                }
                if (region == "TEST")
                {
                    return 123456789000000000;
                }
            }
            // 1v1 Queue Info
            if (channelType == 1)
            {
                if (region == "NA")
                {
                    return 401167888762929153;
                }
                if (region == "EU")
                {
                    return 525425865262104576;
                }
                if (region == "AUS")
                {
                    return 423372016922525697;
                }
                if (region == "SEA")
                {
                    return 529011508659748872;
                }
                if (region == "TEST")
                {
                    return 123456789000000001;
                }
            }
            // 2v2 Queue Info
            if (channelType == 2)
            {
                if (region == "NA")
                {
                    return 404558855771521024;
                }
                if (region == "EU")
                {
                    return 525425886195875861;
                }
                if (region == "AUS")
                {
                    return 437510787951493121;
                }
                if (region == "SEA")
                {
                    return 529012003415654400;
                }
                if (region == "TEST")
                {
                    return 123456789000000002;
                }
            }
            // Bank Status
            if (channelType == 3)
            {
                if (region == "NA")
                {
                    return 525950148581523466;
                }
                if (region == "EU")
                {
                    return 525950398297669639;
                }
                if (region == "AUS")
                {
                    return 529012722432737290;
                }
                if (region == "SEA")
                {
                    return 529012939735302144;
                }
            }

            return 0;
        }

        public static Color GetRegionColor(string region)
        {
            if (region == "NA") return Color.Blue;
            else if (region == "EU") return Color.Green;
            else if (region == "SEA") return Color.Orange;
            else if (region == "AUS") return Color.DarkBlue;
            else if (region == "BRZ") return Color.DarkGreen;
            else return Color.LightGrey;
        }

        public static Color GetTierColor(int tier)
        {
            if (tier == 1) return new Color(0xfa7298);
            else if (tier == 2) return new Color(0xff8201);
            else if (tier == 3) return new Color(0x0071cd);
            else return Color.LighterGrey;
        }

    }

    [RequireUserPermission(GuildPermission.Administrator)]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("CheckRoleId")]
        [Summary("Gets name of role from given id.")]
        public async Task CheckRoleIdAsync([Remainder] ulong id)
        {
            await Context.Channel.SendMessageAsync(Context.Guild.GetRole(id).Name);
        }

        [Command("LastMatch")]
        [Summary("Gets the number of days since last match played")]
        public async Task LastMatchAsync(ulong id, string region, int gameMode)
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();

            DateTime nowTime = DateTime.Now;
            int timeDif = 0;

            string query = $"SELECT decaytimer FROM leaderboard{region}{gameMode} WHERE id = {id};";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DateTime oldTime = new DateTime(reader.GetInt64(0));
                    TimeSpan timeDifBinary = nowTime - oldTime;
                    timeDif = (int)timeDifBinary.TotalDays;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            await Context.Channel.SendMessageAsync($"{Context.Guild.GetUser(id).Username} has been decaying for {timeDif - 3} days.");
        }

        [Command("CurrentDecay")]
        [Summary("Gets the ammount of elo a user is supposed to have lost up to now")]
        public async Task CurrentDecayAsync(ulong id, string region, int gameMode)
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();

            DateTime nowTime = DateTime.Now;
            int timeDif = 0;

            string query = $"SELECT decaytimer FROM leaderboard{region}{gameMode} WHERE id = {id};";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DateTime oldTime = new DateTime(reader.GetInt64(0));
                    TimeSpan timeDifBinary = nowTime - oldTime;
                    timeDif = (int)timeDifBinary.TotalDays;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            int eloloss = 0;
            for (int i = 5; i < timeDif + 3; i++)
            {
                eloloss += i;
            }

            await Context.Channel.SendMessageAsync($"{Context.Guild.GetUser(id).Username} hast lost {eloloss} elo.");
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

        [Command("SendMessages", RunMode = RunMode.Async)]
        [Summary("Send a specified amount of messages to test rate limit")]
        public async Task SendXMessages(int x)
        {
            await Context.Message.DeleteAsync();
            for (int i = 0; i < x; i++)
            {
                await Task.Delay(10000);
                await Context.Channel.SendMessageAsync($"Message #{i} sent");
            }
        }

        [Command("Silent")]
        [Summary("Deletes the message")]
        public async Task Silent()
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync($"Message from {userInfo.Username} has been deleted.");
        }

        [Command("DecayGif")]
        [Summary("Plays the kung fu panda decay gif")]
        public async Task DecayGif()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendFileAsync(@"/root/BPR/decay.gif");
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

        [Command("GetGuildId")]
        [Summary("Returns ID of the Guild")]
        public async Task GetGuildId()
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync($"{Context.Guild.Id}");
        }

        [Command("GetUserMention")]
        [Summary("Gets username from mention")]
        public async Task GetUserMention(Discord.WebSocket.SocketUser user)
        {
            if (user != null) await Context.Channel.SendMessageAsync($"This user is {user.Username}");
            else await Context.Channel.SendMessageAsync($"This user does not exist");
        }

        [Command("GetModeFromCommand")]
        public async Task GetModeFromCommand([Remainder] string command)
        {
            int lengthOfCommand = 19;
            int gameMode = (int)(Context.Message.Content[lengthOfCommand + 1] - '0');
            await Context.Channel.SendMessageAsync($"Game Mode is {gameMode}");
        }

        [Command("CheckQueue")]
        public async Task CheckQueue(string region, int gameMode)
        {
            await Context.Message.DeleteAsync();
            if (HelperFunctions.GetRoleId(region, gameMode, 1) == 0)
            {
                await Context.Channel.SendMessageAsync($"Incorrect region or game mode");
                return;
            }

            string inQueue = (gameMode == 1 ? Globals.regionList[region].inQueue1 : Globals.regionList[region].inQueue2) ? "nonempty" : "empty";
            await Context.Channel.SendMessageAsync($"{region} {gameMode}v{gameMode} queue is {inQueue}");
        }
    }

    [Group("q")]
    public class QueueModule : ModuleBase<SocketCommandContext>
    {
        [Command("j")]
        [Summary("DEPRACATED: Join the queue")]
        public async Task MatchReportAsync([Remainder] string literallyAnythingElse)
        {
            var userInfo = Context.User;
            await Context.Channel.SendMessageAsync($"<@{userInfo.Id}> there is no queue anymore, you don't need to use that command.");
        }
    }

    [Group("match")]
    [Alias("m")]
    public class MatchModule : ModuleBase<SocketCommandContext>
    {
        public ICommandContext localContext;

        [Command("report")]
        [Summary("Allows user to report current match")]
        [Alias("score", "result", "r")]
        public async Task MatchReportAsync([Remainder][Summary("The winner, \"Y\" or \"N\"")] string winner)
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is reporting a result");
            try
            {
                bool? isWinner = null;

                // Check if a config exists
                if (!Globals.config.HasValue)
                {
                    await localContext.Channel.SendMessageAsync($"A tournament hasn't started yet");
                    return;
                }

                // Get users' info
                Match? matchResult = await BHP.GetMatch(userInfo.Id);
                if (!matchResult.HasValue)
                {
                    await localContext.Channel.SendMessageAsync($"You are currently not in a match against anyone");
                    return;
                }
                Match match = matchResult.Value;

                // Indicate which player won
                if (winner == "W" || winner == "w" || winner == "Y" || winner == "y")
                {
                    isWinner = true;
                }
                else if (winner == "L" || winner == "l" || winner == "N" || winner == "n")
                {
                    isWinner = false;
                }
                else
                {
                    Console.WriteLine($"{userInfo.Username} entered the wrong result type");
                    await localContext.Channel.SendMessageAsync("Invalid results entered");
                    return;
                }

                List<ulong> playerIds = Globals.config.Value.gameMode == 1 ?
                    new List<ulong>() { match.id1, match.id2 } :
                    new List<ulong>() { match.id1, match.id2, (ulong)match.id3, (ulong)match.id4 };

                // Get player info
                Dictionary<ulong, LeaderboardUser> leaderboardUsers = await BHP.GetLeaderboardUsers(playerIds);

                // Get rid of old matches that could potentially be reverted for either player
                foreach (ulong id in playerIds) await BHP.DeleteMatchHistory(id);

                // Add this match to history
                await BHP.PutMatchHistory(playerIds, leaderboardUsers, userInfo.Id, (bool)isWinner);

                Dictionary<ulong, int> scores = new Dictionary<ulong, int>(playerIds.Count);
                for (int i = 0; i < playerIds.Count; i++)
                {
                    /**
                     * Magical formula to get this mapping
                     * 0 -> 1
                     * 1 -> 0
                     * 2 -> 3
                     * 3 -> 2
                     */
                    int teammateIndex = 1 - (i % 2) + (i / 2 * 2);

                    // Instead of trying to make one conditional for the score of the player, make one conditional for the score of a winner
                    int winnerToNum = (bool)isWinner ? 1 : 0;

                    // then another conditional if the player is the winner
                    int playerWinner = (playerIds[i] == userInfo.Id || (Globals.config.Value.gameMode == 2 && playerIds[teammateIndex] == userInfo.Id)) ? winnerToNum : 1 - winnerToNum;

                    // And finally, use the winstreak to get the actual number of points the player earns
                    int playerScoreAddition = playerWinner * Math.Min(leaderboardUsers[playerIds[i]].streak + 1, 3);

                    scores.Add(playerIds[i], playerScoreAddition);
                }



                // Print results in embed
                var embed = new EmbedBuilder
                {
                    Color = Color.Blue
                };
                string scoreString = "";
                foreach (var score in scores)
                {
                    if (score.Value > 0)
                    {
                        scoreString += $"<@{score.Key}> +{Convert.ToInt32(score.Value)}\n";
                    }
                }
                embed.AddField(x =>
                {
                    x.Name = "Match Result";
                    x.Value = scoreString;
                });
                await localContext.Channel.SendMessageAsync("", embed: embed);


                // Update player's elo in leaderboard
                foreach (var leaderboardUser in leaderboardUsers)
                {
                    Console.WriteLine($"Giving {leaderboardUser.Value.username} {scores[leaderboardUser.Key]} points");
                    await BHP.UpdateLeaderboardUserScore(leaderboardUser.Key, scores[leaderboardUser.Key]);
                }

                await BHP.DeleteMatch(match.id1);
                Console.WriteLine($"Match with {userInfo.Username} ended.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await localContext.Channel.SendMessageAsync("Something went wrong <@106136559744466944>");
            }
        }

        [Command("revert")]
        [Summary("Reverts the last reported match for a player")]
        public async Task RevertMatchAsync()
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is reverting a match");
            int revertTotal = Globals.config.Value.gameMode * 2;

            // Get match info
            MatchHistory? matchHistoryResult = await BHP.GetMatchHistory(userInfo.Id);
            if (!matchHistoryResult.HasValue)
            {
                await localContext.Channel.SendMessageAsync($"You have not had a match recently that can be reverted");
                return;
            }
            MatchHistory matchHistory = matchHistoryResult.Value;

            if (matchHistory.HasAlreadyReverted(userInfo.Id))
            {
                await localContext.Channel.SendMessageAsync("A player tried to revert the match twice.");
            }
            else
            {
                await localContext.Channel.SendMessageAsync($"{matchHistory.GetTotalReverts() + 1}/{revertTotal} players have requested the last match to be reverted");

                if (matchHistory.GetTotalReverts() < revertTotal - 1)
                {
                    // Add indication that a player has requested a revert
                    await BHP.UpdateMatchHistoryRevert(userInfo.Id, matchHistory.GetPlayerNum(userInfo.Id));
                }
                else
                {
                    for (int i = 0; i < revertTotal; i++)
                    {
                        ulong id = matchHistory.GetIdFromNum(i + 1);
                        /**
                         * Magical formula to get this mapping
                         * 0 -> 1
                         * 1 -> 0
                         * 2 -> 3
                         * 3 -> 2
                         */
                        int teammateIndex = 1 - (i % 2) + (i / 2 * 2);
                        bool playerWinner = (id == matchHistory.reporter || (Globals.config.Value.gameMode == 2 && matchHistory.GetIdFromNum(teammateIndex + 1) == matchHistory.reporter)) ? matchHistory.isReporterWinner : !matchHistory.isReporterWinner;

                        await BHP.UpdateLeaderboardRevert(id, matchHistory.GetPlayerScore(id), matchHistory.GetPlayerStreak(id), playerWinner);
                    }

                    await BHP.PutMatchFromHistory(matchHistory);

                    await localContext.Channel.SendMessageAsync("The last match has been reverted. Please report the match correctly now.");
                }
            }
        }

        [Command("room")]
        [Summary("Adds room number to match info")]
        public async Task AddRoomNumberAsync([Remainder] int room)
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");

            // Get player info
            Match? match = await BHP.GetMatch(userInfo.Id);

            if (match.HasValue)
            {
                // Set room in match info
                await BHP.PutMatchRoom(userInfo.Id, match.Value.GetNum(userInfo.Id), (uint)room);
                await localContext.Channel.SendMessageAsync($"{userInfo.Username} set room number to {room}");
            }
            else
            {
                await localContext.Channel.SendMessageAsync($"You are not currently in a match.");
            }
            await localContext.Message.DeleteAsync();
        }
    }

    [Group("session")]
    [Alias("s")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class SessionModule : ModuleBase<SocketCommandContext>
    {
        public ICommandContext localContext;

        [Command("start")]
        [Summary("Starts a session")]
        public async Task StartSessionAsync(int gameMode, int minutes)
        {
            localContext = localContext ?? Context;
            Console.WriteLine($"Starting a {gameMode}v{gameMode} session for {minutes} minutes");
            await BHP.ClearLeaderboard();

            GameConfig config = new GameConfig()
            {
                serverId = localContext.Guild.Id,
                gameMode = gameMode,
                startTime = DateTime.Now.Ticks,
                endTime = DateTime.Now.AddMinutes(minutes).Ticks,
                state = 0
            };

            Globals.config = config;
            await BHP.PutConfig(config);

            string atTeammate = gameMode == 2 ? " @teammate" : "";
            await localContext.Channel.SendMessageAsync($"@everyone Starting a {gameMode}v{gameMode} session! Please use command `checkin{atTeammate}` in the next 5 minutes to check in to the tournament.");
        }
    }

    public class CheckinModule : ModuleBase<SocketCommandContext>
    {
        public ICommandContext localContext;

        [Command("checkin")]
        [Summary("Allows a user to checkin when a session starts")]
        public async Task CheckinAsync()
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is checking in");

            if (!Globals.config.HasValue)
            {
                Console.WriteLine("There is no active session to check in to");
                return;
            }

            if (Globals.config.Value.state == 1)
            {
                await localContext.Channel.SendMessageAsync("Checkin has ended");
                return;
            }

            if (Globals.config.Value.gameMode == 1)
            {
                await BHP.PutLeaderboardUser(userInfo.Id, userInfo.Username);
                await localContext.Channel.SendMessageAsync($"{userInfo.Username} is now checked in");
            }
            else
            {
                await localContext.Channel.SendMessageAsync("There was an issue with checkin. Make sure you @ your teammate if you are checking in for 2v2.");
            }

        }

        [Command("checkin")]
        [Summary("Allows a user to checkin when a session starts")]
        public async Task CheckinAsync(SocketUser teammateInfo)
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is checking in with {teammateInfo.Username}");

            if (!Globals.config.HasValue)
            {
                await localContext.Channel.SendMessageAsync("There is no active session to check in to");
                return;
            }

            if (Globals.config.Value.state == 1)
            {
                await localContext.Channel.SendMessageAsync("Checkin has ended");
                return;
            }

            if (userInfo.Id == teammateInfo.Id)
            {
                await localContext.Channel.SendMessageAsync("You cannot check in with yourself");
                return;
            }

            if (Globals.config.Value.gameMode == 2)
            {
                await BHP.PutLeaderboardUser(userInfo.Id, userInfo.Username, teammateInfo.Id);

                LeaderboardUser? leaderboardUser = await BHP.GetLeaderboardUser(teammateInfo.Id);
                if (!leaderboardUser.HasValue)
                {
                    await localContext.Channel.SendMessageAsync($"{userInfo.Username} is now checked in. Their teammate must also check in for their team to properly be registered");
                }
                else
                {
                    if (leaderboardUser.Value.teammateId != userInfo.Id)
                    {
                        await localContext.Channel.SendMessageAsync($"{userInfo.Username} is now checked in. However, {teammateInfo.Username} is checked in without them. If {teammateInfo.Username} does not check in with {userInfo.Username} then {userInfo.Username} will not be registered into the tournament");
                    }
                    else
                    {
                        await localContext.Channel.SendMessageAsync($"{userInfo.Username} and {teammateInfo.Username} are now checked in as a team");
                    }
                    
                }
            }
            else
            {
                await localContext.Channel.SendMessageAsync("There was an issue with checkin. Make sure you @ your teammate if you are checking in for 2v2.");
            }

        }
    }

    public class LeaveModule : ModuleBase<SocketCommandContext>
    {
        public ICommandContext localContext;

        [Command("leave")]
        [Summary("Allows a user to leave midsession")]
        public async Task LeaveAsync([Remainder] string nonsense)
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave");

            if (await BHP.GetLeavePlayer(userInfo.Id))
            {
                LeaderboardUser? leaderboardUser = await BHP.GetLeaderboardUser(userInfo.Id);
                if (!leaderboardUser.HasValue)
                {
                    await localContext.Channel.SendMessageAsync("You are not even playing, you have nothing to leave.");
                    return;
                }

                List<ulong> playersToDelete = new List<ulong>(2)
                {
                    leaderboardUser.Value.id
                };

                if (leaderboardUser.Value.teammateId != 0)
                {
                    playersToDelete.Add(leaderboardUser.Value.teammateId);
                }

                await BHP.DeleteLeaderboardUsers(playersToDelete);

                if (playersToDelete.Count == 1)
                {
                    await localContext.Channel.SendMessageAsync($"<@{leaderboardUser.Value.id}> has been removed from the tournament");
                }
                else if (playersToDelete.Count == 2)
                {
                    await localContext.Channel.SendMessageAsync($"<@{leaderboardUser.Value.id}> and <@{leaderboardUser.Value.teammateId}> have been removed from the tournament");
                }
                else
                {
                    await localContext.Channel.SendMessageAsync("Hey <@106136559744466944> something went wrong");
                }
            }
            else
            {
                await BHP.PutLeave(userInfo.Id);
                await localContext.Channel.SendMessageAsync("Are you sure you want to leave the tournament? Type `leave` again to confirm. Type `cancel` to cancel");
            }
        }

        [Command("cancel")]
        [Summary("Cancels the last leave")]
        public async Task CancelLeaveAsync([Remainder] string nonsense)
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is cancelling leave");

            if (await BHP.GetLeavePlayer(userInfo.Id))
            {
                await BHP.DeleteLeave(userInfo.Id);
                await localContext.Channel.SendMessageAsync("Leave has been cancelled");
            }
        }
    }
}