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
                var type = Type.GetType("ElEasy.Plugins." + Player.ChampionName);
                if (type != null)
                {
                    Base.Load();
                    Notifications.AddNotification("ElEasy - " + Player.ChampionName + " 1.0.3.4", 8000);
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