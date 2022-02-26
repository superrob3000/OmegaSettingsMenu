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




    class BackupFavoritesMenuItem : NoValueTypeMenuItem
    {
        public BackupFavoritesMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Export Favorites List") { }
        protected override void perform_the_no_value_action()
        {
            my_parent.show_status("Please Wait...");
            try
            {
                //Use a new thread so as not to block the UI thread
                Thread t = new Thread(() => {
                    backup_favorites();
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

        private void backup_favorites()
        {
            XDocument xSettingsDoc;
            int count = 0;

            //Prompt for the backup file location
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = true;
            dlg.CheckPathExists = true;
            dlg.Title = "Where do you want to save the backup file?";
            dlg.ValidateNames = true;
            dlg.Filter = "binary files|*.bin";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".bin";
            dlg.InitialDirectory = "C:\\Users\\Administrator\\Desktop";
            dlg.FileName = "favorites_backup.bin";
            DialogResult result = dlg.ShowDialog(ForegroundWindow.CurrentWindow);
            if (result != DialogResult.OK) // Test result.
            {
                return;
            }

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
            if (File.Exists(dlg.FileName))
                File.Delete(dlg.FileName);
            xSettingsDoc = new XDocument();
            xSettingsDoc.Add(FavoritesBackup);
            xSettingsDoc.Save(dlg.FileName);

            my_parent.show_status("Exported " + count + " favorites to " + dlg.FileName + ".");
            Thread.Sleep(5000);
            my_parent.hide_status();

        }
    }

    class ImportFavoritesMenuItem : NoValueTypeMenuItem
    {
        public ImportFavoritesMenuItem(OmegaSettingsForm parent, Point location) : base(parent, location, "Import Favorites List") { }
        protected override void perform_the_no_value_action()
        {
            my_parent.show_status("Please Wait...");
            try
            {
                //Use a new thread so as not to block the UI thread
                Thread t = new Thread(() => {
                    import_favorites();
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

        private void import_favorites()
        {
            XDocument xSettingsDoc;
            IGame game;
            int count = 0;

            //Prompt for the backup file location
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.Title = "Choose the favorites file you want to import.";
            dlg.ValidateNames = true;
            dlg.Filter = "binary files|*.bin";
            dlg.InitialDirectory = "C:\\Users\\Administrator\\Desktop";
            DialogResult result = dlg.ShowDialog(ForegroundWindow.CurrentWindow);
            if (result != DialogResult.OK) // Test result.
            {
                return;
            }

            try { xSettingsDoc = XDocument.Load(dlg.FileName); }
            catch 
            {
                my_parent.show_status("Invalid backup file.");
                Thread.Sleep(5);
                my_parent.hide_status();
                return;
            }

            XElement root = xSettingsDoc
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

            my_parent.show_status("Imported " + count + " favorites from " + dlg.FileName + "." +
                                                            "\r\nA restart is required for this change to take effect.");
            Thread.Sleep(5000);
            my_parent.hide_status();
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
