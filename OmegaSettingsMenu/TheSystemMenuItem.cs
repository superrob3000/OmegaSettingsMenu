using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using Application = System.Windows.Forms.Application;

namespace OmegaSettingsMenu
{
    public class TheSystemMenuItem : ISystemMenuItemPlugin
    {
        public TheSystemMenuItem() : base() 
        {
            marquee_frm = new MarqueeForm(this);
            frm = new OmegaSettingsForm(this, marquee_frm);
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
            marquee_frm.set_screen_number();
            marquee_frm.update_marquee(Marquee.Width, Marquee.Height, Marquee.Stretch, Marquee.VerticalAlignment);

            if (messageLoopRunning)
            {
                frm.Show();
                marquee_frm.Show();
            }
            else
            {
                messageLoopRunning = true;
                System.Windows.Forms.Application.EnableVisualStyles();
                Task.Factory.StartNew(() => { System.Windows.Forms.Application.Run(frm); });
                Task.Factory.StartNew(() => { System.Windows.Forms.Application.Run(marquee_frm); });

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
        private MarqueeForm marquee_frm;
        private bool messageLoopRunning = false;
    }

    public static class Marquee
    {
        public static double Height = 0;
        public static double Width = 0;
        public static System.Windows.Media.Stretch Stretch = new System.Windows.Media.Stretch();
        public static VerticalAlignment VerticalAlignment;
    }

    //Helper class to get video file marquee paths from xaml
    public class VideoMarqueePathConverter : System.Windows.Data.IMultiValueConverter
    {
        public string Type { get; set; } = string.Empty;
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string filename = string.Empty;
            string[] extensions = new string[] {".mov",".mp4",".m4p",".m4v",".wmv",".avi",".mpg",".mpeg",".flv"};

            string platform = (values[0] == null) ? string.Empty : values[0].ToString();
            string game = ((Type != "Game")||(values[1] == null)) ? string.Empty : values[1].ToString();

            if (platform != string.Empty)
            {
                if (game == string.Empty)
                {
                    IPlatform p = PluginHelper.DataManager.GetPlatformByName(platform);
                    if (p != null)
                    {
                        /* Platform video marquee */
                        filename = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Videos/Platforms/Marquee/" + platform;
                    }
                    else
                    {
                        /* Couldn't find the platform. Assume it's a playlist video marquee. */
                        filename = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Videos/Playlists/Marquee/" + platform;
                    }
                }
                else
                {
                    /* Game video marquee */
                    filename = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Videos/" + platform + "/Marquee/" + game;
                }

                for (int index = 0; index < extensions.Length; index++)
                {
                    if (File.Exists(filename + extensions[index]))
                        return new Uri(filename + extensions[index]);
                }
            }
            return new Uri("about:blank");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


