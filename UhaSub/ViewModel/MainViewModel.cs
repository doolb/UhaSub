using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UhaSub.Model;
using Setting = UhaSub.Properties.Settings;

namespace UhaSub.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        

        public string VideoFileName { get; set; }


        public MainViewModel()
        {
            // set title
            this.Title = "UhaSub";
        }

        #region Commands

        #region menu--file command
        private ICommand fileCommand;
        public ICommand FileCommand
        {
            get
            {
                return this.fileCommand ??
                    (
                    this.fileCommand = new EasyCommand
                    {
                        ExecuteDelegate = x=>
                        {
                            switch (x as string)
                            {
                                case "Exit":
                                    Application.Current.Shutdown();
                                    return;
                                case "New":
                                    FileNew();
                                    return;
                                

                                default:
                                    return;
                            }
                        }
                    }
                    );
            }
        }
        
        public void FileNew()
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter =
                    "Video files (*.mp4)|*.mp4;*.mkv|All files (*.*)|*.*";

                if (fileDialog.ShowDialog() == true)
                {
                    // set video file name
                    this.VideoFileName = fileDialog.FileName;
                    RaisePropertyChanged("VideoFileName");


                    // set title
                    Title = UhaSub.Properties.Resources.Title + "  -  " +
                        fileDialog.FileName;
                    RaisePropertyChanged("Title");

                }
            }
            catch (Exception)
            { }
        }

        #endregion

        #region menu--setting command
        private ICommand settingCommand;

        public ICommand SettingCommand
        {
            get
            {
                return settingCommand ?? (
                    this.settingCommand = new EasyCommand
                    {
                        ExecuteDelegate = x =>
                            {
                                switch (x as string)
                                {
                                    case "UI":
                                        return;

                                    default:
                                        return;
                                }
                            }
                    }
                    );
            }
        }

        #endregion
        #endregion


    }

}
