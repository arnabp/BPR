using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPR
{
    public class TierModule
    {
        private string region;
        private int gameMode;
        public int inTier1;
        public int inTier2;
        public int inTier3;
        private Dictionary<ulong, int> tierList;

        const int T1ELO = 1400;
        const int T2ELO = 1200;
        const int T1LIMIT = 10;
        const int T2LIMIT = 16;

        public TierModule(string regionParam, int gameModeParam)
        {
            region = regionParam;
            gameMode = gameModeParam;
            tierList = new Dictionary<ulong, int>();

            inTier1 = 0;
            inTier2 = 0;
            inTier3 = 0;
        }

        public async Task InitTierList()
        {
            string query = $"SELECT id, elo FROM leaderboard{region}{gameMode} ORDER BY elo DESC;";
            await HelperFunctions.CheckSQLStateAsync();
            await Globals.conn.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Globals.conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int elo = reader.GetInt16(1);
                    if (elo >= T1ELO && inTier1 < T1LIMIT)
                    {
                        inTier1++;
                        tierList[reader.GetUInt64(0)] = elo;
                    }
                    else if (elo >= T2ELO && inTier2 < T2LIMIT)
                    {
                        inTier2++;
                        tierList[reader.GetUInt64(0)] = elo;
                    }
                    else
                    {
                        inTier3++;
                        tierList[reader.GetUInt64(0)] = elo;
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

        public int PlayerEloChange(ulong id, int newElo)
        {
            int oldTier = tierList[id];
            if (oldTier == 1)
                inTier1--;
            else if (oldTier == 2)
                inTier2--;
            else
                inTier3--;

            if (newElo >= T1ELO)
            {
                inTier1++;
                tierList[id] = 1;
                if (oldTier != 1)
                    return 1;
            }
            else if (newElo >= T2ELO)
            {
                inTier2++;
                tierList[id] = 2;
                if (oldTier > 2)
                    return -2;
                else if (oldTier < 2)
                    return 2;
            }
            else
            {
                inTier3++;
                tierList[id] = 3;
                if (oldTier != 3)
                    return -3;
            }

            return 0;
        }

        public void NormalizeTiers()
        {
            if (inTier1 > T1LIMIT || inTier1 == T1LIMIT - 1)
                NormalizeTier1();
            if (inTier2 > T2LIMIT || inTier2 == T2LIMIT - 1)
                NormalizeTier2();
        }

        public int getPlayerTier(ulong id)
        {
            return tierList[id];
        }

        private void NormalizeTier1()
        {
            if (inTier1 > T1LIMIT)
            {
                var tier1List = tierList.Where(player => player.Key == 1).ToList();
                tier1List.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));

                for (int i = T1LIMIT; i < inTier1; i++)
                {
                    tierList[tier1List[i].Key] = 2;
                }
            }
            else if (inTier1 < T1LIMIT)
            {
                var tier2List = tierList.Where(player => player.Key == 2).ToList();
                tier2List.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));

                for (int i = 0; i < T1LIMIT - inTier1; i++)
                {
                    if (tier2List[i].Value > T1ELO)
                        tierList[tier2List[i].Key] = 1;
                }
            }
        }

        private void NormalizeTier2()
        {
            if (inTier2 > T2LIMIT)
            {
                var tier2List = tierList.Where(player => player.Key == 2).ToList();
                tier2List.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));

                for (int i = T2LIMIT; i < inTier2; i++)
                {
                    tierList[tier2List[i].Key] = 2;
                }
            }
            else if (inTier2 < T2LIMIT)
            {
                var tier3List = tierList.Where(player => player.Key == 3).ToList();
                tier3List.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));

                for (int i = 0; i < T2LIMIT - inTier2; i++)
                {
                    if (tier3List[i].Value > T1ELO)
                        tierList[tier3List[i].Key] = 1;
                }
            }
        }

        public static async Task AnnounceTierChange(SocketCommandContext context, int change)
        {
            if (change == 0)
                return;

            string direction = (change > 0) ? "promoted" : "demoted";
            change = Math.Abs(change);

            await context.Channel.SendMessageAsync($"@<{context.User.Id}> you have been {direction} to Tier {change}");
            await ChangeRoleToTier(context, change);
        }

        public static async Task ChangeRoleToTier(SocketCommandContext context, int change)
        {
            var userInfo = context.User;
            var guildUser = (IGuildUser)userInfo;
            int gameMode = (int)(context.Message.Content[1] - '0');
            string region = "";
            foreach (Discord.WebSocket.SocketRole role in context.Guild.GetUser(userInfo.Id).Roles)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(role.Id);
                    if (thisRole.gameMode == gameMode)
                    {
                        await guildUser.RemoveRoleAsync(role);
                        region = thisRole.region;
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }

            ulong newRoleId = HelperFunctions.GetRoleId(region, gameMode, change);
            await guildUser.AddRoleAsync(context.Guild.GetRole(newRoleId));
        }

        public static TierModule GetTierModule(string region, int gameMode)
        {
            if (region == "NA")
            {
                if (gameMode == 1)
                    return Globals.tiersNA1;
                else if (gameMode == 2)
                    return Globals.tiersNA2;
            }
            else if (region == "EU")
            {
                if (gameMode == 1)
                    return Globals.tiersEU1;
                else if (gameMode == 2)
                    return Globals.tiersEU2;
            }

            return null;
        }

        public static int binaryAnd(int tier, int targetTier)
        {
            int bTier = binaryConvert(tier);
            int bTargetTier = binaryConvert(tier);

            int queueTier = tier & targetTier;
            if (queueTier == 5)
                return 7;
            return queueTier;
        }

        public static int binaryConvert(int tier)
        {
            if (tier == 1)
                return 0b100;
            if (tier == 2)
                return 0b010;
            if (tier == 3)
                return 0b001;
            return 0;
        }
    }
}
