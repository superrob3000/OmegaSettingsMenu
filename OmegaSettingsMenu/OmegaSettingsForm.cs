﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OmegaSettingsMenu
{
    public partial class OmegaSettingsForm : Form
    {
        public OmegaSettingsForm(TheSystemMenuItem parent)
        {
            InitializeComponent();

            this.my_parent = parent;

            this.Bounds = Screen.PrimaryScreen.Bounds;

            // 
            // panel1
            // 
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.AutoScroll = false;
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(2*(this.Size.Width/3), this.Size.Height);
            this.panel1.TabIndex = 1;
            this.panel1.BorderStyle = BorderStyle.None;
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Margin = new Padding(0);
            this.panel1.Location = new Point((this.Size.Width / 3),
                (this.Size.Height / 2) - (panel1.Size.Height / 2));
            this.Controls.Add(this.panel1);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);



            this.panelWait.Location = new Point((this.Size.Width / 2) - (panelWait.Size.Width / 2),
                (this.Size.Height / 2) - (panelWait.Size.Height / 2));
            panelWait.Hide();



            Logo = new MenuLogo(this, new Point(3, 10));



            reset_offset();
            ItemList.Add(new MarqueeWidthMenuItem(this, new_offset()));
            /***************************************************************************************/
            /* After adding the first item, we have the size of the item and can clean up spacing. */
            /***************************************************************************************/
            this.stride = Math.Max(80, ItemList[0].labelItem.Size.Height + 2 * this.border_spacing);
            this.border_spacing = (stride - ItemList[0].labelItem.Size.Height) / 2;
            reset_offset();
            ItemList[0].set_location(new_offset());
            /***************************************************************************************/
            /***************************************************************************************/
            ItemList.Add(new MarqueeHeightMenuItem(this, new_offset()));
            ItemList.Add(new MarqueeStretchMenuItem(this, new_offset()));
            ItemList.Add(new MarqueeVerticalAlignmentMenuItem(this, new_offset()));
            ItemList.Add(new ScanlinesMenuItem(this, new_offset()));
            ItemList.Add(new AdultsOnlyGamesMenuItem(this, new_offset()));
            ItemList.Add(new BackupFavoritesMenuItem(this, new_offset()));
            ItemList.Add(new ImportFavoritesMenuItem(this, new_offset()));
            ItemList.Add(new CancelMenuItem(this, new_offset()));
            ItemList.Add(new ExitMenuItem(this, new_offset()));
            


            //Add borders
            reset_offset(ItemList[0].labelItem.Location.Y - this.border_spacing);
            for (int index = 0; index <= ItemList.Count; index++)
            {
                BorderList.Add(new MenuBorder(this, new_offset(), (index < 2) ? true : false));
            }

            //Set initial focus on first item
            ItemList[ItemIndex].set_focus(true);

            //Add Tips display
            Tips = new MenuTips(this, new Point(Logo.Location.X + Logo.Width, Logo.Location.Y),
                new Size(BorderList[0].length - (Logo.Location.X + Logo.Width), BorderList[0].location.Y - Logo.Location.Y));

            Tips.set_tips(ItemList[ItemIndex].get_tips());

            //Get values from XML file
            xml_path = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Data/OmegaSettings.xml";

            try { xSettingsDoc = XDocument.Load(xml_path); }
            catch { xSettingsDoc = null; }

            load_values_from_xml();

            //apply the settings
            apply();

            MovePlayer.URL = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Sounds/Classic/Move.wav";
            SelectPlayer.URL = Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).ToString() + "/Sounds/Multi-Sound Default/Select/SELECT042.wav";
            //Skipping past the end of the sound let's us preload it without hearing it. Without this,
            //the sounds would play when setting the URL (during bigbox startup)
            MovePlayer.controls.currentPosition = 10;
            SelectPlayer.controls.currentPosition = 10;
        }

        /****************************************************************
         ****************************************************************
          Topmost by itself was not working consitently. The menu was 
          showing up behind BigBox most of the time on the arcade (but 
          never on my development machine). After much experimentation, 
          the following is the workaround.                             
         ****************************************************************
         ****************************************************************/
        internal bool is_menu_loaded = false;
        private void MenuForm_Load(object sender, EventArgs e)
        {
            is_menu_loaded = true;
        }

        /* Having the plugin call this modified version of Show after the
         * form has loaded seems to work 100% of the time. So after the 
         * Application.Run(frm) call on the first selection, the plugin will 
         * also wait for is_menu_loaded and call frm.Show().
         */
        public new void Show()
        {
            this.TopLevel = true;
            this.TopMost = true;
            this.Focus();
            this.TopMost = true;
            base.Show();
        }
        /****************************************************************
         ****************************************************************
         ****************************************************************/
        private void load_values_from_xml()
        {
            if (xSettingsDoc == null)
            {
                //Create the settings file. Use default values.
                foreach (var item in ItemList)
                {
                    item.set_value("");
                }
                save_values_to_xml();
            }
            else
            {
                foreach (var item in ItemList)
                {
                    String tag = item.get_xmltag();
                    if (!String.IsNullOrEmpty(tag))
                    {
                        try
                        {
                            String val = xSettingsDoc
                            .XPathSelectElement("/OmegaSettings")
                            .Element(tag)
                            .Value;

                            item.set_value(val);
                        }
                        catch
                        {
                            //default val
                            item.set_value("");

                            //Add it to the settings file
                            XElement root = xSettingsDoc
                            .XPathSelectElement("/OmegaSettings");

                            root.Add(new XElement(tag, item.get_value()));
                            xSettingsDoc.Save(xml_path);
                        }
                    }
                }
            }
        }

        private void reset_offset()
        {
            reset_offset(Logo.Location.Y + Logo.Height + border_spacing + 3, this.stride); //leave room for logo, current stride
        }

        private void reset_offset(int y)
        {
            reset_offset(y, this.stride); //new location, current stride
        }

        private void reset_offset(int y, int stride) //new location, new stride
        {
            this.offset.Y = y;
            this.stride = stride;
        }

        private Point new_offset()
        {
            Point ret = this.offset;
            this.offset.Y += this.stride;
            return ret;
        }

        private void reorder_items()
        {
            int position, index;

            reset_offset();
            for (position = 0; position < ItemList.Count; position++)
            {
                index = (position + ItemIndex) % ItemList.Count;
                ItemList[index].set_location(new_offset());
            }
            Tips.set_tips(ItemList[ItemIndex].get_tips());
            play_move();
        }

        internal void apply_all_settings_and_exit(bool apply)
        {
            if (apply)
            {
                //Put up the Please Wait panel.
                panel1.Hide();
                panelWait.Show();

                applying_settings = true;
                timerExit.Enabled = true;
                //Run this in a new thread so as not to block the UI thread.
                Task.Factory.StartNew(() => { apply_and_save(); });
            }
            else
            {
                //Reload values from XML file
                load_values_from_xml();

                //Hide this form
                this.Hide();
            }

        }

        private void apply()
        {
            //Apply the settings
            foreach (var item in ItemList)
            {
                item.apply_setting();
            }

            //Tell my parent to grab what it needs
            my_parent.grab_settings_from_xml(this);
        }

        private void save_values_to_xml()
        {
            XElement OmegaSettings = new XElement("OmegaSettings");
            foreach (var item in ItemList)
            {
                string tag = item.get_xmltag();
                if (!String.IsNullOrEmpty(tag))
                {
                    OmegaSettings.Add(new XElement(tag, item.get_value()));
                }
            }
            xSettingsDoc = new XDocument();
            xSettingsDoc.Add(OmegaSettings);
            xSettingsDoc.Save(xml_path);
        }

        public void apply_and_save()
        {
            apply();
            save_values_to_xml();

            applying_settings = false;
        }

        private void timerExit_Tick(object sender, EventArgs e)
        {
            if (applying_settings == false)
            {
                this.timerExit.Enabled = false;
                this.Hide();
                panelWait.Hide();
                panel1.Show();
            }
        }

        private void play_move()
        {
            MovePlayer.controls.stop();
            MovePlayer.controls.play();
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //handle your keys here

            if (keyData.Equals(Keys.Enter) || keyData.Equals(Keys.D1))
            {
                ItemList[ItemIndex].on_enter();
                return true;
            }
            if (keyData.Equals(Keys.Right))
            {
                ItemList[ItemIndex].on_right();
                return true;
            }
            if (keyData.Equals(Keys.Left))
            {
                ItemList[ItemIndex].on_left();
                return true;
            }
            if (keyData.Equals(Keys.Down))
            {
                ItemList[ItemIndex].set_focus(false);
                ItemIndex = (ItemIndex + 1) % ItemList.Count;
                reorder_items();
                ItemList[ItemIndex].set_focus(true);

                return true;
            }
            if (keyData.Equals(Keys.Up))
            {
                ItemList[ItemIndex].set_focus(false);
                if(ItemIndex==0)
                    ItemIndex = ItemList.Count - 1;
                else
                    ItemIndex = (ItemIndex - 1);
                reorder_items();
                ItemList[ItemIndex].set_focus(true);

                return true;
            }

            if (keyData.Equals(Keys.Escape))
            {
                ItemList[ItemIndex].set_focus(false);
                ItemIndex = ItemList.Count - 2; //Cancel item
                reorder_items();
                ItemList[ItemIndex].set_focus(true);
                return true;
            }

            return false;
        }
        private System.Windows.Forms.Panel panel1;
        private Point offset = new Point(3, 3);
        private int stride = 80; //default stride
        private int border_spacing = 3;

        private List<MenuBorder> BorderList = new List<MenuBorder>();
        private List<MenuItem> ItemList = new List<MenuItem>();
        private int ItemIndex = 0;

        private MenuLogo Logo;
        internal MenuTips Tips;

        private WMPLib.WindowsMediaPlayer MovePlayer = new WMPLib.WindowsMediaPlayer();
        public WMPLib.WindowsMediaPlayer SelectPlayer = new WMPLib.WindowsMediaPlayer();

        public Panel get_panel()
        {
            return this.panel1;
        }

        public String get_value_by_name(String name)
        {
            foreach (var item in ItemList)
            {
                if (item.get_name() == name)
                    return item.get_value();
            }
            return "";
        }
        public String get_value_by_xmltag(String tag)
        {
            foreach (var item in ItemList)
            {
                if (item.get_xmltag() == tag)
                    return item.get_value();
            }
            return "";
        }

        private TheSystemMenuItem my_parent;

        private bool applying_settings = false;
        private String xml_path;
        XDocument xSettingsDoc;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (var border in BorderList)
            {
                g.DrawLine(border.borderpen, 0, border.location.Y, border.length, border.location.Y);
            }

            if(BorderList.Count > 2)
            {
                Pen borderpen = BorderList[BorderList.Count - 1].borderpen;

                //Top Horizontal line
                g.DrawLine(borderpen, 0,                    borderpen.Width, 
                                      BorderList[0].length, borderpen.Width);

                //Left Vertical line
                g.DrawLine(borderpen, 0, borderpen.Width, 
                                      0, BorderList[BorderList.Count - 1].location.Y);


                borderpen = BorderList[0].borderpen;

                //Left focused Vertical line on focused item
                g.DrawLine(borderpen, 0, BorderList[0].location.Y, 
                                      0, BorderList[1].location.Y);
            }

            if (Tips.Visible)
            {
                foreach (var line in Tips.LineList)
                {
                    g.DrawLine(Tips.pen, line.P1.X, line.P1.Y,
                                         line.P2.X, line.P2.Y);
                }
            }

            g.Dispose();
        }
    }
}
