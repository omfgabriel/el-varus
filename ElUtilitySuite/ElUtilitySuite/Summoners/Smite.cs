namespace ElUtilitySuite.Summoners
{
    using System;
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

        public static Spell smite;

        public static SpellSlot smiteSlot;

        private static readonly string[] BuffsThatActuallyMakeSenseToSmite =
            {
                "SRU_Red", "SRU_Blue", "SRU_Dragon",
                "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf",
                "SRU_Razorbeak", "SRU_RiftHerald",
                "SRU_Krug", "Sru_Crab", "TT_Spiderboss",
                "TTNGolem", "TTNWolf", "TTNWraith"
            };

        private static SpellDataInst slot1;

        private static SpellDataInst slot2;

        #endregion

        #region Properties

        private Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        public static double SmiteDamage()
        {
            var damage = new[]
                             {
                                 20 * Entry.Player.Level + 370, 30 * Entry.Player.Level + 330,
                                 40 * +Entry.Player.Level + 240, 50 * Entry.Player.Level + 100
                             };

            return Entry.Player.Spellbook.CanUseSpell(smite.Slot) == SpellState.Ready ? damage.Max() : 0;
        }

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

                if (Entry.IsSummonersRift)
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

                if (Entry.IsTwistedTreeline)
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
                slot1 = Entry.Player.Spellbook.GetSpell(SpellSlot.Summoner1);
                slot2 = Entry.Player.Spellbook.GetSpell(SpellSlot.Summoner2);
                var smiteNames = new[]
                                     {
                                         "s5_summonersmiteplayerganker", "itemsmiteaoe", "s5_summonersmitequick",
                                         "s5_summonersmiteduel", "summonersmite"
                                     };

                if (smiteNames.Contains(slot1.Name))
                {
                    smite = new Spell(SpellSlot.Summoner1, 550f);
                    smiteSlot = SpellSlot.Summoner1;
                }
                else if (smiteNames.Contains(slot2.Name))
                {
                    smite = new Spell(SpellSlot.Summoner2, 550f);
                    smiteSlot = SpellSlot.Summoner2;
                }
                else
                {
                    Console.WriteLine("You don't have smite faggot");
                    return;
                }

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

        private static double SmiteChampDamage()
        {
            if (smite.Slot == Entry.Player.GetSpellSlot("s5_summonersmiteduel"))
            {
                var damage = new[] { 54 + 6 * Entry.Player.Level };
                return Entry.Player.Spellbook.CanUseSpell(smite.Slot) == SpellState.Ready ? damage.Max() : 0;
            }

            if (smite.Slot == Entry.Player.GetSpellSlot("s5_summonersmiteplayerganker"))
            {
                var damage = new[] { 20 + 8 * Entry.Player.Level };
                return Entry.Player.Spellbook.CanUseSpell(smite.Slot) == SpellState.Ready ? damage.Max() : 0;
            }

            return 0;
        }

        private void JungleSmite()
        {
            if (!this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active)
            {
                return;
            }

            minion =
                (Obj_AI_Minion)
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 500, MinionTypes.All, MinionTeam.Neutral)
                    .ToList()
                    .FirstOrDefault(
                        buff =>
                        buff.IsValidTarget() && BuffsThatActuallyMakeSenseToSmite.Contains(buff.CharData.BaseSkinName));

            if (minion == null)
            {
                return;
            }

            if (this.Menu.Item(minion.CharData.BaseSkinName).GetValue<bool>())
            {
                if (SmiteDamage() > minion.Health)
                {
                    Entry.Player.Spellbook.CastSpell(smite.Slot, minion);
                }
            }
        }

        private void OnDraw(EventArgs args)
        {
            var smiteActive = this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active;
            var drawSmite = this.Menu.Item("ElSmite.Draw.Range").GetValue<Circle>();
            var drawText = this.Menu.Item("ElSmite.Draw.Text").GetValue<bool>();
            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var drawDamage = this.Menu.Item("ElSmite.Draw.Damage").GetValue<bool>();

            if (smiteActive)
            {
                if (drawText && Entry.Player.Spellbook.CanUseSpell(smite.Slot) == SpellState.Ready)
                {
                    Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.GhostWhite, "Smite active");
                }

                if (drawText && Entry.Player.Spellbook.CanUseSpell(smite.Slot) != SpellState.Ready)
                {
                    Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.Red, "Smite cooldown");
                }

                if (drawDamage && SmiteDamage() != 0)
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
                        var sDamage = SmiteDamage();
                        //SmiteDamage : MaxHealth = x : 100
                        //Ratio math for this ^
                        var x = SmiteDamage() / maxHealth;
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

            if (smiteActive && drawSmite.Active && Entry.Player.Spellbook.CanUseSpell(smite.Slot) == SpellState.Ready)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 500, Color.Green);
            }

            if (drawSmite.Active && Entry.Player.Spellbook.CanUseSpell(smite.Slot) != SpellState.Ready)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 500, Color.Red);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Entry.Player.IsDead)
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

        private void SmiteKill()
        {
            if (!this.Menu.Item("ElSmite.KS.Activated").GetValue<bool>())
            {
                return;
            }

            var kSableEnemy =
                HeroManager.Enemies.FirstOrDefault(hero => hero.IsValidTarget(550) && SmiteChampDamage() >= hero.Health);
            if (kSableEnemy != null)
            {
                Entry.Player.Spellbook.CastSpell(smite.Slot, kSableEnemy);
            }
        }

        #endregion
    }
}