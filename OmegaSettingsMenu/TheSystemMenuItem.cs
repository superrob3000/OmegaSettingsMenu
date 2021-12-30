using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace OmegaSettingsMenu
{
    public class TheSystemMenuItem : ISystemMenuItemPlugin
    {
        public TheSystemMenuItem() : base() 
        {
            frm = new OmegaSettingsForm(this);
        }

        string ISystemMenuItemPlugin.Caption
        {
            get { return "Omega Settings"; }
        }

        System.Drawing.Image ISystemMenuItemPlugin.IconImage { get { return null; } }

        bool ISystemMenuItemPlugin.ShowInLaunchBox { get { return true; } }

        bool ISystemMenuItemPlugin.ShowInBigBox { get { return true; } }

        bool ISystemMenuItemPlugin.AllowInBigBoxWhenLocked { get { return true; } }

        void ISystemMenuItemPlugin.OnSelected()
        {
            if (messageLoopRunning)
            {
                frm.Show();
            }
            else
            {
                messageLoopRunning = true;
                System.Windows.Forms.Application.EnableVisualStyles();
                Task.Factory.StartNew(() => { System.Windows.Forms.Application.Run(frm); });

                /***************************************************************/
                /* Half of the workaround to get topmost working consistently. *
                 * See the form's Load and Show routines for the other half.   */
                /***************************************************************/
                while (!frm.is_menu_loaded)
                { }
                frm.Show();
                /***************************************************************/
                /***************************************************************/
                /***************************************************************/
            }

        }

        internal void grab_settings_from_xml(OmegaSettingsForm theFrm)
        {
            Marquee.Height = Convert.ToDouble(theFrm.get_value_by_xmltag("MarqueeHeight"));
            
            Marquee.Width = Convert.ToDouble(theFrm.get_value_by_xmltag("MarqueeWidth"));
            
            if(theFrm.get_value_by_xmltag("MarqueeStretch") == "Fill")
                Marquee.Stretch = System.Windows.Media.Stretch.Fill;
            else
                Marquee.Stretch = System.Windows.Media.Stretch.Uniform;

            if (theFrm.get_value_by_xmltag("MarqueeVerticalAlignment") == "Center")
                Marquee.VerticalAlignment = VerticalAlignment.Center;
            else
                Marquee.VerticalAlignment = VerticalAlignment.Top;
        }


        private OmegaSettingsForm frm;
        private bool messageLoopRunning = false;
    }

    public static class Marquee
    {
        public static double Height = 0;
        public static double Width = 0;
        public static System.Windows.Media.Stretch Stretch = new System.Windows.Media.Stretch();
        public static VerticalAlignment VerticalAlignment;
    }
}
