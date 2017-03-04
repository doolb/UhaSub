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
    /// Interaction logic for Sub.xaml
    /// </summary>
    public partial class Sub : UserControl
    {
        public Sub()
        {
            InitializeComponent();
        }

        /*
         * update change
         */
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox tb = sender as TextBox;

            int t = int.Parse(tb.Text);

            switch(tb.Tag as string)
            {
                case "start":
                    UhaSub.Properties.Settings.Default.StartTime = t;
                    break;
                case "end":
                    UhaSub.Properties.Settings.Default.EndTime = t;
                    break;

                default: return;
            }
            
            
            
            UhaSub.Properties.Settings.Default.Save();

        }
    }
}
