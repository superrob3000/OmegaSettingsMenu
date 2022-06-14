using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;

namespace OmegaSettingsMenu
{
    class UpdateCheckMenuItem : NoValueTypeMenuItem
    {
        public UpdateCheckMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Check For Updates") { }

        protected override void perform_the_no_value_action()
        {
            my_parent.show_status("Checking for updates...");

            try
            {
                //Use a new thread so as not to block the UI thread
                Thread t = new Thread(() =>
                {
                    Thread.Sleep(3000);
                    String LatestVersion;
                    XDocument xUpdatesDoc;

                    try
                    {
                        //Try to load the update info file
                        String URLString = "http://omegaupdates.ddns.net/updates.xml";
                        xUpdatesDoc = XDocument.Load(URLString);

                        LatestVersion = xUpdatesDoc
                            .XPathSelectElement("/OmegaUpdates")
                            .Element("LatestVersion")
                            .Value;
                    }
                    catch
                    {
                        my_parent.show_status("Could not connect to server.");
                        Thread.Sleep(4000);
                        my_parent.hide_status();
                        return;
                    }

                    Double CurrentVersion;
                    try
                    {
                        CurrentVersion = Convert.ToDouble(Version.version);
                    }
                    catch { CurrentVersion = 0; }

                    if(CurrentVersion == 0)
                    {
                        //Could not determine the version.
                        //So just use v1.11 so that we get all updates.
                        //v1.11 was the first version where OTA updates were available.
                        CurrentVersion = 1.11;
                    }

                    if (Convert.ToDouble(LatestVersion) <= CurrentVersion)
                    {
                        my_parent.show_status("You are up to date.");
                        Thread.Sleep(4000);
                        my_parent.hide_status();
                        return;
                    }

                    //Determine the file to download. Find the mapping with the highest version
                    //number that is less than or equal to our version
                    String BestMatch = "0";
                    String NewVersion = "0";
                    String Filename = "null";
                    String SignatureFilename = "null";
                    try
                    {
                        var UpdateMappings = xUpdatesDoc.Element("OmegaUpdates").Element("UpdateMappings");
                        foreach (var Mapping in UpdateMappings.Elements())
                        {
                            String OldVersion = (String)Mapping.Element("OldVersion").Value;

                            if (Convert.ToDouble(OldVersion) <= CurrentVersion)
                            {
                                if (Convert.ToDouble(OldVersion) > Convert.ToDouble(BestMatch))
                                {
                                    BestMatch = OldVersion;
                                    Filename = (String)Mapping.Element("Filename").Value;
                                    NewVersion = (String)Mapping.Element("NewVersion").Value;
                                }
                            }
                        }
                    }
                    catch
                    {
                        my_parent.show_status("Error 25.");
                        Thread.Sleep(4000);
                        my_parent.hide_status();
                        return;
                    }

                    if (BestMatch.Equals("0"))
                    {
                        my_parent.show_status("Error 26.");
                        Thread.Sleep(4000);
                        my_parent.hide_status();
                        return;
                    }

                    SignatureFilename = Filename + ".signature";

                    my_parent.show_status("Omega Support Package v" + LatestVersion + " is available.");
                    Thread.Sleep(4000);

                    //Sometimes we need to update in steps, so the new version being
                    //installed might not be the latest version.
                    if (Convert.ToDouble(NewVersion) < Convert.ToDouble(LatestVersion))
                    {
                        my_parent.show_status("v" + NewVersion + " must be installed first.\r\nPlease check for additional updates after installation is complete.");
                        Thread.Sleep(5000);
                    }

                    //Download the installer and its signature
                    my_parent.show_status("Downloading Omega Support Package v" + NewVersion + " (0%)");

                    if (!Directory.Exists(LaunchBoxFolder + "/Updates"))
                        Directory.CreateDirectory(LaunchBoxFolder + "/Updates");

                    String InstallerPath = LaunchBoxFolder + "/Updates/" + Filename;
                    String InstallerSignaturePath = LaunchBoxFolder + "/Updates/" + SignatureFilename;

                    //Delete the files if they exist.
                    if (File.Exists(InstallerPath))
                        File.Delete(InstallerPath);

                    if (File.Exists(InstallerSignaturePath))
                        File.Delete(InstallerSignaturePath);

                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("User-Agent: Other");

                    //First grab the signature
                    try
                    {
                        webClient.DownloadFile(new Uri("http://omegaupdates.ddns.net/" + SignatureFilename),
                            InstallerSignaturePath);
                    }
                    catch
                    {
                        if (File.Exists(InstallerSignaturePath))
                            File.Delete(InstallerSignaturePath);

                        my_parent.show_status("Signature not found.");
                        Thread.Sleep(6000);
                        webClient.Dispose();
                        my_parent.hide_status();
                        return;
                    }

                    //Now download the installer
                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        my_parent.show_status("Downloading Omega Support Package v" + NewVersion + " (" + e.ProgressPercentage + "%)");
                    };
                    webClient.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Cancelled)
                        {
                            if (File.Exists(InstallerPath))
                                File.Delete(InstallerPath);

                            my_parent.show_status("File download cancelled.");
                            Thread.Sleep(6000);
                            webClient.Dispose();
                            my_parent.hide_status();
                            return;
                        }

                        if (e.Error != null)
                        {
                            if (File.Exists(InstallerPath))
                                File.Delete(InstallerPath);

                            if (File.Exists(InstallerSignaturePath))
                                File.Delete(InstallerSignaturePath);

                            my_parent.show_status(e.Error.ToString());
                            Thread.Sleep(10000);
                            webClient.Dispose();
                            my_parent.hide_status();
                            return;
                        }

                        Thread.Sleep(4000);
                        my_parent.show_status("File download completed.");
                        Thread.Sleep(3000);

                        my_parent.show_status("Authenticating the download.");
                        Thread.Sleep(3000);

                        //Authenticate using our public key (no need to hide the public key).
                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                        rsa.FromXmlString("<RSAKeyValue><Modulus>vZHO1qeP+k2FMexrTfKnDEPRHBG11qm9KkKpRb+5OGC04vwYcTgOEQGKnV0cripyh80p6t0ZLSqpEWxuDNszXeTvLbfJEF0bASMtOEFeTVSZkqM1sf80hOfbQGU6B1dqxP/szZ9JJJzWv1lRMc1u13+1TPWMWa+3BJXktw+zTGk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

                        //Hash the file
                        byte[] hashValue;
                        byte[] signedHashValue = File.ReadAllBytes(InstallerSignaturePath);
                        using (FileStream fs = new FileStream(InstallerPath, FileMode.Open))
                        using (BufferedStream bs = new BufferedStream(fs))
                        {
                            using (SHA1Managed sha1 = new SHA1Managed())
                            {
                                hashValue = sha1.ComputeHash(bs);
                            }
                        }

                        //Create an RSAPKCS1SignatureDeformatter object and pass it the
                        //RSA instance to transfer the public key.
                        RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);

                        //Set the hash algorithm to SHA1.
                        rsaDeformatter.SetHashAlgorithm("SHA1");

                        //Verify the hash using the public key.
                        if (!rsaDeformatter.VerifySignature(hashValue, signedHashValue))
                        {
                            if (File.Exists(InstallerPath))
                                File.Delete(InstallerPath);

                            if (File.Exists(InstallerSignaturePath))
                                File.Delete(InstallerSignaturePath);

                            my_parent.show_status("Authentication failed!!");
                            Thread.Sleep(10000);
                            webClient.Dispose();
                            my_parent.hide_status();
                            return;

                        }

                        //The update is from the Omega group and is unmodified. Install it.
                        my_parent.show_status("Preparing to install.");
                        Thread.Sleep(3000);

                        //Run the installer
                        Process ps_instaler = null;
                        ps_instaler = new Process();
                        ps_instaler.StartInfo.UseShellExecute = false;
                        ps_instaler.StartInfo.RedirectStandardInput = false;
                        ps_instaler.StartInfo.RedirectStandardOutput = false;
                        ps_instaler.StartInfo.CreateNoWindow = true;
                        ps_instaler.StartInfo.UserName = null;
                        ps_instaler.StartInfo.Password = null;
                        ps_instaler.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        ps_instaler.StartInfo.Arguments = "\"" + LaunchBoxFolder + "\"";
                        ps_instaler.StartInfo.FileName = InstallerPath;

                        if (File.Exists(ps_instaler.StartInfo.FileName))
                        {
                            bool result = ps_instaler.Start();
                        }
                        else
                        {
                            my_parent.show_status("Installer not found.");
                            Thread.Sleep(5000);
                        }

                        //Close the status window
                        webClient.Dispose();
                        my_parent.hide_status();
                    };
                    webClient.DownloadFileAsync(new Uri("http://omegaupdates.ddns.net/" + Filename),
                        InstallerPath);
                });
                t.Start();
            }
            catch
            {
                my_parent.hide_status();
            }
        }
    }
}
