using Microsoft.Win32;
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

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {


        public MainWindow()
        {
            InitializeComponent();

            this.sub.SubSelected += this.control.OnSubChanged;

            this.video.EndReached += this.control.ReachEnd;
            this.video.TotalTimeChanged += this.spec.Init;



            /*
             * set refer to control
             */
            this.control.main = this;
            this.control.DataContext = this.video;
            
            this.control.vlc = video.vlc;
            this.control.sub = this.sub;
            this.control.cfg = this.cfg;
            this.control.spec = this.spec;
            this.control.video = this.video;

            this.spec.video = this.video;

            
            this.Closing += MainWindow_Closing;

            /*
             * set column width from config
             */
            if (UhaSub.Properties.Settings.Default.cfg_col_size_1<0 ||
                UhaSub.Properties.Settings.Default.cfg_col_size_2<0)
            {
                UhaSub.Properties.Settings.Default.cfg_col_size_1 = 1.4;
                UhaSub.Properties.Settings.Default.cfg_col_size_2 = 1;
                UhaSub.Properties.Settings.Default.Save();
            }
            col_def1.Width = new GridLength(UhaSub.Properties.Settings.Default.cfg_col_size_1, GridUnitType.Star);
            col_def2.Width = new GridLength(UhaSub.Properties.Settings.Default.cfg_col_size_2, GridUnitType.Star);

            update_spec();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            audio.Close();

            if (sub.is_changed)
            {
                if (MessageBox.Show(UhaSub.Properties.Resources.msg_app_end, "", MessageBoxButton.YesNo)
                   == MessageBoxResult.Yes)
                {
                    sub.Save();
                }
            }
        }


        void update_spec()
        {
            col_def1.MinWidth = 450;
            col_def2.MinWidth = 300;

            if(col_def1.Width.Value >(1.6f * col_def2.Width.Value))
            {
                // layout : video > sub
                Grid.SetRowSpan(this.video, 2);
                Grid.SetRowSpan(this.sub, 1);

                Grid.SetColumn(this.spec, 2); // with sub
            }
            else
            {

                // layout : video < sub
                Grid.SetRowSpan(this.sub, 2);
                Grid.SetRowSpan(this.video, 1);

                Grid.SetColumn(this.spec, 0); // with sub
            }
        }



        Config cfg = new Config();


        bool special_start = false;
        
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                // if is editing now, skip
                if (sub.is_editing) return;

                /*
                 * video control
                 */
                if (e.Key == cfg.After) { video.Time += cfg.GoAfterTime;return; }

                if (e.Key == cfg.Before) { video.Time -= cfg.GoBeforeTime;return; }

                if (e.Key == cfg.Pause) { control.Pause(); return; }


                /* 
                 * sub control
                 */
                if (e.Key == cfg.Special)
                {
                    if (special_start) return;

                    sub.Start(video.Time - cfg.StartTime);
                    special_start = true;
                    return;
                }

                if (e.Key == cfg.Start) { sub.Start(video.Time - cfg.StartTime); return; }

                if (e.Key == cfg.End) { sub.End(video.Time - cfg.EndTime); return; }

                if (e.Key == cfg.Save) { sub.Save(); return; }

                if (e.Key == cfg.Up) { sub.Up(); return; }
                if (e.Key == cfg.Down) { sub.Down(); return; }
            }
            catch (Exception)
            {

            }

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == cfg.Special)
                {
                    if (special_start)
                    {
                        sub.End(video.Time - cfg.EndTime);
                        special_start = false;
                        return;
                    }
                }
            }
            catch (Exception)
            {

            }
        }


        /*
         * change size 
         * refer:[Microsoft_Press]_Programming_Windows_6th_Edition (book-270)
         */
        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            // store original size
            col_def1.Width = new GridLength(col_def1.ActualWidth, GridUnitType.Star);
            col_def2.Width = new GridLength(col_def2.ActualWidth, GridUnitType.Star); 
        }



        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            // compute new size
            double newWidth1 = Math.Max(0, col_def1.Width.Value + e.HorizontalChange); 
            double newWidth2 = Math.Max(0, col_def2.Width.Value - e.HorizontalChange);
            
            // set new size
            col_def1.Width = new GridLength(newWidth1, GridUnitType.Star);
            col_def2.Width = new GridLength(newWidth2, GridUnitType.Star); 
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            UhaSub.Properties.Settings.Default.cfg_col_size_1 = col_def1.Width.Value + e.HorizontalChange;
            UhaSub.Properties.Settings.Default.cfg_col_size_2 = col_def2.Width.Value + e.HorizontalChange;

            UhaSub.Properties.Settings.Default.Save();

            update_spec();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.sub.subs.CommitEdit();
        }


    }
}
