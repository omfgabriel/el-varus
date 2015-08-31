﻿namespace ElRengarRevamped
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    public enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Rengar : Standards
    {
        #region Public Methods and Operators

        public static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Rengar")
            {
                return;
            }

            try
            {
                Ignite = Player.GetSpellSlot("summonerdot");
                Notifications.AddNotification(string.Format("ElRengarRevamped by jQuery v{0}", ScriptVersion), 6000);
                spells[Spells.E].SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                CustomEvents.Unit.OnDash += OnDash;
                Drawing.OnEndScene += OnDrawEndScene;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Methods

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (sender.IsMe)
            {
                if (IsListActive("Combo.Prio").SelectedIndex == 0 && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(target);
                    Console.WriteLine("E Ondash cast");
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var drawW = MenuInit.Menu.Item("Misc.Drawings.W").GetValue<Circle>();
            var drawE = MenuInit.Menu.Item("Misc.Drawings.E").GetValue<Circle>();

            if (IsActive("Misc.Drawings.Off"))
            {
                return;
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (IsActive("Misc.Drawings.Prioritized"))
            {
                switch (IsListActive("Combo.Prio").SelectedIndex)
                {
                    case 0:
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.White, "Prioritized spell: E");

                        break;
                    case 1:
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.White, "Prioritized spell: Q");
                        break;
                }
            }
        }

        private static void OnDrawEndScene(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (IsActive("Misc.Drawings.Minimap") && spells[Spells.R].Level > 0)
            {
                Utility.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White, 1, 23, true);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (RengarQ)
                {
                    Orbwalking.ResetAutoAttackTimer();
                }

                if (args.SData.Name == "RengarR" && Items.CanUseItem(3142))
                {
                    Utility.DelayAction.Add(1500, () => Items.UseItem(3142));
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            try
            {
                SwitchCombo();
                SmiteCombo();
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        ActiveModes.Combo();
                        break;

                    case Orbwalking.OrbwalkingMode.LaneClear:
                        ActiveModes.Laneclear();
                        ActiveModes.Jungleclear();
                        break;

                    case Orbwalking.OrbwalkingMode.Mixed:
                        ActiveModes.Harass();
                        break;
                }

                spells[Spells.R].Range = 1000 + spells[Spells.R].Level * 1000;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SwitchCombo()
        {
            var switchTime = Environment.TickCount - LastSwitch;

            if (MenuInit.Menu.Item("Combo.Switch").GetValue<KeyBind>().Active && switchTime >= 350)
            {
                switch (IsListActive("Combo.Prio").SelectedIndex)
                {
                    case 0:
                        MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "Q" }, 1));
                        LastSwitch = Environment.TickCount;
                        break;
                    case 1:
                        MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "Q" }, 0));
                        LastSwitch = Environment.TickCount;
                        break;

                    default:
                        MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "Q" }, 0));
                        LastSwitch = Environment.TickCount;
                        break;
                }
            }
        }

        #endregion
    }
}