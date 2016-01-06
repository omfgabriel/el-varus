namespace ElFizz
{
    using LeagueSharp.Common;

    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Fizz.OnGameLoad;
        }
    }
}