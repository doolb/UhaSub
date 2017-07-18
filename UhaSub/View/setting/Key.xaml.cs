using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UhaSub.View.setting
{
    /// <summary>
    /// Interaction logic for Key.xaml
    /// </summary>
    public partial class Key : UserControl
    {
        public Key()
        {
            InitializeComponent();
            
            // set timer
            // for 3 second
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += timer_Tick;
        }


        DispatcherTimer timer = new DispatcherTimer();

        // tag for button
        string tag = null;
        // is time_out
        bool time_out = true;
        // store button reference
        Button btn = null;

        SolidColorBrush bk = new SolidColorBrush(Colors.DarkCyan);
        SolidColorBrush obk = new SolidColorBrush(Colors.Gray);

        void timer_Tick(object sender, EventArgs e)
        {
            // time out
            time_out = true;
            // clear button flag
            btn.Background = obk;
            timer.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!time_out)
                return;

            // set background
            btn = sender as Button;
            btn.Background = bk;

            // save tag
            tag = btn.Tag as string;

            // start timer
            time_out = false;
            timer.Start();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (time_out)
                return;

            string s = e.Key.ToString();
            switch(tag)
            {   
                    // video
                case "play_pause":
                    UhaSub.Properties.Settings.Default.play_pause = s;
                    break;
                case "go_before":
                    UhaSub.Properties.Settings.Default.go_before = s;
                    break;
                case "go_after":
                    UhaSub.Properties.Settings.Default.go_after = s;
                    break;

                    // sub
                case "sub_start":
                    UhaSub.Properties.Settings.Default.sub_start = s;
                    break;
                case "sub_end":
                    UhaSub.Properties.Settings.Default.sub_end = s;
                    break;
                case "sub_time":
                    UhaSub.Properties.Settings.Default.sub_time = s;
                    break;
                case "sub_save":
                    UhaSub.Properties.Settings.Default.sub_save = s;
                    break;
                default: break;
            }

            // stop timer
            timer.Stop();
            // call it by hand
            timer_Tick(null, null);
            
            // save config
            UhaSub.Properties.Settings.Default.Save();
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            try
            {
                int t = int.Parse(tb.Text);

                switch (tb.Tag as string)
                {
                    case "after":
                        UhaSub.Properties.Settings.Default.cfg_video_go_after = t;
                        break;
                    case "before":
                        UhaSub.Properties.Settings.Default.cfg_video_go_before = t;
                        break;


                    default: return;
                }
            }
            catch (Exception)
            {

            }
        }



    }
}
