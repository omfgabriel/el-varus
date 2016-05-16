﻿namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class FaceOfTheMountain : Item
    {
        #region Static Fields

        /// <summary>
        ///     Targetted ally
        /// </summary>
        private static Obj_AI_Hero aggroTarget;

        /// <summary>
        ///     Incoming hero damage
        /// </summary>
        private static float incomingDamage;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public FaceOfTheMountain()
        {
            Game.OnUpdate += this.Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id
        {
            get
            {
                return ItemId.Face_of_the_Mountain;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name
        {
            get
            {
                return "Face of the Mountain";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseFaceCombo", "Activate").SetValue(true));
            this.Menu.AddItem(new MenuItem("Mode", "Activation mode: ")).SetValue(new StringList(new[] { "Use always", "Use in combo" }, 1));
            this.Menu.AddItem(new MenuItem("FaceHp", "Use on Hp %").SetValue(new Slider(50)));
            this.Menu.AddItem(new MenuItem("Face.Damage", "Incoming damage percentage").SetValue(new Slider(50, 1)));

            this.Menu.AddItem(new MenuItem("blank-line", ""));
            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
            {
                this.Menu.AddItem(new MenuItem("Faceon" + x.ChampionName, "Use for " + x.ChampionName)).SetValue(true);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the allies, old sucks need to rewrite soontm
        /// </summary>
        /// <returns>Allies</returns>
        private Obj_AI_Hero Allies()
        {
            try
            {
                var target = this.Player;

                foreach (
                    var unit in
                        HeroManager.Allies.Where(x => x.IsValidTarget(900, false))
                            .OrderByDescending(h => h.Health / h.MaxHealth * 100))
                {
                    target = unit;
                }

                return target;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
            return null;
        }

        /// <summary>
        ///     Called when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            try
            {
                if (!Items.HasItem((int)this.Id) || !Items.CanUseItem((int)this.Id)
                    || !this.Menu.Item("UseFaceCombo").IsActive())
                {
                    return;
                }

                if (this.Menu.Item("Mode").GetValue<StringList>().SelectedIndex == 1 && !this.ComboModeActive)
                {
                    return;
                }

                this.UseItem(700f);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!Items.HasItem((int)this.Id) || !Items.CanUseItem((int)this.Id) || !this.Menu.Item("UseFaceCombo").IsActive())
                {
                    return;
                }

                if (sender.Type != GameObjectType.obj_AI_Hero && !sender.IsEnemy)
                {
                    return;
                }

                var heroSender = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                if (heroSender.GetSpellSlot(args.SData.Name) == SpellSlot.Unknown && args.Target.Type == this.Player.Type)
                {
                    aggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);
                    incomingDamage = (float)heroSender.GetAutoAttackDamage(aggroTarget);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Old use item, need to rewrite this soontm
        /// </summary>
        /// <param name="itemRange"></param>
        private void UseItem(float itemRange = float.MaxValue)
        {
            try
            {
                if (this.Player.InFountain())
                {
                    return;
                }

                var target = itemRange > 5000 ? this.Player : this.Allies();
                if (target.Distance(this.Player.ServerPosition, true) > itemRange * itemRange)
                {
                    return;
                }

                var allyHealthPercent = (int)((target.Health / target.MaxHealth) * 100);
                var incomingDamagePercent = (int)(incomingDamage / target.MaxHealth * 100);

                if (!this.Menu.Item(string.Format("Faceon{0}", target.ChampionName)).IsActive() || target.IsRecalling())
                {
                    return;
                }

                if (allyHealthPercent <= this.Menu.Item("FaceHp").GetValue<Slider>().Value)
                {
                    if ((incomingDamagePercent >= 1 || incomingDamage >= target.Health))
                    {
                        if (aggroTarget.NetworkId == target.NetworkId)
                        {
                            Items.UseItem((int)this.Id, target);
                        }
                    }

                    if (incomingDamagePercent >= this.Menu.Item("Face.Damage").GetValue<Slider>().Value * 100)
                    {
                        if (aggroTarget.NetworkId == target.NetworkId)
                        {
                            Items.UseItem((int)this.Id, target);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}