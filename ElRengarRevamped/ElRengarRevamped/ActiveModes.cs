namespace ElRengarRevamped
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class ActiveModes : Standards
    {
        #region Public Methods and Operators

        public static void Combo()
        {
            try
            {
                // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
                var target = TargetSelector.GetSelectedTarget() != null
                                 ? TargetSelector.GetSelectedTarget()
                                 : TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Physical);

                if (target == null)
                {
                    return;
                }

                if (Rengar.SelectedEnemy.IsValidTarget(spells[Spells.E].Range))
                {
                    TargetSelector.SetTarget(target);
                    if (Hud.SelectedUnit != null)
                    {
                        Hud.SelectedUnit = Rengar.SelectedEnemy;
                    }
                }

                CastItems(target);

                #region RengarR

                if (Ferocity <= 4)
                {
                    if (IsActive("Combo.Use.W") && spells[Spells.W].IsReady())
                    {
                        CastW(target);
                    }

                    if (spells[Spells.Q].IsReady() && IsActive("Combo.Use.Q")
                        && target.IsValidTarget(spells[Spells.Q].Range))
                    {
                        spells[Spells.Q].Cast();
                    }

                    if (!HasPassive && IsActive("Combo.Use.E") && spells[Spells.E].IsReady())
                    {
                        if (target.IsValidTarget(spells[Spells.E].Range) && !RengarR)
                        {
                            CastE(target);
                        }
                    }
                }

                if (Ferocity == 5)
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            if (!RengarR && target.IsValidTarget(spells[Spells.E].Range) && spells[Spells.E].IsReady())
                            {
                                var prediction = spells[Spells.E].GetPrediction(target);
                                if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count == 0)
                                {
                                    if (spells[Spells.E].Cast(target).IsCasted())
                                    {
                                        if (IsActive("Combo.Switch.E") && Utils.GameTimeTickCount - LastSwitch >= 350)
                                        {
                                            MenuInit.Menu.Item("Combo.Prio")
                                                .SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                                            LastSwitch = Utils.GameTimeTickCount;
                                        }
                                    }
                                }
                            }
                            break;
                        case 1:
                            if (IsActive("Combo.Use.W") && spells[Spells.W].IsReady()
                                && target.IsValidTarget(spells[Spells.W].Range) && !HasPassive)
                            {
                                CastW(target);
                            }
                            break;
                        case 2:
                            if (IsActive("Combo.Use.Q") && spells[Spells.Q].IsReady()
                                && target.IsValidTarget(spells[Spells.Q].Range))
                            {
                                spells[Spells.Q].Cast();
                            }
                            break;
                    }

                    if (!RengarR)
                    {
                        if (IsActive("Combo.Use.E.OutOfRange") && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            var prediction = spells[Spells.E].GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.VeryHigh && prediction.CollisionObjects.Count == 0)
                            {
                                spells[Spells.E].Cast(target);
                            }
                        }
                    }
                }

                #region Summoner spells

                if (Youmuu.IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    Youmuu.Cast();
                }

                if (IsActive("Combo.Use.Smite") && !RengarR && Smite != SpellSlot.Unknown
                    && Player.Spellbook.CanUseSpell(Smite) == SpellState.Ready && !target.IsZombie)
                {
                    Player.Spellbook.CastSpell(Smite, target);
                }

                if (IsActive("Combo.Use.Ignite") && target.IsValidTarget(600f) && IgniteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }

                #endregion
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        #endregion

        #endregion

        #region Methods

        private static void CastE(Obj_AI_Base target)
        {
            if (!spells[Spells.E].IsReady() || !target.IsValidTarget(spells[Spells.E].Range))
            {
                return;
            }

            var prediction = spells[Spells.E].GetPrediction(target);

            if (prediction.Hitchance >= HitChance.High)
            {
                spells[Spells.E].Cast(target.ServerPosition);
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(spells[Spells.W].Range) || !spells[Spells.W].IsReady())
            {
                return;
            }
            spells[Spells.W].Cast();
        }

        #endregion

        public static void Harass()
        {
            // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
            var target = TargetSelector.GetSelectedTarget() != null
                             ? TargetSelector.GetSelectedTarget()
                             : TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

            if (target == null)
            {
                return;
            }

            #region RengarR

            if (Ferocity == 5)
            {
                switch (IsListActive("Harass.Prio").SelectedIndex)
                {
                    case 0:
                        if (!HasPassive && IsActive("Harass.Use.E") && spells[Spells.E].IsReady()
                            && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            var prediction = spells[Spells.E].GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count == 0)
                            {
                                spells[Spells.E].Cast(target);
                            }
                        }
                        break;

                    case 1:
                        if (IsActive("Harass.Use.Q") && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast();
                        }
                        break;
                }
            }

            if (Ferocity <= 4)
            {
                if (IsActive("Harass.Use.Q") && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast();
                }

                if (RengarR)
                {
                    return;
                }

                CastItems(target);

                if (!HasPassive && IsActive("Harass.Use.E") && spells[Spells.E].IsReady()
                    && target.IsValidTarget(spells[Spells.E].Range))
                {
                    var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count == 0)
                    {
                        spells[Spells.E].Cast(target);
                    }
                }

                if (IsActive("Harass.Use.W") && target.IsValidTarget(spells[Spells.W].Range))
                {
                    spells[Spells.W].Cast();
                }
            }
        }

        public static void Jungleclear()
        {
            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minion == null)
            {
                return;
            }

            CastItems(minion);

            if (Ferocity == 5 && IsActive("Jungle.Save.Ferocity"))
            {
                if (minion.IsValidTarget(spells[Spells.W].Range) && !HasPassive)
                {
                    CastItems(minion);
                }
                return;
            }

            if (IsActive("Jungle.Use.Q") && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range + 100))
            {
                spells[Spells.Q].Cast();
            }

            if (IsActive("Jungle.Use.W") && spells[Spells.W].IsReady() && minion.IsValidTarget(spells[Spells.W].Range)
                && !HasPassive)
            {
                spells[Spells.W].Cast();
            }

            if (IsActive("Jungle.Use.E") && spells[Spells.E].IsReady() && minion.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(minion.Position);
            }
        }

        public static void Laneclear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            if (Player.Spellbook.IsAutoAttacking || Player.IsWindingUp)
            {
                return;
            }
            if (Ferocity == 5 && IsActive("Clear.Save.Ferocity"))
            {
                if (minion.IsValidTarget(spells[Spells.W].Range))
                {
                    CastItems(minion);
                }
                return;
            }

            if (IsActive("Clear.Use.Q") && spells[Spells.Q].IsReady() && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
            }

            if (IsActive("Clear.Use.W") && spells[Spells.W].IsReady() && minion.IsValidTarget(spells[Spells.W].Range))
            {
                CastItems(minion);
                spells[Spells.W].Cast();
            }

            if (IsActive("Clear.Use.E") && spells[Spells.E].GetDamage(minion) > minion.Health
                && spells[Spells.E].IsReady() && minion.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(minion.Position);
            }
        }

        public static void CastItems(Obj_AI_Base target)
        {
            if (target.IsValidTarget(400f))
            {
                if (Items.HasItem(3074) || Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }

                if (Items.HasItem(3077) || Items.CanUseItem(3077)) 
                {
                    Items.UseItem(3077);
                }
            }

            if (target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
            {
                if (Items.HasItem(3748) || Items.CanUseItem(3748))
                {
                    Items.UseItem(3748);
                }
            }
        }

        #endregion
    }
}