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

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.Tick += new EventHandler(OnProcessViewports);
            dt.Start();
        }

        
        private void OnProcessViewports(object sender, EventArgs e)
        {
            //timeText.Text = DateTime.Now.ToString("HH:mm:ss.fff");

            psText.Text = TimeSpan.FromMilliseconds(video.Time).ToString(@"hh\:mm\:ss\.ff");
            asText.Text = TimeSpan.FromMilliseconds(video.TotalTime).ToString(@"hh\:mm\:ss\.ff");

            if(time_drag_end)
                time.Value = (double)video.Position;

        }

    
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                //case Key.Right: video.Pause(); video.Time += 3000; video.Play(); break;
                //case Key.Left: video.Pause();   video.Time -= 3000; video.Play(); break;

                case Key.Right: video.Time += 3000; break;
                case Key.Left:  video.Time -= 3000; break;
            }
        }

        private void OnOpenFile(object sender, RoutedEventArgs e)
        {

            var fileDialog = new OpenFileDialog();
            fileDialog.Filter =
                "Video files (*.mp4)|*.mp4;*.mkv|All files (*.*)|*.*";

            if (fileDialog.ShowDialog() == true)
            {
                video.Source = new Uri(fileDialog.FileName);    
                video.Play();
 
                asText.Text = TimeSpan.FromMilliseconds(video.TotalTime).ToString().Substring(0, 8);
            }
        }

        /*
         * use slider change play position
         */
        bool time_drag_end = true;
        private void time_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            time_drag_end = true;
            video.Position = (float)time.Value;
        }

        private void time_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            time_drag_end = false;
        }


    }
}
