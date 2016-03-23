namespace ElJayce
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Program
    {
        #region Static Fields

        private static readonly int cannonRange = 500;

        private static readonly int hammerRange = 125;

        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        private static int blockCount;

        private static bool cancelMovt;

        private static Spell cannonE;

        private static Spell cannonQ;

        private static Spell cannonQ2;

        private static Spell cannonW;

        private static double castQSecond;

        private static Menu configMenu;

        private static string gapMode = string.Empty;

        private static Spell hammerE;

        private static Spell hammerQ;

        private static Spell hammerW;

        private static double lastAttackSecond = 0;

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell R;

        private static string shotMode = string.Empty;

        #endregion

        #region Public Methods and Operators

        public static bool IsHammer()
        {
            return Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name.ToLower().Equals("jayceshockblast");
        }

        #endregion

        #region Methods

        private static void Combo()
        {
            Orbwalker.SetAttack(true);

            var useQ = configMenu.SubMenu("Combo").Item("ElJayce.Cannon.Q").IsActive();
            var useW = configMenu.SubMenu("Combo").Item("ElJayce.Cannon.W").IsActive();
            var useE = configMenu.SubMenu("Combo").Item("ElJayce.Cannon.E").IsActive();
            var useR = configMenu.SubMenu("Combo").Item("ElJayce.Cannon.R").IsActive();

            var useHammerQ = configMenu.SubMenu("Combo").Item("ElJayce.Hammer.Q").IsActive();
            var useHammerW = configMenu.SubMenu("Combo").Item("ElJayce.Hammer.W").IsActive();
            var useHammerE = configMenu.SubMenu("Combo").Item("ElJayce.Hammer.E").IsActive();
            var useHammerR = configMenu.SubMenu("Combo").Item("ElJayce.Hammer.R").IsActive();

            if (IsHammer())
            {
                var target =
                    (TargetSelector.GetTarget(hammerQ.Range, TargetSelector.DamageType.Physical)) as Obj_AI_Base;

                if (target != null && hammerQ.IsReady())
                {
                    if (Player.Distance(target) < hammerQ.Range && hammerQ.IsReady() && useHammerQ)
                    {
                        hammerQ.CastOnUnit(target);
                        gapMode = "Combo";
                    }
                }
                else if (target != null)
                {
                    if (Player.Distance(target) > hammerRange)
                    {
                        if ((Player.Distance(target) + 80 <= hammerE.Range && hammerE.IsReady() && useHammerE)
                            && !hammerQ.IsReady())
                        {
                            hammerE.CastOnUnit(target);
                        }
                    }
                    else if (Player.GetSpellDamage(target, SpellSlot.E) >= target.Health && hammerE.IsReady()
                             && useHammerE && Player.Distance(target) <= hammerE.Range)
                    {
                        hammerE.CastOnUnit(target);
                    }

                    if (!hammerQ.IsReady() && Player.Distance(target) < cannonQ2.Range && useHammerQ)
                    {
                        if (hammerW.IsReady() && useHammerW)
                        {
                            hammerW.Cast();
                        }

                        if (R.IsReady() && useHammerR)
                        {
                            if (!hammerE.IsReady() && useHammerE)
                            {
                                R.Cast();
                            }
                            else if (!useHammerE)
                            {
                                R.Cast();
                            }
                        }
                    }
                    else if (!useHammerQ)
                    {
                        if (!hammerE.IsReady() && useHammerE)
                        {
                            R.Cast(true);
                        }
                        else if (!useHammerE)
                        {
                            R.Cast(true);
                        }
                    }
                }
            }
            else
            {
                var target = TargetSelector.GetTarget(cannonQ2.Range, TargetSelector.DamageType.Physical);
                var target2 = TargetSelector.GetTarget(cannonQ.Range, TargetSelector.DamageType.Physical);
                var target3 = TargetSelector.GetTarget(hammerQ.Range, TargetSelector.DamageType.Physical);

                if (target != null && cannonQ2.IsReady() && cannonE.IsReady() && useQ && useE)
                {
                    var pred = cannonQ2.GetPrediction(target);

                    if (pred.Hitchance >= HitChance.High)
                    {
                        cannonQ2.Cast(pred.CastPosition, true);
                        shotMode = "Combo";
                    }
                }

                else if (target2 != null && cannonQ.IsReady() && useQ)
                {
                    var pred = cannonQ.GetPrediction(target2);

                    if (pred.Hitchance >= HitChance.High)
                    {
                        cannonQ.Cast(pred.CastPosition);
                    }
                }

                if (target3 != null && useR)
                {
                    if (!cannonQ.IsReady() && useQ)
                    {
                        if (!cannonW.IsReady() && useW)
                        {
                            R.Cast();
                        }
                        else if (!useW)
                        {
                            R.Cast();
                        }
                    }
                    else if (!useQ)
                    {
                        if (!cannonW.IsReady() && useW)
                        {
                            R.Cast();
                        }
                        else if (!useW)
                        {
                            R.Cast();
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawingQ = configMenu.SubMenu("Drawing").Item("QRange").GetValue<Circle>();
            if (drawingQ.Active)
            {
                if (IsHammer())
                {
                    Render.Circle.DrawCircle(Player.Position, hammerQ.Range, Color.DeepSkyBlue);
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, cannonQ.Range, drawingQ.Color);
                }
            }

            var drawingW = configMenu.SubMenu("Drawing").Item("WRange").GetValue<Circle>();
            if (drawingW.Active)
            {
                if (IsHammer())
                {
                    Render.Circle.DrawCircle(Player.Position, hammerW.Range, drawingQ.Color);
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, cannonW.Range, drawingQ.Color);
                }
            }

            var drawingE = configMenu.SubMenu("Drawing").Item("ERange").GetValue<Circle>();
            if (drawingE.Active)
            {
                if (IsHammer())
                {
                    Render.Circle.DrawCircle(Player.Position, hammerE.Range, drawingQ.Color);
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, cannonE.Range, drawingQ.Color);
                }
            }
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                if (Player.ChampionName != "Jayce")
                {
                    return;
                }

                cannonQ = new Spell(SpellSlot.Q, 1050);
                cannonQ2 = new Spell(SpellSlot.Q, 1460);
                cannonW = new Spell(SpellSlot.W, cannonRange);
                cannonE = new Spell(SpellSlot.E, 650);

                hammerQ = new Spell(SpellSlot.Q, 600);
                hammerW = new Spell(SpellSlot.W, 285);
                hammerE = new Spell(SpellSlot.E, 230);

                R = new Spell(SpellSlot.R);

                cannonQ.SetSkillshot(0.15f, 70, 1200, true, SkillshotType.SkillshotLine);
                cannonQ2.SetSkillshot(0.15f, 70, 1680, true, SkillshotType.SkillshotLine);
                cannonE.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
                hammerQ.SetTargetted(0.15f, float.MaxValue);
                hammerE.SetTargetted(0.15f, float.MaxValue);

                configMenu = new Menu("ElJayce", "Jayce", true);

                configMenu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

                TargetSelector.AddToMenu(configMenu.SubMenu("Target Selector"));
                TargetSelector.Mode = TargetSelector.TargetingMode.LessAttack;

                configMenu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Orbwalker = new Orbwalking.Orbwalker(configMenu.SubMenu("Orbwalker"));

                configMenu.AddSubMenu(new Menu("Combo", "Combo"));

                configMenu.AddSubMenu(new Menu("Harass", "Harass"));

                configMenu.SubMenu("Harass").AddItem(new MenuItem("ElJayce.Cannon.Q", "Use Cannon Q").SetValue(true));
                configMenu.SubMenu("Harass").AddItem(new MenuItem("ElJayce.Cannon.W", "Use Cannon W").SetValue(true));
                configMenu.SubMenu("Harass").AddItem(new MenuItem("ElJayce.Cannon.E", "Use Cannon E").SetValue(true));

                configMenu.AddSubMenu(new Menu("Misc", "Misc"));
                configMenu.SubMenu("Misc").AddItem(new MenuItem("AutoGate", "Auto Gate On").SetValue(true));

                configMenu.AddToMainMenu();

                InitEvent();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }

            var currentTime = new DateTime(2001, 1, 1);
            var currentSpan = new TimeSpan(currentTime.Ticks);
        }

        private static void Harass()
        {
            var useQ = configMenu.SubMenu("Harass").Item("ElJayce.Cannon.Q").IsActive();
            var useE = configMenu.SubMenu("Harass").Item("ElJayce.Cannon.E").IsActive();

            var target = TargetSelector.GetTarget(cannonQ2.Range, TargetSelector.DamageType.Physical);
            var target2 = TargetSelector.GetTarget(cannonQ.Range, TargetSelector.DamageType.Physical);

            if (target != null && cannonQ2.IsReady() && cannonE.IsReady() && useQ && useE)
            {
                var pred = cannonQ2.GetPrediction(target);

                if (pred.Hitchance >= HitChance.High)
                {
                    cannonQ2.Cast(pred.CastPosition);
                    shotMode = "Harass";
                }
            }
            else if (target2 != null && cannonQ.IsReady() && useQ)
            {
                var pred = cannonQ.GetPrediction(target2);

                if (pred.Hitchance >= HitChance.High)
                {
                    cannonQ.Cast(pred.CastPosition);
                }
            }
        }

        private static void InitEvent()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Obj_AI_Base.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Orbwalking.AfterAttack += OrbwalkingAfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
           if (!sender.IsMe)
            {
                return;
            }

            if (cancelMovt)
            {
                blockCount++;

                if (blockCount == 10)
                {
                    cancelMovt = false;
                    blockCount = 0;
                }
                else
                {
                    args.Process = false;
                }
            }
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower().Equals("jayceshockblast"))
            {
                var ePosition = Player.ServerPosition + Vector3.Normalize(args.End - Player.ServerPosition) * 50;
                if (cannonE.IsReady())
                {
                    var currentTime = new DateTime(2001, 1, 1);
                    var currentSpan = new TimeSpan(currentTime.Ticks);

                    castQSecond = currentSpan.TotalSeconds;
                    if (shotMode.Equals("Combo") && configMenu.SubMenu("Combo").Item("ElJayce.Cannon.E").IsActive())
                    {
                        cancelMovt = true;
                        cannonE.Cast(ePosition);
                        shotMode = string.Empty;
                    }
                    else if (shotMode.Equals("Harass")
                             && configMenu.SubMenu("Harass").Item("ElJayce.Cannon.E").IsActive())
                    {
                        cancelMovt = true;
                        cannonE.Cast(ePosition);
                        shotMode = string.Empty;
                    }
                    else if (configMenu.SubMenu("Misc").Item("AutoGate").IsActive())
                    {
                        cancelMovt = true;
                        cannonE.Cast(ePosition);
                    }
                }
            }
            else if (sender.IsMe && args.SData.Name.Equals("jayceaccelerationgate"))
            {
                cancelMovt = false;
                blockCount = 0;
            }
            else if (sender.IsMe && args.SData.Name.Equals("jaycetotheskies"))
            {
                var target = TargetSelector.GetTarget(hammerW.Range, TargetSelector.DamageType.Physical);

                if (target != null && hammerW.IsReady())
                {
                    if (!gapMode.Equals("Combo") || !configMenu.SubMenu("Combo").Item("ElJayce.Hammer.W").IsActive())
                    {
                        return;
                    }

                    hammerW.Cast();
                    gapMode = string.Empty;
                }
            }
        }

        private static void OrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var useW = configMenu.SubMenu("Combo").Item("ElJayce.Cannon.W").IsActive();

            if (IsHammer() || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !useW || !cannonW.IsReady() || !unit.IsMe)
            {
                return;
            }
            Orbwalking.ResetAutoAttackTimer();
            cannonW.Cast();
        }

        #endregion
    }
}