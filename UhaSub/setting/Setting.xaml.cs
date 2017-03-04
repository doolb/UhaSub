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
using System.Windows.Shapes;

namespace UhaSub.setting
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();

            host.Children.Add(ui);
        }

        
        UI ui = new UI();
        Key key = new Key();
        Sub sub = new Sub();

        private void OnNavigate(object sender, RoutedEventArgs e)
        {
            host.Children.Clear();
            Button btn = sender as Button;
            switch (btn.Tag as string)
            {
                case "ui": host.Children.Add(ui); return;
                case "key": host.Children.Add(key); return;
                case "sub": host.Children.Add(sub); return;
                default: break;
            }
        }
    }
}
