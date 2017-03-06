
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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




using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Drawing;
using Vlc.DotNet.Core;
using System.IO;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for Video.xaml
    /// </summary>
    public partial class Video : UserControl
    {
        public Video() 
        {

            InitializeComponent();

            LoadVlc();

        }

        

        void LoadVlc()
        {
            try
            {
                // set libvlc dir
                vlc.MediaPlayer.VlcLibDirectoryNeeded +=
                    OnVlcControlNeedsLibDirectory;

                vlc.MediaPlayer.EndInit();

                vlc.MediaPlayer.BackColor = ColorTranslator.FromHtml(UhaSub.Properties.Settings.Default.background);

                vlc.MediaPlayer.PositionChanged += new EventHandler<VlcMediaPlayerPositionChangedEventArgs>(
                  Events_PlayerPositionChanged);
                vlc.MediaPlayer.TimeChanged += new EventHandler<VlcMediaPlayerTimeChangedEventArgs>(
                    Events_TimeChanged);

                vlc.MediaPlayer.LengthChanged += new EventHandler<VlcMediaPlayerLengthChangedEventArgs>(
                    Events_LengthChanged);

            }
            catch (Exception e)
            {
                /*
                 * exit when can't load libvlc
                 */
                MessageBox.Show(UhaSub.Properties.Resources.msg_vlc_no_found);
                Environment.Exit(-1);
            }
        }
        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            
            // now only support x86
            //e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"..\..\..\lib\x86\"));
            e.VlcLibDirectory = new DirectoryInfo(currentDirectory);
        }


        void Events_TimeChanged(object sender, VlcMediaPlayerTimeChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                time = e.NewTime;
                this.ctText.Text = time.ToString();
            }));
        }

        void Events_PlayerPositionChanged(object sender, VlcMediaPlayerPositionChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                position = e.NewPosition;
                this.tmSlider.Value = position;
            }));
        }

        void Events_LengthChanged(object sender, VlcMediaPlayerLengthChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                totalTime = (long)e.NewLength;
                this.ttText.Text = totalTime.ToString();
            }));
        }


        /*
         * open a media 
         * but not play it
         */
        private void OpenMedia()
        {
            // open media
            vlc.MediaPlayer.SetMedia(new FileInfo(source.OriginalString), null);


            // set volume
            vlc.MediaPlayer.Audio.Volume = (int)vlSlider.Value;

            // change button to play
            is_playing = false;
            plBtn.Content = s_play;
            plBtn.ToolTip = UhaSub.Properties.Resources.video_play_tip;
        }



      


        /*  
         * play 
         * pause
         * and stop
         */
        public void Play()
        {
            vlc.MediaPlayer.Play();
        }

        /*
         * play state
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
         * the source file to play
         */
        private Uri source;
        public Uri Source
        {
            get { return source; }
            set { source = value; OpenMedia(); }
        }

        /*  
         * the media current position
         * between 0 and 1
         */
        private double position;
        public double Position
        {
            get { return position; }
            set 
            {
                if (value < 0) return;
                if (value == position) return;
                position = value;
                vlc.MediaPlayer.Position = (float)value;
            }
        }


        /* 
         * current time for media
         * as 1ms
         */
        private Time time = 0;
        public Time Time
        {
            get { return time; }
            set 
            {
                if (value == time) return;
                time = value;
                vlc.MediaPlayer.Time = value;
            }
        }

        /* 
         * current time for media
         * as 1ms
         * // read only
         */
        private Time totalTime = 0;
        public Time TotalTime
        {
            get { return totalTime; }
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

            if(is_mutex)
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
         * open file
         */
        public MainWindow main;
        public void OnOpenFile(object sender, RoutedEventArgs e)
        {
            main.OnOpenFile(sender, e);
        }

        public void OnOpenSub(object sender, RoutedEventArgs e)
        {
            main.OnOpenSub(sender, e);
        }

        public void OnOpenSetting(object sender, RoutedEventArgs e)
        {
            main.OnOpenSetting(sender, e);
        }

        public void OnSaveAs(object sender, RoutedEventArgs e)
        {
            main.OnSaveAs(sender, e);
        }

        public void OnSave(object sender, RoutedEventArgs e)
        {
            main.OnSave(sender, e);
        }
    }

}