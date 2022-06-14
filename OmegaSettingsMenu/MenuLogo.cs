using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmegaSettingsMenu
{
    class MenuLogo
    {
        public MenuLogo(OmegaSettingsForm parent, Point location)
        {
            my_parent = parent;

            labelVersion = new Label();
            my_parent.get_panel().Controls.Add(this.labelVersion);

            labelLogo = new Label();
            my_parent.get_panel().Controls.Add(this.labelLogo);
            this.labelLogo.AutoSize = true;
            this.labelLogo.BackColor = System.Drawing.Color.Black;
            this.labelLogo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelLogo.Enabled = true;
            this.labelLogo.Font = new System.Drawing.Font("Microsoft Sans Serif", 128F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLogo.ForeColor = System.Drawing.Color.White;
            this.labelLogo.Location = location;
            this.labelLogo.Name = "labelLogo";
            this.labelLogo.Size = new System.Drawing.Size(240, 37);
            this.labelLogo.TabIndex = 0;
            this.labelLogo.TabStop = false;
            this.labelLogo.TextAlign = ContentAlignment.MiddleLeft;
            this.labelLogo.Text = "Ω";

            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Black;
            this.labelVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelVersion.Enabled = true;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.ForeColor = System.Drawing.Color.White;
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(240, 1);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.TabStop = false;
            this.labelVersion.TextAlign = ContentAlignment.TopCenter;
            this.labelVersion.Text = "Omega Support Package v" + Version.version;
            this.labelVersion.Location = new Point(location.X + this.labelLogo.Size.Width/2 - this.labelVersion.Size.Width / 2, location.Y + this.labelLogo.Size.Height - this.labelVersion.Size.Height);

            Location = location;
            Width = Math.Max(labelLogo.Size.Width, labelVersion.Size.Width); 
            Height = labelLogo.Size.Height + labelVersion.Size.Height;
        }

        public Point Location = new Point();
        public int Height;
        public int Width;
        private Label labelLogo;
        private Label labelVersion;
        OmegaSettingsForm my_parent;
    }
}
