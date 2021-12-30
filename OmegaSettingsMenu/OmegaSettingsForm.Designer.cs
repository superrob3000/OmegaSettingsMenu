
namespace OmegaSettingsMenu
{
    partial class OmegaSettingsForm
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
            this.components = new System.ComponentModel.Container();
            this.panelWait = new System.Windows.Forms.Panel();
            this.labelWait = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timerExit = new System.Windows.Forms.Timer(this.components);
            this.panelWait.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelWait
            // 
            this.panelWait.BackColor = System.Drawing.Color.Black;
            this.panelWait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelWait.Controls.Add(this.labelWait);
            this.panelWait.Controls.Add(this.pictureBox1);
            this.panelWait.ForeColor = System.Drawing.Color.White;
            this.panelWait.Location = new System.Drawing.Point(155, 303);
            this.panelWait.Name = "panelWait";
            this.panelWait.Size = new System.Drawing.Size(600, 200);
            this.panelWait.TabIndex = 0;
            // 
            // labelWait
            // 
            this.labelWait.AutoSize = true;
            this.labelWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWait.ForeColor = System.Drawing.Color.White;
            this.labelWait.Location = new System.Drawing.Point(210, 54);
            this.labelWait.Name = "labelWait";
            this.labelWait.Size = new System.Drawing.Size(214, 37);
            this.labelWait.TabIndex = 0;
            this.labelWait.Text = "Please Wait...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::OmegaSettingsMenu.Properties.Resources.Omega;
            this.pictureBox1.Location = new System.Drawing.Point(246, 77);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(110, 110);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // timerExit
            // 
            this.timerExit.Interval = 2000;
            this.timerExit.Tick += new System.EventHandler(this.timerExit_Tick);
            // 
            // OmegaSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.LawnGreen;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.ControlBox = false;
            this.Controls.Add(this.panelWait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OmegaSettingsForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Omega Settings";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.LawnGreen;
            this.Load += new System.EventHandler(this.MenuForm_Load);
            this.panelWait.ResumeLayout(false);
            this.panelWait.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelWait;
        private System.Windows.Forms.Label labelWait;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timerExit;
    }
}

