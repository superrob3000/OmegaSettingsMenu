using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmegaSettingsMenu
{
    class MenuBorder
    {
        public MenuBorder(OmegaSettingsForm parent, Point location, bool focus)
        {
            this.my_parent = parent;
            this.length = my_parent.get_panel().Size.Width - 100;
            this.location = new Point(1, location.Y);
            this.set_focus(focus);
        }

        public void set_focus(bool on)
        {
            if (on)
            {
                this.borderpen = new Pen(System.Drawing.Color.CornflowerBlue, 2);
            }
            else
            {
                this.borderpen = new Pen(System.Drawing.Color.Gray, 2);
            }
        }

        public int length;
        public Point location;
        public Pen borderpen;
        private OmegaSettingsForm my_parent;
    }
}
