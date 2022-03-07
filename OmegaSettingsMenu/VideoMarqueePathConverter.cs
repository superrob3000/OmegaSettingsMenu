using System;
using System.IO;
using System.Windows.Forms;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace OmegaSettingsMenu
{
    //Helper class to get video file marquee paths from xaml
    public class VideoMarqueePathConverter : System.Windows.Data.IMultiValueConverter
    {
        public string Type { get; set; } = string.Empty;
        public bool PathIsString { get; set; } = false;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string filename = string.Empty;
            string[] extensions = new string[] { ".mov", ".mp4", ".m4p", ".m4v", ".wmv", ".avi", ".mpg", ".mpeg", ".flv" };

            string platform = (values[0] == null) ? string.Empty : values[0].ToString();
            string game = ((Type != "Game") || (values[1] == null)) ? string.Empty : values[1].ToString();

            if (platform != string.Empty)
            {
                if (game == string.Empty)
                {
                    IPlatform p = PluginHelper.DataManager.GetPlatformByName(platform);
                    if (p != null)
                    {
                        /* Platform video marquee */
                        filename = System.IO.Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Videos/Platforms/Marquee/" + platform;
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

                    /* Removed support for custom game video marquee paths because LaunchBox * 
                     * now supports video marquee paths for games natively.                  */
                    if (PathIsString)
                        return "about:blank";
                    else
                        return new Uri("about:blank");
                }

                for (int index = 0; index < extensions.Length; index++)
                {
                    if (File.Exists(filename + extensions[index]))
                        if (PathIsString)
                            return filename + extensions[index];
                        else
                            return new Uri(filename + extensions[index]);
                }
            }

            if (PathIsString)
                return "about:blank";
            else
                return new Uri("about:blank");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
