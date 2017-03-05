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

            this.sub.SubSelected += this.video.OnSubChanged;
            this.video.main = this;

            this.Closed += MainWindow_Closed;

            /*
             * set column width from config
             */
            col_def1.Width = new GridLength(UhaSub.Properties.Settings.Default.cfg_col_size_1, GridUnitType.Star);
            col_def2.Width = new GridLength(UhaSub.Properties.Settings.Default.cfg_col_size_2, GridUnitType.Star);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            audio.Close();    
        }


        Config cfg = new Config();
        bool special_start = false;
        
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // if is editing now, skip
            if (sub.is_editing) return;

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
        public void OnOpenFile(object sender, RoutedEventArgs e)
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

                
                // set title
                this.Title = UhaSub.Properties.Resources.Title + "  -  " +
                    fileDialog.FileName;

            }
        }
        public void OnOpenSub(object sender, RoutedEventArgs e)
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

        public void OnOpenSetting(object sender, RoutedEventArgs e)
        {
            var set = new setting.Setting();
            set.ShowDialog();

            cfg.ReLoad();
        }

        public void OnSaveAs(object sender, RoutedEventArgs e)
        {
            sub.SaveAs();
        }

        public void OnSave(object sender, RoutedEventArgs e)
        {
            sub.Save();
        }



        /*
         * change size 
         * refer:[Microsoft_Press]_Programming_Windows_6th_Edition (book)
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
        }


    }
}
