using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins;
using System.Diagnostics;
using Unbroken.LaunchBox.Plugins.Data;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace OmegaSettingsMenu
{
    internal class SystemEvents : ISystemEventsPlugin
    {
        void ISystemEventsPlugin.OnEventRaised(string eventType)
        {
            if (eventType == SystemEventTypes.PluginInitialized)
            {
                if (PluginHelper.StateManager.IsBigBox)
                {
                    // Enable BigBoxMonitor to recover from crashes. 
                    String xml_path = System.IO.Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Data/OmegaBigBoxMonitor.xml";
                    XElement OmegaBigBoxMonitorSettings = new XElement("OmegaBigBoxMonitorSettings");
                    OmegaBigBoxMonitorSettings.Add(new XElement("Enabled", "True"));
                    XDocument xSettingsDoc = new XDocument();
                    xSettingsDoc.Add(OmegaBigBoxMonitorSettings);
                    xSettingsDoc.Save(xml_path);
                }
            }

            if (eventType == SystemEventTypes.BigBoxShutdownBeginning)
            {
                // Disable BigBoxMinitor so that any crashes during shutdown get ignored.
                String xml_path = System.IO.Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Data/OmegaBigBoxMonitor.xml";
                XElement OmegaBigBoxMonitorSettings = new XElement("OmegaBigBoxMonitorSettings");
                OmegaBigBoxMonitorSettings.Add(new XElement("Enabled", "False"));
                XDocument xSettingsDoc = new XDocument();
                xSettingsDoc.Add(OmegaBigBoxMonitorSettings);
                xSettingsDoc.Save(xml_path);
            }
        }
    }
}
