using System;
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
            timer = new DispatcherTimer(DispatcherPriority.DataBind);
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            old = DateTime.Now;


        }




        double      width=0;
        DispatcherTimer timer;


        DateTime    old;
        double      scroll_per_ms=0;
        double      offset = 0, offset_head = 115;


        void timer_Tick(object sender, EventArgs e)
        {
            if (scroll_per_ms == 0||
                double.IsInfinity(scroll_per_ms)) return;

            // compute the delta time
            double delta = (DateTime.Now - old).TotalMilliseconds;

            //this.fps.Text = (1000/delta).ToString();

            // calc offset
            offset += delta * scroll_per_ms;
            if (offset > width)
                offset = width;

            Canvas.SetLeft(this.tspec, offset);


            old = DateTime.Now;
        }

        long max_time = 0;

        #region image control

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
        public void Sync(long time)
        {

            double w = time * scroll_per_ms;

            // compute offset
            offset = w % width;

            //Canvas.SetLeft(this.tspec, offset_base);

            // compute page
            int p = (int)(w / width);
            trans.X = home - p * width;
        }

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

        private void ViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.width = e.NewSize.Width;
            this.tspec.Height = e.NewSize.Height;

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

        // change image scale-x
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scale.ScaleX = e.NewValue;

            home  = -offset_head * this.scale.ScaleX;
            end   = -(img.ActualWidth - offset_head) * this.scale.ScaleX + width;

            calc_scroll_per_ms();

            End();
        }

    }
}
