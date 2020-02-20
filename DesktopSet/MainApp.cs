using PCK_LIB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Morlok
{
    static class MainApp
    {
        public static UserSetting g_setting;
        public static SimpleAES g_aes = new SimpleAES();
        public static string    g_user_id = "0";
        public static int   m_timer_interval = 1000;
        public static Color g_col_error = Color.FromArgb(100, Color.DarkRed);
        public static Color g_col_blank = Color.FromArgb(255, Color.White);
        public static Color g_col_working = Color.FromArgb(255, Color.Tomato);
        public static Color g_col_finished = Color.FromArgb(255, Color.Green);
        public static log4net.ILog g_logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static MySQLWrapper g_mysql = new MySQLWrapper();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainFrm());
        }
    }
}
