namespace ElRengarRevamped
{
    using System;
    using System.Collections.Generic;
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

                if (target.IsValidTarget() == false)
                {
                    return;
                }

                #region RengarR

                if (Ferocity <= 4)
                {
                    if (spells[Spells.Q].IsReady() && IsActive("Combo.Use.Q")
                        && Player.Distance(target) <= spells[Spells.Q].Range)
                    {
                        spells[Spells.Q].Cast();
                    }

                    if (!RengarR) 
                    {
                        if (!HasPassive)
                        {
                            if (spells[Spells.E].IsReady() && IsActive("Combo.Use.E"))
                            {
                                CastE(target);
                            }
                        }
                        else
                        {
                            if (spells[Spells.E].IsReady() && IsActive("Combo.Use.E") && Player.IsDashing())
                            {
                                CastE(target);
                            }
                        }
                    }

                    if (spells[Spells.W].IsReady() && IsActive("Combo.Use.W"))
                    {
                        CastW();
                    }
                }

                if (Ferocity == 5)
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            if (!RengarR)
                            {
                                if (spells[Spells.E].IsReady())
                                {
                                    CastE(target);

                                    if (IsActive("Combo.Switch.E") && Environment.TickCount - Rengar.LastE >= 500
                                        && Utils.GameTimeTickCount - LastSwitch >= 350)
                                    {
                                        MenuInit.Menu.Item("Combo.Prio")
                                            .SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                                        LastSwitch = Utils.GameTimeTickCount;
                                    }
                                }
                            }
                            break;
                        case 1:
                            if (IsActive("Combo.Use.W") && spells[Spells.W].IsReady())
                            {
                                CastW();
                            }
                            break;
                        case 2:
                            if (spells[Spells.Q].IsReady() && IsActive("Combo.Use.Q") && Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                            {
                                spells[Spells.Q].Cast();
                            }
                            break;
                    }

                    if (!RengarR)
                    {
                        if (IsActive("Combo.Use.E.OutOfRange"))
                        {
                            CastE(target);
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
            try
            {
                if (!spells[Spells.E].IsReady() || !target.IsValidTarget(spells[Spells.E].Range))
                {
                    return;
                }

                var prediction = spells[Spells.E].GetPrediction(target);
                if (prediction.CollisionObjects.Count == 0)
                {
                    spells[Spells.E].Cast(prediction.CastPosition);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void CastW()
        {
            try
            {
                if (!spells[Spells.W].IsReady() || Environment.TickCount - Rengar.LastE <= 200)
                {
                    return;
                }

                if (GetWHits().Item1 > 0)
                {
                    spells[Spells.W].Cast();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static Tuple<int, List<Obj_AI_Hero>> GetWHits()
        {
            try
            {
                var hits =
                    HeroManager.Enemies.Where(
                        e =>
                        e.IsValidTarget() && e.Distance(Player) < 450f
                        || e.Distance(Player) < 450f && e.IsFacing(Player)).ToList();

                return new Tuple<int, List<Obj_AI_Hero>>(hits.Count, hits);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Tuple<int, List<Obj_AI_Hero>>(0, null);
        }

        #endregion

        public static void Harass()
        {
            // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
            var target = TargetSelector.GetSelectedTarget() != null
                             ? TargetSelector.GetSelectedTarget()
                             : TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget() == false)
            {
                return;
            }

            #region RengarR

            if (Ferocity == 5)
            {
                switch (IsListActive("Harass.Prio").SelectedIndex)
                {
                    case 0:
                        if (!HasPassive && IsActive("Harass.Use.E") && spells[Spells.E].IsReady())
                        {
                            CastE(target);
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

                if (!HasPassive && IsActive("Harass.Use.E") && spells[Spells.E].IsReady())
                {
                    CastE(target);
                }

                if (IsActive("Harass.Use.W"))
                {
                    CastW();
                }
            }
        }

        public static void Jungleclear()
        {
            try
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

                if (IsActive("Jungle.Use.W") && spells[Spells.W].IsReady()
                    && minion.IsValidTarget(spells[Spells.W].Range) && !HasPassive)
                {
                    spells[Spells.W].Cast();
                }

                if (IsActive("Jungle.Use.E") && spells[Spells.E].IsReady()
                    && minion.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(minion.Position);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Laneclear()
        {
            try
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

                if (IsActive("Clear.Use.Q") && spells[Spells.Q].IsReady()
                    && minion.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast();
                }

                if (IsActive("Clear.Use.W") && spells[Spells.W].IsReady()
                    && minion.IsValidTarget(spells[Spells.W].Range))
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        private static Items.Item Youmuu => ItemData.Youmuus_Ghostblade.GetItem();

        /// <summary>
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        private static Items.Item Hydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        private static Items.Item Tiamat => ItemData.Tiamat_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        private static Items.Item Titanic => ItemData.Titanic_Hydra_Melee_Only.GetItem();

        public static bool CastItems(Obj_AI_Base target)
        {
            if (Player.IsDashing() || Player.IsWindingUp)
            {
                return false;
            }

            var units =
                MinionManager.GetMinions(385, MinionTypes.All, MinionTeam.NotAlly).Count(o => !(o is Obj_AI_Turret));
            var heroes = Player.GetEnemiesInRange(385).Count;
            var count = units + heroes;

            var tiamat = Tiamat;
            if (tiamat.IsReady() && count > 0 && tiamat.Cast())
            {
                return true;
            }

            var hydra = Hydra;
            if (Hydra.IsReady() && count > 0 && hydra.Cast())
            {
                return true;
            }

            var youmuus = Youmuu;
            if (Youmuu.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && youmuus.Cast())
            {
                return true;
            }

            var titanic = Titanic;
            return titanic.IsReady() && count > 0 && titanic.Cast();
        }

        #endregion
    }
}