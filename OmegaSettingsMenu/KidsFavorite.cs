using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;


namespace OmegaSettingsMenu
{
    class KidsFavorite : IGameMenuItemPlugin
    {
        bool IGameMenuItemPlugin.SupportsMultipleGames { get { return false; } }

        string IGameMenuItemPlugin.Caption
        {
            get
            {
                return this.caption;
            }
        }

        Image IGameMenuItemPlugin.IconImage { get { return null; } }

        bool IGameMenuItemPlugin.ShowInLaunchBox { get { return false; } }

        bool IGameMenuItemPlugin.ShowInBigBox { get { return false; } }

        bool IGameMenuItemPlugin.GetIsValidForGame(IGame selectedGame)
        {
            if(selectedGame.GenresString.Contains("KidsFavorite"))
                this.caption = "Unfavorite (Kids)";
            else
                this.caption = "Favorite (Kids)";

            return true;
        }

        bool IGameMenuItemPlugin.GetIsValidForGames(IGame[] selectedGames)
        {
            return false;
        }

        void IGameMenuItemPlugin.OnSelected(IGame selectedGame)
        {
            if (selectedGame.GenresString.Contains("KidsFavorite"))
            {
                selectedGame.GenresString = selectedGame.GenresString.Replace("KidsFavorite", "");
                this.caption = "Favorite (Kids)";
            }
            else
            {
                selectedGame.GenresString = selectedGame.GenresString + " KidsFavorite";
                this.caption = "Unfavorite (Kids)";
            }

            PluginHelper.DataManager.Save();
            //Need to update the caption here...

           
        }

        void IGameMenuItemPlugin.OnSelected(IGame[] selectedGames)
        {
            return;
        }

        private String caption;
    }
}
