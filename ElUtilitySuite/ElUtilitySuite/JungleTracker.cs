using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace ElUtilitySuite
{
    internal class JungleTracker
    {
        static JungleTracker()
        {
            JungleCamps = new List<JungleCamp>
            {
                new JungleCamp(75000, new Vector3(6078.15f, 6094.45f, -98.63f),
                    new[] {"TT_NWolf3.1.1", "TT_NWolf23.1.2", "TT_NWolf23.1.3"}, Utility.Map.MapType.TwistedTreeline,
                    GameObjectTeam.Order),
                new JungleCamp(100000, new Vector3(6943.41f, 5422.61f, 52.62f),
                    new[] {"SRU_Razorbeak3.1.1", "SRU_RazorbeakMini3.1.2", "SRU_RazorbeakMini3.1.3"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Order),
                new JungleCamp(100000, new Vector3(2164.34f, 8383.02f, 51.78f), new[] {"SRU_Gromp13.1.1"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Order),
                new JungleCamp(100000, new Vector3(8370.58f, 2718.15f, 51.09f),
                    new[] {"SRU_Krug5.1.2", "SRU_KrugMini5.1.1"}, Utility.Map.MapType.SummonersRift,
                    GameObjectTeam.Order),
                new JungleCamp(180000, new Vector3(4285.04f, 9597.52f, -67.6f), new[] {"SRU_Crab16.1.1"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Neutral),
                new JungleCamp(100000, new Vector3(6476.17f, 12142.51f, 56.48f),
                    new[] {"SRU_Krug11.1.2", "SRU_KrugMini11.1.1"}, Utility.Map.MapType.SummonersRift,
                    GameObjectTeam.Chaos),
                new JungleCamp(75000, new Vector3(11025.95f, 5805.61f, -107.19f),
                    new[] {"TT_NWraith4.1.1", "TT_NWraith24.1.2", "TT_NWraith24.1.3"},
                    Utility.Map.MapType.TwistedTreeline, GameObjectTeam.Chaos),
                new JungleCamp(100000, new Vector3(10983.83f, 8328.73f, 62.22f),
                    new[] {"SRU_Murkwolf8.1.1", "SRU_MurkwolfMini8.1.2", "SRU_MurkwolfMini8.1.3"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Chaos),
                new JungleCamp(100000, new Vector3(12671.83f, 6306.6f, 51.71f), new[] {"SRU_Gromp14.1.1"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Chaos),
                new JungleCamp(360000, new Vector3(7738.3f, 10079.78f, -61.6f), new[] {"TT_Spiderboss8.1.1"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Neutral),
                new JungleCamp(300000, new Vector3(3800.99f, 7883.53f, 52.18f),
                    new[] {"SRU_Blue1.1.1", "SRU_BlueMini1.1.2", "SRU_BlueMini21.1.3"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Order),
                new JungleCamp(75000, new Vector3(4373.14f, 5842.84f, -107.14f),
                    new[] {"TT_NWraith1.1.1", "TT_NWraith21.1.2", "TT_NWraith21.1.3"},
                    Utility.Map.MapType.TwistedTreeline, GameObjectTeam.Order),
                new JungleCamp(300000, new Vector3(4993.14f, 10491.92f, -71.24f), new[] {"SRU_RiftHerald"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Neutral),
                new JungleCamp(75000, new Vector3(5106.94f, 7985.9f, -108.38f),
                    new[] {"TT_NGolem2.1.1", "TT_NGolem22.1.2"}, Utility.Map.MapType.TwistedTreeline,
                    GameObjectTeam.Order),
                new JungleCamp(100000, new Vector3(7852.38f, 9562.62f, 52.3f),
                    new[] {"SRU_Razorbeak9.1.1", "SRU_RazorbeakMini9.1.2", "SRU_RazorbeakMini9.1.3"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Chaos),
                new JungleCamp(300000, new Vector3(10984.11f, 6960.31f, 51.72f),
                    new[] {"SRU_Blue7.1.1", "SRU_BlueMini7.1.2", "SRU_BlueMini27.1.3"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Chaos),
                new JungleCamp(180000, new Vector3(10647.7f, 5144.68f, -62.81f), new[] {"SRU_Crab15.1.1"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Neutral),
                new JungleCamp(75000, new Vector3(9294.02f, 6085.41f, -96.7f),
                    new[] {"TT_NWolf6.1.1", "TT_NWolf26.1.2", "TT_NWolf26.1.3"}, Utility.Map.MapType.TwistedTreeline,
                    GameObjectTeam.Chaos),
                new JungleCamp(420000, new Vector3(4993.14f, 10491.92f, -71.24f), new[] {"SRU_Baron12.1.1"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Neutral),
                new JungleCamp(100000, new Vector3(3849.95f, 6504.36f, 52.46f),
                    new[] {"SRU_Murkwolf2.1.1", "SRU_MurkwolfMini2.1.2", "SRU_MurkwolfMini2.1.3"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Order),
                new JungleCamp(300000, new Vector3(7813.07f, 4051.33f, 53.81f),
                    new[] {"SRU_Red4.1.1", "SRU_RedMini4.1.2", "SRU_RedMini4.1.3"}, Utility.Map.MapType.SummonersRift,
                    GameObjectTeam.Order),
                new JungleCamp(360000, new Vector3(9813.83f, 4360.19f, -71.24f), new[] {"SRU_Dragon6.1.1"},
                    Utility.Map.MapType.SummonersRift, GameObjectTeam.Neutral),
                new JungleCamp(300000, new Vector3(7139.29f, 10779.34f, 56.38f),
                    new[] {"SRU_Red10.1.1", "SRU_RedMini10.1.2", "SRU_RedMini10.1.3"}, Utility.Map.MapType.SummonersRift,
                    GameObjectTeam.Chaos),
                new JungleCamp(75000, new Vector3(10276.81f, 8037.54f, -108.92f),
                    new[] {"TT_NGolem5.1.1", "TT_NGolem25.1.2"}, Utility.Map.MapType.TwistedTreeline,
                    GameObjectTeam.Chaos)
            };
        }

        public static List<JungleCamp> JungleCamps { get; set; }

        public static void Init()
        {
        }

        public class JungleCamp
        {
            public JungleCamp(
                float respawnTime,
                Vector3 position,
                string[] mobNames,
                Utility.Map.MapType mapType,
                GameObjectTeam team)
            {
                RespawnTime = respawnTime;
                Position = position;
                MobNames = mobNames;
                MapType = mapType;
                Team = team;
            }

            public float RespawnTime { get; private set; }
            public Vector3 Position { get; }

            public Vector2 MinimapPosition
            {
                get { return Drawing.WorldToMinimap(Position); }
            }

            public string[] MobNames { get; set; }
            public Utility.Map.MapType MapType { get; set; }
            public GameObjectTeam Team { get; set; }
        }
    }
}