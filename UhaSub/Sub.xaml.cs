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
    /// Interaction logic for Ass.xaml
    /// </summary>
    public partial class Sub : UserControl
    {
        public Sub()
        {
            InitializeComponent();

            subs.ItemsSource = Ass.Load();
            subs.Loaded += subs_Loaded;
        }

        void subs_Loaded(object sender, RoutedEventArgs e)
        {
            /* 
             * set for last column
             * refer:http://stackoverflow.com/questions/3754825/programatically-set-the-width-of-a-datacolumn-for-use-with-a-datagrid
             */
            subs.Columns.Last().Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            /* 
             * locate to row which end-time is zero
             */
            for(int i=0;i<subs.Items.Count;i++)
            {
                Ass ass = subs.Items[i] as Ass;
                if (ass.End == 0)
                {
                    //subs.Items.MoveCurrentTo(ass);  // set current item
                    subs.SelectedIndex = i;         // select current index
                    break;
                }
            }
            // the is a test
            //(subs.Items.CurrentItem as Ass).End = TimeSpan.FromMilliseconds(3);
            //(subs.SelectedItem as Ass).End = new Time(3);
            //subs.Items.Refresh();

        }
        
        private void subs_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

            /*
             * at now we just support ass
             */
                
            // new item
            Ass ass = e.NewItem as Ass;
            ass.ID = subs.Items.Count - 1;
            ass.Start = 0;
            ass.End = 0;
        }

        public void Start(long time)
        {
            (subs.SelectedItem as Ass).Start = time;
            subs.Items.Refresh();
        }

        public void End(long time)
        {
            (subs.SelectedItem as Ass).End = time;
            subs.SelectedIndex += 1;    // go to next line
            subs.Items.Refresh();
        }

        public void Save()
        {

        }

        // select the before item
        public void Up()
        {
            if (subs.SelectedIndex == 0)
                return;

            subs.SelectedIndex -= 1;
        }

        // select next item
        public void Down()
        {
            if (subs.SelectedIndex == subs.Items.Count -1 )
                return;

            subs.SelectedIndex += 1;
        }
    }
}
