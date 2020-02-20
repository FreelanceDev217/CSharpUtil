using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Morlok
{
    public partial class LoginFrm : Form
    {
        Timer m_timer = new Timer();
        public LoginFrm()
        {
            InitializeComponent();
        }

        private void LoginFrm_Load(object sender, EventArgs e)
        {
            // Init UI
            lab_time.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            lab_time.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        }

        void check_user()
        {
            try
            {
                lbl_checking.Visible = true;
                txt_pass.Enabled = false;

                string pwd = txt_pass.Text;

                //if (pwd == "")
                //{
                //    ReturnOK();
                //    return;
                //}

                if( MainApp.g_aes.Encrypt(pwd) != MainApp.g_setting.admin_pwd)
                {
                    MessageBox.Show("User information incorrect.");
                    txt_pass.BackColor = MainApp.g_col_error;
                    return;
                }

                ReturnOK();
            }
            catch (Exception ex)
            {
                MainApp.g_logger.Error("Check User Failed. " + ex.Message);
            }
            finally
            {
                lbl_checking.Visible = false;
                txt_pass.Enabled = true;
            }
        }

        private void LoginFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            if (e.KeyCode == Keys.Enter)
                check_user();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void pic_logo_Click(object sender, EventArgs e)
        {

        }

        private void txt_user_Click(object sender, EventArgs e)
        {
            
        }

        private void txt_pass_TextChanged(object sender, EventArgs e)
        {

        }

        private void ReturnOK()
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
