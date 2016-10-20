namespace Elvarus
{
    using System;
    using System.Drawing;
	using Color = System.Drawing.Color;
    using LeagueSharp;
    using LeagueSharp.Common;
	using System.Collections.Generic;
    using System.Linq;

    internal class Drawings
    {
        #region Public Methods and Operators
        
		private static int GetWStacks(Obj_AI_Base target)
        {
            return target.GetBuffCount("varuswdebuff");
        }

        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }		

        public static void Drawing_OnDraw(EventArgs args)
        {
			
			
            var drawOff = ElVarusMenu.Menu.Item("ElVarus.Draw.off").GetValue<bool>();
            var drawQ = ElVarusMenu.Menu.Item("ElVarus.Draw.Q").GetValue<Circle>();
            var drawW = ElVarusMenu.Menu.Item("ElVarus.Draw.W").GetValue<Circle>();
            var drawE = ElVarusMenu.Menu.Item("ElVarus.Draw.E").GetValue<Circle>();
            var drawR = ElVarusMenu.Menu.Item("ElVarus.Draw.E").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }
			

            if (drawQ.Active)
            {
                if (Varus.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.Q].Range,
                        Varus.spells[Spells.Q].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawW.Active)
			{
					foreach (var enemy in
                        HeroManager.Enemies.Where(
                            e => 
							//e.Distance(Player.Position) < 1500
							e.IsHPBarRendered && e.Position.IsOnScreen() && e.IsValidTarget()))
                    {
                        var stacks = GetWStacks(enemy) - 1;
                        if (stacks > -1)
                        {
                            var x = enemy.HPBarPosition.X + 45;
                            var y = enemy.HPBarPosition.Y - 25;
                            for (var i = 0; 3 > i; i++)
                            {
                                Drawing.DrawLine(
                                    x + i * 20, y, x + i * 20 + 10, y, 10, i > stacks ? Color.DarkGray : Color.Orange);
                            }
                        }
                    }
			}

            if (drawE.Active)
            {
                if (Varus.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.E].Range,
                        Varus.spells[Spells.E].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawR.Active)
            {
                if (Varus.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.R].Range,
                        Varus.spells[Spells.R].IsReady() ? Color.Green : Color.Red);
                }
            }
        }

        #endregion
    }
}