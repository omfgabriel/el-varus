namespace Elvarus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
	using Color = System.Drawing.Color;
    using LeagueSharp;
    using LeagueSharp.Common;
	using System.Drawing;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Varus
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;
        public static int LastQ, LastE;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 925f) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 0) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 1050f) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 1075f) }
                                                             };


        #endregion

        #region Public Properties

        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators
		


        public static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Varus")
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(.225f, 71f, 1835f, false, SkillshotType.SkillshotLine) ;
            spells[Spells.E].SetSkillshot(0.50f, 250f, 1500f, false, SkillshotType.SkillshotCircle);
            spells[Spells.R].SetSkillshot(.25f, 120f, 1950f, false, SkillshotType.SkillshotLine);

            spells[Spells.Q].SetCharged("VarusQ", "VarusQ", 250, 1625, 1.2f);


            ElVarusMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
			Drawing.OnDraw += Drawings.Drawing_OnDraw;
			AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(
                (spells[Spells.Q].ChargedMaxRange + spells[Spells.Q].Width) * 1.1f,
                TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }

			

            if (spells[Spells.E].IsReady() && !spells[Spells.Q].IsCharging
                && ElVarusMenu.Menu.Item("ElVarus.Combo.E").IsActive() && ElVarusMenu.Menu.Item("ElVarus.omfgabriel").GetValue<StringList>().SelectedIndex == 0
				&& LastQ + 2000 < Environment.TickCount && GetWStacks(target) >= ElVarusMenu.Menu.Item("ElVarus.ComboE.Stack.Count").GetValue<Slider>().Value )
            {
		
                    /*var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
						LastE = Environment.TickCount;						
                    }
					*/
                        
                            spells[Spells.E].Cast(target);
							LastE = Environment.TickCount;	
                        

            }
			
            if (spells[Spells.E].IsReady() && !spells[Spells.Q].IsCharging
                && ElVarusMenu.Menu.Item("ElVarus.Combo.E").IsActive() && ElVarusMenu.Menu.Item("ElVarus.omfgabriel").GetValue<StringList>().SelectedIndex == 1)
            {

                    /*var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
						LastE = Environment.TickCount;						
                    }
					*/
                        
                            spells[Spells.E].Cast(target);
							LastE = Environment.TickCount;	
                        
            }


			
            if (spells[Spells.Q].IsReady() && ElVarusMenu.Menu.Item("ElVarus.Combo.Q").IsActive() && ElVarusMenu.Menu.Item("ElVarus.omfgabriel").GetValue<StringList>().SelectedIndex == 0)
            {
                if (( spells[Spells.Q].IsCharging 
				//&& !spells[Spells.E].IsReady() 
				&& Environment.TickCount > LastE + 2000 ||  GetWStacks(target) >= ElVarusMenu.Menu.Item("ElVarus.Combo.Stack.Count").GetValue<Slider>().Value 
				//&& !spells[Spells.E].IsReady() 
				&& Environment.TickCount > LastE + 2000
				|| spells[Spells.Q].IsKillable(target)))
                {
                    if (!spells[Spells.Q].IsCharging)
                    {
                        spells[Spells.Q].StartCharging();
                    }

                    if (spells[Spells.Q].IsCharging)

                        {
                            spells[Spells.Q].Cast(target);
							LastQ = Environment.TickCount;	
                        }

                }
            }
			
            if (spells[Spells.Q].IsReady() && ElVarusMenu.Menu.Item("ElVarus.Combo.Q").IsActive() && ElVarusMenu.Menu.Item("ElVarus.omfgabriel").GetValue<StringList>().SelectedIndex == 1)
            {
                if (spells[Spells.Q].IsCharging || target.Distance(Player) > Orbwalking.GetRealAutoAttackRange(target) * 1.2f && target.Distance(Player) < 1600
                    || GetWStacks(target) >= ElVarusMenu.Menu.Item("ElVarus.Combo.Stack.Count").GetValue<Slider>().Value
                    || spells[Spells.Q].IsKillable(target))
                {
                    if (!spells[Spells.Q].IsCharging)
                    {
                        spells[Spells.Q].StartCharging();
                    }
					
                    if (spells[Spells.Q].IsCharging)
                    {
                        {
                            spells[Spells.Q].Cast(target);	
                        }
                    }					

                }
            }			
			
		
			

			
        }
		
		private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].ChargedMaxRange, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (Player.ManaPercent > ElVarusMenu.Menu.Item("minmanaharass").GetValue<Slider>().Value)
            {
                if (ElVarusMenu.Menu.Item("ElVarus.Harass.E").IsActive() && spells[Spells.E].IsReady() )
                {
                    var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
                    }
                }

                if (ElVarusMenu.Menu.Item("ElVarus.Harass.Q").IsActive() && spells[Spells.Q].IsReady())
                {
                    if (!spells[Spells.Q].IsCharging)
                    {
                        spells[Spells.Q].StartCharging();
                    }

                    if (spells[Spells.Q].IsCharging)
                    {
                        {
                            spells[Spells.Q].Cast(target);
                        }
                    }
                    
					/*if (spells[Spells.Q].IsCharging)
                    {
                        if (Player.Health < Player.MaxHealth * 0.5 && target.Distance(ObjectManager.Player) < 700 || target.Distance(ObjectManager.Player) < 600)		
                            
                        {
                            fart[Spells.Q].Cast(target);
                        }
					}		
					*/
                }
            }
        }

        private static void LogicR()
        {
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(spells[Spells.R].Range)))
            {
				
            if (enemy.CountEnemiesInRange(400) >= ElVarusMenu.Menu.Item("rCount", true).GetValue<Slider>().Value && ElVarusMenu.Menu.Item("rCount", true).GetValue<Slider>().Value > 0)
                {
                    spells[Spells.R].Cast(enemy, true, true);
                }
					

            if ((enemy.CountAlliesInRange(600) == 0 || Player.Health < Player.MaxHealth * 0.5)&& ElVarusMenu.Menu.Item("RKS123" + enemy.ChampionName).GetValue<bool>() && spells[Spells.Q].IsReady() && spells[Spells.R].GetDamage(enemy) + (GetWStacks(enemy) * spells[Spells.Q].GetDamage(enemy, 1)) + spells[Spells.Q].GetDamage(enemy) > enemy.Health && ValidUlt(enemy))
                {
                    spells[Spells.R].Cast(enemy);				
                }
            }
            if (Player.Health < Player.MaxHealth * 0.6 )
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(400) && target.IsMelee && ElVarusMenu.Menu.Item("GapCloser" + target.ChampionName).GetValue<bool>()))
                {
                    spells[Spells.R].Cast(target);
                }
            }
        }

        private static int GetWStacks(Obj_AI_Base target)
        {
            return target.GetBuffCount("varuswdebuff");
        }
		

		public static bool ValidUlt(Obj_AI_Hero target)
        {
            if (target.HasBuffOfType(BuffType.PhysicalImmunity) || target.HasBuffOfType(BuffType.SpellImmunity)
                || target.IsZombie || target.IsInvulnerable || target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuff("kindredrnodeathbuff")
                || target.HasBuffOfType(BuffType.SpellShield) )
                return false;
            else
                return true;
        }

        
        private static void ChainCC()
        {
            if (!spells[Spells.R].IsReady()) return;

            foreach (var target in HeroManager.Enemies.Where(t => t.IsValidTarget(spells[Spells.R].Range)))
            {
                if (target != null)
                {
                    if (ElVarusMenu.Menu.Item("chainCC" + target.ChampionName).GetValue<bool>())
                    {
                        foreach (var buff in target.Buffs)
                        {
                            if (buff.Type == BuffType.Charm || buff.Type == BuffType.Fear ||
                                buff.Type == BuffType.Stun || buff.Type == BuffType.Taunt ||
                                buff.Type == BuffType.Flee || buff.Type == BuffType.Knockup ||
                                buff.Type == BuffType.Polymorph || buff.Type == BuffType.Suppression ||
                                buff.Type == BuffType.Snare)
                            {
                                var buffEndTime = buff.EndTime -
                                                  (target.PercentCCReduction*(buff.EndTime - buff.StartTime));
                                var cctimeleft = buffEndTime - Game.Time;
                                var speed = target.Position.Distance(Player.Position)/spells[Spells.R].Speed;
                                if (cctimeleft <= speed)
                                {
                                    spells[Spells.R].Cast(target);
                                }
                            }
                        }
                    }
                }
            }
			
		}
		
        private static void Killsteal()
        {

            if (ElVarusMenu.Menu.Item("ElVarus.KSSS").IsActive() && spells[Spells.Q].IsReady())
            {
                foreach (var target in
                    HeroManager.Enemies.Where(
                        enemy =>
                        enemy.IsValidTarget() && spells[Spells.Q].IsKillable(enemy)
                        && Player.Distance(enemy.Position) <= spells[Spells.Q].ChargedMaxRange - 100))
                {
                    if (!spells[Spells.Q].IsCharging && ElVarusMenu.Menu.Item("ElVarus.omfgabriel").GetValue<StringList>().SelectedIndex == 0
						&& ((LastE + 2000 < Environment.TickCount && target.Distance(ObjectManager.Player) > 425)|| target.Distance(ObjectManager.Player) <= 425))
                    {
                        spells[Spells.Q].StartCharging();
                    }
					
                    if (!spells[Spells.Q].IsCharging && ElVarusMenu.Menu.Item("ElVarus.omfgabriel").GetValue<StringList>().SelectedIndex == 1)
                    {
                        spells[Spells.Q].StartCharging();
                    }

                    if (spells[Spells.Q].IsCharging )
                    {
						spells[Spells.Q].Cast(target);
						LastQ = Environment.TickCount;	
                    }
					
					/*if (spells[Spells.Q].IsCharging)
                    {
                        if (target.Distance(ObjectManager.Player) < 425)		
                            
                        {
                            fart[Spells.Q].Cast(target);
                        }
					}	
					*/
                }
            }			
			
            if (ElVarusMenu.Menu.Item("ElVarus.KSSS").IsActive() && LastQ + 2000 < Environment.TickCount && spells[Spells.E].IsReady())
            {
                if (spells[Spells.E].IsCharging)
                {
                    return;
                }

                var killableEnemy =
                    HeroManager.Enemies.FirstOrDefault(e => spells[Spells.E].IsKillable(e) && !Orbwalking.InAutoAttackRange(e) && e.IsValidTarget(spells[Spells.E].Range + spells[Spells.E].Width));

                if (killableEnemy != null)
                {
                    spells[Spells.E].Cast(killableEnemy);
					LastE = Environment.TickCount;	
                }
            }			
        }

        //Credits to Sebby for Gapcloser, R KS, R AOE, R etc gapcloser function, some OKTWCommon functions (oktw)
		//Credits to God for his Q/E killsteal, ChainCC (godvarus)
		//Credits to Nathan for a lot of Reworked features I stole and learned from and elvarus base (elvarus, elvarus reworked)
		//Credits to Seph for Safety functions template (lissandra - ice witch)
		//Credits to Kortatu for Interrupter (orianna#)
		//Credits to Lizzarran for W stack drawing (sfxchallenger)
		//Beaving is the best admin
		
		
		/*private static void Safety()
        {
            if (ElVarusMenu.Menu.Item("ElVarus.Safety").IsActive() && spells[Spells.Q].IsCharging)
            {
                foreach (var target in
                    HeroManager.Enemies.Where(
                        enemy =>
                        enemy.IsValidTarget() 
						//&& enemy.IsMelee
						&& Player.Distance(enemy.Position) <= 550))
						{
							if (target.CountAlliesInRange(700) >= 0)
							{
								spells[Spells.Q].Cast(target);
							}
						}						
            }
        }		
		*/
		
		private static void Safety()
		{
			var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsInvulnerable & !x.IsZombie);
			if (ElVarusMenu.Menu.Item("ElVarus.Safety").IsActive() && spells[Spells.Q].IsCharging)
            {
                Obj_AI_Hero qtarget =
                    targets.Where(x => x.Distance(Player.Position) < 525 && x.CountEnemiesInRange(1100) >= 1)
                    .MinOrDefault(x => x.Distance(ObjectManager.Player));
                if (qtarget != null)
                {
					spells[Spells.Q].Cast(qtarget);
                }
            }
			else 
            {
                return;
            }			
		}
           
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (spells[Spells.R].IsReady() && ElVarusMenu.Menu.Item("GapCloser" + gapcloser.Sender.ChampionName).GetValue<bool>())
            {
                var Target = gapcloser.Sender;
                if (Target.IsValidTarget(spells[Spells.R].Range))
                {
                    spells[Spells.R].Cast(Target.ServerPosition, true);
                    //Program.debug("AGC " );
                }
            }
        }	
		
        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!ElVarusMenu.Menu.Item("InterruptSpells").GetValue<bool>())
            {
                return;
            }

            if (args.DangerLevel <= Interrupter2.DangerLevel.Medium)
            {
                return;
            }

            if (sender.IsAlly)
            {
                return;
            }

            if (spells[Spells.R].IsReady() && Player.IsValidTarget(spells[Spells.R].Range))
            {
 
                    spells[Spells.R].Cast(Player.ServerPosition, true);
                    //Program.debug("AGC " );

            }
        }

		private static void OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    break;				
			}
           	    
            LogicR();
			Safety();
			ChainCC();
            Killsteal();
			
            if (spells[Spells.R].IsReady())
            {
                if (ElVarusMenu.Menu.Item("ElVarus.SemiR").GetValue<KeyBind>().Active)
                {
                    var t = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget())
                       spells[Spells.R].Cast(t);
                }
            }
			
        }


        #endregion
    }
}
