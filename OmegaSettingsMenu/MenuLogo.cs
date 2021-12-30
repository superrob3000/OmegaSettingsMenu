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

            Location = location;
            Height = labelLogo.Size.Height;
            Width = labelLogo.Size.Width;
        }

        public Point Location = new Point();
        public int Height;
        public int Width;
        private Label labelLogo;
        OmegaSettingsForm my_parent;
    }
}
