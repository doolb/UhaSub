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

using Setting = UhaSub.Properties.Settings;

namespace UhaSub.ViewModel
{
    public class NewWindowViewModel : EmptyViewModel
    {
        public string VideoFileName { get; set; }

        public string SubFileName { get; set; }

        public string FinalSubFileName { get; set; }

        // encoding
        public bool IsUnicodeCode { get; set; }

        public bool IsDefaultCode { get; set; }

        public int CodePage { get; set; }
        public List<CodePage> CodePages { get; set; }

        public bool HasTimeLine { get; set; }

        public string SubPreview { get; set; }

        public bool IsCancel = true;

        public NewWindowViewModel()
        {
            IsDefaultCode = true;

            CodePages = Model.CodePage.CodePages;
        }

        #region commands

        #region button command
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
                                        HitOk();
                                        goto case "Cancel";
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

            CalcFinalSubName();
         
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

            
            Preview();
        }

        void CalcFinalSubName()
        {
            // remove direction
            string[] ss = this.VideoFileName.Split('\\','/');
            string[] ns = ss.Last().Split('.','[', ']');
            
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            sb.Append(Setting.Default.sub_name);
            sb.Append(']');
            for (int i = 0; i < ns.Length - 1; i++)
            {
                if (ns[i] == "" || ns[i] == " ") continue;

                sb.Append('[');
                sb.Append(ns[i]);
                sb.Append(']');
            }
            sb.Append(".ass");


            this.FinalSubFileName = sb.ToString();
            RaisePropertyChanged("FinalSubFileName");

        }

        public void Preview()
        {
            if (this.SubFileName == null) return;
            if (!File.Exists(this.SubFileName)) return;

            StreamReader sr;
            
            //detech code-page
            if(CodePage!=0)
                sr = new StreamReader(this.SubFileName,Encoding.GetEncoding(CodePage));
            else if(IsUnicodeCode)
                sr = new StreamReader(this.SubFileName, Encoding.Unicode);
            else
                sr = new StreamReader(this.SubFileName);
                
            var strb = new StringBuilder();

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

        public void HitOk()
        {

        }

        internal void CloseWindow()
        {
            Application.Current.Windows[Application.Current.Windows.Count - 1].Close();
        }

        #endregion

        private ICommand codePageCommand;
        public ICommand CodePageCommand
        {
            get
            {
                return codePageCommand ??
                (
                codePageCommand = new Command
                {
                    ExecuteDelegate = x => Preview()
                }
                );
            }
        }
        #endregion
    }
}
