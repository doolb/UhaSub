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

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for Audio.xaml
    /// </summary>
    public partial class Audio : UserControl
    {

        public Audio()
        {
            InitializeComponent();
        }


        private double position=0;
        public double Position
        {
            get { return position; }
            set
            {
                if (position == value) return;

                position = value;
                viewAudio.ScrollToHorizontalOffset(value*
                    viewAudio.ScrollableWidth);
            }
        }

        private Uri source=null;
        public Uri Source
        {
            get { return source; }
            set
            {
                if (source == value) return;
                source = value;

                /*
                 * set source
                 * refer:http://stackoverflow.com/questions/350027/setting-wpf-image-source-in-code
                 */
                img.Source = new BitmapImage(value);
            }
        }

        private Video video = null;
        public Video Video
        {
            get { return video; }
            set
            {
                if (value == null) return;
                video = value;
            }
        }

        
    }
}
