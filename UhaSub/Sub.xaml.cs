using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        public bool is_editing = false;

        void subs_Loaded(object sender, RoutedEventArgs e)
        {
           // locate();

        }

        private string subFileName= null;  // sub file name will used to store
        public string SubFileName
        {
            get { return subFileName; }
        }

        string SubHeader=null;

        /*
         * locate to no 0 row
         */
        void locate()
        {
            /* 
             * locate to row which end-time is zero
             */
            for (int i = 0; i < subs.Items.Count; i++)
            {
                Ass ass = subs.Items[i] as Ass;
                if (ass == null)
                    break;

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

        void check()
        {
            Ass ass = subs.SelectedItem as Ass;

            // check for start > end
            if (ass.Start > ass.End)
            {
                ass.Error = 3;
                return;
            }

            // check too short or long
            // 10 sec or 100 ms
            long dis = ass.End - ass.Start;
            if(dis >= UhaSub.Properties.Settings.Default.cfg_sub_max 
                || dis <= UhaSub.Properties.Settings.Default.cfg_sub_min)
            {
                ass.Error = 2;
                return;
            }
    
            // check overlap
            if(subs.SelectedIndex>0)
            {
                Ass bf = subs.Items[subs.SelectedIndex - 1] as Ass;
                if(ass.Start < bf.End)
                {
                    ass.Error = 1;
                    return;
                }
            }

            // no error
            ass.Error = 0;
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
            Ass ass =subs.SelectedItem as Ass;
            if(ass ==null) return;

            if (time < 0)
                time = 0;


            ass.Start = time;

            check();
            subs.Items.Refresh();
        }

        public void End(long time)
        {
            Ass ass = subs.SelectedItem as Ass;
            if (ass == null) return;


            if (time < 0)
                time = 0;


            ass.End = time;

            check();

            subs.SelectedIndex += 1;    // go to next line
            subs.Items.Refresh();
        }

        
        public void Save()
        {
            if (SubFileName == null)
                return;

            // load a default head
            if (SubHeader == null)
                SubHeader = UhaSub.Properties.Settings.Default.AssHeader;

            Ass.Save(subs.ItemsSource as List<Ass>,SubHeader, SubFileName);

            (this.Resources["stb_save_success"] as Storyboard).Begin();
        }

        public void SaveAs()
        {
            var sd = new SaveFileDialog();
            sd.Filter = "Ass files (*.ass)|*ass|All files (*.*)|*.*";

            if (sd.ShowDialog() != true)
                return;

            // load default head
            if (SubHeader == null)
                SubHeader = UhaSub.Properties.Settings.Default.AssHeader;

            Ass.Save(subs.ItemsSource as List<Ass>, SubHeader, sd.FileName) ;

            (this.Resources["stb_save_success"] as Storyboard).Begin();
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

            subFileName = s.ToString();
        }

        void LoadFile()
        {

            // try to open sub file
            if (!File.Exists(SubFileName))
            {
                var r =MessageBox.Show(UhaSub.Properties.Resources.SubFileNoFound," ",MessageBoxButton.OKCancel);

                if (r != MessageBoxResult.OK)
                    return;

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
            if (subs.SelectedIndex == 0 || subs.SelectedIndex == -1)
                return;

            subs.SelectedIndex -= 1;
            this.subs.ScrollIntoView(this.subs.SelectedItem);
            
        }

        // select next item
        public void Down()
        {
            if (subs.SelectedIndex == subs.Items.Count  - 1)
                return;

            subs.SelectedIndex += 1;
            this.subs.ScrollIntoView(this.subs.SelectedItem);

        }

        /*
         * show-sub callback
         * refer:http://stackoverflow.com/questions/1746332/delegates-and-callbacks
         */
        public delegate void SubCallBackDemo(Ass ass);

        public event SubCallBackDemo SubSelected;

        private void subs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ass ass = this.subs.SelectedItem as Ass;
            if (ass == null) return;
            SubSelected(ass);
            this.subs.ScrollIntoView(this.subs.SelectedItem);

        }

        private void subs_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            this.is_editing = true;
        }

        private void subs_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            this.is_editing = false;
        }

        
       

    }



}
