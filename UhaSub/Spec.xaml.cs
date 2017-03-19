﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for Spec.xaml
    /// </summary>
    public partial class Spec : UserControl
    {
        public Spec()
        {
            InitializeComponent();

            // load ffmpeg path
            var currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            ffmpeg_path = currentDirectory;

            /*
             * set a timer
             */
            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += timer_Tick;


        }

        DispatcherTimer timer;
        public Video video;

        #region Sync image and timeline
        double      width=0;
        double      scroll_per_ms=0;
        double      offset = 0, offset_head = 115,offset_base=0;

        void timer_Tick(object sender, EventArgs e)
        {

            if (!prepared || working)
                return;
            
            if (scroll_per_ms == 0||
                double.IsInfinity(scroll_per_ms)) return;

            Sync(video.Time);

            // compute the delta time
            double delta = (DateTime.Now - video.Clock).TotalMilliseconds;
            if (delta > 300) return;

            // calc offset
            offset = delta * scroll_per_ms * video.vlc.MediaPlayer.Rate;
            if (offset_base + offset > width)
                Canvas.SetLeft(this.tspec, width);
            else
                Canvas.SetLeft(this.tspec, offset_base + offset);
        }

        long max_time = 0;
        internal void calc_scroll_per_ms()
        {
            this.scroll_per_ms = (img.ActualWidth - 2 * offset_head) * this.scale.ScaleX / max_time;
        }
        public void Init(long max_time)
        {
            this.max_time = max_time;

            calc_scroll_per_ms();

            this.offset = 0;
            Home();

        }

        /*
         * sync with vlc
         */
        int p_now = -1; // store current page index
        long time_of_page = 0; // time of one page
        bool need_update = false; // is need update image and timeline
        public void Sync(long time)
        {

            if (scroll_per_ms <= 0||
                time_of_page==0)
            {
                calc_scroll_per_ms();
                time_of_page = (long)(width / scroll_per_ms);
            }

            double w = time * scroll_per_ms;

            // compute offset
            offset_base = w % width;

            // compute page
            int p = (int)(w / width);
            if (p != p_now ||
                need_update)
            {
                need_update = false;
                p_now = p;

                // update image
                trans.X = home - p * width;

                UpdateTimeLine();

               
            }
        }

        #region update time-line
        /*
         * update time line
         * refer:https://github.com/tjscience/audion
         */
        int maj_width_step = 100;

        Brush tickBrush;
        Brush timeBrush;
        void add_bottom()
        {
            var bottomBorder = new Border();
            bottomBorder.Height = 1;
            bottomBorder.Background = tickBrush;
            bottomBorder.VerticalAlignment = VerticalAlignment.Bottom;
            bottomBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            time_line.Children.Add(bottomBorder);

        }

        void add_major(double x,long second)
        {

            // Major tick
            var tick = new Border();
            tick.Width = 1; tick.Background = tickBrush;

            tick.VerticalAlignment = VerticalAlignment.Stretch;
            tick.HorizontalAlignment = HorizontalAlignment.Left;
            tick.Margin = new Thickness(x, 0, 0, 0);
            time_line.Children.Add(tick);

            // Add time label
            var t = new TextBlock();
            t.VerticalAlignment = VerticalAlignment.Bottom;
            t.HorizontalAlignment = HorizontalAlignment.Left;
            var ts = TimeSpan.FromSeconds(second);
            t.Foreground = timeBrush;

            t.Text = ts.TotalHours >= 1 ? ts.ToString(@"h\:mm\:ss") : ts.ToString(@"mm\:ss");
            t.Margin = new Thickness(x + 5, 0, 0, 7);
            time_line.Children.Add(t);
        }

        void add_minor(double x)
        {
            // Minor tick
            var tick = new Border();
            tick.Width = 1;
            tick.Height = 7; tick.Background = tickBrush;

            tick.VerticalAlignment = VerticalAlignment.Bottom;
            tick.HorizontalAlignment = HorizontalAlignment.Left;
            tick.Margin = new Thickness(x, 0, 0, 0);
            time_line.Children.Add(tick);
        }
        void UpdateTimeLine()
        {

            time_line.Children.Clear();

            // freeze brushes
            tickBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(UhaSub.Properties.Settings.Default.foreground));
            timeBrush = tickBrush;
            tickBrush.Freeze();
            timeBrush.Freeze();

            // Draw the bottom border
            add_bottom();

            /*
             * is end is zero
             */
            if(p_now == -1)
            {
                add_major(0,0);
                return;
            }

            /*
             * calc major count and minor count
             */
            double start_time = p_now * time_of_page;
            long start_second = (long)Math.Round(start_time / 1000);
            
            long total_second = time_of_page / 1000;
            long maj_interval = (long)Math.Round(maj_width_step / scroll_per_ms / 1000) * 1000;
            long maj_interval_second = maj_interval / 1000;
            double minor_interval = (double)maj_interval / 10;

            // major start x-axis
            double sx = start_time * scroll_per_ms;
            sx %= width;
            if (sx > width * 2 / 3)
                sx = sx - width;

            // major interval x
            double x = maj_interval * scroll_per_ms;
            // minor interval x
            double mx = minor_interval * scroll_per_ms;
            
            /*
             * add major and time
             */
            double tx = sx;
            for(long t =0;t<=total_second;t+=maj_interval_second)
            {
                add_major(tx,start_second + t);

                // add minor
                // calc minor width intervel
                for(long i = 1; i < 10; i++)
                    add_minor(tx + i * mx);

                tx += x;
            }

            /*
             * add minor
             */


        }
        #endregion
        #endregion
        #region image control


        /*
         * play and pause
         */
        public void Pause()
        {
            timer.Stop();
        }
        
        public void Play()
        {
            timer.Start();
        }

        private double home, end;
        // scroll image to home
        internal void Home()
        {
            trans.X = home;
        }

        internal void End()
        {
            trans.X = end ;
        }

        
        #endregion

        // change view size
        private void ViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.width = e.NewSize.Width;
            this.tspec.Height = e.NewSize.Height;

            time_of_page = (long)(width / scroll_per_ms);
            need_update = true;
        }

        // change image scale-x
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scale.ScaleX = e.NewValue;

            home = -offset_head * this.scale.ScaleX;
            end = -(img.ActualWidth - offset_head) * this.scale.ScaleX + width;

            calc_scroll_per_ms();
            time_of_page = (long)(width / scroll_per_ms);

            need_update = true;
        }



        #region load spectrum by ffmpeg
        /*
         * load spectrum for video
         */
        static int sx = 96000;
        public bool prepared = false;
        public bool working = false;
        string ffmpeg_arg = "-i \"{0}\" -filter_complex showspectrumpic=s={1}x100:color=fruit:scale=sqrt \"{2}\"";
        string ffmpeg_path;

        public async void Open(string path)
        {
            working = true;
            // stop timer
            this.timer.Stop();

            string img = null;


            load.Visibility = Visibility.Visible;
            /*
             * use task for long time run
             * refer:http://stackoverflow.com/questions/27089263/running-and-interacting-with-a-async-task-from-a-wpf-gui
             */

            try
            {
                await Task.Run(() => img = Load(path));

                this.img.Source = new BitmapImage(new Uri(img));
            }
            catch (Exception)
            {
                MessageBox.Show(UhaSub.Properties.Resources.msg_ffmpeg_no_found);
            }

            load.Visibility = Visibility.Hidden;


            p_now = -1;
            // update time-line
            UpdateTimeLine();

            prepared = true;
            Home();
            working = false;

            
        }

         /*
         * create an spectrum image for video
         * return the image-file-name
         */
        public string Load(string path)
        {
            /*
             * get the file-name without ext
             */
            string[] ps = path.Split('.');
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<ps.Length-1;i++)
            {
                sb.Append(ps[i]);
            }

            string s = sb.ToString() + ".spec.png";

            // if file exist
            if (File.Exists(s))
                return s;

            /*
             * build ffmpeg arg
             */
            String arg = string.Format(ffmpeg_arg,
                path, sx.ToString(), s);

            // start ffmpet
            StartFFmpeg(arg);



            return s;
        }

        /*
         * start ffmpeg
         * refer:https://jasonjano.wordpress.com/2010/02/09/a-simple-c-wrapper-for-ffmpeg/
         */
        void StartFFmpeg(string param)
        {
            //create a process info object so we can run our app
            ProcessStartInfo oInfo = new ProcessStartInfo(
                ffmpeg_path+"\\ffmpeg.exe", 
                param);

            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;

            //so we are going to redirect the output and error so that we can parse the return
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            //Create the output and streamreader to get the output
            string output = null; StreamReader srOutput = null;

            //try the process
            try
            {
                //run the process
                Process proc = System.Diagnostics.Process.Start(oInfo);

                proc.WaitForExit();

                //get the output
                srOutput = proc.StandardError;

                //now put it in a string
                output = srOutput.ReadToEnd();

                proc.Close();
            }
            catch (Exception)
            {
                output = string.Empty;
                throw new FileNotFoundException();
            }
            finally
            {
                //now, if we succeded, close out the streamreader
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }
        }

        #endregion

        
    }
}
