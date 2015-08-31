﻿namespace ElRengarRevamped
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    public class Standards
    {
        #region Static Fields

        public static readonly int[] BlueSmite = { 3706, 3710, 3709, 3708, 3707 };

        public static readonly int[] RedSmite = { 3715, 3718, 3717, 3716, 3714 };

        public static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>()
                                                                      {
                                                                          { Spells.Q, new Spell(SpellSlot.Q, 250) },
                                                                          { Spells.W, new Spell(SpellSlot.W, 500) },
                                                                          { Spells.E, new Spell(SpellSlot.E, 1000) },
                                                                          { Spells.R, new Spell(SpellSlot.R, 2000) }
                                                                      };

        public static Orbwalking.Orbwalker Orbwalker;

        protected static SpellSlot Ignite;

        protected static int LastSwitch;

        protected static SpellSlot Smite;

        #endregion

        #region Public Properties

        public static int Ferocity
        {
            get
            {
                return (int)ObjectManager.Player.Mana;
            }
        }

        public static bool HasPassive
        {
            get
            {
                return Player.HasBuff("rengarpassivebuff");
            }
        }

        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public static bool RengarQ
        {
            get
            {
                return Player.Buffs.Any(x => x.Name.Contains("rengarq"));
            }
        }

        public static bool RengarR
        {
            get
            {
                return Player.Buffs.Any(x => x.Name.Contains("RengarR"));
            }
        }

        public static String ScriptVersion
        {
            get
            {
                return typeof(Rengar).Assembly.GetName().Version.ToString();
            }
        }

        #endregion

        #region Public Methods and Operators

        public static StringList IsListActive(string menuItem)
        {
            return MenuInit.Menu.Item(menuItem).GetValue<StringList>();
        }

        #endregion

        #region Methods

        protected static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        protected static bool IsActive(string menuItem)
        {
            return MenuInit.Menu.Item(menuItem).GetValue<bool>();
        }

        protected static void SmiteCombo()
        {
            if (BlueSmite.Any(id => Items.HasItem(id)))
            {
                Smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
                return;
            }

            if (RedSmite.Any(id => Items.HasItem(id)))
            {
                Smite = Player.GetSpellSlot("s5_summonersmiteduel");
                return;
            }

            Smite = Player.GetSpellSlot("summonersmite");
        }

        protected static void UseHydra()
        {
            if (Player.IsWindingUp)
            {
                return;
            }

            if (!ItemData.Tiamat_Melee_Only.GetItem().IsReady()
                && !ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                return;
            }

            ItemData.Tiamat_Melee_Only.GetItem().Cast();
            ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
        }

        #endregion
    }
}