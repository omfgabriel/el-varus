namespace ElEasy
{
    using ElEasy.Plugins;

    using LeagueSharp;

    internal class Base
    {
        #region Methods

        internal static void Load()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Taric":
                    Taric.Load();
                    break;

                case "Leona":
                    Leona.Load();
                    break;

                case "Sona":
                    Sona.Load();
                    break;

                case "Nasus":
                    Nasus.Load();
                    break;

                case "Malphite":
                    Malphite.Load();
                    break;

                case "Darius":
                    Darius.Load();
                    break;

                case "Katarina":
                    Katarina.Load();
                    break;

                case "Ryze":
                    Ryze.Load();
                    break;

                case "Cassiopeia":
                    Cassiopeia.Load();
                    break;
            }
        }

        #endregion
    }
}