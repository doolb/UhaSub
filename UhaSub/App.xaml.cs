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
            // set run-first to true
            //UhaSub.Properties.Settings.Default.RunFirst = true;
            //UhaSub.Properties.Settings.Default.Save();

            if (UhaSub.Properties.Settings.Default.RunFirst)
                RunFirst();


            /* 
             * get localization from app-setting
             * refer:https://social.msdn.microsoft.com/Forums/vstudio/en-US/de49dd67-f9c9-461b-a082-a1dc441b4c9c/wpf-application-settings?forum=wpf
             */
            string l = UhaSub.Properties.Settings.Default.lang;


            /* 
             * localization
             * refer:https://www.tutorialspoint.com/wpf/wpf_localization.htm
             */
            System.Threading.Thread.CurrentThread.CurrentUICulture = 
                new System.Globalization.CultureInfo(l);
            
        }

        
        void RunFirst()
        {
            // set application if the first run

            /*
             * set language
             */
            string l = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            switch(l)
            {
                case "en-US": UhaSub.Properties.Settings.Default.lang= "en"; break;
                case "zh-CN": UhaSub.Properties.Settings.Default.lang= "zh-CN"; break;


                default: UhaSub.Properties.Settings.Default.lang= "en"; break;
            }



            /*
             * update setting
             * refer:https://social.msdn.microsoft.com/Forums/vstudio/en-US/0d86ddc6-83c3-49fd-a478-fbc9b032dc8b/how-to-set-application-setting-in-wpf?forum=wpf
             */
            UhaSub.Properties.Settings.Default.RunFirst = false;
            UhaSub.Properties.Settings.Default.Save();



        }
    }
}
