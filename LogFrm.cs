using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCKLIB
{
    public partial class LogFrm : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public LogFrm()
        {
            InitializeComponent();
            refresh_log();

            timer.Interval = 2000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            refresh_log();
        }

        void refresh_log()
        {
            try {
                lis_log.BeginUpdate();
                lis_log.Items.Clear();
                // log
                string[] hist = File.ReadAllLines("Log_shown.txt");
                List<string> lis = hist.Skip(hist.Length - 100).ToList();
                lis.Reverse();
                lis_log.Items.AddRange(lis.ToArray());
                lis_log.EndUpdate();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
