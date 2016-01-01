namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Smite : IPlugin
    {
        #region Static Fields

        public static Obj_AI_Minion minion;

        //public static Spell smite;

        //public static SpellSlot smiteSlot;

        private static readonly string[] BuffsThatActuallyMakeSenseToSmite =
            {
                "SRU_Red", "SRU_Blue", "SRU_Dragon",
                "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf",
                "SRU_Razorbeak", "SRU_RiftHerald",
                "SRU_Krug", "Sru_Crab", "TT_Spiderboss",
                "TTNGolem", "TTNWolf", "TTNWraith"
            };

        #endregion

        #region Fields

        // <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The spell type.
        /// </value>
        public SpellDataTargetType TargetType;

        #endregion

        #region Constructors and Destructors

        static Smite()
        {
            Spells = new List<Smite>
                         {
                             new Smite
                                 {
                                     ChampionName = "ChoGath", Range = 325f, Slot = SpellSlot.R, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Elise", Range = 475f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Fizz", Range = 550f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "LeeSin", Range = 1100f, Slot = SpellSlot.Q, Stage = 1,
                                     TargetType = SpellDataTargetType.Self
                                 },
                             new Smite
                                 {
                                     ChampionName = "MonkeyKing", Range = 375f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Nunu", Range = 300f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Olaf", Range = 325f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Pantheon", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Volibear", Range = 400f, Slot = SpellSlot.W, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "XinZhao", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Mundo", Range = 1050f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "KhaZix", Range = 325f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Evelynn", Range = 225f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Shen", Range = 520f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                         };
        }

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
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public float Range { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The slot.
        /// </value>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public Spell SmiteSpell { get; set; }

        // <summary>
        /// Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The stage.
        /// </value>
        public int Stage { get; set; }

        #endregion

        #region Properties

        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var smiteMenu = rootMenu.AddSubMenu(new Menu("Smite", "Smite"));
            {
                smiteMenu.AddItem(
                    new MenuItem("ElSmite.Activated", "Activated").SetValue(
                        new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle, true)));

                smiteMenu.AddItem(new MenuItem("Smite.Spell", "Use spell").SetValue(true));

                if (Game.MapId == GameMapId.SummonersRift)
                {
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Dragon", "Dragon").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Baron", "Baron").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Red", "Red buff").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Blue", "Blue buff").SetValue(true));

                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_RiftHerald", "Rift Herald").SetValue(false));

                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Gromp", "Gromp").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Murkwolf", "Wolves").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Krug", "Krug").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Razorbeak", "Chicken camp").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("Sru_Crab", "Crab").SetValue(false));
                }

                if (Game.MapId == GameMapId.TwistedTreeline)
                {
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_Spiderboss", "Vilemaw Enabled").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_NGolem", "Golem Enabled").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_NWolf", "Wolf Enabled").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_NWraith", "Wraith Enabled").SetValue(true));
                }

                //Killsteal submenu
                smiteMenu.SubMenu("Killsteal")
                    .AddItem(new MenuItem("ElSmite.KS.Activated", "Use smite to killsteal").SetValue(true));

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
                var smiteSlot = this.Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite")
                                    ? SpellSlot.Summoner1
                                    : this.Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite")
                                          ? SpellSlot.Summoner2
                                          : SpellSlot.Unknown;

                if (smiteSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.SmiteSpell = new Spell(smiteSlot);

                Drawing.OnDraw += this.OnDraw;
                Game.OnUpdate += this.OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void ChampionSpellSmite(float damage, Obj_AI_Base mob)
        {
            foreach (var spell in
                Spells.Where(
                    x => x.ChampionName.Equals(this.Player.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (this.Player.GetSpellDamage(mob, spell.Slot, spell.Stage) + damage >= mob.Health)
                {
                    if (mob.Distance(this.Player.ServerPosition)
                        <= spell.Range + mob.BoundingRadius + this.Player.BoundingRadius)
                    {
                        if (spell.TargetType == SpellDataTargetType.Unit)
                        {
                            this.Player.Spellbook.CastSpell(spell.Slot, mob);
                        }
                        else if (spell.TargetType == SpellDataTargetType.Self)
                        {
                            this.Player.Spellbook.CastSpell(spell.Slot);
                        }
                    }
                }
            }
        }

        private void JungleSmite()
        {
            if (!this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active)
            {
                return;
            }

            minion =
                (Obj_AI_Minion)
                MinionManager.GetMinions(this.Player.ServerPosition, 900, MinionTypes.All, MinionTeam.Neutral)
                    .FirstOrDefault(
                        buff =>
                        buff.IsValidTarget() && BuffsThatActuallyMakeSenseToSmite.Contains(buff.CharData.BaseSkinName));

            if (minion == null)
            {
                return;
            }

            if (this.Menu.Item(minion.CharData.BaseSkinName).IsActive())
            {
                if (minion.Distance(this.Player.ServerPosition)
                    <= 500 + minion.BoundingRadius + this.Player.BoundingRadius)
                {
                    if (this.Menu.Item("Smite.Spell").IsActive())
                    {
                        this.ChampionSpellSmite(this.SmiteDamage(), minion);
                    }
                    if (this.SmiteDamage() > minion.Health)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, minion);
                    }
                }
            }
        }

        private void OnDraw(EventArgs args)
        {
            var smiteActive = this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active;
            var drawSmite = this.Menu.Item("ElSmite.Draw.Range").GetValue<Circle>();
            var drawText = this.Menu.Item("ElSmite.Draw.Text").IsActive();
            var playerPos = Drawing.WorldToScreen(this.Player.Position);
            var drawDamage = this.Menu.Item("ElSmite.Draw.Damage").IsActive();

            if (smiteActive)
            {
                if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready)
                {
                    Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.GhostWhite, "Smite active");
                }

                if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) != SpellState.Ready)
                {
                    Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.Red, "Smite cooldown");
                }

                if (drawDamage && this.SmiteDamage() != 0)
                {
                    var minions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                m =>
                                m.Team == GameObjectTeam.Neutral && m.IsValidTarget()
                                && BuffsThatActuallyMakeSenseToSmite.Contains(m.CharData.BaseSkinName));

                    foreach (var minion in minions.Where(m => m.IsHPBarRendered))
                    {
                        var hpBarPosition = minion.HPBarPosition;
                        var maxHealth = minion.MaxHealth;
                        var sDamage = this.SmiteDamage();
                        //SmiteDamage : MaxHealth = x : 100
                        //Ratio math for this ^
                        var x = this.SmiteDamage() / maxHealth;
                        var barWidth = 0;

                        /*
                        * DON'T STEAL THE OFFSETS FOUND BY ASUNA DON'T STEAL THEM JUST GET OUT WTF MAN.
                        * EL SMITE IS THE BEST SMITE ASSEMBLY ON LEAGUESHARP AND YOU WILL NOT FIND A BETTER ONE.
                        * THE DRAWINGS ACTUALLY MAKE FUCKING SENSE AND THEY ARE FUCKING GOOD
                        * GTFO HERE SERIOUSLY OR I CALL DETUKS FOR YOU GUYS
                        * NO STEAL OR DMC FUCKING A REPORT.
                        * HELLO COPYRIGHT BY ASUNA 2015 ALL AUSTRALIAN RIGHTS RESERVED BY UNIVERSAL GTFO SERIOUSLY THO
                        * NO ALSO NO CREDITS JUST GET OUT DUDE GET OUTTTTTTTTTTTTTTTTTTTTTTT
                        */

                        switch (minion.CharData.BaseSkinName)
                        {
                            case "SRU_Dragon":
                                barWidth = 145;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 18),
                                    new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 28),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X - 22 + (float)(barWidth * x),
                                    hpBarPosition.Y - 5,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                            case "SRU_Red":
                            case "SRU_Blue":
                                barWidth = 145;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 17),
                                    new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 26),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X - 22 + (float)(barWidth * x),
                                    hpBarPosition.Y - 5,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                            case "SRU_Baron":
                                barWidth = 194;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X - 22 + (float)(barWidth * x), hpBarPosition.Y + 13),
                                    new Vector2(hpBarPosition.X - 22 + (float)(barWidth * x), hpBarPosition.Y + 29),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X - 22 + (float)(barWidth * x),
                                    hpBarPosition.Y - 3,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                            case "Sru_Crab":
                                barWidth = 61;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 45 + (float)(barWidth * x), hpBarPosition.Y + 35),
                                    new Vector2(hpBarPosition.X + 45 + (float)(barWidth * x), hpBarPosition.Y + 37),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + 40 + (float)(barWidth * x),
                                    hpBarPosition.Y + 16,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                            case "SRU_Murkwolf":
                                barWidth = 75;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 54 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                    new Vector2(hpBarPosition.X + 54 + (float)(barWidth * x), hpBarPosition.Y + 24),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + 50 + (float)(barWidth * x),
                                    hpBarPosition.Y,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                            case "SRU_Razorbeak":
                                barWidth = 75;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 54 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                    new Vector2(hpBarPosition.X + 54 + (float)(barWidth * x), hpBarPosition.Y + 24),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + 54 + (float)(barWidth * x),
                                    hpBarPosition.Y,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                            case "SRU_Krug":
                                barWidth = 81;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 58 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                    new Vector2(hpBarPosition.X + 58 + (float)(barWidth * x), hpBarPosition.Y + 24),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + 54 + (float)(barWidth * x),
                                    hpBarPosition.Y,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                            case "SRU_Gromp":
                                barWidth = 87;
                                Drawing.DrawLine(
                                    new Vector2(hpBarPosition.X + 62 + (float)(barWidth * x), hpBarPosition.Y + 18),
                                    new Vector2(hpBarPosition.X + 62 + (float)(barWidth * x), hpBarPosition.Y + 24),
                                    2f,
                                    Color.Chartreuse);
                                Drawing.DrawText(
                                    hpBarPosition.X + 58 + (float)(barWidth * x),
                                    hpBarPosition.Y,
                                    Color.Chartreuse,
                                    sDamage.ToString());
                                break;
                        }
                    }
                }
            }
            else
            {
                if (drawText)
                {
                    Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.Red, "Smite not active");
                }
            }

            if (smiteActive && drawSmite.Active
                && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready)
            {
                Render.Circle.DrawCircle(this.Player.Position, 500, Color.Green);
            }

            if (drawSmite.Active && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) != SpellState.Ready)
            {
                Render.Circle.DrawCircle(this.Player.Position, 500, Color.Red);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.Player.IsDead)
            {
                return;
            }

            try
            {
                this.JungleSmite();
                this.SmiteKill();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private float SmiteDamage()
        {
            return this.Player.Spellbook.GetSpell(this.SmiteSpell.Slot).State == SpellState.Ready
                       ? (float)this.Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                       : 0;
        }

        private void SmiteKill()
        {
            if (!this.Menu.Item("ElSmite.KS.Activated").GetValue<bool>())
            {
                return;
            }

            if (this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() != "s5_summonersmiteplayerganker")
            {
                return;
            }

            var kSableEnemy =
                HeroManager.Enemies.FirstOrDefault(
                    hero => !hero.IsZombie && hero.IsValidTarget(500) && 20 + 8 * this.Player.Level >= hero.Health);
            if (kSableEnemy != null)
            {
                this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, kSableEnemy);
            }
        }

        #endregion
    }
}