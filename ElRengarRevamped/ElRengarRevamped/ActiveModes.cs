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
                                 : TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);

                if (target == null || !target.IsValidTarget())
                {
                    return;
                }

                if (Rengar.SelectedEnemy.IsValidTarget())
                {
                    Rengar.SelectedEnemy = target;
                }

                if (Youmuu.IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    Youmuu.Cast();
                }

                if (IsActive("Combo.Use.Smite") && !RengarR && Smite != SpellSlot.Unknown
                    && Player.Spellbook.CanUseSpell(Smite) == SpellState.Ready && !target.IsZombie)
                {
                    Player.Spellbook.CastSpell(Smite, target);
                }

                UseItems(target);


                #region RengarR

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
                            if (IsActive("Combo.Use.W") && target.IsValidTarget(spells[Spells.W].Range) && spells[Spells.W].IsReady() && !RengarR && !Player.IsDashing() && !HasPassive)
                            {
                                CastW(target);
                            }
                            break;
                        case 2:
                            if (IsActive("Combo.Use.Q") && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                            {
                                spells[Spells.Q].Cast();
                            }
                            break;
                    }
                }

                if (Ferocity <= 4)
                {
                    if (IsActive("Combo.Use.W") && spells[Spells.W].IsReady())
                    {
                        CastW(target);
                    }

                    if (spells[Spells.Q].IsReady() && IsActive("Combo.Use.Q") && target.IsValidTarget(spells[Spells.Q].Range))
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

                if (Ferocity == 5 && !RengarR)
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

                #region Summoner spells

                if (IsActive("Combo.Use.Ignite") && target.IsValidTarget(600f)
                    && IgniteDamage(target) >= target.Health)
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
            if (!spells[Spells.E].IsReady() || !target.IsValidTarget(spells[Spells.E].Range) || Player.IsDashing())
            {
                return;
            }

            var prediction = spells[Spells.E].GetPrediction(target);

            if (prediction.Hitchance >= HitChance.High)
            {
                spells[Spells.E].Cast(prediction.CastPosition);
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(spells[Spells.W].Range))
            {
                return;
            }
            spells[Spells.W].Cast();
        }

        private static void UseItems(Obj_AI_Base target)
        {
            if (target.Distance(Player.ServerPosition) <= spells[Spells.W].Range)
            {
                if (Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }

                if (Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }
            }

            if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady()
                && ItemData.Blade_of_the_Ruined_King.Range > Player.Distance(target))
            {
                ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
            }

            if (ItemData.Bilgewater_Cutlass.GetItem().IsReady()
                && ItemData.Bilgewater_Cutlass.Range > Player.Distance(target))
            {
                ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()
                    && Orbwalking.GetRealAutoAttackRange(Player) > Player.Distance(target))
                {
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
                }
            }
        }

        #endregion

        public static void Harass()
        {
            // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
            var target = TargetSelector.GetSelectedTarget() != null
                                ? TargetSelector.GetSelectedTarget()
                                : TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValidTarget())
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
                    UseHydra();
                    spells[Spells.W].Cast();
                }

                if (!IsActive("Harass.Use.W")
                    || !spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                {
                    UseHydra();
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

            UseItems(minion);

            if (Ferocity == 5 && IsActive("Jungle.Save.Ferocity"))
            {
                if (minion.IsValidTarget(spells[Spells.W].Range) && !HasPassive)
                {
                    UseHydra();
                }
                return;
            }

            if (IsActive("Jungle.Use.Q") && spells[Spells.Q].IsReady() && minion.IsValidTarget(spells[Spells.Q].Range + 100))
            {
                spells[Spells.Q].Cast();
            }

            if (IsActive("Jungle.Use.W") && spells[Spells.W].IsReady() && minion.IsValidTarget(spells[Spells.W].Range) && !HasPassive)
            {
                UseHydra();
                spells[Spells.W].Cast();
            }

            if (IsActive("Jungle.Use.E") && spells[Spells.E].IsReady() && minion.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(minion.Position);
            }
        }

        public static void LastHit()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            if (Ferocity == 5 && IsActive("Clear.Save.Ferocity"))
            {
                if (minion.IsValidTarget(spells[Spells.W].Range))
                {
                    UseHydra();
                }
                return;
            }

            if (IsActive("Clear.Use.Q") && spells[Spells.Q].IsReady() && minion.IsValidTarget(spells[Spells.Q].Range + 150) && spells[Spells.Q].GetDamage(minion) > minion.Health)
            {
                spells[Spells.Q].Cast();
            }

            if (IsActive("Clear.Use.E") && spells[Spells.E].GetDamage(minion) > minion.Health
               && spells[Spells.E].IsReady() && minion.IsValidTarget(spells[Spells.E].Range))
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
                    UseHydra();
                }
                return;
            }

            if (IsActive("Clear.Use.Q") && spells[Spells.Q].IsReady() && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
            }

            if (IsActive("Clear.Use.W") && spells[Spells.W].IsReady() && minion.IsValidTarget(spells[Spells.W].Range))
            {
                UseHydra();
                spells[Spells.W].Cast();
            }

            if (IsActive("Clear.Use.E") && spells[Spells.E].GetDamage(minion) > minion.Health
                && spells[Spells.E].IsReady() && minion.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(minion.Position);
            }
        }

        #endregion
    }
}