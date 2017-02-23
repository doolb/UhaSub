using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for Video.xaml
    /// </summary>
    public partial class Video : UserControl
    {
        public Video()
        {
            InitializeComponent();
        }

        bool Ex = true;

        bool useEx(string uri)
        {
            if (uri.Contains("mkv"))
            {
                Ex = true;
                mePlayerex.Visibility = Visibility.Visible;
                return true;
            }
            Ex = false;
            return false;
        }


        public Uri Source 
        {
            get
            {
                if (Ex)
                {
                    return mePlayerex.Source;
                }
                return mePlayer.Source;
            }
            set 
            {
                if (useEx(value.OriginalString)) 
                    mePlayerex.Source = value;
                else 
                    mePlayer.Source = value;

            } 
        }

        public decimal Position
        {
            get
            {
                if (Ex)
                {
                    return mePlayerex.Position;
                }
                return (decimal)mePlayer.Position.TotalSeconds;
            }
            set
            {
                if (Ex)
                    mePlayerex.Position = value;
                else
                    mePlayer.Position = TimeSpan.FromSeconds((double)value);

            }
        }

        public void Play()
        {
            if (Ex) 
                mePlayerex.Play();
            else
                mePlayer.Play();

        }

        public void Pause()
        {
            if (Ex)
                mePlayerex.Pause();
            else
                mePlayer.Pause();

        }
    }
}
