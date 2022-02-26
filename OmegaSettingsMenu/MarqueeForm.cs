using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using VerticalAlignment = System.Windows.VerticalAlignment;
using System.Xml.Linq;
using System.Xml.XPath;
using Application = System.Windows.Forms.Application;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace OmegaSettingsMenu
{
    public partial class MarqueeForm : Form
    {
        private string MarqueeMonitorIndex;
        double ScalingFactorX;
        double ScalingFactorY;
        Screen marquee_screen;
        Size ImageSize;
        bool marquee_enabled = false;
        private TheSystemMenuItem my_parent;

        double MarqueeHeight;
        double MarqueeWidth;
        System.Windows.Media.Stretch MarqueeStretch;
        VerticalAlignment MarqueeVerticalAlignment;

        public MarqueeForm(TheSystemMenuItem parent)
        {
            InitializeComponent();
            this.my_parent = parent;

            ImageSize = new Size(pictureBox1.Size.Width, pictureBox1.Size.Height);
            
            set_screen_number();

            //Set default invalid values (these will be updated before the form is shown)
            MarqueeHeight = 1;
            MarqueeWidth = 1;
            MarqueeStretch = System.Windows.Media.Stretch.None;
            MarqueeVerticalAlignment = VerticalAlignment.Bottom;
        }
        public void set_screen_number()
        {
            uint dpiX, dpiY;

            try
            {
                //Get BigBox settings from XML file
                string xml_path = Path.GetDirectoryName(Application.ExecutablePath).ToString() + "/Data/BigBoxSettings.xml";
                XDocument xSettingsDoc;
                xSettingsDoc = XDocument.Load(xml_path);

                MarqueeMonitorIndex = xSettingsDoc
                .XPathSelectElement("/LaunchBox/BigBoxSettings")
                .Element("MarqueeMonitorIndex")
                .Value;

                if ((Convert.ToInt32(MarqueeMonitorIndex) < 0) || (Convert.ToInt32(MarqueeMonitorIndex) > Screen.AllScreens.GetUpperBound(0)))
                {
                    marquee_enabled = false;
                }
                else
                {
                    marquee_screen = Screen.AllScreens[Convert.ToInt32(MarqueeMonitorIndex)];

                    ScreenExtensions.GetDpi(marquee_screen, DpiType.Effective, out dpiX, out dpiY);

                    ScalingFactorX = (double)dpiX / (double)96;
                    ScalingFactorY = (double)dpiY / (double)96;

                    marquee_enabled = true;
                }
            }
            catch
            {
                marquee_enabled = false;
            }

            if (!marquee_enabled)
                this.Hide();
        }

        public void update_marquee_width(string width)
        {
            update_marquee(
                Convert.ToDouble(width),
                MarqueeHeight,
                MarqueeStretch,
                MarqueeVerticalAlignment
                );
        }
        public void update_marquee_height(string height)
        {
            update_marquee(
                MarqueeWidth,
                Convert.ToDouble(height),
                MarqueeStretch,
                MarqueeVerticalAlignment
                );
        }

        public void update_marquee_stretch(string stretch)
        {
            update_marquee(
                MarqueeWidth,
                MarqueeHeight,
                stretch.Equals("Fill") ? System.Windows.Media.Stretch.Fill : System.Windows.Media.Stretch.Uniform,
                MarqueeVerticalAlignment
                );
        }
        public void update_marquee_vertical_alignment(string verticalAlignment)
        {
            update_marquee(
                MarqueeWidth,
                MarqueeHeight,
                MarqueeStretch,
                verticalAlignment.Equals("Center") ? VerticalAlignment.Center : VerticalAlignment.Top
                );
        }

        public void update_marquee(string width, string height, string stretch, string verticalAlignment)
        {
            update_marquee(
                Convert.ToDouble(width),
                Convert.ToDouble(height),
                stretch.Equals("Fill") ? System.Windows.Media.Stretch.Fill : System.Windows.Media.Stretch.Uniform,
                verticalAlignment.Equals("Center") ? VerticalAlignment.Center : VerticalAlignment.Top
                );
        }

        public void update_marquee(double width, double height, System.Windows.Media.Stretch stretch, VerticalAlignment verticalAlignment)
        {
            if(!marquee_enabled)
                return;

            if ((width != MarqueeWidth) || (height != MarqueeHeight) || (verticalAlignment != MarqueeVerticalAlignment))
            {
                ///Set size and location of this form
                if (width != MarqueeWidth)
                    this.Width = (int)(width * ScalingFactorX);

                if (height != MarqueeHeight)
                    this.Height = (int)(height * ScalingFactorY);

                //Center the form Horizontally, Align the Form vertically as per MarqueeVerticalAlignment
                this.Location = new Point(
                marquee_screen.Bounds.Location.X + ((marquee_screen.Bounds.Size.Width - this.Width) / 2),
                marquee_screen.Bounds.Location.Y + (verticalAlignment.Equals(VerticalAlignment.Center) ? ((marquee_screen.Bounds.Size.Height - this.Height) / 2) : 0));
            }

            if ((stretch != MarqueeStretch) || (width != MarqueeWidth) || (height != MarqueeHeight))
            {
                if (stretch == System.Windows.Media.Stretch.Fill)
                {
                    pictureBox1.Size = new Size(this.Width, this.Height);
                    pictureBox1.Location = new Point(0, 0);
                }
                else
                {
                    //Preserve aspect ratio...

                    // Figure out the ratio
                    double ratioX = (double)this.Width / (double)ImageSize.Width;
                    double ratioY = (double)this.Height / (double)ImageSize.Height;
                    // use whichever multiplier is smaller
                    double ratio = ratioX < ratioY ? ratioX : ratioY;

                    // now we can get the new height and width
                    int newHeight = Convert.ToInt32(ImageSize.Height * ratio);
                    int newWidth = Convert.ToInt32(ImageSize.Width * ratio);
                    pictureBox1.Size = new Size(newWidth, newHeight);

                    // Now calculate the X,Y position of the upper-left corner 
                    // (one of these will always be zero)
                    int posX = Convert.ToInt32((this.Width - (ImageSize.Width * ratio)) / 2);
                    int posY = Convert.ToInt32((this.Height - (ImageSize.Height * ratio)) / 2);
                    pictureBox1.Location = new Point(posX, posY);
                }
            }

            //Save off current values
            MarqueeHeight = height;
            MarqueeWidth = width;
            MarqueeStretch = stretch;
            MarqueeVerticalAlignment = verticalAlignment;
        }
        private void MarqueeForm_Shown(object sender, EventArgs e)
        {
            if (!marquee_enabled)
                this.Hide();
        }

        private void MarqueeForm_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                if (!marquee_enabled)
                    this.Hide();
            }
        }
    }


    public static class ScreenExtensions
    {
        public static void GetDpi(this System.Windows.Forms.Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }

}
