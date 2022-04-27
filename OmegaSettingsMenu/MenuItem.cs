using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Media;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Diagnostics;
using System.Net;

namespace OmegaSettingsMenu
{
    class MenuItem
    {
        public MenuItem(OmegaSettingsForm parent, Point location)
        {
            MenuItemSetup(parent, location, "");
        }

        public MenuItem(OmegaSettingsForm parent, Point location, String name)
        {
            MenuItemSetup(parent, location, name);
        }
        private void MenuItemSetup(OmegaSettingsForm parent, Point location, String name)
        {
            my_parent = parent;

            LaunchBoxFolder = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString();

            labelItem = new Label();
            my_parent.get_panel().Controls.Add(this.labelItem);
            this.labelItem.BackColor = System.Drawing.Color.Black;
            this.labelItem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelItem.Enabled = true;
            this.labelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelItem.ForeColor = System.Drawing.Color.White;
            this.labelItem.Location = location;
            this.labelItem.Name = "label1";
            this.labelItem.Size = new System.Drawing.Size(240, 80);
            this.labelItem.TabIndex = 0;
            this.labelItem.TabStop = false;
            this.labelItem.AutoSize = true;
            this.labelItem.TextAlign = ContentAlignment.MiddleLeft;

            this.add_values();
            this.set_name(name);

            //Strip whitespace to create the XML tag.
            this.XMLTag = Regex.Replace(this.Name, @"\s+", "");
        }

        public void set_focus(bool on)
        {
            if (on)
            {
                this.labelItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            }
            else
            {
                this.labelItem.ForeColor = System.Drawing.Color.White;
            }
        }
        
        public String get_name() {return Name;}


        public void set_value_by_index(int index)
        {
            if(index < ValueList.Count)
            {
                ValueIndex = index;
                Value = ValueList.ElementAtOrDefault(ValueIndex);
                this.update_display();
            }
        }


        public String get_value() {return Value;}

        



        public void set_location(Point location)
        {
            this.labelItem.Location = new Point(location.X, location.Y);
        }

        protected string Name;
        protected string Value = "";
        protected string XMLTag = "";
        internal Label labelItem;

        protected OmegaSettingsForm my_parent;
        protected String LaunchBoxFolder;

        public List<String> ValueList = new List<String>();
        protected int ValueIndex = 0;

        public void set_name(string name)
        {
            Name = name;
            this.update_display();
        }


        //Override these:
        public virtual void on_right()
        {
        }

        public virtual void on_left()
        {
        }

        public virtual void on_enter()
        {
            if(ValueList.Count > 0)
            {
                set_value_by_index((ValueIndex + 1) % ValueList.Count);

                my_parent.SelectPlayer.controls.stop();
                my_parent.SelectPlayer.controls.play();
            }
        }

        public virtual void add_values()
        {

        }

        public virtual void apply_setting()
        {

        }

        public virtual String get_default_value()
        {
            if (ValueList.Count > 0)
                return ValueList[0];
            else
                return "";
        }

        public virtual String get_tips()
        {
            return "";
        }

        public virtual String get_xmltag() { return XMLTag; }
        protected virtual void update_display() { this.labelItem.Text = " " + Name + ": " + Value; }

        public virtual void set_value(string value)
        {
            ValueIndex = Math.Max(0, ValueList.IndexOf(value));
            Value = ValueList.ElementAtOrDefault(ValueIndex);

            if (String.IsNullOrEmpty(Value))
            {
                Value = get_default_value();
            }
            this.update_display();
        }

    }
}




namespace OmegaSettingsMenu
{
    class CounterTypeMenuItem : MenuItem
    {
        public CounterTypeMenuItem(OmegaSettingsForm parent, Point location, String name) : base(parent, location, name) { }

        public override string get_default_value() { return "0"; }

        public override void on_right()
        {
            my_parent.SelectPlayer.controls.stop();
            my_parent.SelectPlayer.controls.play();

            int val;
            Int32.TryParse(Value, out val);

            set_value((val + 1).ToString());
        }
        public override void on_left()
        {
            my_parent.SelectPlayer.controls.stop();
            my_parent.SelectPlayer.controls.play();

            int val;
            Int32.TryParse(Value, out val);

            if (val > 0)
                set_value((val - 1).ToString());
        }

        public override void set_value(string value)
        {
            Value = value;
            if (String.IsNullOrEmpty(Value))
            {
                Value = get_default_value();
            }
            this.update_display();
        }
    }


    class EnabledDisabledTypeMenuItem : MenuItem
    {
        public EnabledDisabledTypeMenuItem(OmegaSettingsForm parent, Point location, String name) : base(parent, location, name) { }

        public override void add_values()
        {
            ValueList.Add("Enabled");
            ValueList.Add("Disabled");
        }
    }

    class EnabledDisabledManualTypeMenuItem : MenuItem
    {
        public EnabledDisabledManualTypeMenuItem(OmegaSettingsForm parent, Point location, String name) : base(parent, location, name) { }

        public override void add_values()
        {
            ValueList.Add("Enabled");
            ValueList.Add("Disabled");
            ValueList.Add("Manual");
        }
    }

    class HideWheelTypeMenuItem : MenuItem
    {
        public HideWheelTypeMenuItem(OmegaSettingsForm parent, Point location, String name) : base(parent, location, name) { }

        public override void add_values()
        {
            ValueList.Add("Show Playlist");
            ValueList.Add("Hide Playlist Only");
            ValueList.Add("Hide Playlist and Games");
        }

        public override void on_enter()
        {
            if (PluginHelper.StateManager.IsBigBox)
            {
                my_parent.restart_required = true;
            }

            base.on_enter();

            my_parent.Tips.set_tips(this.get_tips());
        }

        public override String get_tips()
        {
            if (this.get_value().Equals("Show Playlist"))
            {
                return ("Games will be accessible from both this playlist and their platform wheels.");
            }
            else if (this.get_value().Equals("Hide Playlist Only"))
            {
                return ("Playlist will be hidden, but games will still be accessible from their platform wheels.");
            }
            else if (this.get_value().Equals("Hide Playlist and Games"))
            {
                return ("Playlist will be hidden, and games will also be hidden on their platform wheels.");
            }
            else
                return "";
        }

    }

        class NoValueTypeMenuItem : MenuItem
    {
        public NoValueTypeMenuItem(OmegaSettingsForm parent, Point location, String name) : base(parent, location, name) { }

        public override String get_xmltag() { return ""; } //Don't query XML file for a value
        protected override void update_display() { this.labelItem.Text = " " + Name; }

        public override void on_enter()
        {
            my_parent.SelectPlayer.controls.stop();
            my_parent.SelectPlayer.controls.play();

            perform_the_no_value_action();
        }

        protected virtual void perform_the_no_value_action() { }
    }






    /* ------------------------------------------------------------------------ */
    /* ------------------------------------------------------------------------ */
    /* ------------------------------------------------------------------------ */
    /* ------------------------------------------------------------------------ */
    /* ------------------------------------------------------------------------ */




    class MarqueeWidthMenuItem : CounterTypeMenuItem
    {
        public MarqueeWidthMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Marquee Width"){ }

        public override void on_left()
        {
            base.on_left();
            my_parent.marquee_frm.update_marquee_width(get_value());
        }
        public override void on_right()
        {
            base.on_right();
            my_parent.marquee_frm.update_marquee_width(get_value());
        }

        public override string get_default_value() { return "1920"; }
    }


    class MarqueeHeightMenuItem : CounterTypeMenuItem
    {
        public MarqueeHeightMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Marquee Height") { }

        public override void on_left()
        {
            base.on_left();
            my_parent.marquee_frm.update_marquee_height(get_value());
        }
        public override void on_right()
        {
            base.on_right();
            my_parent.marquee_frm.update_marquee_height(get_value());
        }

        public override string get_default_value() { return "340"; }
    }

    class MarqueeStretchMenuItem : MenuItem
    {
        public MarqueeStretchMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Marquee Stretch") { }

        public override void add_values()
        {
            ValueList.Add("Fill");
            ValueList.Add("Preserve Aspect Ratio (Horton Style)");
        }

        public override void on_enter()
        {
            base.on_enter();
            my_parent.marquee_frm.update_marquee_stretch(get_value());
        }
     }

    class MarqueeVerticalAlignmentMenuItem : MenuItem
    {
        public MarqueeVerticalAlignmentMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Marquee Vertical Alignment") { }

        public override void add_values()
        {
            ValueList.Add("Top");
            ValueList.Add("Center");
        }

        public override void on_enter()
        {
            base.on_enter();
            my_parent.marquee_frm.update_marquee_vertical_alignment(get_value());
        }
    }


    class ScanlinesMenuItem : EnabledDisabledManualTypeMenuItem
    {
        public ScanlinesMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Scanlines") { }

        public override void apply_setting()
        {
            if (this.get_value().Equals("Manual"))
            {
                foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
                {
                    if (emulator.Title.StartsWith("MAME"))
                    {
                        //Get command line with video override removed
                        String cmdline = emulator.CommandLine.Replace(" -video opengl", "");

                        emulator.CommandLine = cmdline;
                    }

                }
            }
            else foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
            {
                if (emulator.Title.StartsWith("MAME"))
                {
                    //Get command line with video override removed
                    String cmdline = emulator.CommandLine.Replace(" -video opengl", "");

                    if (this.get_value().Equals("Disabled"))
                    {
                        //Override the video option to disable scanlines
                        cmdline = cmdline + " -video opengl";
                    }

                    emulator.CommandLine = cmdline;
                }

                else if (emulator.Title.StartsWith("Retroarch"))
                {
                    String filename = Path.GetDirectoryName(emulator.ApplicationPath) + "/Retroarch.cfg";

                    if(File.Exists(filename))
                    {
                        string text;
                        try 
                        { 
                            text = File.ReadAllText(filename);

                            if (this.get_value().Equals("Disabled"))
                                text = text.Replace("video_shader_enable = \"true\"", "video_shader_enable = \"false\"");
                            else
                                text = text.Replace("video_shader_enable = \"false\"", "video_shader_enable = \"true\"");
                            
                            File.WriteAllText(filename, text);
                        }
                        catch 
                        { 
                        }
                        
                    }
                }
            }
        }

        public override String get_tips()
        {
            return "The Enabled/Disabled settings control scanlines globally for MAME and all Retroarch cores. Manual is for advanced users who want to alter scanline settings themselves in the settings files for MAME and the individual Retroarch cores.";
        }
    }

    class AdultsOnlyGamesMenuItem : HideWheelTypeMenuItem
    {
        public AdultsOnlyGamesMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Adults-Only Wheel") { }

        public override void apply_setting()
        {
            IPlaylist thePlaylist = null;

            foreach (var playlist in PluginHelper.DataManager.GetAllPlaylists())
            {
                if (playlist.Name.Equals("Arcade - Mature Games"))
                {
                    //PLaylist exists
                    thePlaylist = playlist;
                    break;
                }
            }

            if (thePlaylist != null)
            {
                Boolean hide_wheel;
                Boolean hide_games;

                if (this.get_value().Equals("Show Playlist"))
                {
                    hide_wheel = false;
                    hide_games = false;
                }
                else if (this.get_value().Equals("Hide Playlist Only"))
                {
                    hide_wheel = true;
                    hide_games = false;
                }
                else if (this.get_value().Equals("Hide Playlist and Games"))
                {
                    hide_wheel = true;
                    hide_games = true;
                }
                else
                {
                    return;
                }

                //Note that playlist.GetAllGames will not return hidden games,
                //but playlist.GetAllPlaylistGames will return all games in the playlist.
                foreach (var playlist_game in thePlaylist.GetAllPlaylistGames())
                {
                    IGame game = null;

                    game = PluginHelper.DataManager.GetGameById(playlist_game.GameId);
                    if(game != null)
                        game.Hide = hide_games;
                }

                if (hide_wheel)
                    thePlaylist.IncludeWithPlatforms = false;
                else
                    thePlaylist.IncludeWithPlatforms = true;

                //Save & Refresh
                PluginHelper.DataManager.Save();
                if (PluginHelper.LaunchBoxMainViewModel != null)
                {
                    PluginHelper.LaunchBoxMainViewModel.RefreshData();
                }
            }
        }
    }


    class GunTypeMenuItem : MenuItem  //Deprecated
    {
        public GunTypeMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Gun Type") { }

        public override void add_values()
        {
            ValueList.Add("IR (AimTrak / GUN4IR)");
            ValueList.Add("Sinden");
        }
        public override void apply_setting()
        {
            IPlaylist thePlaylist = null;
            IEmulator theMameEmulator = null;

            foreach (var playlist in PluginHelper.DataManager.GetAllPlaylists())
            {
                if (playlist.Name.Equals("Arcade Lightgun Games"))
                {
                    //PLaylist exists
                    thePlaylist = playlist;
                    break;
                }
            }

            if (thePlaylist != null)
            {
                String mame_emulator_tile = this.get_value().Equals("Sinden") ? "MAME-SINDEN":"MAME";
                foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
                {
                    if (emulator.Title.Equals(mame_emulator_tile))
                    {
                        theMameEmulator = emulator;
                        break;
                    }
                }

                if (theMameEmulator != null)
                {
                    foreach (var game in thePlaylist.GetAllGames(false))
                    {
                        if (game.Platform.Equals("Arcade"))
                        {
                            game.EmulatorId = theMameEmulator.Id;
                        }
                    }
                }
                
                //Save & Refresh
                PluginHelper.DataManager.Save();
                if (PluginHelper.LaunchBoxMainViewModel != null)
                {
                    PluginHelper.LaunchBoxMainViewModel.RefreshData();
                }
                PluginHelper.DataManager.ReloadIfNeeded();
            }
        }
    }




    class BackupMenuItem : NoValueTypeMenuItem
    {
        public BackupMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Backup Omega Content") { }
        protected override void perform_the_no_value_action()
        {
            my_parent.show_status("Please Wait...");
            try
            {
                //Use a new thread so as not to block the UI thread
                Thread t = new Thread(() => {

                    //Dialog box to choose folder destination for backups
                    var dlg = new FolderBrowserDialog();
                    dlg.Description = "Select the USB drive to save your Omega Backup to";

                    DialogResult srcFolder = dlg.ShowDialog(ForegroundWindow.CurrentWindow);

                    if (srcFolder != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.SelectedPath))
                    {
                        my_parent.hide_status();
                        return;
                    }
                    else {

                        //Create a Omega Backup Folder to store all settings
                        string strDestPath = dlg.SelectedPath;
                        string strDestFolder = strDestPath + "/Omega Backup";
                        
                        //If there's already a Omega Backup folder on the destination drive, Trash that Shit!
                        if (Directory.Exists(strDestFolder))
                        {
                            my_parent.show_status("Deleting old Omega backup folder...");
                            Directory.Delete(strDestFolder, true);
                            Thread.Sleep(2000);
                        }

                        //Create a fresh Omega Backup folder
                        Directory.CreateDirectory(strDestFolder);

                        //Backup Favorites 
                        backup_favorites(strDestFolder);

                        //Backup Bigbox License
                        backup_bigbox_license(strDestFolder);

                        //Backup Gun4IR License
                        backup_Gun4IR_license(strDestFolder);

                        //Backup Ledblinky
                        backup_Ledblinky(strDestFolder);

                        //Backup High Scores
                        backup_mame_high_scores(strDestFolder);

                        //Backup Startup Video & Marquee
                        backup_intro_media(strDestFolder);

                        //Backup Wii save_data
                        backup_wii_saves(strDestFolder);

                        //Backup Mame Stable IDs
                        backup_mame_stableIDs(strDestFolder);

                        //Close the status window
                        my_parent.hide_status();

                    }
                });

                //Thread must be STA for the file dialogue to show
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            catch
            {
                my_parent.hide_status();
            }
        }

        private void backup_favorites(string destPath)
        {

            string destFileName = destPath + "/favorites_backup.bin";
            XDocument xFavoritesDoc;
            int count = 0;

            //Add all favorites to our backup XML
            XElement FavoritesBackup = new XElement("FavoritesBackup");
            foreach (var game in PluginHelper.DataManager.GetAllGames())
            {
                //Add each favorite game to the backup playlist
                if (game.Favorite)
                {
                    FavoritesBackup.Add(
                        new XElement("Favorite",
                              new XElement("GameId", game.Id),
                              new XElement("LaunchBoxDbId", game.LaunchBoxDbId),
                              new XElement("GameTitle", game.Title),
                              new XElement("GameFileName", Path.GetFileName(game.ApplicationPath)),
                              new XElement("GamePlatform", game.Platform)
                                )
                        );
                    count++;
                }
            }

            //Write the backup XML file
            if (File.Exists(destFileName))
                File.Delete(destFileName);
            xFavoritesDoc = new XDocument();
            xFavoritesDoc.Add(FavoritesBackup);
            xFavoritesDoc.Save(destFileName);

            my_parent.show_status("Exported " + count + " Favorites... Now backing up Bigbox License");
            Thread.Sleep(5000);

        }
        private void backup_bigbox_license(string destPath)
        {
            string fileToCopy = LaunchBoxFolder + @"\License.xml";
            string destFileName = destPath + "/License.xml";

            if (File.Exists(fileToCopy))
            {
                //Save file to location
                if (File.Exists(destFileName))
                    File.Delete(destFileName);
                System.IO.File.Copy(fileToCopy, destFileName, true);
                my_parent.show_status("Exported Bigbox license... Now backing up Gun4IR License");
            }
            else
            {
                my_parent.show_status("No license file... Now backing up Ledblinky");
            }

            Thread.Sleep(5000);

        }
        private void backup_Gun4IR_license(string destPath)
        {
            string fileToCopy = LaunchBoxFolder + @"\Tools\JB_GUN4IR_GUI_1.26_FW_2.64\gui.lic";
            string destFileName = destPath + "/gui.lic";

            if (File.Exists(fileToCopy))
            {
                //Save file to location
                if (File.Exists(destFileName))
                    File.Delete(destFileName);
                System.IO.File.Copy(fileToCopy, destFileName, true);
                my_parent.show_status("Exported Gun4IR license... Now backing up Ledblinky");
            }
            else
            {
                my_parent.show_status("No license file... Now backing up Ledblinky");
            }

            Thread.Sleep(5000);

        }
        private void backup_Ledblinky(string destPath)
        {
            string sourcePath = LaunchBoxFolder + @"\Tools\LEDBlinky";
            string destFileLocation = destPath + "/LEDBlinky";
            
            if (Directory.Exists(sourcePath))
            {
                //Save folder to location
                var diSource = new DirectoryInfo(sourcePath);
                var diTarget = new DirectoryInfo(destFileLocation);

                //Copy all the files and subdirectories in Ledblinky folder
                CopyAll(diSource, diTarget);

                my_parent.show_status("Exported LedBlinky Folder... Now backing up Mame High Scores");
            }
            else
            {
                my_parent.show_status("LedBlinky not found... Now backing up Mame High Scores");
            }

            Thread.Sleep(5000);

        }
        private void backup_mame_high_scores(string destPath)
        {
            //Backup high scores for each MAME emulator that Launchbox is using
            foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
            {
                if (emulator.Title.StartsWith("MAME"))
                {
                    //Get path to this mame instance
                    String mame_path = Path.GetDirectoryName(emulator.ApplicationPath);
                    string mame_folder = new DirectoryInfo(mame_path).Name;

                    string sourcePath = mame_path + "/nvram";
                    string destFileLocation = destPath + "/mame_highscores/" + mame_folder + "/nvram";

                    if (Directory.Exists(sourcePath))
                    {
                        //Save folder to location
                        var diSource = new DirectoryInfo(sourcePath);
                        var diTarget = new DirectoryInfo(destFileLocation);

                        //Copy all the mame game nvram folders and files
                        CopyAll(diSource, diTarget);
                    }
                }
            }

            my_parent.show_status("Exported Mame High Scores... Now backing up Intro Marquee and Startup Video");
            Thread.Sleep(5000);
        }
        private void backup_intro_media(string destPath)
        {
            string destFolder = destPath + "/Intro Media";
            string srcIntroMarquee = LaunchBoxFolder + @"\Videos\StartupMarquee";
            string srcIntroStartup = LaunchBoxFolder + @"\Videos\Startup";
            string srcIntroVideo = LaunchBoxFolder + @"\Videos\startup.mp4";
            string destIntroMarqueeLocation = destFolder + "/StartupMarquee";
            string destIntroStartupLocation = destFolder + "/Startup";
            string destIntroVideoFileName = destFolder + "/startup.mp4";

            var diIntroMarqueeIntro = new DirectoryInfo(srcIntroMarquee);
            var diMarqueeTarget = new DirectoryInfo(destIntroMarqueeLocation);
            var diStartupIntro = new DirectoryInfo(srcIntroStartup);
            var diStartupTarget = new DirectoryInfo(destIntroStartupLocation);

            //Create folder to store intro media in on the Omega Backup directory
            Directory.CreateDirectory(destFolder);

            //Save Intro Video
            if(File.Exists(srcIntroVideo))
                System.IO.File.Copy(srcIntroVideo, destIntroVideoFileName, true);

            //Copy the Startup Marquee video folder if it exists
            if (Directory.Exists(srcIntroMarquee))
                CopyAll(diIntroMarqueeIntro, diMarqueeTarget);

            //Copy the Startup video folder if it exists
            if (Directory.Exists(srcIntroStartup))
                CopyAll(diStartupIntro, diStartupTarget);

            my_parent.show_status("Exported Intro Marquee & Video... Now backing up Wii save data");
            Thread.Sleep(5000);

        }
        private void backup_wii_saves(string destPath)
        {
            //Backup high scores for each MAME emulator that Launchbox is using
            foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
            {
                if (emulator.Title.StartsWith("Dolphin"))
                {
                    //Get path to this dolphin instance
                    String wii_path = Path.GetDirectoryName(emulator.ApplicationPath);
                    string wii_folder = new DirectoryInfo(wii_path).Name;

                    string sourcePath = wii_path + "/User/Wii";
                    string destFileLocation = destPath + "/wii_saves/" + wii_folder + "/User/Wii";

                    if (Directory.Exists(sourcePath))
                    {
                        //Save folder to location
                        var diSource = new DirectoryInfo(sourcePath);
                        var diTarget = new DirectoryInfo(destFileLocation);

                        //Copy all the wii user data
                        CopyAll(diSource, diTarget);
                    }
                }
            }

            my_parent.show_status("Exported Wii save data... Now backing up Mame Stable Device Ids");
            Thread.Sleep(5000);
        }

        private void backup_mame_stableIDs(string destPath)
        {
            //Backup stable IDs for each MAME emulator that Launchbox is using
            foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
            {
                if (emulator.Title.StartsWith("MAME"))
                {
                    //Get path to this mame instance
                    String mame_path = Path.GetDirectoryName(emulator.ApplicationPath);
                    string mame_folder = new DirectoryInfo(mame_path).Name;

                    string fileToCopy = mame_path + "/ctrlr/xarcade.cfg";
                    string destFileName = destPath + "/mame_stableIDs/" + mame_folder + "/ctrlr/xarcade.cfg";

                    if (File.Exists(fileToCopy))
                    {
                        if (File.Exists(destFileName))
                            File.Delete(destFileName);

                        //Create any missing folders for destination file
                        Directory.CreateDirectory(Path.GetDirectoryName(destFileName));

                        //Save file to location
                        System.IO.File.Copy(fileToCopy, destFileName, true);
                    }
                }
            }
            my_parent.show_status("Exported Stable Device IDs... Backup Complete... ALL HAIL OMEGA!!!");

            Thread.Sleep(5000);

        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }

    class ImportBackupMenuItem : NoValueTypeMenuItem
    {
        public ImportBackupMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Import Omega Backup") { }
        protected override void perform_the_no_value_action()
        {
            my_parent.show_status("Please Wait...");
            try
            {
                //Use a new thread so as not to block the UI thread
                Thread t = new Thread(() => {
                    //Dialog box to choose folder destination for backups
                    var dlg = new FolderBrowserDialog();
                    dlg.Description = "Select your Omega Backup folder";

                    DialogResult destFolder = dlg.ShowDialog(ForegroundWindow.CurrentWindow);

                    if (destFolder != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.SelectedPath))
                    {
                        my_parent.hide_status();
                        return;
                    }
                    else
                    {
                        //Set the source folder
                        string strSrcFolder = dlg.SelectedPath;
                        //string strSrcFolder = strSrcPath + "/Omega Backup";

                        //Sanity check the import folder. At least one of the following should exist.
                        if (
                        (!Directory.Exists(strSrcFolder + "/Intro Media")) &&
                        (!Directory.Exists(strSrcFolder + "/LEDBlinky")) &&
                        (!Directory.Exists(strSrcFolder + "/mame_highscores")) &&
                        (!Directory.Exists(strSrcFolder + "/mame_stableIDs")) &&
                        (!File.Exists(strSrcFolder + "/favorites_backup.bin")) &&
                        (!File.Exists(strSrcFolder + "/License.xml"))
                        )
                        {
                            my_parent.show_status("Invalid backup folder.");
                            Thread.Sleep(5000);
                            my_parent.hide_status();
                            return;
                        }

                        //Restore Favorites 
                        import_favorites(strSrcFolder);

                        //Restore Bigbox License
                        import_bigbox_license(strSrcFolder);

                        //Restore Gun4IR License
                        import_Gun4IR_license(strSrcFolder);

                        //Restore Ledblinky
                        import_Ledblinky(strSrcFolder);

                        //Restore High Scores
                        import_mame_high_scores(strSrcFolder);

                        //Restore Startup Video & Marquee
                        import_intro_media(strSrcFolder);

                        //Restore Wii save_data
                        import_wii_saves(strSrcFolder);

                        //Restore Mame Stable IDs
                        import_mame_stableIDs(strSrcFolder);

                        //Close the status window
                        my_parent.hide_status();

                    }
                });

                //Thread must be STA for the file dialogue to show
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            catch
            {
                my_parent.hide_status();
            }
        }

        private void import_favorites(string srcPath)
        {
            XDocument xFavoritesDoc;
            IGame game;
            int count = 0;
            string srcFile = srcPath + "/favorites_backup.bin";

            try { xFavoritesDoc = XDocument.Load(srcFile); }
            catch 
            {
                my_parent.show_status("Invalid backup file.");
                Thread.Sleep(5);
                my_parent.hide_status();
                return;
            }

            XElement root = xFavoritesDoc
            .XPathSelectElement("/FavoritesBackup");

            foreach(XElement fav in root.Descendants("Favorite"))
            {
                game = null;
                game = PluginHelper.DataManager.GetGameById(fav.Element("GameId").Value);

                if(game!= null)
                {
                    game.Favorite = true;
                    count++;
                }
                else
                {
                    // There was no direct match for gameId but let's try one more time with <platform,title,filename>
                    IPlatform platform = null;
                    platform = PluginHelper.DataManager.GetPlatformByName(fav.Element("GamePlatform").Value);

                    if(platform != null)
                    {
                        foreach(var platform_game in platform.GetAllGames(true, false))
                        {
                            if(platform_game.Title.Equals(fav.Element("GameTitle").Value) &&
                                Path.GetFileName(platform_game.ApplicationPath).Equals(fav.Element("GameFileName").Value))
                            {
                                platform_game.Favorite = true;
                                count++;
                                break;
                            }
                        }   
                    }
                }
            }

            PluginHelper.DataManager.Save();

            if (PluginHelper.StateManager.IsBigBox == false)
            {
                PluginHelper.LaunchBoxMainViewModel.RefreshData();
            }
            else
            {
                my_parent.restart_required = true;
            }

            my_parent.show_status("Imported " + count + " Favorites" + "\r\nA RESTART IS REQUIRED for this change to take effect... \r\nNow Restoring Bigbox License");
            Thread.Sleep(5000);
        }

        private void import_bigbox_license(string srcFile)
        {
            string destFileName = LaunchBoxFolder + @"\License.xml";
            string fileToCopy = srcFile + "/License.xml";

            if (File.Exists(fileToCopy))
            {
                //Save file to location
                if (File.Exists(destFileName))
                    File.Delete(destFileName);
                System.IO.File.Copy(fileToCopy, destFileName, true);

                my_parent.show_status("Imported Bigbox License... Now restoring Ledblinky");
            }
            else
            {
                my_parent.show_status("No Bigbox License to restore... Now restoring Ledblinky");
            }
            Thread.Sleep(5000);
        }

        private void import_Gun4IR_license(string srcFile)
        {
            string destFileName = LaunchBoxFolder + @"\Tools\JB_GUN4IR_GUI_1.26_FW_2.64\gui.lic";
            string fileToCopy = srcFile + "/gui.lic";

            if (File.Exists(fileToCopy))
            {
                //Save file to location
                if (File.Exists(destFileName))
                    File.Delete(destFileName);
                System.IO.File.Copy(fileToCopy, destFileName, true);

                my_parent.show_status("Imported Gun4IR License... Now restoring Ledblinky");
            }
            else
            {
                my_parent.show_status("No Gun4IR License to restore... Now restoring Ledblinky");
            }
            Thread.Sleep(5000);
        }

        private void import_Ledblinky(string srcFile)
        {
            string destFolderPath = LaunchBoxFolder + @"\Tools\LEDBlinky";
            string backupFolderPath = LaunchBoxFolder + @"\Tools\LEDBlinky_Backup";
            string srcFileLocation = srcFile + "/LEDBlinky";

            if (Directory.Exists(srcFileLocation))
            {
                //Save folder to location
                var diSource = new DirectoryInfo(srcFileLocation);
                var diTarget = new DirectoryInfo(destFolderPath);

                //If Ledblinky is running, kill it
                foreach (var process in Process.GetProcessesByName("LEDBlinky"))
                {
                    process.Kill();
                }

                //If already a backup folder, nuke it
                if (Directory.Exists(backupFolderPath))
                    Directory.Delete(backupFolderPath, true);

                //Make a backup of the existing Ledblinky folder on the drive
                if(Directory.Exists(destFolderPath))
                    Directory.Move(destFolderPath, backupFolderPath);

                //Copy all the files and subdirectories in Ledblinky folder
                CopyAll(diSource, diTarget);

                my_parent.show_status("Imported LedBlinky Folder... Now restoring Mame High Scores");
            }
            else
            {
                my_parent.show_status("No LedBlinky Folder to restore... Now restoring Mame High Scores");
            }
            Thread.Sleep(5000);

        }

        private void import_mame_high_scores(string srcFile)
        {
            //Import high scores for each MAME emulator that Launchbox is using
            foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
            {
                if (emulator.Title.StartsWith("MAME"))
                {
                    //Get path to this mame instance
                    String mame_path = Path.GetDirectoryName(emulator.ApplicationPath);
                    string mame_folder = new DirectoryInfo(mame_path).Name;

                    string destFolderPath = mame_path + "/nvram";
                    string srcFileLocation = srcFile + "/mame_highscores/" + mame_folder + "/nvram";

                    if (Directory.Exists(srcFileLocation))
                    {
                        //Save folder to location
                        var diSource = new DirectoryInfo(srcFileLocation);
                        var diTarget = new DirectoryInfo(destFolderPath);

                        //Copy all the mame game nvram folders and files
                        CopyAll(diSource, diTarget);

                    }
                }
            }

            my_parent.show_status("Imported Mame High Scores... Now restoring Intro Marquee and Startup Video");
            Thread.Sleep(5000);

        }

        private void import_intro_media(string srcFile)
        {
            string srcFolderPath = srcFile + "/Intro Media";
            string destIntroMarquee = LaunchBoxFolder + @"\Videos\StartupMarquee";
            string destIntroBackupMarquee = LaunchBoxFolder + @"\Videos\StartupMarquee_Backup";
            string destIntroVideo = LaunchBoxFolder + @"\Videos\startup.mp4";
            string destIntroVideoOldFileName = LaunchBoxFolder + @"\Videos\startup_original.mp4";
            string destIntroVideoFolder = LaunchBoxFolder + @"\Videos\Startup";
            string destIntroVideoBackupFolder = LaunchBoxFolder + @"\Videos\Startup_Backup";
            string srcIntroMarqueeLocation = srcFolderPath + "/StartupMarquee";
            string srcIntroVideoLocation = srcFolderPath + "/Startup";
            string srcIntroVideoFileName = srcFolderPath + "/startup.mp4";
            string srcIntroVideoOldFileName = srcFolderPath + "/startup_original.mp4";

            var diIntroMarqueeIntro = new DirectoryInfo(srcIntroMarqueeLocation);
            var diVideoTarget = new DirectoryInfo(destIntroMarquee);
            var diStartupFolder = new DirectoryInfo(srcIntroVideoLocation);
            var diStartupFolderTarget = new DirectoryInfo(destIntroVideoFolder);

            //If someone has backed up a startup video
            if (File.Exists(srcIntroVideoFileName))
            {
                //If there is already a backup of the startup file, get rid of it first
                if (File.Exists(destIntroVideoOldFileName))
                    System.IO.File.Delete(destIntroVideoOldFileName);

                //Rename the existing startup.mp4
                if (File.Exists(destIntroVideo))
                    System.IO.File.Move(destIntroVideo, destIntroVideoOldFileName);
                //Import Intro Video
                System.IO.File.Copy(srcIntroVideoFileName, destIntroVideo, true);
            }

            //If someone has backed up a StartupMarquee folder
            if (Directory.Exists(srcIntroMarqueeLocation))
            {
                //If already a backup folder, nuke it
                if (Directory.Exists(destIntroBackupMarquee))
                    Directory.Delete(destIntroBackupMarquee, true);

                //If there is a StartupMarquee Folder
                if (System.IO.Directory.Exists(destIntroMarquee))
                {
                    //Create backup of the Startup Marquee folder
                    System.IO.Directory.Move(destIntroMarquee, destIntroBackupMarquee);
                }
                //Copy the Startup Marquee folder
                CopyAll(diIntroMarqueeIntro, diVideoTarget);
            }

            //If someone has backed up a Startup folder
            if (Directory.Exists(srcIntroVideoLocation)) {

                //If already a backup folder, nuke it
                if (Directory.Exists(destIntroVideoBackupFolder))
                    Directory.Delete(destIntroVideoBackupFolder, true);

                //If there is a Startup Folder
                if (Directory.Exists(destIntroVideoFolder))
                {
                    //Make a backup of the existing Startup folder
                    System.IO.Directory.Move(destIntroVideoFolder, destIntroVideoBackupFolder);
                }
                //Import the backup folder
                CopyAll(diStartupFolder, diStartupFolderTarget);
            }

            my_parent.show_status("Imported Intro Marquee & Video... Now restoring Wii save data");
            Thread.Sleep(5000);

        }

        private void import_wii_saves(string srcFile)
        {
            //Import high scores for each MAME emulator that Launchbox is using
            foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
            {
                if (emulator.Title.StartsWith("Dolphin"))
                {
                    //Get path to this dolphin instance
                    String wii_path = Path.GetDirectoryName(emulator.ApplicationPath);
                    string wii_folder = new DirectoryInfo(wii_path).Name;

                    string destFolderPath = wii_path + "/User/Wii";
                    string srcFileLocation = srcFile + "/wii_saves/" + wii_folder + "/User/Wii";

                    if (Directory.Exists(srcFileLocation))
                    {
                        //Save folder to location
                        var diSource = new DirectoryInfo(srcFileLocation);
                        var diTarget = new DirectoryInfo(destFolderPath);

                        //Copy all the wii data
                        CopyAll(diSource, diTarget);

                    }
                }
            }

            my_parent.show_status("Imported Wii save data... Now restoring Mame Stable Device Ids");
            Thread.Sleep(5000);
        }

        private void import_mame_stableIDs(string srcFile)
        {
            //Import stable IDs for each MAME emulator that Launchbox is using
            foreach (var emulator in PluginHelper.DataManager.GetAllEmulators())
            {
                if (emulator.Title.StartsWith("MAME"))
                {
                    //Get path to this mame instance
                    String mame_path = Path.GetDirectoryName(emulator.ApplicationPath);
                    string mame_folder = new DirectoryInfo(mame_path).Name;

                    string destFileName = mame_path + "/ctrlr/xarcade.cfg";
                    string fileToCopy = srcFile + "/mame_stableIDs/" + mame_folder + "/ctrlr/xarcade.cfg";

                    if (File.Exists(fileToCopy))
                    {
                        if (File.Exists(destFileName))
                            File.Delete(destFileName);

                        //Create any missing folders for destination file
                        Directory.CreateDirectory(Path.GetDirectoryName(destFileName));

                        //Save file to location
                        System.IO.File.Copy(fileToCopy, destFileName, true);
                    }
                }
            }

            my_parent.show_status("Imported Stable Device IDs... Restore Complete... ALL HAIL OMEGA!!!");
            Thread.Sleep(5000);

        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName))
                Directory.Delete(target.FullName,true);
            
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }


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

                    String CurrentVersion = Version.version.Split('v').Last();

                    if (Convert.ToDouble(LatestVersion) <= Convert.ToDouble(CurrentVersion))
                    {
                        my_parent.show_status("You are up to date.");
                        Thread.Sleep(4000);
                        my_parent.hide_status();
                        return;
                    }

                    my_parent.show_status("Omega Support Package v" + LatestVersion + " is available.");
                    Thread.Sleep(3000);

                    //Determine the file to download. Find the mapping with the highest version
                    //number that is less than or equal to our version
                    String BestMatch = "0";
                    String Filename = "null";
                    try
                    {
                        var UpdateMappings = xUpdatesDoc.Element("OmegaUpdates").Element("UpdateMappings");
                        foreach (var Mapping in UpdateMappings.Elements())
                        {
                            String OldVersion = (String)Mapping.Element("OldVersion").Value;

                            if (Convert.ToDouble(OldVersion) <= Convert.ToDouble(CurrentVersion))
                            {
                                if (Convert.ToDouble(OldVersion) > Convert.ToDouble(BestMatch))
                                {
                                    BestMatch = OldVersion;
                                    Filename = (String)Mapping.Element("Filename").Value;
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

                    if (!Directory.Exists(LaunchBoxFolder + "/Updates"))
                        Directory.CreateDirectory(LaunchBoxFolder + "/Updates");

                    String InstallerPath = LaunchBoxFolder + "/Updates/" + Filename;

                    //Delete the file if it exists.
                    if (File.Exists(InstallerPath))
                        File.Delete(InstallerPath);

                    //Download the installer
                    my_parent.show_status("Downloading Omega Support Package v" + LatestVersion + " (0%)");

                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("User-Agent: Other");
                    //webClient.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");

                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        my_parent.show_status("Downloading Omega Support Package v" + LatestVersion + " (" + e.ProgressPercentage + "%)");
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

                            my_parent.show_status(e.Error.ToString());
                            Thread.Sleep(10000);
                            webClient.Dispose();
                            my_parent.hide_status();
                            return;
                        }

                        Thread.Sleep(4000);
                        my_parent.show_status("File download completed.");
                        Thread.Sleep(3000);

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
                    webClient.Dispose();
                });
                t.Start();
            }
            catch
            {
                my_parent.hide_status();
            }
        }
    }

    class CancelMenuItem : NoValueTypeMenuItem
    {
        public CancelMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Cancel and Exit (updates will be discarded)") { }

        protected override void perform_the_no_value_action()
        {
            my_parent.restart_required = false;
            my_parent.apply_all_settings_and_exit(false);
        }
    }
    class ExitMenuItem : NoValueTypeMenuItem
    {
        public ExitMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Apply Settings and Exit") { }
        protected override void perform_the_no_value_action()
        {
            my_parent.apply_all_settings_and_exit(true);
        }
    }

}




class ForegroundWindow : IWin32Window
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    static ForegroundWindow obj = null;
    public static ForegroundWindow CurrentWindow
    {
        get
        {
            if (obj == null)
                obj = new ForegroundWindow();
            return obj;
        }
    }
    public IntPtr Handle
    {
        get { return GetForegroundWindow(); }
    }
}
