// To be used for demo project
// David Piao

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PCKLIB
{
    public class LicenseAlert
    {
        public static Timer timer = new Timer();

        public LicenseAlert(int m)
        {
            timer.Interval = m * 60 * 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("This program is still in the phase of development. Thanks. pck2016217@gmail.com");
        }
    }
}
