
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


        public Control control = null;
        void Events_TimeChanged(object sender, VlcMediaPlayerTimeChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                time = e.NewTime;
                control.ctText.Text = time.ToString();
            }));
        }

        void Events_PlayerPositionChanged(object sender, VlcMediaPlayerPositionChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                position = e.NewPosition;
                control.tmSlider.Value = position;
            }));
        }


        public Spec spec;
        void Events_LengthChanged(object sender, VlcMediaPlayerLengthChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                //totalTime = (long)e.NewLength; // this lenght is error
                totalTime = vlc.MediaPlayer.Length;
                control.ttText.Text = totalTime.ToString();

                // sync totaltime
                spec.Sync(totalTime);
            }));
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
                if (!vlc.MediaPlayer.IsPlaying)
                    return;
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
                if (!vlc.MediaPlayer.IsPlaying)
                    return;
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

        

        
        
    }

}