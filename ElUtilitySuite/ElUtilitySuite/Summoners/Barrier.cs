namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Utility;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Barrier : IPlugin
    {
        /// <summary>
        /// Gets or sets the barrier spell.
        /// </summary>
        /// <value>
        /// The barrier spell.
        /// </value>
        public Spell BarrierSpell { get; set; }

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        private Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            var barrierSlot = this.Player.GetSpell(SpellSlot.Summoner1).Name == "summonerbarrier"
                               ? SpellSlot.Summoner1
                               : this.Player.GetSpell(SpellSlot.Summoner2).Name == "summonerbarrier"
                                     ? SpellSlot.Summoner2
                                     : SpellSlot.Unknown;

            if (barrierSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.BarrierSpell = new Spell(barrierSlot, 550);

            AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
        }

        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!InitializeMenu.Menu.Item("Barrier.Activated").IsActive())
            {
                return;
            }

            var obj = ObjectManager.GetUnitByNetworkId<GameObject>(args.TargetNetworkId);

            if (obj.Type != GameObjectType.obj_AI_Hero)
            {
                return;
            }

            var hero = (Obj_AI_Hero)obj;

            if (hero.IsEnemy)
            {
                return;
            }

            if (
                ObjectManager.Get<Obj_AI_Hero>()
                    .Any(
                        x =>
                        x.IsMe && ((int)(args.Damage / x.MaxHealth * 100)
                            > InitializeMenu.Menu.Item("Barrier.Damage").GetValue<Slider>().Value
                            || x.HealthPercent < InitializeMenu.Menu.Item("Barrier.HP").GetValue<Slider>().Value)))
            {
                this.BarrierSpell.Cast();
            }
        }
    }
}