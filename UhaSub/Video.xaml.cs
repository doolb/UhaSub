
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


/*
 * using for nVlc
 */
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Drawing;
using Implementation.Exceptions;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for Video.xaml
    /// </summary>
    public partial class Video : UserControl, INotifyPropertyChanged
    {
        public Video() 
        {
            InitializeComponent();

            LoadVlc();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // varial for nVlc
        IMediaPlayerFactory m_factory;
        public IVideoPlayer m_player;
        public IMedia m_media;

        

        void LoadVlc()
        {
            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
            
            // backgroud
            // use html type
            // refer:http://stackoverflow.com/questions/1914487/how-to-create-a-system-drawing-color-from-its-hexadecimal-rgb-string
            p.BackColor = ColorTranslator.FromHtml(UhaSub.Properties.Settings.Default.background);
            windowsFormsHost1.Child = p;

            try
            {
                // find vlc = true
                // so you can install you own vlc
                m_factory = new MediaPlayerFactory(true);

                m_player = m_factory.CreatePlayer<IVideoPlayer>();


                m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(
                  Events_PlayerPositionChanged);
                m_player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(
                    Events_TimeChanged);
                m_player.Events.MediaEnded += new EventHandler(
                  Events_MediaEnded);
                m_player.Events.PlayerStopped += new EventHandler(
                  Events_PlayerStopped);

                m_player.WindowHandle = p.Handle;
            }
            catch (Exception)
            {
                /*
                 * exit when can't load libvlc
                 */
                MessageBox.Show(UhaSub.Properties.Resources.msg_vlc_no_found);
                Environment.Exit(-1);
            }
        }

        void Events_PlayerStopped(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                
            }));
        }

        void Events_MediaEnded(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                
            }));
        }


        void Events_TimeChanged(object sender, MediaPlayerTimeChanged e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                time = e.NewTime;
                this.ctText.Text = time.ToString();
                NotifyPropertyChanged("Time");
            }));
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                position = e.NewPosition;
                this.tmSlider.Value = position;
                NotifyPropertyChanged("Position");
            }));
        }

        /*
         * open a media 
         * but not play it
         */
        private void OpenMedia()
        {
            if (m_media != null)
            {
                m_media.Dispose();
            }

            m_media = m_factory.CreateMedia<IMediaFromFile>(source.OriginalString);

            m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(
                    Events_DurationChanged);
            m_media.Events.StateChanged += new EventHandler<MediaStateChange>(
                    Events_StateChanged);


            // open mediao
            m_player.Open(m_media);


            // set volume
            m_player.Volume = (int)vlSlider.Value;

            // chage button to play
            is_playing = false;
            plBtn.Content = s_play;
            plBtn.ToolTip = UhaSub.Properties.Resources.video_play_tip;
        }


        void Events_StateChanged(object sender, MediaStateChange e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                
            }));
        }

        void Events_DurationChanged(object sender, MediaDurationChange e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                totalTime = e.NewDuration;
                this.ttText.Text = totalTime.ToString();
                NotifyPropertyChanged("TotalTime");
            }));
        }


        /*  
         * play 
         * pause
         * and stop
         */
        public void Play()
        {
            m_player.Play();
        }

        /*
         * play state
         */
        private bool is_playing = false;

        private string s_play = "";
        private string s_pause = "";
        public void Pause()
        {
            if (m_media == null)
                return;

            Button btn = this.plBtn;

            if (is_playing)
            {
                btn.Content = s_play;
                btn.ToolTip = UhaSub.Properties.Resources.video_play_tip;
                m_player.Pause();
            }
            else
            {

                btn.Content = s_pause;
                btn.ToolTip = UhaSub.Properties.Resources.video_pause_tip;
                m_player.Play();
            }

            is_playing = !is_playing;
        }

        public void Stop()
        {
            m_player.Stop();
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
                m_player.Position = (float)value;
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
                if (m_media == null) return;
                if (value == time) return;
                time = value;
                m_player.Time = value;
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
            m_player.Position = (float)(sender as Slider).Value;
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
            if (m_player == null) return;
            m_player.Volume = (int)e.NewValue;
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
                m_player.ToggleMute();
            }
            else
            {
                btn.Content = s_mutex;
                btn.ToolTip = UhaSub.Properties.Resources.video_nomutex_tip;
                m_player.ToggleMute();
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