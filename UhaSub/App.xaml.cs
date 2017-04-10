using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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


        /*
             * make the slider more easy to control
             * refer:https://social.msdn.mcrosoft.com/Forums/vstudio/en-US/5fa7cbc2-c99f-4b71-b46c-f156bdf0a75a/making-the-slider-slide-with-one-click-anywhere-on-the-slider?forum=wpf
             */
        private void thumb_MouseEnter(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed

                && e.MouseDevice.Captured == null)
            {

                // the left button is pressed on mouse enter

                // but the mouse isn't captured, so the thumb

                // must have been moved under the mouse in response

                // to a click on the track.

                // Generate a MouseLeftButtonDown event.

                MouseButtonEventArgs args = new MouseButtonEventArgs(

                    e.MouseDevice, e.Timestamp, MouseButton.Left);

                args.RoutedEvent = UIElement.MouseLeftButtonDownEvent;

                (sender as Thumb).RaiseEvent(args);

            }

        }

    }
}
