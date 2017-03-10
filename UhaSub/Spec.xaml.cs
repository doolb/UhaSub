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
            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += timer_Tick;
            old = DateTime.Now;


            this.view.ScrollToEnd();
        }

        double      width=0;

        DateTime    old;
        double      scroll_per_ms=0;
        double      offset=0;
        void timer_Tick(object sender, EventArgs e)
        {
            if (scroll_per_ms == 0) return;

            // compute the delta time
            int delta = (int)(DateTime.Now - old).TotalMilliseconds;
            
            //this.fps.Text = (1000/delta).ToString();

            offset += delta * scroll_per_ms;


            Canvas.SetLeft(this.tspec, offset);


            /*
             * if scroll to right end
             * then stop
             */
            if (this.view.HorizontalOffset == this.view.ScrollableWidth &&
                offset >= width)
                timer.Stop();


            if (offset > width)
            {
                // one page end
                this.view.PageRight();
                offset = 0;
            }

            old = DateTime.Now;
        }

        public void Sync(long max_time)
        {
            this.scroll_per_ms = view.ScrollableWidth / max_time;
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

        string ffmpeg_arg = "-i \"{0}\" -filter_complex showspectrumpic=s={1}x100:color=fruit:scale=sqrt \"{2}\"";
        string ffmpeg_path;

        static int sx = 48000;

        DispatcherTimer timer;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.width = e.NewSize.Width;
            this.tspec.Height = e.NewSize.Height;
        }


        public async void Open(string path)
        {
            string img = null;

            load.Visibility = Visibility.Visible;
            /*
             * use task for long time run
             * refer:http://stackoverflow.com/questions/27089263/running-and-interacting-with-a-async-task-from-a-wpf-gui
             */
            await Task.Run(() => img = Load(path));
            
            ispec.Source = new BitmapImage(new Uri(img));
            load.Visibility = Visibility.Hidden;

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
            Start(arg);

            return s;
        }

        /*
         * start ffmpeg
         * refer:https://jasonjano.wordpress.com/2010/02/09/a-simple-c-wrapper-for-ffmpeg/
         */
        bool Start(string param)
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
    }
}
