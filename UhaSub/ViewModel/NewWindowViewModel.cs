using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UhaSub.Model;

namespace UhaSub.ViewModel
{
    public class NewWindowViewModel : EmptyViewModel
    {
        public string VideoFileName { get; set; }

        public string SubFileName { get; set; }

        public string FinalSubFileName { get; set; }

        public bool IsUnicodeCode { get; set; }

        public string SubPreview { get; set; }

        public bool IsCancel = true;

        public NewWindowViewModel()
        {
        }

        #region commands

        private ICommand buttonCommand;
        public ICommand ButtonCommand
        {
            get
            {
                return buttonCommand ??
                (
                    buttonCommand = new Command
                    {
                        ExecuteDelegate = x =>
                        {
                            try
                            {
                                switch(x as string)
                                {
                                    case "OK":
                                        IsCancel = false;
                                        CloseWindow();
                                        return;
                                    case "Cancel":
                                        // close the last window
                                        CloseWindow();
                                        return;
                                    case "Video":
                                        SelectVideo();
                                        return;
                                    case "Sub":
                                        SelectSub();
                                        return;


                                    default:
                                        return;
                                }
                            }
                            catch(Exception)
                            {

                            }
                        }
                    }
                );
            }
        }

        public void SelectVideo()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter =
                "Video files (*.mp4)|*.mp4;*.mkv|All files (*.*)|*.*";
                
            if(fileDialog.ShowDialog() == true)
            {
                this.VideoFileName = fileDialog.FileName;
                RaisePropertyChanged("VideoFileName");
            }
        }

        public void SelectSub()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter =
                "Text or Ass files (*.txt)|*.txt;*.ass|All files (*.*)|*.*";

            if (fileDialog.ShowDialog() == true)
            {
                this.SubFileName = fileDialog.FileName;
                RaisePropertyChanged("SubFileName");
            }

            // compute the final sub file name
            this.FinalSubFileName = this.SubFileName;
            RaisePropertyChanged("FinalSubFileName");

            Preview();
        }

        public void Preview()
        {
            var strb = new StringBuilder();
            StreamReader sr = new StreamReader(this.SubFileName,Encoding.GetEncoding(2312));
            
            // just read ten line
            for (int i = 0; i < 10;i++ )
            {
                if (sr.EndOfStream) break;

                strb.AppendLine(sr.ReadLine());
            }


            // read the preview sub file
            this.SubPreview = strb.ToString();
            RaisePropertyChanged("SubPreview");
        }



        internal void CloseWindow()
        {
            Application.Current.Windows[Application.Current.Windows.Count - 1].Close();
        }

        #endregion
    }
}
