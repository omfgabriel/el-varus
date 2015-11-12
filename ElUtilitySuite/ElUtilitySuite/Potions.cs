namespace ElUtilitySuite
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal static class Potions
    {
        #region Public Methods and Operators

        public static void Load()
        {
            try
            {
                Game.OnUpdate += OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
        //2031
        #region Methods

        private static void OnUpdate(EventArgs args)
        {
            if (Entry.Player.IsDead)
            {
                return;
            }

            try
            {
                //Console.WriteLine("Buffs: {0}", string.Join(" | ", Entry.Player.Buffs.Select(b => b.DisplayName)));
                PotionManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static bool PotCheck()
        {
            return Entry.Player.HasBuff("RegenerationPotion") || Entry.Player.HasBuff("ItemMiniRegenPotion")
                   || Entry.Player.HasBuff("ItemCrystalFlask") || Entry.Player.HasBuff("ItemCrystalFlaskJungle")
                   || Entry.Player.HasBuff("ItemDarkCrystalFlask");
        }

        

        private static void PotionManager()
        {
            if (!InitializeMenu.Menu.Item("Potions.Activated").GetValue<bool>() || Entry.Player.InFountain()
                || Entry.Player.IsRecalling() || PotCheck())
            {
                return;
            }

            var healthPotion = ItemData.Health_Potion.GetItem();
            var biscuit = ItemData.Total_Biscuit_of_Rejuvenation2.GetItem();
            var refillablePotion = ItemData.Refillable_Potion.GetItem();
            var huntersPotion = ItemData.Hunters_Potion.GetItem();
            var corruptingPotion = ItemData.Corrupting_Potion.GetItem();

            if (Entry.Player.Health / Entry.Player.MaxHealth * 100
                < InitializeMenu.Menu.Item("Potions.Player.Health").GetValue<Slider>().Value)
            {
                if (healthPotion.IsOwned(Entry.Player) && healthPotion.IsReady()
                    && InitializeMenu.Menu.Item("Potions.Health").GetValue<bool>())
                {
                    healthPotion.Cast();
                }

                if (biscuit.IsOwned(Entry.Player) && InitializeMenu.Menu.Item("Potions.Biscuit").GetValue<bool>())
                {
                    biscuit.Cast();
                }

                if (refillablePotion.IsOwned(Entry.Player) && InitializeMenu.Menu.Item("Potions.RefillablePotion").GetValue<bool>())
                {
                    refillablePotion.Cast();
                }

                if (huntersPotion.IsOwned(Entry.Player) && InitializeMenu.Menu.Item("Potions.HuntersPotion").GetValue<bool>())
                {
                    huntersPotion.Cast();
                }

                if (corruptingPotion.IsOwned(Entry.Player) && InitializeMenu.Menu.Item("Potions.CorruptingPotion").GetValue<bool>())
                {
                    corruptingPotion.Cast();
                }
            }
        }

        #endregion
    }
}