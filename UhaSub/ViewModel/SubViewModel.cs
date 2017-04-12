using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UhaSub.ViewModel
{
    public class SubViewModel : EmptyViewModel
    {
        public int SelectedIndex { get; set; }
        public string AssHead { get; set; }

        public string FileName { get; set; }

        public List<Ass> AssList{get;set;}

        private string VideoFileName;

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
        public void Open(string path)
        {
            string head = null;
            AssList = Ass.LoadAss(path, ref head);
            AssHead = head;
            RaisePropertyChanged("AssList");

        }
        #endregion
    }
}
