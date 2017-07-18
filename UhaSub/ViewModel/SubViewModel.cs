using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Setting = UhaSub.Properties.Settings;

namespace UhaSub.ViewModel
{
    public class SubViewModel : EmptyViewModel
    {
        private int selectIndex;

        public int SelectedIndex
        {
            get { return selectIndex; }
            set
            {
                selectIndex = value;
                RaisePropertyChanged("SelectedIndex");
                RaisePropertyChanged("SelectedItem");

                if (SelectedItem != null)
                {
                    AssText = SelectedItem.Text;
                    RaisePropertyChanged("AssText");
                }
            }
        }
        public string AssHead { get; set; }

        public string FileName { get; set; }

        public List<Ass> AssList{get;set;}

        private string VideoFileName;

        public Ass SelectedItem
        {
            get {
                if (SelectedIndex == -1 || SelectedIndex >= AssList.Count) return null;
                return AssList[SelectedIndex]; }
            set { SelectedIndex = AssList.FindIndex(x => x.ID == value.ID); }
        }

        public string AssText { set; get; }

        public SubViewModel()
        {
            AssList = Ass.Load();

            SelectedIndex = 0;
        }

        #region open function
        public void Open(string video , string sub)
        {
            if (video != null)
            {
                VideoFileName = video;
            }

            string[] ss = sub.Split('.');

            switch (ss.Last())
            {
                case "txt":
                    // load txt 
                    AssList = Ass.LoadTxt(sub);
                    // load default header
                    AssHead = UhaSub.Properties.Settings.Default.AssHeader;
                    break;

                case "ass":
                    string head = null;
                    AssList = Ass.LoadAss(sub, ref head);
                    AssHead = head;
                    break;

                default:
                    MessageBox.Show(UhaSub.Properties.Resources.FileNoSupport);
                    break;
            }

            RaisePropertyChanged("AssList");
            // locate to no 0 line
            //locate();
        }
        public void Open(string path, bool has_time_line = true, bool unicode = false, int code_page = 0)
        {
            string head = null;

            if(has_time_line)
            {
                AssList = Ass.LoadAss(path, ref head, unicode, code_page);
                AssHead = head;
            }
            else
            {
                AssList = Ass.LoadTxt(path,unicode,code_page);
                AssHead = Setting.Default.AssHeader;
            }
            RaisePropertyChanged("AssList");

        }

        #endregion
    }
}
