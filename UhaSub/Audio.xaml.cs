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

        public bool SetTime(double t)
        {
            if (t < this.viewAudio.ScrollableWidth)
            {
                viewAudio.ScrollToHorizontalOffset(t);
                return true;
            }
            return false;
        }
        public double GetMax()
        {
            return this.viewAudio.ScrollableWidth;
        }
    }
}
