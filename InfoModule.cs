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
using System.Security.Policy;

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

        public static ulong GetChannelId(int channelType)
        {
            switch (channelType)
            {
                case 0:
                    return 401167888762929153;
                case 1:
                    return 739957742323630082;
                case 2:
                    return 739957886754619442;
                default:
                    break;
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
        public async Task QueueJoinAsync([Remainder] string literallyAnythingElse)
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

            // Check for report type
            if (winner == "W" || winner == "w" || winner == "Y" || winner == "y")
            {
                Console.WriteLine($"{userInfo.Username} is reporting a win");
            }
            else if (winner == "L" || winner == "l" || winner == "N" || winner == "n")
            {
                Console.WriteLine($"{userInfo.Username} reported a loss");
                await localContext.Channel.SendMessageAsync("Only the winner may report the match");
                return;
            }
            else
            {
                Console.WriteLine($"{userInfo.Username} entered the wrong result type");
                await localContext.Channel.SendMessageAsync("Invalid results entered");
                return;
            }

            await ResolveMatchAsync(userInfo.Id, localContext.Channel);
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

                    HashSet<ulong> removedPlayers = await BHP.DeleteMatchesFromHistory(matchHistory);
                    await BHP.PutMatchFromHistory(matchHistory);

                    await localContext.Channel.SendMessageAsync("The last match has been reverted. Please report the match correctly now.");
                    if (removedPlayers.Count > 0)
                    {
                        string removedPlayersString = "";
                        foreach (ulong id in removedPlayers)
                        {
                            removedPlayersString += $"<@{id}> ";
                        }
                        await localContext.Channel.SendMessageAsync($"{removedPlayersString} - Due to a match revert, your most recent game has been cancelled");
                    }
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
                List<ulong> players = new List<ulong>(4)
                {
                    match.Value.id1,
                    match.Value.id2
                };
                if (match.Value.id3.Value != 0) players.Add(match.Value.id3.Value);
                if (match.Value.id4.Value != 0) players.Add(match.Value.id4.Value);

                string playersString = "";
                foreach (ulong id in players)
                {
                    playersString += $"<@{id}> ";
                }

                // Set room in match info
                await BHP.PutMatchRoom(userInfo.Id, match.Value.GetNum(userInfo.Id), (uint)room);
                await localContext.Channel.SendMessageAsync($"{userInfo.Username} set room number to {room} for match {playersString}");
            }
            else
            {
                await localContext.Channel.SendMessageAsync($"You are not currently in a match.");
            }
            await localContext.Message.DeleteAsync();
        }

        public static async Task ResolveMatchAsync(ulong userId, IMessageChannel channel)
        {
            try
            {
                // Get users' info
                Match? matchResult = await BHP.GetMatch(userId);
                if (!matchResult.HasValue)
                {
                    await channel.SendMessageAsync($"You are currently not in a match against anyone");
                    return;
                }
                Match match = matchResult.Value;

                List<ulong> playerIds = Globals.config.Value.gameMode == 1 ?
                    new List<ulong>() { match.id1, match.id2 } :
                    new List<ulong>() { match.id1, match.id2, (ulong)match.id3, (ulong)match.id4 };

                // Get player info
                Dictionary<ulong, LeaderboardUser> leaderboardUsers = await BHP.GetLeaderboardUsers(playerIds);

                // Get rid of old matches that could potentially be reverted for either player
                foreach (ulong id in playerIds) await BHP.DeleteMatchHistory(id);

                // Add this match to history
                await BHP.PutMatchHistory(playerIds, leaderboardUsers, userId, true);

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

                    // then another conditional if the player is the winner
                    int playerWinner = (playerIds[i] == userId || (Globals.config.Value.gameMode == 2 && playerIds[teammateIndex] == userId)) ? 1 : 0;

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
                await channel.SendMessageAsync("", embed: embed);


                // Update player's elo in leaderboard
                foreach (var leaderboardUser in leaderboardUsers)
                {
                    Console.WriteLine($"Giving {leaderboardUser.Value.username} {scores[leaderboardUser.Key]} points");
                    await BHP.UpdateLeaderboardUserScore(leaderboardUser.Key, scores[leaderboardUser.Key]);
                }

                await BHP.DeleteMatch(match.id1);
                Console.WriteLine($"Match with {userId} ended.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await channel.SendMessageAsync("Something went wrong <@106136559744466944>");
            }
        }
    }

    [Group("session")]
    [Alias("s")]
    public class SessionModule : ModuleBase<SocketCommandContext>
    {
        public ICommandContext localContext;

        [Command("start")]
        [Summary("Starts a session")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task StartSessionAsync(int gameMode, int checkinMinutes, int totalMinutes)
        {
            localContext = localContext ?? Context;
            Console.WriteLine($"Starting a {gameMode}v{gameMode} session for {totalMinutes} minutes with checkin {checkinMinutes} minutes");
            await BHP.BackupLeaderboard();
            await BHP.ClearConfig();

            GameConfig config = new GameConfig()
            {
                serverId = localContext.Guild.Id,
                gameMode = gameMode,
                startTime = DateTime.Now.Ticks,
                checkinTime = DateTime.Now.AddMinutes(checkinMinutes).Ticks,
                endTime = DateTime.Now.AddMinutes(checkinMinutes + totalMinutes).Ticks,
                state = 0
            };

            Globals.config = config;
            await BHP.PutConfig(config);

            string atTeammate = gameMode == 2 ? " @teammate" : "";
            await localContext.Channel.SendMessageAsync($"@everyone Starting a {gameMode}v{gameMode} session! Please use command `session join{atTeammate}` in the next {checkinMinutes} minutes to check in to the tournament.");
        }

        [Command("update")]
        [Summary("Updates the leaderboard manually")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ManualUpdateLeaderboardAsync()
        {
            localContext = localContext ?? Context;
            Console.WriteLine($"{localContext.User.Username} is manually updating leaderboard");

            if (await localContext.Guild.GetChannelAsync(HelperFunctions.GetChannelId(0)) is IMessageChannel leaderboardChannel)
            {
                IUserMessage message = await TimerService.GetMessageFromChannel(leaderboardChannel);
                if (message != null) await TimerService.UpdateLeaderboardAsync(message);
            }
        }

        [Command("join")]
        [Summary("Join the leaderboard for this session")]
        public async Task JoinAsync()
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is checking in");

            if (Globals.config.Value.state == -1)
            {
                Console.WriteLine("There is no active session to join");
                return;
            }

            if (Globals.config.Value.gameMode == 1)
            {
                await BHP.PutLeaderboardUser(userInfo.Id, userInfo.Username);
                await localContext.Channel.SendMessageAsync($"{userInfo.Username} joined the tournament");
            }
            else
            {
                await localContext.Channel.SendMessageAsync("There was an issue joining the tournament. Make sure you @ your teammate if you are checking in for 2v2.");
            }
        }

        [Command("join")]
        [Summary("Join the leaderboard for this session")]
        public async Task JoinAsync(SocketUser teammateInfo)
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is checking in with {teammateInfo.Username}");

            try
            {

                if (Globals.config.Value.state == -1)
                {
                    await localContext.Channel.SendMessageAsync("There is no active tournament to join");
                    return;
                }

                if (userInfo.Id == teammateInfo.Id)
                {
                    await localContext.Channel.SendMessageAsync("You cannot join with yourself");
                    return;
                }

                if (Globals.config.Value.gameMode == 2)
                {
                    LeaderboardUser oldUser = await BHP.GetLeaderboardUser(userInfo.Id);
                    await BHP.PutLeaderboardUser(userInfo.Id, userInfo.Username, teammateInfo.Id, false);

                    LeaderboardUser teammateUser = await BHP.GetLeaderboardUser(teammateInfo.Id);
                    if (teammateUser == null)
                    {
                        await localContext.Channel.SendMessageAsync($"{userInfo.Username} joined the tournament. Their teammate must also join for their team to properly be registered");
                    }
                    else
                    {
                        if (oldUser != null && oldUser.active)
                        {
                            await BHP.PutLeaderboardActivate(false, oldUser.teammateId);
                            await localContext.Channel.SendMessageAsync($"<@{oldUser.teammateId}> has been removed from the tournament due to their teammate switching teams, please rejoin with a new teammate to join back in to the tournament");
                        }

                        if (teammateUser.teammateId != userInfo.Id)
                        {
                            await localContext.Channel.SendMessageAsync($"{userInfo.Username} joined the tournament. However, {teammateInfo.Username} joined without them. If {teammateInfo.Username} does not join with {userInfo.Username} then {userInfo.Username} will not be registered into the tournament");
                        }
                        else
                        {
                            await BHP.PutLeaderboardActivate(true, userInfo.Id, teammateInfo.Id);
                            await localContext.Channel.SendMessageAsync($"{userInfo.Username} and {teammateInfo.Username} are now checked in as a team");
                        }

                    }
                }
                else
                {
                    await localContext.Channel.SendMessageAsync("There was an issue with joining. Make sure you @ your teammate if you are joining for 2v2.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await localContext.Channel.SendMessageAsync("Something went wrong <@106136559744466944>");
            }

        }

        [Command("leave")]
        [Summary("Allows a user to leave midsession")]
        public async Task LeaveAsync()
        {
            localContext = localContext ?? Context;
            var userInfo = localContext.User;
            Console.WriteLine($"{userInfo.Username} is attempting to leave");

            LeaderboardUser leaderboardUser = await BHP.GetLeaderboardUser(userInfo.Id);
            if (leaderboardUser == null)
            {
                await localContext.Channel.SendMessageAsync("You are not even playing, you have nothing to leave.");
                return;
            }

            List<ulong> playersToDelete = new List<ulong>(2)
            {
                leaderboardUser.id
            };

            if (leaderboardUser.teammateId != 0)
            {
                playersToDelete.Add(leaderboardUser.teammateId);
            }

            HashSet<ulong> players = new HashSet<ulong>(2)
            {
                leaderboardUser.id
            };
            if (playersToDelete.Count == 1)
            {
                await localContext.Channel.SendMessageAsync($"<@{leaderboardUser.id}> has been removed from the tournament");
            }
            else if (playersToDelete.Count == 2)
            {
                players.Add(leaderboardUser.teammateId);
                await localContext.Channel.SendMessageAsync($"<@{leaderboardUser.id}> and <@{leaderboardUser.teammateId}> have been removed from the tournament");
            }
            else
            {
                await localContext.Channel.SendMessageAsync("Hey <@106136559744466944> something went wrong");
                Console.WriteLine("Found wrong number of players to delete from a leave");
                return;
            }

            HashSet<ulong> removedPlayers = await BHP.GetIdsOfOpponentsInMatches(players);
            if (removedPlayers.Count > 0)
            {
                string removedPlayersString = "";
                foreach (ulong id in removedPlayers)
                {
                    await MatchModule.ResolveMatchAsync(id, localContext.Channel);
                    removedPlayersString += $"<@{id}> ";
                }
                await localContext.Channel.SendMessageAsync($"{removedPlayersString} - Due to a leaving opponent, your most recent game have been reported as wins");
            }

            if (playersToDelete.Count > 1)
            {
                await BHP.PutLeaderboardActivate(false, playersToDelete[0], playersToDelete[1]);
            }
            else
            {
                await BHP.PutLeaderboardActivate(false, playersToDelete[0]);
            }
            foreach (ulong playerId in playersToDelete) await BHP.DeleteLeave(playerId);
        }
    }
}