using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UhaSub.ViewModel;
using Vlc.DotNet.Wpf;
using WpfVlc;



namespace UhaSub.View
{
    /// <summary>
    /// Interaction logic for Control.xaml
    /// </summary>
    public partial class Control : UserControl
    {
        public Control()
        {
            InitializeComponent();
            
            this.Loaded+=Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            video = MainViewModel.Instance.VideoVM;
        }

        public VlcControl video;


        #region callback

        public void ReachEnd()
        {
            this.tmSlider.Value = 0;

            Button btn = this.plBtn;
            btn.Content = s_play;
            btn.ToolTip = UhaSub.Properties.Resources.video_play_tip;
        }


        /*
        * mutex
        */
        private string s_mutex = "";
        private string s_no_mutex = "";

        private void OnMutexClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            video.Mutex = !video.Mutex;
            if (!video.Mutex)
            {
                btn.Content = s_no_mutex;
                btn.ToolTip = UhaSub.Properties.Resources.video_mutex_tip;
            }
            else
            {
                btn.Content = s_mutex;
                btn.ToolTip = UhaSub.Properties.Resources.video_nomutex_tip;
            }

        }

        /*
         * play or pause
         */
        private string s_play = "";
        private string s_pause = "";
        private void OnPlayClick(object sender, RoutedEventArgs e)
        {

            Button btn = this.plBtn;

            video.IsPlay = !video.IsPlay;

            if (!video.IsPlay)
            {
                btn.Content = s_play;
                btn.ToolTip = UhaSub.Properties.Resources.video_play_tip;
            }
            else
            {
                btn.Content = s_pause;
                btn.ToolTip = UhaSub.Properties.Resources.video_pause_tip;
            }

        }


        /*
         * change play rate
         */
        private void MenuPlayRateClick(object sender, RoutedEventArgs e)
        {
            /*
             * unchecked all item
             */
            foreach (var c in cm_play_rate.Items)
            {
                if (c is MenuItem)
                    (c as MenuItem).IsChecked = false;
            }
            MenuItem m = sender as MenuItem;

            switch (m.Header as string)
            {
                case "50%": video.Rate = 0.5f; break;
                case "100%": video.Rate = 1.0f; break;
                case "125%": video.Rate = 1.25f; break;
                case "150%": video.Rate = 1.5f; break;
                case "200%": video.Rate = 2.0f; break;

                default: return;
            }

            m.IsChecked = true;
        }


        /*
         * change display sub
         */
        private void MenuSubClick(object sender, RoutedEventArgs e)
        {
            /*
             * unchecked all item
             */
            foreach (var c in cm_play_rate.Items)
            {
                if (c is MenuItem)
                    (c as MenuItem).IsChecked = false;
            }
            MenuItem m = sender as MenuItem;

            switch (m.Tag as string)
            {
                case "nul": video.SubFile = null; break;
                case "now": video.SubFile = MainViewModel.Instance.SubVM.FileName; break;

                default: return;
            }

            m.IsChecked = true;
        }


        #endregion


        

    }
}
