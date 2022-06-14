using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Application = System.Windows.Forms.Application;

namespace OmegaSettingsMenu
{
    public class OmegaMediaElement : MediaElement
    {
        public OmegaMediaElement() : base() { }

        public static DependencyProperty VideoPathProperty = DependencyProperty.Register("VideoPath", typeof(string),
                typeof(OmegaMediaElement),
                new PropertyMetadata(null, VideoPathChanged));
        public string VideoPath { get => (string)GetValue(VideoPathProperty); set => SetValue(VideoPathProperty, value); }

        private static void VideoPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = (OmegaMediaElement)d;
            if (!string.IsNullOrEmpty((string)e.NewValue)
                && !((string)e.NewValue).Equals("about:blank")
                && File.Exists((string)e.NewValue))
            {
                String LaunchBoxFolder = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString();
                String TempFolder = Path.Combine(LaunchBoxFolder, "temp");
                String NewSource = Path.Combine(TempFolder, "OmegaMarqeeLoopHelper.asx");
                if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }

                StreamWriter sw = File.CreateText(NewSource);
                sw.WriteLine(
                    "<ASX VERSION=\"3.0\"><REPEAT><ENTRY> <REF HREF=\""
                    + (string)e.NewValue
                    + "\"/> </ENTRY></REPEAT></ASX>");
                sw.Close();

                p.Source = new Uri(NewSource);
            }
        }
    }
}