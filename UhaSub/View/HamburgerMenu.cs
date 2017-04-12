using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UhaSub.View
{
    public class HamburgerMenu : MahApps.Metro.Controls.HamburgerMenu
    {
        public HamburgerMenu()
        {
            this.ItemClick += HamburgerMenu_ItemClick;
            this.OptionsItemClick += HamburgerMenu_ItemClick;

            this.Loaded += HamburgerMenu_Loaded;
        }

        #region is-use-parent-datacontext-property
        private bool IsUseParentDataContext;
        public static readonly DependencyProperty ParentDataContextProperty =
         DependencyProperty.Register("SetText", typeof(bool), typeof(HamburgerMenu), new
            PropertyMetadata(false, new PropertyChangedCallback(OnParentDataContextChanged)));

        public bool ParentDataContext
        {
            get { return (bool)GetValue(ParentDataContextProperty); }
            set { SetValue(ParentDataContextProperty, value); }
        }

        private static void OnParentDataContextChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            (d as HamburgerMenu).OnParentDataContextChanged(e);
        }

        private void OnParentDataContextChanged(DependencyPropertyChangedEventArgs e)
        {
            this.IsUseParentDataContext = (bool)e.NewValue;

        }
        #endregion

        void HamburgerMenu_ItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs e)
        {
            // open selected
            HamburgerMenu hbg = sender as HamburgerMenu;
            hbg.Content = e.ClickedItem;
            hbg.IsPaneOpen = false;

            // set data context
            if (IsUseParentDataContext)
            {
                UserControl user = (hbg.Content as HamburgerMenuItem).Tag as UserControl;
                if (user != null)
                    user.DataContext = this.DataContext;
            }
        }

        void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if(IsUseParentDataContext)
            {
                ContentControl ctl = (this.SelectedItem as HamburgerMenuItem).Tag as ContentControl;
                if(ctl !=null)
                {
                    ctl.DataContext = this.DataContext;
                }
            }
        }
    }
}
