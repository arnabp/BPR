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
    public struct Role
    {
        public string region;
        public int gameMode;
        public int tier;
    }

    public static class HelperFunctions
    {
        public static async Task CheckSQLStateAsync()
        {
            while (Globals.conn.State != System.Data.ConnectionState.Closed)
            {
                await Task.Delay(100);
            }
        }

        public static async Task ExecuteSQLQueryAsync(string query)
        {
            await CheckSQLStateAsync();
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

        public static Role GetRoleRegion(ulong roleID)
        {
            if (Globals.roleList == null)
            {
                Globals.roleList = new Dictionary<ulong, Role>();
                Globals.roleList.Add(GetRoleId("NA", 1, 1), new Role { region = "NA", gameMode = 1, tier = 1 });
                Globals.roleList.Add(GetRoleId("NA", 1, 2), new Role { region = "NA", gameMode = 1, tier = 2 });
                Globals.roleList.Add(GetRoleId("NA", 1, 3), new Role { region = "NA", gameMode = 1, tier = 3 });
                Globals.roleList.Add(GetRoleId("NA", 2, 1), new Role { region = "NA", gameMode = 2, tier = 1 });
                Globals.roleList.Add(GetRoleId("NA", 2, 2), new Role { region = "NA", gameMode = 2, tier = 2 });
                Globals.roleList.Add(GetRoleId("NA", 2, 3), new Role { region = "NA", gameMode = 2, tier = 3 });
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

            return 0;
        }

        public static ulong GetChannelId(string region, int channelType)
        {
            // General
            if (channelType == 0) {
                if (region == "NA" || region == "EU") {
                    return 392829581192855554;
                }
                if (region == "AUS" || region == "SEA") {
                    return 422045385612328973;
                }
            }
            // 1v1 Queue Info
            if (channelType == 1) {
                if (region == "NA") {
                    return 401167888762929153;
                }
                if (region == "EU") {
                    return 525425865262104576;
                }
                if (region == "AUS") {
                    return 423372016922525697;
                }
                if (region == "SEA") {
                    return 529011508659748872;
                }
            }
            // 2v2 Queue Info
            if (channelType == 2) {
                if (region == "NA") {
                    return 404558855771521024;
                }
                if (region == "EU") {
                    return 525425886195875861;
                }
                if (region == "AUS") {
                    return 437510787951493121;
                }
                if (region == "SEA") {
                    return 529012003415654400;
                }
            }
            // Bank Status
            if (channelType == 3) {
                if (region == "NA") {
                    return 525950148581523466;
                }
                if (region == "EU") {
                    return 525950398297669639;
                }
                if (region == "AUS") {
                    return 529012722432737290;
                }
                if (region == "SEA") {
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

        public static async Task ResetDecayTimer(ulong id, string region, int gameMode)
        {
            // This function is depracated
            string query = $"UPDATE leaderboard{region}{gameMode} SET decaytimer = {DateTime.UtcNow.Ticks} WHERE id = {id};";
            await ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboard{region}{gameMode} SET decayed = -1 WHERE id = {id};";
            await ExecuteSQLQueryAsync(query);
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
        
        [Command("GetPlayerTier")]
        public async Task GetPlayerTier(string region, int gameMode, [Remainder] ulong id)
        {
            TierModule thisTier = TierModule.GetTierModule(region, gameMode);
            int tier = thisTier.getPlayerTier(id);

            await Context.Channel.SendMessageAsync($"Player tier is {tier}");
        }

        [Command("ClearQueue")]
        public async Task ClearQueue(string region, int gameMode)
        {
            await Context.Message.DeleteAsync();
            if (HelperFunctions.GetRoleId(region, gameMode, 1) == 0)
            {
                await Context.Channel.SendMessageAsync($"Incorrect region or game mode");
                return;
            }

            string query = $"TRUNCATE TABLE queue{region}{gameMode};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            await Context.Channel.SendMessageAsync($"{region} {gameMode}v{gameMode} queue cleared");
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

    [Group("queue1")]
    [Alias("q1")]
    public class Queue1Module : ModuleBase<SocketCommandContext>
    {

        [Command("join")]
        [Alias("j")]
        [Summary("Joins the 1v1 queue")]
        public async Task JoinAsync(int targetTier = 0)
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            Console.WriteLine($"{userInfo.Username} is attempting to join 1v1 queue with target {targetTier}");

            // Get User's information from Role
            string region = "";
            int roleTier = 0;
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 1)
                    {
                        region = thisRole.region;
                        roleTier = thisRole.tier;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if(region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }
            // Check if server has tiers
            bool tiersExist = Globals.regionList[region].tiers > 1;
            int queueTier = roleTier;
            // Check if role tier matches dictionary tier
            if (tiersExist)
            {
                int tier = TierModule.GetTierModule(region, 1).getPlayerTier(userInfo.Id);
                if (tier != roleTier)
                {
                    Console.WriteLine($"{userInfo.Username} had the wrong tier assigned to them, fixing.");
                    await TierModule.ChangeRoleToTier(Context, tier);
                }
                // Setting all non-standard tiers to default to user's tier
                if (targetTier < 1 || targetTier > 3)
                    targetTier = tier;
                // Check that user can expand queue to desired tier
                if (targetTier < tier)
                {
                    await Context.Channel.SendMessageAsync("You cannot expand queue to a tier higher than you. You will be queued into your tier.");
                    targetTier = tier;
                }
                // Convert tier and targetTier to binary tier queue number
                queueTier = TierModule.binaryOr(tier, targetTier);
            }
            
            bool isInQueue = false, isInMatch = false;
            int inLeaderboard = 0, oldTier = 0;

            // Check that user is in appropriate leaderboard
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
                await Context.Channel.SendMessageAsync("You are not on the leaderboard for this game mode. Please contact the server admin to fix this.");
                return;
            }

            // Check if user is already in the queue
            query = $"SELECT id, tier FROM queue{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == userInfo.Id)
                    {
                        isInQueue = true;
                        oldTier = reader.GetInt16(1);
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

            // Check if user is already in a match
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

            // Return messages to restrict user from joining queue
            if (isInQueue)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 1v1 queue has now refreshed their queue timer");

                query = $"UPDATE queue{region}1 SET time = {DateTime.UtcNow.Ticks} WHERE id = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                if (oldTier != queueTier)
                {
                    query = $"UPDATE queue{region}2 SET tier = {queueTier} WHERE id = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }

                Console.WriteLine($"{userInfo.Username} updated their queue");
            }
            else if (isInMatch)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 1v1 match tried to queue");
                Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
            }
            else
            {
                // Add user to queue
                query = $"INSERT INTO queue{region}1(time, username, id, tier) VALUES({DateTime.UtcNow.Ticks}, '{userInfo.Username}', {userInfo.Id}, {queueTier});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                await Context.Channel.SendMessageAsync($"A player has been added to {region} 1v1 queue");

                // Tell region that a user exists in queue
                if (!Globals.regionList[region].inQueue1)
                {
                    Region thisRegion = Globals.regionList[region];
                    thisRegion.inQueue1 = true;
                    Globals.regionList[region] = thisRegion;
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

            // Get User's information from Role
            string region = "";
            int tier = 0;
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 1)
                    {
                        region = thisRole.region;
                        tier = thisRole.tier;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }

            bool isInQueue = false;
            string query;

            // Check if user is already in queue
            if (Globals.regionList[region].inQueue1)
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

            // Return messages to indicate queue leave success
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

        private async Task NewMatch(int p1, int p2, string region)
        {
            long p1time = 0, p2time = 0;
            string p1name = "", p2name = "";
            ulong p1id = 0, p2id = 0;
            bool is1InQueue = false, is2InQueue = false;
            string query = $"SELECT time, username, id FROM queue{region}1;";
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

            query = $"SELECT id FROM queue{region}2;";
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
                query = $"DELETE FROM queue{region}2 WHERE id = {p1id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 2v2 queue");
            }

            if (is2InQueue)
            {
                query = $"DELETE FROM queue{region}2 WHERE id = {p2id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 2v2 queue");
            }

            query = $"INSERT INTO matches{region}1(id1, id2, username1, username2, time, reverted) VALUES({p1id}, {p2id}, '{p1name}', '{p2name}', {DateTime.UtcNow.Ticks}, 0);";
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
                    if (reader.GetUInt64(0) == p1id)
                    {
                        p1elo = reader.GetDouble(1);
                    }
                    else if (reader.GetUInt64(0) == p2id)
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

            if(p1elo > p2elo)
            {
                await Context.Channel.SendMessageAsync($"New match has started between <@{p1id}> and <@{p2id}>");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"New match has started between <@{p2id}> and <@{p1id}>");
            }
            
            Console.WriteLine($"{region} 1v1 Match #{matchCount} has started.");

            query = $"DELETE FROM queue{region}1 WHERE id = {p1id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}1 WHERE id = {p2id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync($"Please remember to add your room number with match1 room 00000");
        }
    }

    [Group("queue2")]
    [Alias("q2")]
    public class Queue2Module : ModuleBase<SocketCommandContext>
    {

        [Command("join")]
        [Alias("j")]
        [Summary("Joins the 2v2 queue")]
        public async Task JoinAsync(int targetTier = 0, int targetTeammateTier = 0)
        {
            var userInfo = Context.User;
            await Context.Message.DeleteAsync();
            Console.WriteLine($"{userInfo.Username} is attempting to join 2v2 queue with target {targetTier} and teammate {targetTeammateTier}");

            // Get User's information from Role
            string region = "";
            int roleTier = 0;
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 2)
                    {
                        region = thisRole.region;
                        roleTier = thisRole.tier;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }
            // Check if server has tiers
            bool tiersExist = Globals.regionList[region].tiers > 1;
            int queueTier = roleTier;
            int teammateTier = roleTier;
            // Check if role tier matches dictionary tier
            if (tiersExist)
            {
                int tier = TierModule.GetTierModule(region, 2).getPlayerTier(userInfo.Id);
                if (tier != roleTier)
                {
                    Console.WriteLine($"{userInfo.Username} had the wrong tier assigned to them, fixing.");
                    await TierModule.ChangeRoleToTier(Context, tier);
                }
                // Setting all non-standard tiers to default to user's tier
                if (targetTier < 1 || targetTier > 3)
                    targetTier = tier;
                if (targetTeammateTier < 1 || targetTeammateTier > 3)
                    targetTeammateTier = tier;
                // Check that user can expand queue to desired tier
                if (targetTier < tier)
                {
                    await Context.Channel.SendMessageAsync("You cannot expand queue to a tier higher than you. You will be queued into your tier.");
                    targetTier = tier;
                }
                if (targetTeammateTier < tier)
                {
                    await Context.Channel.SendMessageAsync("You cannot request a teammate with a tier higher than you. You will automatically get a teammate higher than you if they have requested this.");
                    targetTeammateTier = tier;
                }
                // Convert tier and targetTier to binary tier queue number
                queueTier = TierModule.binaryOr(tier, targetTier);
                teammateTier = TierModule.binaryOr(tier, targetTeammateTier);
            }

            bool isInQueue = false, isInMatch = false;
            int inLeaderboard = 0, oldTier = 0, oldTeammateTier = 0;

            // Check that user is in appropriate leaderboard
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
                await Context.Channel.SendMessageAsync("You are not on the leaderboard for this game mode. Please contact the server admin to fix this.");
                return;
            }

            // Check if user is already in the queue
            query = $"SELECT id, tier, tierTeammate FROM queue{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetUInt64(0) == userInfo.Id)
                    {
                        isInQueue = true;
                        oldTier = reader.GetInt16(1);
                        oldTeammateTier = reader.GetInt16(2);
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
            
            // Check if user is already in a match
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

            // Return messages to restrict user from joining queue
            if (isInQueue)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 2v2 queue has now refreshed their queue timer");

                query = $"UPDATE queue{region}2 SET time = {DateTime.UtcNow.Ticks} WHERE id = {userInfo.Id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                if (oldTier != queueTier)
                {
                    query = $"UPDATE queue{region}2 SET tier = {queueTier} WHERE id = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }

                if (oldTeammateTier != teammateTier)
                {
                    query = $"UPDATE queue{region}2 SET tierTeammate = {teammateTier} WHERE id = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }

                Console.WriteLine($"{userInfo.Username} updated their queue");
            }
            else if (isInMatch)
            {
                await Context.Channel.SendMessageAsync($"Player already in {region} 2v2 match tried to queue");
                Console.WriteLine($"{userInfo.Username} tried to join the queue while in match");
            }
            else
            {
                query = $"INSERT INTO queue{region}2(time, username, id, tier, tierTeammate) VALUES({DateTime.UtcNow.Ticks}, '{userInfo.Username}', {userInfo.Id}, {queueTier}, {teammateTier});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                await Context.Channel.SendMessageAsync($"A player has been added to {region} 2v2 queue");

                // Tell region that a user exists in queue
                if (!Globals.regionList[region].inQueue2)
                {
                    Region thisRegion = Globals.regionList[region];
                    thisRegion.inQueue2 = true;
                    Globals.regionList[region] = thisRegion;
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

            // Get User's information from Role
            string region = "";
            int tier = 0;
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 2)
                    {
                        region = thisRole.region;
                        tier = thisRole.tier;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }


            bool isInQueue = false;
            string query;

            if (Globals.regionList[region].inQueue2)
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

            // Return messages to indicate queue leave success
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

        private async Task NewMatch(int p1, int p2, int p3, int p4, string region, bool randomize)
        {
            if (randomize)
            {
                int[] playerPositions = new int[4] { p1, p2, p3, p4 };
                Random rnd = new Random();
                int[] newplayerPositions = playerPositions.OrderBy(x => rnd.Next()).ToArray();
                p1 = newplayerPositions[0];
                p2 = newplayerPositions[1];
                p3 = newplayerPositions[2];
                p4 = newplayerPositions[3];
            }

            long p1time = 0, p2time = 0, p3time = 0, p4time = 0;
            string p1name = "", p2name = "", p3name = "", p4name = "";
            ulong p1id = 0, p2id = 0, p3id = 0, p4id = 0;
            bool is1InQueue = false, is2InQueue = false, is3InQueue = false, is4InQueue = false;
            string query = $"SELECT time, username, id FROM queue{region}2 ORDER BY time;";
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

            query = $"SELECT id FROM queue{region}1;";
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
                query = $"DELETE FROM queue{region}1 WHERE id = {p1id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }
            if (is2InQueue)
            {
                query = $"DELETE FROM queue{region}1 WHERE id = {p2id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }
            if (is3InQueue)
            {
                query = $"DELETE FROM queue{region}1 WHERE id = {p3id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }
            if (is4InQueue)
            {
                query = $"DELETE FROM queue{region}1 WHERE id = {p4id};";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                await Context.Channel.SendMessageAsync($"A player has left the {region} 1v1 queue");
            }

            query = $"INSERT INTO matches{region}2(id1, id2, id3, id4, username1, username2, username3, username4, time, reverted) " +
                $"VALUES({p1id}, {p2id}, {p3id}, {p4id}, '{p1name}', '{p2name}', '{p3name}', '{p4name}', {DateTime.UtcNow.Ticks}, 0);";
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
            
            Console.WriteLine($"{region} 2v2 Match #{matchCount} has started.");

            query = $"DELETE FROM queue{region}2 WHERE id = {p1id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}2 WHERE id = {p2id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}2 WHERE id = {p3id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM queue{region}2 WHERE id = {p4id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync($"Please remember to add your room number with match2 room 00000");
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

            // Get User's information from Role
            string region = "";
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 1)
                    {
                        region = thisRole.region;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }

            // Get users' info
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

            // Indicate which player won
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

            // Debugging line that doubles as in indicator of a 1v1 match being logged
            Console.WriteLine($"isP1: {isP1} in {region}");

            // Create tuple for storing results
            var results = new Tuple<double, double>(0, 0);
            double p1elo = 0, p2elo = 0;

            // Get player elo
            query = $"SELECT id, elo FROM leaderboard{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetDouble(0) == p1ID) p1elo = reader.GetDouble(1);
                    if (reader.GetDouble(0) == p2ID) p2elo = reader.GetDouble(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Globals.conn.CloseAsync();
                throw;
            }
            await Globals.conn.CloseAsync();

            // Get rid of old matches that could potentially be reverted for either player
            query = $"DELETE FROM matchesHistory1 WHERE id1 = {p1ID} OR id2 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory1 WHERE id1 = {p2ID} OR id2 = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            // Add this match to history
            query = $"INSERT INTO matchesHistory1(id1, id2, oldElo1, oldElo2, isP1, region, username1, username2, reporter) " +
                $"VALUES({p1ID}, {p2ID}, {p1elo}, {p2elo}, {isP1}, '{region}', '{p1Username}', '{p2Username}', {userInfo.Id});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            // Get new elo changes
            results = EloConvert(p1elo, p2elo, (bool)isP1);

            // Get new elo amounts
            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;

            // Print results in embed
            var embed = new EmbedBuilder
            {
                Title = "Match Result",
                Color = HelperFunctions.GetRegionColor(region)
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

            // Update player's elo in leaderboard
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

            // Check for any tier changes
            TierModule thisTierModule = TierModule.GetTierModule(region, 1);
            thisTierModule.PlayerEloChange(p1ID, new1);
            thisTierModule.PlayerEloChange(p2ID, new2);
            thisTierModule.NormalizeTiers();
            await TierModule.AnnounceTierChanges(Context);

            // Add banked game
            query = $"UPDATE leaderboard{region}1 SET bank = bank + 1 WHERE id = {p1ID} AND bank < 7;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}1 SET bank = bank + 1 WHERE id = {p2ID} AND bank < 7;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            // Remove the match from the matches list
            query = $"DELETE FROM matches{region}1 WHERE id1 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            Console.WriteLine($"{region} 1v1 Match #{thisMatchNum} has ended.");
        }

        [Command("revert")]
        [Summary("Reverts the last reported match for a player")]
        public async Task RevertMatchAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reverting a match");
            int thisMatchNum = 1, revertRequests = 0, thisPlayerNum = 0;
            bool hasAlreadyReverted = false;
            
            // Get users' info
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
                if (thisPlayerNum == 0)
                {
                    await Context.Channel.SendMessageAsync("You have not had a match recently that can be reverted.");
                    return;
                }
                else await Context.Channel.SendMessageAsync($"{revertRequests + 1}/2 players have requested the last 1v1 match to be reverted");

                if (revertRequests < 1)
                {
                    // Add indication that a player has requested a revert
                    query = $"UPDATE matchesHistory1 SET revert{thisPlayerNum} = 1 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
                else
                {
                    // Now revert the match
                    ulong p1ID = 0, p2ID = 0, reporter = 0;
                    double p1elo = 0, p2elo = 0;
                    int isP1 = 0;
                    string region = "", p1Username = "", p2Username = "";

                    // Get info of match from history
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

                    // Revert elo
                    query = $"UPDATE leaderboard{region}1 SET elo = {p1elo} WHERE id = {p1ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}1 SET elo = {p2elo} WHERE id = {p2ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    // Readd match to matches
                    query = $"INSERT INTO matches{region}1(id1, id2, username1, username2, time, reverted) VALUES({p1ID}, {p2ID}, '{p1Username}', '{p2Username}', {DateTime.UtcNow.Ticks}, {reporter});";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    // Find winner of reverted match
                    string p1ResultString = "wins";
                    string p2ResultString = "loss";
                    if (isP1 == 0)
                    {
                        p1ResultString = "loss";
                        p2ResultString = "wins";
                    }

                    // Revert to previous tiers
                    TierModule thisTierModule = TierModule.GetTierModule(region, 1);
                    thisTierModule.PlayerEloChange(p1ID, p1elo);
                    thisTierModule.PlayerEloChange(p2ID, p2elo);
                    thisTierModule.NormalizeTiers();
                    await TierModule.AnnounceTierChanges(Context);

                    // Revert banked game
                    query = $"UPDATE leaderboard{region}1 SET bank = bank - 1 WHERE id = {p1ID} AND bank > 0;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}1 SET bank = bank - 1 WHERE id = {p2ID} AND bank > 0;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    // Revert wincount
                    query = $"UPDATE leaderboard{region}1 SET {p1ResultString} = {p1ResultString} - 1 WHERE id = {p1ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}1 SET {p2ResultString} = {p2ResultString} - 1 WHERE id = {p2ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    await Context.Channel.SendMessageAsync("The last 1v1 match has been reverted. Please report the match correctly now.");
                }
            }
        }

        [Command("cancel")]
        [Summary("Cancels the match the player is currently in")]
        public async Task CancelMatchAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is canceling a match");
            int thisMatchNum = 1, cancelRequests = 0, thisPlayerNum = 0;
            bool hasAlreadyCancelled = false;

            // Get User's information from Role
            string region = "";
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 1)
                    {
                        region = thisRole.region;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }

            // Get player info
            string query = $"SELECT id1, id2, cancel1, cancel2 FROM matches{region}1;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (userInfo.Id == reader.GetUInt64(0))
                    {
                        cancelRequests += reader.GetInt16(2);
                        cancelRequests += reader.GetInt16(3);
                        if (reader.GetInt16(2) == 1) hasAlreadyCancelled = true;
                        thisPlayerNum = 1;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(1))
                    {
                        cancelRequests += reader.GetInt16(2);
                        cancelRequests += reader.GetInt16(3);
                        if (reader.GetInt16(3) == 1) hasAlreadyCancelled = true;
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

            if (hasAlreadyCancelled)
            {
                await Context.Channel.SendMessageAsync("A player tried to cancel the match twice.");
            }
            else
            {
                if (thisPlayerNum == 0)
                {
                    await Context.Channel.SendMessageAsync("You are not in a match that can be cancelled.");
                    return;
                }
                else await Context.Channel.SendMessageAsync($"{cancelRequests + 1}/2 players have requested the last 1v1 match to be cancelled");

                if (cancelRequests < 1)
                {
                    // Add indication that a player has requested a cancel
                    query = $"UPDATE matches{region}1 SET cancel{thisPlayerNum} = 1 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
                else
                {
                    // Now cancel the match
                    query = $"DELETE FROM matches{region}1 WHERE id1 = {userInfo.Id} OR id2 = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    await Context.Channel.SendMessageAsync("The last 1v1 match has been cancelled. You may now queue again.");
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

        [Command("room")]
        [Summary("Adds room number to match info")]
        public async Task AddRoomNumberAsync([Remainder] int room)
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is adding a room number");
            int thisMatchNum = 1, idnum = 0;
            bool isInMatch = false;

            // Get User's information from Role
            string region = "";
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 1)
                    {
                        region = thisRole.region;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }

            // Get player info
            string query = $"SELECT id1, id2 FROM matches{region}1;";
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
                // Set room in match info
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

            double change1 = (32 * (Convert.ToDouble(isP1) - expected1)) + 1;
            double change2 = (32 * (Convert.ToDouble(!isP1) - expected2)) + 1;

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

            // Get User's information from Role
            string region = "";
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 2)
                    {
                        region = thisRole.region;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }

            // Get user's info
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

            // Indicate which team won
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

            // Debugging line that doubles as an indicator of a 2v2 match being logged
            Console.WriteLine($"isT1: {isT1} in {region}");

            // Create tuple for storing results
            var results = new Tuple<double, double, double, double>(0, 0, 0, 0);
            double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0;

            // Get player elo
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

            // Get rid of old matches that couple potentially be reverted for any player
            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p1ID} OR id2 = {p1ID} OR id3 = {p1ID} OR id4 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p2ID} OR id2 = {p2ID} OR id3 = {p2ID} OR id4 = {p2ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p3ID} OR id2 = {p3ID} OR id3 = {p3ID} OR id4 = {p3ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"DELETE FROM matchesHistory2 WHERE id1 = {p4ID} OR id2 = {p4ID} OR id3 = {p4ID} OR id4 = {p4ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            // Add this match to history
            query = $"INSERT INTO matchesHistory2(id1, id2, id3, id4, oldElo1, oldElo2, oldElo3, oldElo4, isT1, region, username1, username2, username3, username4, reporter) " +
                $"VALUES({p1ID}, {p2ID}, {p3ID}, {p4ID}, {p1elo}, {p2elo}, {p3elo}, {p4elo}, {isT1}, '{region}', '{p1Username}', '{p2Username}', '{p3Username}', '{p4Username}', {userInfo.Id});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            // Get new elo changes
            results = EloConvert(p1elo, p2elo, p3elo, p4elo, (bool)isT1);

            // Great new elo amounts
            double new1 = p1elo + results.Item1;
            double new2 = p2elo + results.Item2;
            double new3 = p3elo + results.Item3;
            double new4 = p4elo + results.Item4;

            // Print results in embed
            var embed = new EmbedBuilder
            {
                Title = "Match Result",
                Color = HelperFunctions.GetRegionColor(region)
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

            // Update player's elo in leaderboard
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

            // Check for any tier changes
            TierModule thisTierModule = TierModule.GetTierModule(region, 2);
            thisTierModule.PlayerEloChange(p1ID, new1);
            thisTierModule.PlayerEloChange(p2ID, new2);
            thisTierModule.PlayerEloChange(p3ID, new3);
            thisTierModule.PlayerEloChange(p4ID, new4);
            thisTierModule.NormalizeTiers();
            await TierModule.AnnounceTierChanges(Context);

            // Add banked game
            query = $"UPDATE leaderboard{region}2 SET bank = bank + 1 WHERE id = {p1ID} AND bank < 7;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}2 SET bank = bank + 1 WHERE id = {p2ID} AND bank < 7;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}2 SET bank = bank + 1 WHERE id = {p3ID} AND bank < 7;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            query = $"UPDATE leaderboard{region}2 SET bank = bank + 1 WHERE id = {p4ID} AND bank < 7;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            // Remove the match from the matches list
            query = $"DELETE FROM matches{region}2 WHERE id1 = {p1ID};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);
            Console.WriteLine($"Match #{thisMatchNum} has ended.");
        }

        [Command("revert")]
        [Summary("Reverts the last reported match for a player")]
        public async Task RevertMatchAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is reverting a match");
            int thisMatchNum = 1, revertRequests = 0, thisPlayerNum = 0;
            bool hasAlreadyReverted = false;

            // Get users' info
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
                if(thisPlayerNum == 0)
                {
                    await Context.Channel.SendMessageAsync("You have not had a match recently that can be reverted.");
                    return;
                }
                else await Context.Channel.SendMessageAsync($"{revertRequests + 1}/3 players have requested the last 2v2 match to be reverted");

                if (revertRequests < 2)
                {
                    // Add indication that a player has requested a revert
                    query = $"UPDATE matchesHistory2 SET revert{thisPlayerNum} = 1 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
                else
                {
                    // Now revert the match
                    ulong p1ID = 0, p2ID = 0, p3ID = 0, p4ID = 0;
                    double p1elo = 0, p2elo = 0, p3elo = 0, p4elo = 0, reporter = 0;
                    int isT1 = 0;
                    string region = "", p1Username = "", p2Username = "", p3Username = "", p4Username = "";

                    // Get info of the match from history
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

                    // Revert elo
                    query = $"UPDATE leaderboard{region}2 SET elo = {p1elo} WHERE id = {p1ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}2 SET elo = {p2elo} WHERE id = {p2ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}2 SET elo = {p3elo} WHERE id = {p3ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    query = $"UPDATE leaderboard{region}2 SET elo = {p4elo} WHERE id = {p4ID};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    // Readd match to matches
                    query = $"INSERT INTO matches{region}2(id1, id2, id3, id4, username1, username2, username3, username4, time, reverted) " +
                        $"VALUES({p1ID}, {p2ID}, {p3ID}, {p4ID}, '{p1Username}', '{p2Username}', '{p3Username}', '{p4Username}', {DateTime.UtcNow.Ticks}, {reporter});";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    // Find winner of reverted match
                    string t1ResultString = "wins";
                    string t2ResultString = "loss";
                    if (isT1 == 0)
                    {
                        t1ResultString = "loss";
                        t2ResultString = "wins";
                    }

                    // Rever to previous tiers
                    TierModule thisTierModule = TierModule.GetTierModule(region, 2);
                    thisTierModule.PlayerEloChange(p1ID, p1elo);
                    thisTierModule.PlayerEloChange(p2ID, p2elo);
                    thisTierModule.PlayerEloChange(p3ID, p3elo);
                    thisTierModule.PlayerEloChange(p4ID, p4elo);
                    thisTierModule.NormalizeTiers();
                    await TierModule.AnnounceTierChanges(Context);

                    // Add banked game
                    query = $"UPDATE leaderboard{region}2 SET bank = bank - 1 WHERE id = {p1ID} AND bank > 0;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}2 SET bank = bank - 1 WHERE id = {p2ID} AND bank > 0;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}2 SET bank = bank - 1 WHERE id = {p3ID} AND bank > 0;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                    query = $"UPDATE leaderboard{region}2 SET bank = bank - 1 WHERE id = {p4ID} AND bank > 0;";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    // Revert wincount
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

        [Command("cancel")]
        [Summary("Cancels the last match for a player")]
        public async Task CancelMatchAsync()
        {
            var userInfo = Context.User;
            Console.WriteLine($"{userInfo.Username} is cancelling a match");
            int thisMatchNum = 1, cancelRequests = 0, thisPlayerNum = 0;
            bool hasAlreadyCancelled = false;

            // Get User's information from Role
            string region = "";
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 2)
                    {
                        region = thisRole.region;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }

            // Get player info
            string query = $"SELECT id1, id2, id3, id4, cancel1, cancel2, cancel3, cancel4 FROM matches{region}2;";
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (userInfo.Id == reader.GetUInt64(0))
                    {
                        cancelRequests += reader.GetInt16(4);
                        cancelRequests += reader.GetInt16(5);
                        cancelRequests += reader.GetInt16(6);
                        cancelRequests += reader.GetInt16(7);
                        if (reader.GetInt16(4) == 1) hasAlreadyCancelled = true;
                        thisPlayerNum = 1;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(1))
                    {
                        cancelRequests += reader.GetInt16(4);
                        cancelRequests += reader.GetInt16(5);
                        cancelRequests += reader.GetInt16(6);
                        cancelRequests += reader.GetInt16(7);
                        if (reader.GetInt16(5) == 1) hasAlreadyCancelled = true;
                        thisPlayerNum = 2;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(2))
                    {
                        cancelRequests += reader.GetInt16(4);
                        cancelRequests += reader.GetInt16(5);
                        cancelRequests += reader.GetInt16(6);
                        cancelRequests += reader.GetInt16(7);
                        if (reader.GetInt16(6) == 1) hasAlreadyCancelled = true;
                        thisPlayerNum = 3;
                        break;
                    }
                    else if (userInfo.Id == reader.GetUInt64(3))
                    {
                        cancelRequests += reader.GetInt16(4);
                        cancelRequests += reader.GetInt16(5);
                        cancelRequests += reader.GetInt16(6);
                        cancelRequests += reader.GetInt16(7);
                        if (reader.GetInt16(7) == 1) hasAlreadyCancelled = true;
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

            if (hasAlreadyCancelled)
            {
                await Context.Channel.SendMessageAsync("A player tried to cancel a match twice.");
            }
            else
            {
                if (thisPlayerNum == 0)
                {
                    await Context.Channel.SendMessageAsync("You are not in a match that can be cancelled.");
                    return;
                }
                else await Context.Channel.SendMessageAsync($"{cancelRequests + 1}/3 players have requested the last 2v2 match to be cancelled");

                if (cancelRequests < 2)
                {
                    // Add indication that a player has requested a cancel
                    query = $"UPDATE matches{region}2 SET cancel{thisPlayerNum} = 1 WHERE id{thisPlayerNum} = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);
                }
                else
                {
                    // Now cancel the match
                    query = $"DELETE FROM matches{region}2 WHERE id1 = {userInfo.Id} OR id2 = {userInfo.Id} OR id3 = {userInfo.Id} OR id4 = {userInfo.Id};";
                    await HelperFunctions.ExecuteSQLQueryAsync(query);

                    await Context.Channel.SendMessageAsync("The last 2v2 match has been cancelled. You may now queue again.");
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

            // Get User's information from Role
            string region = "";
            foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == 2)
                    {
                        region = thisRole.region;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
            // Check if user is qualified to queue
            if (region == "")
            {
                await Context.Channel.SendMessageAsync($"You are not currently in a region. Please check that you have been added to a leaderboard.");
                return;
            }
            if (!Globals.regionList[region].status)
            {
                await Context.Channel.SendMessageAsync("The season has ended. Please wait for the new season to begin before queueing");
                return;
            }

            // Get player info
            string query = $"SELECT id1, id2, id3, id4 FROM matches{region}2;";
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
                // Set room in match info
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

            double change1 = (32 * (Convert.ToDouble(isT1) - expected1)) + 1;
            double change2 = (32 * (Convert.ToDouble(isT1) - expected2)) + 1;
            double change3 = (32 * (Convert.ToDouble(!isT1) - expected3)) + 1;
            double change4 = (32 * (Convert.ToDouble(!isT1) - expected4)) + 1;

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
        public async Task AddLeaderbardAsync(ulong id, int tier)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            if (tier < 1 || tier > 3)
            {
                await Context.Channel.SendMessageAsync($"Entered invalid tier");
                return;
            }

            double elo = 0;
            if (tier == 1)
                elo = TierModule.SEED1ELO;
            else if (tier == 2)
                elo = TierModule.SEED2ELO;
            else
                elo = TierModule.SEED3ELO;

            string query = $"INSERT INTO leaderboardNA1(id, username, elo) VALUES({id}, '{user.Username}', {elo});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(HelperFunctions.GetRoleId("NA", 1, 3)));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the NA 1v1 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("auto")]
        [Summary("Automatically adds users to the leaderboard based on their roles")]
        public async Task AutoLeaderboardAsync()
        {
            await Context.Message.DeleteAsync();

            foreach (var user in Context.Guild.Users)
            {
                int roleTier = 0;
                foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(user.Id).Roles)
                {
                    try
                    {
                        Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                        if (thisRole.gameMode == 1 && thisRole.region == "NA")
                        {
                            roleTier = thisRole.tier;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }

                if (roleTier == 0)
                    continue;

                double elo = 0;
                if (roleTier == 1)
                    elo = TierModule.SEED1ELO;
                else if (roleTier == 2)
                    elo = TierModule.SEED2ELO;
                else
                    elo = TierModule.SEED3ELO;

                string query = $"INSERT INTO leaderboardNA1(id, username, elo) VALUES({user.Id}, '{user.Username}', {elo});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                
                Console.WriteLine($"{user.Username} has been registered");
            }
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

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardNA1 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardNA1 SET decaytimer = {DateTime.UtcNow.Ticks};";
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
        public async Task AddLeaderbardAsync(ulong id, int tier)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            if (tier < 1 || tier > 3)
            {
                await Context.Channel.SendMessageAsync($"Entered invalid tier");
                return;
            }

            double elo = 0;
            if (tier == 1)
                elo = TierModule.SEED1ELO;
            else if (tier == 2)
                elo = TierModule.SEED2ELO;
            else
                elo = TierModule.SEED3ELO;

            string query = $"INSERT INTO leaderboardNA2(id, username, elo) VALUES({id}, '{user.Username}', {elo});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(HelperFunctions.GetRoleId("NA", 2, 3)));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the NA 2v2 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("auto")]
        [Summary("Automatically adds users to the leaderboard based on their roles")]
        public async Task AutoLeaderboardAsync()
        {
            await Context.Message.DeleteAsync();

            foreach (var user in Context.Guild.Users)
            {
                int roleTier = 0;
                foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(user.Id).Roles)
                {
                    try
                    {
                        Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                        if (thisRole.gameMode == 2 && thisRole.region == "NA")
                        {
                            roleTier = thisRole.tier;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }

                if (roleTier == 0)
                    continue;

                double elo = 0;
                if (roleTier == 1)
                    elo = TierModule.SEED1ELO;
                else if (roleTier == 2)
                    elo = TierModule.SEED2ELO;
                else
                    elo = TierModule.SEED3ELO;

                string query = $"INSERT INTO leaderboardNA2(id, username, elo) VALUES({user.Id}, '{user.Username}', {elo});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);
                
                Console.WriteLine($"{user.Username} has been registered");
            }
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

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardNA2 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardNA2 SET decaytimer = {DateTime.UtcNow.Ticks};";
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
        public async Task AddLeaderbardAsync(ulong id, int tier)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            if (tier < 1 || tier > 3)
            {
                await Context.Channel.SendMessageAsync($"Entered invalid tier");
                return;
            }

            double elo = 0;
            if (tier == 1)
                elo = TierModule.SEED1ELO;
            else if (tier == 2)
                elo = TierModule.SEED2ELO;
            else
                elo = TierModule.SEED3ELO;

            string query = $"INSERT INTO leaderboardEU1(id, username, elo) VALUES({id}, '{user.Username}', {elo});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(HelperFunctions.GetRoleId("EU", 1, 3)));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the EU 1v1 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("auto")]
        [Summary("Automatically adds users to the leaderboard based on their roles")]
        public async Task AutoLeaderboardAsync()
        {
            await Context.Message.DeleteAsync();

            foreach (var user in Context.Guild.Users)
            {
                int roleTier = 0;
                foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(user.Id).Roles)
                {
                    try
                    {
                        Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                        if (thisRole.gameMode == 1 && thisRole.region == "EU")
                        {
                            roleTier = thisRole.tier;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }

                if (roleTier == 0)
                    continue;

                double elo = 0;
                if (roleTier == 1)
                    elo = TierModule.SEED1ELO;
                else if (roleTier == 2)
                    elo = TierModule.SEED2ELO;
                else
                    elo = TierModule.SEED3ELO;

                string query = $"INSERT INTO leaderboardEU1(id, username, elo) VALUES({user.Id}, '{user.Username}', {elo});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                Console.WriteLine($"{user.Username} has been registered");
            }
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

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardEU1 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardEU1 SET decaytimer = {DateTime.UtcNow.Ticks};";
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
        public async Task AddLeaderbardAsync(ulong id, int tier)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            if (tier < 1 || tier > 3)
            {
                await Context.Channel.SendMessageAsync($"Entered invalid tier");
                return;
            }

            double elo = 0;
            if (tier == 1)
                elo = TierModule.SEED1ELO;
            else if (tier == 2)
                elo = TierModule.SEED2ELO;
            else
                elo = TierModule.SEED3ELO;

            string query = $"INSERT INTO leaderboardEU2(id, username, elo) VALUES({id}, '{user.Username}', {elo});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(HelperFunctions.GetRoleId("EU", 2, 3)));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the EU 2v2 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("auto")]
        [Summary("Automatically adds users to the leaderboard based on their roles")]
        public async Task AutoLeaderboardAsync()
        {
            await Context.Message.DeleteAsync();

            foreach (var user in Context.Guild.Users)
            {
                int roleTier = 0;
                foreach (Discord.WebSocket.SocketRole role in Context.Guild.GetUser(user.Id).Roles)
                {
                    try
                    {
                        Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                        if (thisRole.gameMode == 2 && thisRole.region == "EU")
                        {
                            roleTier = thisRole.tier;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }

                if (roleTier == 0)
                    continue;

                double elo = 0;
                if (roleTier == 1)
                    elo = TierModule.SEED1ELO;
                else if (roleTier == 2)
                    elo = TierModule.SEED2ELO;
                else
                    elo = TierModule.SEED3ELO;

                string query = $"INSERT INTO leaderboardEU2(id, username, elo) VALUES({user.Id}, '{user.Username}', {elo});";
                await HelperFunctions.ExecuteSQLQueryAsync(query);

                Console.WriteLine($"{user.Username} has been registered");
            }
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

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardEU2 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardEU2 SET decaytimer = {DateTime.UtcNow.Ticks};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for EU 2v2 leaderboard");
        }
    }

    [Group("leaderboardAUS1")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardAUS1Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardAUS1(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.UtcNow.Ticks});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(423095293039607809));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the AUS 1v1 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardAUS1 WHERE id = {id};";
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
            query = $"DELETE FROM leaderboardAUS1 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(423095293039607809));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the 1v1 leaderboards");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardAUS1 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardAUS1 SET decaytimer = {DateTime.UtcNow.Ticks};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for AUS 1v1 leaderboard");
        }
    }

    [Group("leaderboardAUS2")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardAUS2Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardAUS2(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.UtcNow.Ticks});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(423095293039607809));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the AUS 2v2 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardAUS2 WHERE id = {id};";
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
            query = $"DELETE FROM leaderboardAUS2 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(423095293039607809));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the AUS 2v2 leaderboards");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardAUS2 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardAUS2 SET decaytimer = {DateTime.UtcNow.Ticks};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for AUS 2v2 leaderboard");
        }
    }

    [Group("leaderboardSEA1")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardSEA1Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardSEA1(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.UtcNow.Ticks});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(423095346131107853));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the SEA 1v1 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardSEA1 WHERE id = {id};";
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
            query = $"DELETE FROM leaderboardSEA1 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(423095346131107853));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the 1v1 leaderboards");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardSEA1 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardSEA1 SET decaytimer = {DateTime.UtcNow.Ticks};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for SEA 1v1 leaderboard");
        }
    }

    [Group("leaderboardSEA2")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardSEA2Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardSEA2(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.UtcNow.Ticks});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(423095346131107853));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the SEA 2v2 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardSEA2 WHERE id = {id};";
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
            query = $"DELETE FROM leaderboardSEA2 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(423095346131107853));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the SEA 2v2 leaderboards");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardSEA2 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardSEA2 SET decaytimer = {DateTime.UtcNow.Ticks};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for SEA 2v2 leaderboard");
        }
    }

    [Group("leaderboardBRZ1")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardBRZ1Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardBRZ1(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.UtcNow.Ticks});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(410986909507256320));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the BRZ 1v1 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardBRZ1 WHERE id = {id};";
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
            query = $"DELETE FROM leaderboardBRZ1 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(410986909507256320));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the 1v1 leaderboards");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardBRZ1 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardBRZ1 SET decaytimer = {DateTime.UtcNow.Ticks};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for BRZ 1v1 leaderboard");
        }
    }

    [Group("leaderboardBRZ2")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LeaderboardBRZ2Module : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Adds new users to the leaderboard")]
        public async Task AddLeaderbardAsync(ulong id)
        {
            var user = Context.Guild.GetUser(id);
            await Context.Message.DeleteAsync();

            string query = $"INSERT INTO leaderboardBRZ2(id, username, decaytimer) VALUES({id}, '{user.Username}', {DateTime.UtcNow.Ticks});";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await user.AddRoleAsync(Context.Guild.GetRole(410986909507256320));

            await Context.Channel.SendMessageAsync($"{user.Username} has been succesfully registered to the BRZ 2v2 leaderboard!");
            Console.WriteLine($"{user.Username} has been registered");
        }

        [Command("delete")]
        [Summary("Allows admin to delete user from leaderboard")]
        public async Task DeleteLeaderboardAsync([Remainder] ulong id)
        {
            var user = Context.Guild.GetUser(id);
            string username = "";
            string query = $"SELECT username FROM leaderboardBRZ2 WHERE id = {id};";
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
            query = $"DELETE FROM leaderboardBRZ2 WHERE id = {id};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            if (user != null) await user.RemoveRoleAsync(Context.Guild.GetRole(410986909507256320));

            await Context.Channel.SendMessageAsync($"{username} has been deleted from the BRZ 2v2 leaderboards");
        }

        [Command("refresh")]
        [Summary("Resets the elo decay for all people in this leaderboard")]
        public async Task RefreshDecayTimerAsync()
        {
            await Context.Message.DeleteAsync();

            string query = $"UPDATE leaderboardBRZ2 SET decayed = -1;";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            query = $"UPDATE leaderboardBRZ2 SET decaytimer = {DateTime.UtcNow.Ticks};";
            await HelperFunctions.ExecuteSQLQueryAsync(query);

            await Context.Channel.SendMessageAsync("All elo decay timers refreshed for BRZ 2v2 leaderboard");
        }
    }
}
