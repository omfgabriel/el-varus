namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using Font = SharpDX.Direct3D9.Font;

    internal class HealthTracker : IPlugin
    {
        #region Fields

        /// <summary>
        ///     The HP bar height
        /// </summary>
        private readonly int BarHeight = 10;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        private static Font Font { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "Trackers");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Trackers", "Trackers"))
                           : rootMenu.Children.First(predicate);

            var enemySidebarMenu = menu.AddSubMenu(new Menu("Enemy healthbars", "healthenemies")).SetFontStyle(FontStyle.Regular, SharpDX.Color.Red);
            {
                enemySidebarMenu.AddItem(new MenuItem("DrawHealth_", "Activated").SetValue(true));
                enemySidebarMenu.AddItem(new MenuItem("OffsetTop", "Offset Top").SetValue(new Slider(75, 0, 500)));
                enemySidebarMenu.AddItem(new MenuItem("OffsetRight", "Offset Right").SetValue(new Slider(170, 0, 500)));
                enemySidebarMenu.AddItem(new MenuItem("FontSize", "Font size").SetValue(new Slider(15, 13, 30)));
            }

            this.Menu = menu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                    {
                        FaceName = "Tahoma", Height = this.Menu.Item("FontSize").GetValue<Slider>().Value,
                        OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased
                    });

            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Drawing.OnPreReset += args => { Font.OnLostDevice(); };
            Drawing.OnPostReset += args => { Font.OnResetDevice(); };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the scene is completely rendered.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!this.Menu.Item("DrawHealth_").GetValue<bool>())
            {
                return;
            }

            float i = 0;

            foreach (var hero in HeroManager.Enemies.Where(x => !x.IsDead))
            {
                var champion = hero.ChampionName;
                if (champion.Length > 12)
                {
                    champion = champion.Remove(7) + "...";
                }

                Font.DrawText(
                    null,
                    champion,
                    (int)
                    ((float)(Drawing.Width - this.Menu.Item("OffsetRight").GetValue<Slider>().Value) - 60f
                     - Font.MeasureText(null, champion, FontDrawFlags.Right).Width / 2f),
                    (int)
                    (this.Menu.Item("OffsetTop").GetValue<Slider>().Value + i + 4
                     - Font.MeasureText(null, champion, FontDrawFlags.Right).Height / 2f),
                    new ColorBGRA(255, 255, 255, 255));

                this.DrawRect(
                    Drawing.Width - this.Menu.Item("OffsetRight").GetValue<Slider>().Value,
                    this.Menu.Item("OffsetTop").GetValue<Slider>().Value + i,
                    100,
                    this.BarHeight,
                    1,
                    Color.LightBlue);

                this.DrawRect(
                    Drawing.Width - this.Menu.Item("OffsetRight").GetValue<Slider>().Value,
                    this.Menu.Item("OffsetTop").GetValue<Slider>().Value + i,
                    (int)(hero.HealthPercent),
                    this.BarHeight,
                    1,
                    hero.HealthPercent < 30 ? Color.Orange : Color.Green);

                i += 20f;
            }
        }

        /// <summary>
        ///     Draws a rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        private void DrawRect(float x, float y, int width, float height, float thickness, Color color)
        {
            for (var i = 0; i < height; i++)
            {
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
            }
        }

        #endregion
    }
}