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
    public class MainViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }
        public List<CultureInfo> CultureInfos { get; set; }


        public MainViewModel()
        {
            
            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.AppThemes
                                           .Select(a => new AppThemeMenuData() { Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();

            CultureInfos = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).ToList();


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

        #endregion


        #region interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


    }

    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = 
                new SimpleCommand { 
                    CanExecuteDelegate = x => true, 
                    ExecuteDelegate = x => this.DoChangeTheme(x) }); }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);

            // store current accent name, so it can be auto load when applicaion start next time
            Setting.Default.foreground = this.Name;
            Setting.Default.Save();
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }
}
