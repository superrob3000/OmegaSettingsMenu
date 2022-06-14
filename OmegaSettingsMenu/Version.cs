
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OmegaSettingsMenu
{
    class Version
    {
        static public string version;
        static public void SetVersion()
        {
            try
            {
                string xml_path = Path.GetDirectoryName(Application.ExecutablePath).ToString() + "/Data/OmegaSettings.xml";
                XDocument xSettingsDoc;
                xSettingsDoc = XDocument.Load(xml_path);

                version = xSettingsDoc
                .XPathSelectElement("/OmegaSettings")
                .Element("SupportPackageVersion")
                .Value;
            }
            catch
            {
                version = "0";
            }
        }
    }
}
