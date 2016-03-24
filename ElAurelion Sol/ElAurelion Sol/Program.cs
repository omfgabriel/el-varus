namespace ElAurelion_Sol
{
    using LeagueSharp.Common;

    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += AurelionSol.OnGameLoad;
        }
    }
}