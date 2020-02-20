using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCKLIB;

namespace AES
{
    public partial class AES : Form
    {
        SimpleAES aes = new SimpleAES("GPOSD");
        public AES()
        {
            InitializeComponent();
            txt_normal.Text = ThumbPrint.Value();
        }

        private void btn_enc_Click(object sender, EventArgs e)
        {
            try {
                txt_crypt.Text = aes.Encrypt(txt_normal.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_dec_Click(object sender, EventArgs e)
        {
            try
            {
                txt_normal.Text = aes.Decrypt(txt_crypt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
