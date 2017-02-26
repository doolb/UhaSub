
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

        // varial for nVlc
        IMediaPlayerFactory m_factory;
        public IVideoPlayer m_player;
        public IMedia m_media;

        

        void LoadVlc()
        {
            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
            p.BackColor = System.Drawing.Color.FromArgb(33,33,33);
            windowsFormsHost1.Child = p;

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
                
            }));
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                position = e.NewPosition;
            }));
        }

        /*
         * open a media 
         * but not play it
         */
        private void OpenMedia()
        {
                m_media = m_factory.CreateMedia<IMediaFromFile>(source.OriginalString);

                m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(
                    Events_DurationChanged);
                m_media.Events.StateChanged += new EventHandler<MediaStateChange>(
                    Events_StateChanged);

                m_player.Open(m_media);
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

        public void Pause()
        {
            m_player.Pause();
        }

        public void Stop()
        {
            m_player.Stop();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            m_player.ToggleMute();
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
                position = value;
                m_player.Position = (float)value;
            }
        }


        /* 
         * current time for media
         * as 1ms
         */
        private long time;
        public long Time
        {
            get { return time; }
            set 
            { 
                time = value;
                m_player.Time = value;
            }
        }

        /* 
         * current time for media
         * as 1ms
         * // read only
         */
        private long totalTime;
        public long TotalTime
        {
            get { return totalTime; }
        }

        /*
         * volume of play
         * between 0 and 100
         */
        private int volume;
        public int Volume
        {
            get { return volume; }
            set 
            { 
                volume = value;
                m_player.Volume = value;
            }
        }
    }

}