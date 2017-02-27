using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

            locate();        
        }

        string SubFileName = null;  // sub file name will used to store
        string SubHeader=null;
        
        void locate()
        {
            /* 
             * locate to row which end-time is zero
             */
            for (int i = 0; i < subs.Items.Count; i++)
            {
                Ass ass = subs.Items[i] as Ass;
                if (ass.End == 0)
                {
                    //subs.Items.MoveCurrentTo(ass);  // set current item
                    subs.SelectedIndex = i;         // select current index
                    return ;
                }
            }
            // the is a test
            //(subs.Items.CurrentItem as Ass).End = TimeSpan.FromMilliseconds(3);
            //(subs.SelectedItem as Ass).End = new Time(3);
            //subs.Items.Refresh();


            subs.SelectedIndex = 0;
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

        public async void Save()
        {
            if (SubFileName == null)
                return;
            await Ass.Save(subs.ItemsSource as List<Ass>,SubHeader, SubFileName);
        }

        public void Open(string file_name)
        {
            // compute the correct file name for sub
            GetFileName(file_name);

            // load ass header
            LoadFile();

            
                      
            // locate to no 0 line
            locate();
        }

        public void OpenNewSub(string file_name)
        {
            string[] ss = file_name.Split('.');

            switch(ss.Last())
            {
                case "txt":
                    // load txt 
                    this.subs.ItemsSource = Ass.LoadTxt(file_name);    
                    // load default header
                    SubHeader = UhaSub.Properties.Settings.Default.AssHeader;
                    break;

                case "ass":
                    this.subs.ItemsSource = Ass.LoadAss(file_name, ref SubHeader);
                    break;                

                default:
                    MessageBox.Show(UhaSub.Properties.Resources.FileNoSupport);
                    break;
            }

            // locate to no 0 line
            locate();
        }

        void GetFileName(string file_name)
        {
            /*
             * file-name should be a video file-name
             */

            string[] ss = file_name.Split('.');

            /* 
             * compute the correct file name
             */
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < ss.Length - 1; i++)
            {
                s.Append(ss[i]);
            }
            s.Append(".ass");

            SubFileName = s.ToString();
        }

        void LoadFile()
        {

            // try to open sub file
            if (!File.Exists(SubFileName))
            {
                MessageBox.Show(UhaSub.Properties.Resources.SubFileNoFound);

                // sub file is not exist
                // so need open a txt file 
                var fd = new OpenFileDialog();
                fd.Filter = "Text files (*.txt)|*.txt;*ass|All files (*.*)|*.*";

                if(fd.ShowDialog() == true)
                {
                    OpenNewSub(fd.FileName);
                }
                return;
            }
            

            /*
             * ass file is exist
             * so just open it
             */
            this.subs.ItemsSource = Ass.LoadAss(SubFileName,ref SubHeader);

            locate();
        }

        

        // select the before item
        public void Up()
        {
            if (subs.SelectedIndex == 0 )
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
