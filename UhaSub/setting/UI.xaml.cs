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

namespace UhaSub.setting
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class UI : UserControl
    {
        public UI()
        {
            InitializeComponent();
        }

        string tag = null;

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            // store tag 
            tag = (string)(sender as Rectangle).Tag;

            /*
             * pick a color
             */
            ColorPicker.MainWindow picker = new ColorPicker.MainWindow();
            picker.Apply += picker_Apply;
            picker.ShowDialog();
        }

        void picker_Apply(object sender, EventArgs e)
        {
            var picker = sender as ColorPicker.MainWindow;
            var clr = picker.GetColor();
            picker.Close();

            // get html type
            string c = clr.ToString();

            /*
             * save color
             */
            switch (tag)
            {
                case "foreground":
                    UhaSub.Properties.Settings.Default.foreground = c;
                    break;
                case "text":
                    UhaSub.Properties.Settings.Default.text = c;
                    break;

                default: return;
            }

            UhaSub.Properties.Settings.Default.Save();
        }

        

    }
}
