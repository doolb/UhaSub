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
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += timer_Tick;
            old = DateTime.Now;


        }




        double      width=0;
        DispatcherTimer timer;


        DateTime    old;
        double      scroll_per_ms=0;
        double      offset = 0, offset_head = 115;
        double      offset_base=0;
        int         page=0;

        void timer_Tick(object sender, EventArgs e)
        {
            if (scroll_per_ms == 0) return;



            // compute the delta time
            double delta = (DateTime.Now - old).TotalMilliseconds;

            if (delta > 100)
                delta = 40;

            //this.fps.Text = (1000/delta).ToString();

            offset += delta * scroll_per_ms;

            Canvas.SetLeft(this.tspec,offset_base + offset);

            /*
             * if scroll to right end
             * then stop
             
            if (IsEnd())
                timer.Stop();
            */

            old = DateTime.Now;
        }

        #region image control
        public void Init(long max_time)
        {
            this.scroll_per_ms = (img.ActualWidth - 2*offset_head) / max_time;


            this.offset = 0;
            Home();

        }

        /*
         * sync with vlc
         */
        public void Sync(long time)
        {
            double ow = time * scroll_per_ms;
            double w = (time+450 + page *30) * scroll_per_ms;

            offset_base = w % width;
            offset = 0;
         
            /*
             * page
             */
            int p = (int)(ow / width);

            if (p > page)
            {
                for (int i = page; i < p;i++)
                    PageDown();
                page = p;
            }
            if (p < page)
            {
                for (int i = p; i < page; i++)
                    PageUp();
                page = p;
            }
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
            //timer.Start();
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


        internal void PageDown()
        {
            // go to next page
            trans.X -= width;

            if(end >0)
                end = -(img.ActualWidth - offset_head) * this.scale.ScaleX + width;

            // is reach to end
            if (trans.X <= end)
                trans.X = end;
        }

        internal void PageUp()
        {
            // go to next page
            trans.X += width;

            // is reach to end
            if (trans.X >= home)
                trans.X = home;
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
        string ffmpeg_arg = "-i \"{0}\" -filter_complex showspectrumpic=s={1}x100:color=fruit:scale=sqrt \"{2}\"";
        string ffmpeg_path;

        public async void Open(string path)
        {
            // stop timer
            this.timer.Stop();

            string img = null;

            load.Visibility = Visibility.Visible;
            /*
             * use task for long time run
             * refer:http://stackoverflow.com/questions/27089263/running-and-interacting-with-a-async-task-from-a-wpf-gui
             */
            await Task.Run(() => img = Load(path));
            
            this.img.Source = new BitmapImage(new Uri(img));
            
            load.Visibility = Visibility.Hidden;
            
            prepared = true;

            Home();

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
        bool StartFFmpeg(string param)
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
                MessageBox.Show(UhaSub.Properties.Resources.msg_ffmpeg_no_found);
                output = string.Empty;
                return false;
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
            return true;
        }

        #endregion

        // change image scale-x
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scale.ScaleX = e.NewValue;

            home  = -offset_head * this.scale.ScaleX;
            end   = -(img.ActualWidth - offset_head) * this.scale.ScaleX + width;

            trans.X = home;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.M:
                    PageDown();
                    break;
                case Key.N:
                    PageUp();
                    break;
            }
        }
    }
}
