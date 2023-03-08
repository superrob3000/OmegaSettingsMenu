using System;
using System.Threading.Tasks;
using System.Windows;
using Unbroken.LaunchBox.Plugins;

namespace OmegaSettingsMenu
{
    public class TheSystemMenuItem : ISystemMenuItemPlugin
    {
        public TheSystemMenuItem() : base() 
        {
            //marquee_frm = new MarqueeForm(this);
            frm = new OmegaSettingsForm(this /* ,marquee_frm */);
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
            //Always update marquee screen number in case it has changed in bigbox settings 
            //marquee_frm.set_screen_number();
            //marquee_frm.update_marquee(Marquee.Width, Marquee.Height, Marquee.Stretch, Marquee.VerticalAlignment);

            if (messageLoopRunning)
            {
                frm.Show();
                //marquee_frm.Show();
            }
            else
            {
                messageLoopRunning = true;
                System.Windows.Forms.Application.EnableVisualStyles();
                Task.Factory.StartNew(() => { System.Windows.Forms.Application.Run(frm); });
                //Task.Factory.StartNew(() => { System.Windows.Forms.Application.Run(marquee_frm); });

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
            try
            {
                Marquee.Height = Convert.ToDouble(theFrm.get_value_by_xmltag("MarqueeHeight"));

                Marquee.Width = Convert.ToDouble(theFrm.get_value_by_xmltag("MarqueeWidth"));

                if (theFrm.get_value_by_xmltag("MarqueeStretch") == "Fill")
                    Marquee.Stretch = System.Windows.Media.Stretch.Fill;
                else
                    Marquee.Stretch = System.Windows.Media.Stretch.Uniform;

                if (theFrm.get_value_by_xmltag("MarqueeVerticalAlignment") == "Center")
                    Marquee.VerticalAlignment = VerticalAlignment.Center;
                else
                    Marquee.VerticalAlignment = VerticalAlignment.Top;
            }
            catch{ }
        }


        private OmegaSettingsForm frm;
        //private MarqueeForm marquee_frm;
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


