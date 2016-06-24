namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = SharpDX.Color;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Smite : IPlugin
    {
        #region Constants

        /// <summary>
        ///     The smite range
        /// </summary>
        public const float SmiteRange = 570f;

        #endregion

        #region Static Fields

        public static Obj_AI_Minion Minion;

        #endregion

        #region Fields

        /// <summary>
        /// </summary>
        private string[] minionNames = new string[0];

        /// <summary>
        /// </summary>
        private Obj_AI_Minion targetedMinion;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<Smite> Spells { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string ChampionName { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive
            =>
                Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active
                || Orbwalking.Orbwalker.Instances.Any(x => x.ActiveMode == Orbwalking.OrbwalkingMode.Combo);

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public Spell SmiteSpell { get; set; }

        #endregion

        #region Properties

        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private Obj_AI_Hero Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

            if (smiteSlot == null)
            {
                return;
            }

            var predicate = new Func<Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Summoners", "SummonersMenu"))
                           : rootMenu.Children.First(predicate);

            var smiteMenu = menu.AddSubMenu(new Menu("Smite", "Smite"));
            {
                smiteMenu.AddItem(new MenuItem("SmiteBig", "Smite big mobs").SetValue(true));

                smiteMenu.AddItem(
                    new MenuItem("ElSmite.Activated", "Smite Activated").SetValue(
                        new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle, true)));

                smiteMenu.AddItem(new MenuItem("Smite.Ammo", "Save 1 smite charge").SetValue(true))
                    .SetTooltip("Will not smite a champion when there is only 1 smite charge!")
                    .SetFontStyle(FontStyle.Regular, Color.Green);

                //Champion Smite
                smiteMenu.SubMenu("Champion smite")
                    .AddItem(new MenuItem("ElSmite.KS.Activated", "Use smite to killsteal").SetValue(true));
                smiteMenu.SubMenu("Champion smite")
                    .AddItem(new MenuItem("ElSmite.KS.Combo", "Use smite in combo").SetValue(true));

                //Drawings
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Range", "Draw smite Range").SetValue(new Circle()));
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Text", "Draw smite text").SetValue(true));
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Damage", "Draw smite Damage").SetValue(false));
            }

            this.Menu = smiteMenu;
        }

        public void Load()
        {
            try
            {
                var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

                if (smiteSlot != null)
                {
                    this.SmiteSpell = new Spell(smiteSlot.Slot, SmiteRange, TargetSelector.DamageType.True);

                    this.minionNames = new[]
                                           {
                                               "SRU_Baron12.1.1", "SRU_Blue1.1.1", "SRU_Red4.1.1", "SRU_Blue7.1.1",
                                               "SRU_Red10.1.1", "SRU_Dragon_Elder6.5.1", "SRU_Dragon_Air6.1.1",
                                               "SRU_Dragon_Fire6.2.1", "SRU_Dragon_Water6.3.1", "SRU_Dragon_Elder6.5.1",
                                               "SRU_Dragon_Earth6.4.1", "SRU_RiftHerald17.1.1", "TT_Spiderboss8.1.1"
                                           };

                    Drawing.OnDraw += this.OnDraw;
                    Game.OnUpdate += this.OnUpdate;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        #endregion

        #region Methods

        private void OnDraw(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }

                var smiteActive = this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active;
                var drawSmite = this.Menu.Item("ElSmite.Draw.Range").GetValue<Circle>();
                var drawText = this.Menu.Item("ElSmite.Draw.Text").IsActive();
                var playerPos = Drawing.WorldToScreen(this.Player.Position);
                var drawDamage = this.Menu.Item("ElSmite.Draw.Damage").IsActive();

                if (smiteActive && this.SmiteSpell != null)
                {
                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready)
                    {
                        Drawing.DrawText(
                            playerPos.X - 70,
                            playerPos.Y + 40,
                            System.Drawing.Color.GreenYellow,
                            "Smite active");
                    }

                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) != SpellState.Ready)
                    {
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, System.Drawing.Color.Red, "Smite cooldown");
                    }

                    if (drawDamage && this.SmiteDamage() != 0)
                    {
                        var minions =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(m => m.Team == GameObjectTeam.Neutral && m.IsValidTarget());

                        foreach (var minion in minions.Where(m => m.IsHPBarRendered))
                        {
                            var hpBarPosition = minion.HPBarPosition;
                            var maxHealth = minion.MaxHealth;
                            var sDamage = this.SmiteDamage();
                            var x = this.SmiteDamage() / maxHealth;
                            var barWidth = 0;

                            switch (minion.CharData.BaseSkinName)
                            {
                                case "SRU_RiftHerald":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 17),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Dragon_Air":
                                case "SRU_Dragon_Water":
                                case "SRU_Dragon_Fire":
                                case "SRU_Dragon_Elder":
                                case "SRU_Dragon_Earth":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 22),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Orange);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Red":
                                case "SRU_Blue":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Orange);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Baron":
                                    barWidth = 194;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 18 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 18 + (float)(barWidth * x), hpBarPosition.Y + 35),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 3,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Gromp":
                                    barWidth = 87;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Murkwolf":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "Sru_Crab":
                                    barWidth = 61;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 8),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Razorbeak":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Krug":
                                    barWidth = 81;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (drawText && this.SmiteSpell != null)
                    {
                        Drawing.DrawText(
                            playerPos.X - 70,
                            playerPos.Y + 40,
                            System.Drawing.Color.Red,
                            "Smite not active!");
                    }
                }

                var smiteSpell = this.SmiteSpell;
                if (smiteSpell != null)
                {
                    if (smiteActive && drawSmite.Active
                        && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) == SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Green);
                    }

                    if (drawSmite.Active && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) != SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Red);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.SmiteSpell == null || !this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active
                    || this.Player.IsDead)
                {
                    return;
                }

                if (this.SmiteSpell != null && this.Menu.Item("SmiteBig").IsActive())
                {
                    if (this.targetedMinion == null)
                    {
                        this.targetedMinion = ObjectManager.Player.ServerPosition.GetMinionFastByNames(
                            SmiteRange,
                            this.minionNames);
                    }

                    if (this.SmiteSpell.IsReady())
                    {
                        if (this.targetedMinion.IsValidTarget(SmiteRange))
                        {
                            if (ObjectManager.Player.GetSummonerSpellDamage(
                                this.targetedMinion,
                                Damage.SummonerSpell.Smite) > this.targetedMinion.Health)
                            {
                                this.SmiteSpell.Cast(this.targetedMinion);
                            }
                        }
                        else
                        {
                            this.targetedMinion = null;
                        }
                    }
                    return;
                }

                if (this.Menu.Item("Smite.Ammo").IsActive() && this.Player.GetSpell(this.SmiteSpell.Slot).Ammo == 1)
                {
                    return;
                }

                if (this.Menu.Item("ElSmite.KS.Combo").IsActive()
                    && this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() == "s5_summonersmiteduel"
                    && this.ComboModeActive)
                {
                    var smiteComboEnemy =
                        HeroManager.Enemies.FirstOrDefault(hero => !hero.IsZombie && hero.IsValidTarget(500f));
                    if (smiteComboEnemy != null)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, smiteComboEnemy);
                    }
                }

                if (this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() != "s5_summonersmiteplayerganker")
                {
                    return;
                }

                if (this.Menu.Item("ElSmite.KS.Activated").IsActive())
                {
                    var kSableEnemy =
                        HeroManager.Enemies.FirstOrDefault(
                            hero =>
                            !hero.IsZombie && hero.IsValidTarget(SmiteRange)
                            && this.SmiteSpell.GetDamage(hero) >= hero.Health);

                    if (kSableEnemy != null)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, kSableEnemy);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        private float SmiteDamage()
        {
            try
            {
                return this.Player.Spellbook.GetSpell(this.SmiteSpell.Slot).State == SpellState.Ready
                           ? (float)this.Player.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite)
                           : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }

            return 0;
        }

        #endregion
    }
}