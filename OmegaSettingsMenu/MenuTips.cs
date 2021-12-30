using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmegaSettingsMenu
{
    class MenuTips
    {
        public MenuTips(OmegaSettingsForm parent, Point location, Size size)
        {
            my_parent = parent;
            Location = location;
            Width = size.Width;
            Height = size.Height;

            labelTips = new Label();
            my_parent.get_panel().Controls.Add(this.labelTips);
            this.labelTips.BackColor = System.Drawing.Color.Black;
            this.labelTips.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelTips.Enabled = true;
            this.labelTips.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTips.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.labelTips.Name = "label1";
            this.labelTips.Size = new System.Drawing.Size(Width, Height);
            this.labelTips.TabIndex = 0;
            this.labelTips.TabStop = false;
            this.labelTips.AutoSize = true;
            this.labelTips.Margin = new Padding(0);
            this.labelTips.TextAlign = ContentAlignment.TopLeft;
            this.labelTips.MaximumSize = new Size(2*(size.Width / 3), size.Height - 2*VerticalPadding);
            this.labelTips.MinimumSize = new Size(labelTips.MaximumSize.Width, 40);
            this.labelTips.Location = new Point(Location.X + Width / 3, Location.Y + Height - labelTips.Size.Height - VerticalPadding);
            this.labelTips.Visible = Visible = false;

            this.pen = new Pen(System.Drawing.Color.CornflowerBlue, 2);

            set_tips("");
        }

        public bool Visible;
        public void set_tips(String tips)
        {
            if (labelTips.Text != tips)
            {
                labelTips.Text = tips;

                if (String.IsNullOrEmpty(tips))
                    labelTips.Visible = Visible = false;
                else
                {
                    labelTips.Location = new Point(Location.X + Width / 3, Location.Y + Height - labelTips.Size.Height - VerticalPadding);

                    LineList.Clear();


                    LineList.Add(new TipLine(new Point(labelTips.Location.X - 1, labelTips.Location.Y - 1),
                                             new Point(labelTips.Location.X - 1, labelTips.Location.Y + labelTips.Size.Height + VerticalPadding - 1)));

                    LineList.Add(new TipLine(new Point(labelTips.Location.X + labelTips.Size.Width, labelTips.Location.Y - 1),
                                             new Point(labelTips.Location.X, labelTips.Location.Y - 1)));




                    labelTips.Visible = Visible = true;
                }

                my_parent.Refresh();
            }
        }

        private Label labelTips;
        public Point Location;
        public int Width;
        public int Height;
        public Pen pen;
        private OmegaSettingsForm my_parent;

        private int VerticalPadding = 20;

        internal class TipLine
        {
            public TipLine(Point p1, Point p2)
            {
                P1 = p1;
                P2 = p2;
            }

            public Point P1;
            public Point P2;
        }
        public List<TipLine> LineList = new List<TipLine>();

    }
}
