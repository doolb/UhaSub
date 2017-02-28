﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            /*
             * fix notifychanged issue
             * refer:http://stackoverflow.com/questions/8062436/inotifypropertychanged-in-wpf
             */
            this.DataContext = video;
            this.audio.Video = video;
        }


        Config cfg = new Config();
        bool special_start = false;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            /*
             * video control
             */
            if (e.Key == cfg.After)  {video.Time += 3000;return;}

            if(e.Key == cfg.Before) {video.Time -= 3000;return;}

            if (e.Key == cfg.Pause) {video.Pause();     return;}


            /* 
             * sub control
             */
            if(e.Key == cfg.Special)
            {
                    if (special_start)      return;

                    sub.Start(video.Time - cfg.StartTime);
                    special_start = true;
                    return;
            }

            if (e.Key == cfg.Start) { sub.Start(video.Time - cfg.StartTime);    return; }

            if (e.Key == cfg.End)   { sub.End(video.Time + cfg.EndTime);      return; }

            if (e.Key == cfg.Save) { sub.Save(); return; }

            if (e.Key == cfg.Up)    { sub.Up();     return; }
            if (e.Key == cfg.Down)  { sub.Down();   return; }

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == cfg.Special)
            {
                if (special_start)
                {
                    sub.End(video.Time + cfg.EndTime);
                    special_start = false;
                    return;
                }
            }
        }
        private void OnOpenFile(object sender, RoutedEventArgs e)
        {

            var fileDialog = new OpenFileDialog();
            fileDialog.Filter =
                "Video files (*.mp4)|*.mp4;*.mkv|All files (*.*)|*.*";

            if (fileDialog.ShowDialog() == true)
            {

                // open video
                video.Source = new Uri(fileDialog.FileName);    

                // open sub
                sub.Open(fileDialog.FileName);

                // open audio
                audio.Open(fileDialog.FileName);

                // set title
                this.Title = UhaSub.Properties.Resources.Title + "  -  " +
                    fileDialog.FileName;

                video.Play();

            }
        }
        private void OnOpenSub(object sender, RoutedEventArgs e)
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

        /*
         * use slider change play position
         */
        bool time_drag_end = true;
        private void time_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            time_drag_end = true;
            video.Position = (float)(sender as Slider).Value;
        }

        private void time_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            time_drag_end = false;
        }

        


    }
}
