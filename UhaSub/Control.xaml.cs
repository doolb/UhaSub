using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vlc.DotNet.Wpf;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for Control.xaml
    /// </summary>
    public partial class Control : UserControl
    {
        public Control()
        {
            InitializeComponent();
        }

        public Config cfg;    // config for setting
        public Sub sub;        // sub data-grid
        public VlcControl vlc; //  vlc player
        public MainWindow  main;
        public Spec spec;

        public void OnOpenFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter =
                    "Video files (*.mp4)|*.mp4;*.mkv|All files (*.*)|*.*";

                if (fileDialog.ShowDialog() == true)
                {

                    // open video
                    this.OpenMedia(fileDialog.FileName);

                    // open sub
                    sub.Open(fileDialog.FileName);

                    // set title
                    main.Title = UhaSub.Properties.Resources.Title + "  -  " +
                        fileDialog.FileName;

                    spec.Open(fileDialog.FileName);

                }
            }
            catch (Exception _e)
            { }
        }
        public void OnOpenSub(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter =
                    "Video files (*.txt)|*.txt;*.ass|All files (*.*)|*.*";

                if (fileDialog.ShowDialog() == true)
                {
                    // open sub
                    sub.OpenNewSub(fileDialog.FileName);

                }
            }
            catch (Exception _e)
            {

            }
        }

        public void OnOpenSetting(object sender, RoutedEventArgs e)
        {
            var set = new setting.Setting();
            set.ShowDialog();

            cfg.ReLoad();
        }

        public void OnSaveAs(object sender, RoutedEventArgs e)
        {
            try
            {
                sub.SaveAs();
            }
            catch (Exception _e)
            {

            }
        }

        public void OnSave(object sender, RoutedEventArgs e)
        {
            try
            {
                sub.Save();
            }
            catch (Exception _e)
            {

            }
        }


        /*
         * use slider change play position
         */
        bool time_drag_end = true;
        private void time_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            time_drag_end = true;
            vlc.MediaPlayer.Position = (float)(sender as Slider).Value;
        }

        private void time_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            time_drag_end = false;
        }

        /*
         * chage volume
         */
        private void vlSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vlc == null) return;
            if (vlc.MediaPlayer.Audio == null) return;
            vlc.MediaPlayer.Audio.Volume = (int)e.NewValue;
        }



        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            this.Pause();
        }


        /*
         * sub selected changed
         */
        public void OnSubChanged(Ass ass)
        {
            this.subView.Text = ass.Text;
        }

        /*
         * mutex
         */
        private bool is_mutex = false;

        private string s_mutex = "";
        private string s_no_mutex = "";

        private void OnMutexClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (is_mutex)
            {
                btn.Content = s_no_mutex;
                btn.ToolTip = UhaSub.Properties.Resources.video_mutex_tip;
                vlc.MediaPlayer.Audio.ToggleMute();

            }
            else
            {
                btn.Content = s_mutex;
                btn.ToolTip = UhaSub.Properties.Resources.video_nomutex_tip;
                vlc.MediaPlayer.Audio.ToggleMute();
            }

            is_mutex = !is_mutex;
        }


        /*
         * open a media 
         * but not play it
         */
        private void OpenMedia(string path)
        {
            // open media
            vlc.MediaPlayer.SetMedia(new FileInfo(path), null);


            // set volume
            vlc.MediaPlayer.Audio.Volume = (int)vlSlider.Value;

            // change button to play
            is_playing = false;
            plBtn.Content = s_play;
            plBtn.ToolTip = UhaSub.Properties.Resources.video_play_tip;
        }


        /*
         * play state
         * play or pause
         */
        private bool is_playing = false;

        private string s_play = "";
        private string s_pause = "";
        public void Pause()
        {

            Button btn = this.plBtn;

            if (is_playing)
            {
                btn.Content = s_play;
                btn.ToolTip = UhaSub.Properties.Resources.video_play_tip;
                vlc.MediaPlayer.Pause();
            }
            else
            {

                btn.Content = s_pause;
                btn.ToolTip = UhaSub.Properties.Resources.video_pause_tip;
                vlc.MediaPlayer.Play();
            }

            is_playing = !is_playing;
        }

        public void Stop()
        {
            vlc.MediaPlayer.Stop();
        }


        /*
         * select sub which will be show
         * 
         * chech menu, refer:https://social.msdn.microsoft.com/Forums/vstudio/en-US/8f0314b3-9afb-4c44-b684-c23cef325690/check-mark-in-wpf-context-menu?forum=wpf
         * 
         */

        private void MenuSubLoaded(object sender, RoutedEventArgs e)
        {
            if (vlc.MediaPlayer.SubTitles.Count == 0)
            {
                mi_nul.IsChecked = true;
            }
        }

        private void MenuSubClick(object sender, RoutedEventArgs e)
        {
            if (vlc.MediaPlayer.SubTitles.Count == 0)
                return;

            MenuItem it = sender as MenuItem;

            switch (it.Tag as string)
            {
                case "nul": // disable sub
                    vlc.MediaPlayer.SubTitles.Current = vlc.MediaPlayer.SubTitles.All.First();

                    mi_now.IsChecked = false;
                    break;
                case "now": // use current work
                    this.OnSave(null, null); // save sub

                    // set sub
                    vlc.MediaPlayer.SetSubTitles(main.sub.SubFileName);

                    mi_nul.IsChecked = false;
                    break;
            }

            it.IsChecked = true;

        }

        /*
         * change play rate
         */
        private void MenuPlayRateClick(object sender, RoutedEventArgs e)
        {
            /*
             * uncheck all item
             */
            foreach (var c in cm_play_rate.Items)
            {
                if (c is MenuItem)
                    (c as MenuItem).IsChecked = false;
            }
            MenuItem m = sender as MenuItem;

            switch (m.Header as string)
            {
                case "50%": vlc.MediaPlayer.Rate = 0.5f; break;
                case "100%": vlc.MediaPlayer.Rate = 1.0f; break;
                case "125%": vlc.MediaPlayer.Rate = 1.25f; break;
                case "150%": vlc.MediaPlayer.Rate = 1.5f; break;
                case "200%": vlc.MediaPlayer.Rate = 2.0f; break;

                default: return;
            }

            m.IsChecked = true;
        }

    }
}
