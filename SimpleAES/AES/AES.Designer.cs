namespace AES
{
    partial class AES
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txt_normal = new System.Windows.Forms.TextBox();
            this.txt_crypt = new System.Windows.Forms.TextBox();
            this.btn_enc = new System.Windows.Forms.Button();
            this.btn_dec = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_normal
            // 
            this.txt_normal.Location = new System.Drawing.Point(26, 24);
            this.txt_normal.Margin = new System.Windows.Forms.Padding(4);
            this.txt_normal.Name = "txt_normal";
            this.txt_normal.Size = new System.Drawing.Size(499, 24);
            this.txt_normal.TabIndex = 0;
            // 
            // txt_crypt
            // 
            this.txt_crypt.Location = new System.Drawing.Point(26, 117);
            this.txt_crypt.Margin = new System.Windows.Forms.Padding(4);
            this.txt_crypt.Name = "txt_crypt";
            this.txt_crypt.Size = new System.Drawing.Size(499, 24);
            this.txt_crypt.TabIndex = 0;
            // 
            // btn_enc
            // 
            this.btn_enc.Location = new System.Drawing.Point(126, 64);
            this.btn_enc.Name = "btn_enc";
            this.btn_enc.Size = new System.Drawing.Size(129, 34);
            this.btn_enc.TabIndex = 1;
            this.btn_enc.Text = "Encrypt";
            this.btn_enc.UseVisualStyleBackColor = true;
            this.btn_enc.Click += new System.EventHandler(this.btn_enc_Click);
            // 
            // btn_dec
            // 
            this.btn_dec.Location = new System.Drawing.Point(295, 64);
            this.btn_dec.Name = "btn_dec";
            this.btn_dec.Size = new System.Drawing.Size(129, 34);
            this.btn_dec.TabIndex = 1;
            this.btn_dec.Text = "Decrypt";
            this.btn_dec.UseVisualStyleBackColor = true;
            this.btn_dec.Click += new System.EventHandler(this.btn_dec_Click);
            // 
            // AES
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 175);
            this.Controls.Add(this.btn_dec);
            this.Controls.Add(this.btn_enc);
            this.Controls.Add(this.txt_crypt);
            this.Controls.Add(this.txt_normal);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AES";
            this.Text = "AES";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_normal;
        private System.Windows.Forms.TextBox txt_crypt;
        private System.Windows.Forms.Button btn_enc;
        private System.Windows.Forms.Button btn_dec;
    }
}

