using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace ElUtilitySuite
{
    internal class JungleTracker
    {
        public static List<JungleCamp> JungleCamps { get; set; }
         
        static JungleTracker()
        {
            JungleCamps = new List<JungleCamp>();
        }

        public static void Init()
        {
            
        }

        public class JungleCamp
        {
            public JungleCamp(
                float respawnTime,
                Vector3 position,
                string[] mobnames,
                Utility.Map.MapType mapType)
            {
                RespawnTime = respawnTime;
                Position = position;
                MobNames = mobnames;
                MapType = mapType;
            }

            public float RespawnTime { get; private set; }
            public Vector3 Position { get; }

            public Vector2 MinimapPosition
            {
                get { return Drawing.WorldToMinimap(Position); }
            }

            public string[] MobNames { get; set; }
            public Utility.Map.MapType MapType { get; set; }
        }
    }
}