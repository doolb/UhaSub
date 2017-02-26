using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /* 
         * must set public
         * refer:http://stackoverflow.com/questions/33611419/cant-find-resources-file-in-wpf
         */
        App()
        {


            /* 
             * get localization from app-setting
             * refer:https://social.msdn.microsoft.com/Forums/vstudio/en-US/de49dd67-f9c9-461b-a082-a1dc441b4c9c/wpf-application-settings?forum=wpf
             */
            string l = UhaSub.Properties.Settings.Default.localization;


            /* 
             * localization
             * refer:https://www.tutorialspoint.com/wpf/wpf_localization.htm
             */
            System.Threading.Thread.CurrentThread.CurrentUICulture = 
                new System.Globalization.CultureInfo(l);
            
        } 
    }
}
