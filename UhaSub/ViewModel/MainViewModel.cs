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
using UhaSub.View;
using Setting = UhaSub.Properties.Settings;

namespace UhaSub.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        

        public string VideoFileName { get; set; }


        public SubViewModel Sub { get; set; }

        public MainViewModel()
        {
            // set title
            this.Title = "UhaSub";

            Sub = new SubViewModel();

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

                                case "Open":
                                    FileOpen();
                                    return;

                                case "Save":

                                    return;

                                case "SaveAs":

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
                var win = new NewWindow();
                var vm = new NewWindowViewModel();
                win.DataContext = vm;
                win.ShowDialog();

                
                if (!vm.IsCancel)
                {
                    // set video file name
                    this.VideoFileName = vm.VideoFileName;
                    RaisePropertyChanged("VideoFileName");


                    // set title
                    Title = UhaSub.Properties.Resources.Title + "  -  " +
                        vm.VideoFileName;
                    RaisePropertyChanged("Title");

                }
            }
            catch (Exception)
            { }
        }

        public void FileOpen()
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter = "ASS files (*.ass)|*.ass";

                if (fileDialog.ShowDialog() == true)
                {
                    Sub.Open(fileDialog.FileName);
                }
            }
            catch
            {

            }
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
                                    case "UI": SettingExe(0,-1);return;
                                    case "Key": SettingExe(1, -1); return;
                                    case "Sub": SettingExe(2, -1); return;
                                    case "About": SettingExe(-1,0); return;

                                    default: SettingExe(0, -1); return;
                                }
                            }
                    }
                    );
            }
        }

        void SettingExe(int index =0,int option_index=0)
        {
            var set = new UhaSub.View.setting.Setting();
            set.DataContext = new ViewModel.SettingViewModel(index,option_index);
            set.ShowDialog();

            UhaSub.Config.Instance.ReLoad();
        }
        #endregion

        #endregion


    }

}
