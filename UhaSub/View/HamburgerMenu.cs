using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UhaSub.View
{
    public class HamburgerMenu : MahApps.Metro.Controls.HamburgerMenu
    {
        public HamburgerMenu()
        {
            this.ItemClick += HamburgerMenu_ItemClick;
            this.OptionsItemClick += HamburgerMenu_ItemClick;
        }

        void HamburgerMenu_ItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs e)
        {
            HamburgerMenu hbg = sender as HamburgerMenu;
            hbg.Content = e.ClickedItem;
            hbg.IsPaneOpen = false;
        }
    }
}
