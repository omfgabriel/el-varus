namespace ElEasy
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        #region Public Properties

        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            try
            {
                var type = Type.GetType("ElEasy.Plugins." + Player.ChampionName );
                if (type != null)
                {
                    Base.Load();
                    Game.PrintChat("<font color='#f9eb0b'>ElEasy</font> - " + Player.ChampionName + " loaded, have fun gl and don't flame");
                    Game.PrintChat(
                    "[00:01] <font color='#f9eb0b'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}