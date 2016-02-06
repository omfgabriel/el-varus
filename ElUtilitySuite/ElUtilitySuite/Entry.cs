namespace ElUtilitySuite
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Security.Permissions;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Entry
    {
        #region Delegates

        internal delegate T ObjectActivator<out T>(params object[] args);

        #endregion

        #region Public Properties

        public static Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets script version
        /// </summary>
        /// <value>
        ///     The script version
        /// </value>
        public static string ScriptVersion
        {
            get
            {
                return typeof(Entry).Assembly.GetName().Version.ToString();
            }
        }

        #endregion

        #region Public Methods and Operators

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();
            var param = Expression.Parameter(typeof(object[]), "args");
            var argsExp = new Expression[paramsInfo.Length];

            for (var i = 0; i < paramsInfo.Length; i++)
            {
                var paramCastExp = Expression.Convert(
                    Expression.ArrayIndex(param, Expression.Constant(i)),
                    paramsInfo[i].ParameterType);

                argsExp[i] = paramCastExp;
            }

            return
                (ObjectActivator<T>)
                Expression.Lambda(typeof(ObjectActivator<T>), Expression.New(ctor, argsExp), param).Compile();
        }


        public static void OnLoad(EventArgs args)
        {
            try
            {
                var plugins =
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(x => typeof(IPlugin).IsAssignableFrom(x) && !x.IsInterface)
                        .Select(x => GetActivator<IPlugin>(x.GetConstructors().First())(null));

                var menu = new Menu("ElUtilitySuite", "ElUtilitySuite", true);

                foreach (var plugin in plugins)
                {
                    plugin.CreateMenu(menu);
                    plugin.Load();
                }

                menu.AddItem(new MenuItem("seperator1", ""));
                menu.AddItem(new MenuItem("usecombo", "Combo (Active)").SetValue(new KeyBind(32, KeyBindType.Press)));
                menu.AddItem(new MenuItem("seperator", ""));
                menu.AddItem(new MenuItem("Versionnumber", string.Format("Version: {0}", ScriptVersion)));
                menu.AddItem(new MenuItem("by.jQuery", "jQuery / ChewyMoon"));
                menu.AddToMainMenu();

                Menu = menu;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}