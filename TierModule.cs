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
    public class MutableTuple<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public MutableTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public class TierModule
    {
        private string region;
        private int gameMode;
        public int inTier1;
        public int inTier2;
        public int inTier3;
        private static Dictionary<ulong, int> changeAnnouncements = new Dictionary<ulong, int>();
        private Dictionary<ulong, MutableTuple<double, int>> tierList;

        const double T1ELO = 1400;
        const double T2ELO = 1200;
        const int T1LIMIT = 10;
        const int T2LIMIT = 16;

        const double SEED1ELO = 1150;
        const double SEED2ELO = 1075;
        const double SEED3ELO = 1000;

        public TierModule(string regionParam, int gameModeParam)
        {
            region = regionParam;
            gameMode = gameModeParam;
            tierList = new Dictionary<ulong, MutableTuple<double, int>>();

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
                    double elo = reader.GetDouble(1);
                    if (elo >= T1ELO && inTier1 < T1LIMIT)
                    {
                        inTier1++;
                        tierList[reader.GetUInt64(0)] = new MutableTuple<double, int>(elo, 1);
                    }
                    else if (elo >= T2ELO && inTier2 < T2LIMIT)
                    {
                        inTier2++;
                        tierList[reader.GetUInt64(0)] = new MutableTuple<double, int>(elo, 2);
                    }
                    else
                    {
                        inTier3++;
                        tierList[reader.GetUInt64(0)] = new MutableTuple<double, int>(elo, 3);
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

        public void PlayerEloChange(ulong id, double newElo)
        {
            int oldTier = tierList[id].Item2;
            tierList[id].Item1 = newElo;

            if (oldTier == 1)
                inTier1--;
            else if (oldTier == 2)
                inTier2--;
            else
                inTier3--;

            if (newElo >= T1ELO)
            {
                inTier1++;
                tierList[id].Item2 = 1;
                changeAnnouncements[id] = GetChangeValue(oldTier, 1);
            }
            else if (newElo >= T2ELO)
            {
                inTier2++;
                tierList[id].Item2 = 2;
                changeAnnouncements[id] = GetChangeValue(oldTier, 2);
            }
            else
            {
                inTier3++;
                tierList[id].Item2 = 3;
                changeAnnouncements[id] = GetChangeValue(oldTier, 3);
            }
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
            return tierList[id].Item2;
        }

        private void NormalizeTier1()
        {
            if (inTier1 > T1LIMIT)
            {
                var tier1List = tierList.Where(player => player.Value.Item2 == 1).ToList();
                tier1List.Sort((pair2, pair1) => pair1.Value.Item1.CompareTo(pair2.Value.Item1));

                for (int i = T1LIMIT; i < inTier1; i++)
                {
                    tierList[tier1List[i].Key].Item2 = 2;
                    if (changeAnnouncements.ContainsKey(tier1List[i].Key))
                        changeAnnouncements.Remove(tier1List[i].Key);
                    else
                        changeAnnouncements[tier1List[i].Key] = -2;
                }
            }
            else if (inTier1 < T1LIMIT)
            {
                var tier2List = tierList.Where(player => player.Value.Item2 == 2).ToList();
                tier2List.Sort((pair2, pair1) => pair1.Value.Item1.CompareTo(pair2.Value.Item1));

                for (int i = 0; i < T1LIMIT - inTier1; i++)
                {
                    if (tier2List[i].Value.Item1 > T1ELO)
                    {
                        tierList[tier2List[i].Key].Item2 = 1;
                        if (changeAnnouncements.ContainsKey(tier2List[i].Key))
                            changeAnnouncements.Remove(tier2List[i].Key);
                        else
                            changeAnnouncements[tier2List[i].Key] = 1;
                    }

                }
            }
        }

        private void NormalizeTier2()
        {
            if (inTier2 > T2LIMIT)
            {
                var tier2List = tierList.Where(player => player.Value.Item2 == 2).ToList();
                tier2List.Sort((pair2, pair1) => pair1.Value.Item1.CompareTo(pair2.Value.Item1));

                for (int i = T2LIMIT; i < inTier2; i++)
                {
                    tierList[tier2List[i].Key].Item2 = 2;
                }
            }
            else if (inTier2 < T2LIMIT)
            {
                var tier3List = tierList.Where(player => player.Value.Item2 == 3).ToList();
                tier3List.Sort((pair2, pair1) => pair1.Value.Item1.CompareTo(pair2.Value.Item1));

                for (int i = 0; i < T2LIMIT - inTier2; i++)
                {
                    if (tier3List[i].Value.Item1 > T1ELO)
                        tierList[tier3List[i].Key].Item2 = 1;
                }
            }
        }

        private int GetChangeValue(int oldTier, int newTier)
        {
            if (oldTier == newTier)
                return 0;
            if (oldTier > newTier)
                return -newTier;
            return newTier;
        }

        public static async Task AnnounceTierChanges(SocketCommandContext context)
        {
            foreach (var change in changeAnnouncements)
            {
                int newTier = change.Value;
                if (newTier == 0)
                    continue;

                string direction = (newTier > 0) ? "promoted" : "demoted";
                newTier = Math.Abs(newTier);

                await context.Channel.SendMessageAsync($"@<{change.Key}> you have been {direction} to Tier {newTier}");
                await ChangeRoleToTier(context, newTier);

                changeAnnouncements.Remove(change.Key);
            }
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
