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
        private readonly string region;
        private readonly int gameMode;
        public int inTier1;
        public int inTier2;
        public int inTier3;
        public int inTier4;
        private static Dictionary<ulong, int> changeAnnouncements = new Dictionary<ulong, int>();
        private readonly Dictionary<ulong, MutableTuple<double, int>> tierList;

        const double T1ELO = 1400;
        const double T2ELO = 1200;
        const double T3ELO = 1000;

        public const double SEED1ELO = 1150;
        public const double SEED2ELO = 1075;
        public const double SEED3ELO = 1000;

        public TierModule(string regionParam, int gameModeParam)
        {
            region = regionParam;
            gameMode = gameModeParam;
            tierList = new Dictionary<ulong, MutableTuple<double, int>>();

            inTier1 = 0;
            inTier2 = 0;
            inTier3 = 0;
            inTier4 = 0;
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
                    if (elo >= T1ELO)
                    {
                        inTier1++;
                        tierList[reader.GetUInt64(0)] = new MutableTuple<double, int>(elo, 1);
                    }
                    else if (elo >= T2ELO)
                    {
                        inTier2++;
                        tierList[reader.GetUInt64(0)] = new MutableTuple<double, int>(elo, 2);
                    }
                    else if (elo >= T3ELO)
                    {
                        inTier3++;
                        tierList[reader.GetUInt64(0)] = new MutableTuple<double, int>(elo, 3);
                    }
                    else
                    {
                        inTier4++;
                        tierList[reader.GetUInt64(0)] = new MutableTuple<double, int>(elo, 4);
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
            else if (oldTier == 3)
                inTier3--;
            else
                inTier4--;

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
            else if (newElo >= T3ELO)
            {
                inTier3++;
                tierList[id].Item2 = 3;
                changeAnnouncements[id] = GetChangeValue(oldTier, 3);
            }
            else
            {
                inTier4++;
                tierList[id].Item2 = 4;
                changeAnnouncements[id] = GetChangeValue(oldTier, 4);
            }
        }

        public int getPlayerTier(ulong id)
        {
            return tierList[id].Item2;
        }

        private int GetChangeValue(int oldTier, int newTier)
        {
            if (oldTier == newTier)
                return 0;
            if (oldTier < newTier)
                return -newTier;
            return newTier;
        }

        public static async Task AnnounceTierChanges(ICommandContext context)
        {
            foreach (var change in changeAnnouncements.ToList())
            {
                int newTier = change.Value;
                if (newTier == 0)
                    continue;

                string direction = (newTier > 0) ? "promoted" : "demoted";
                newTier = Math.Abs(newTier);

                await context.Channel.SendMessageAsync($"<@{change.Key}> you have been {direction} to Tier {newTier}");
                await ChangeRoleToTier(context, newTier);

                changeAnnouncements.Remove(change.Key);
            }
        }

        public static async Task ChangeRoleToTier(ICommandContext context, int change)
        {
            var userInfo = context.User;
            var guildUser = (IGuildUser)userInfo;
            int gameMode = 0;
            try
            {
                gameMode = (int)(context.Message.Content[1] - '0');
            }
            catch (ArgumentException)
            {
                gameMode = (int)(context.Message.Content[5] - '0');
            }
            string region = "";
            IGuild server = context.Guild as IGuild;
            IGuildUser user = await server.GetUserAsync(userInfo.Id);
            foreach (ulong roleId in user.RoleIds)
            {
                try
                {
                    Role thisRole = HelperFunctions.GetRoleRegion(roleId);
                    if (thisRole.gameMode == gameMode)
                    {
                        await guildUser.RemoveRoleAsync(server.GetRole(roleId));
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
            else if (region == "TEST")
            {
                if (gameMode == 1)
                    return Globals.tiersTEST1;
                else if (gameMode == 2)
                    return Globals.tiersTEST2;
            }

            return null;
        }

        public static int binaryOr(int tier, int targetTier)
        {
            int bTier = binaryConvert(tier);
            int bTargetTier = binaryConvert(targetTier);

            int queueTier = bTier | bTargetTier;
            return queueTier;
        }

        public static int binaryConvert(int tier)
        {
            if (tier == 1)
                return 0b1000;
            if (tier == 2)
                return 0b0100;
            if (tier == 3)
                return 0b0010;
            if (tier == 4)
                return 0b0001;
            return 0;
        }
    }
}
