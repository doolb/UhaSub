using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UhaSub
{
    public class Config
    {
        /*
         * video control
         */
        private Key pause; // play or pause
        public Key Pause { get { return pause; } }

        private Key before;
        public Key Before { get { return before; } }

        private Key after;
        public Key After { get { return after; } }


        /* 
         * sub control
         */
        private Key start;
        public Key Start { get { return start; } }

        private Key end;
        public Key End { get { return end; } }

        private Key special; // pull down for start and release for end
        public Key Special { get { return special; } }


        private Key up;
        public Key Up { get { return up; } }

        private Key down;
        public Key Down { get { return down; } }



        private Key save;
        public Key Save { get { return save; } }


        public int StartTime;
        public int EndTime;

        public int GoBeforeTime;
        public int GoAfterTime;

        public Config()
        {
            ReLoad();
        }

        public void ReLoad()
        {
            /*
             * key-converter
             * refer:http://stackoverflow.com/questions/6245887/string-to-system-windows-input-key
             */
            KeyConverter kc = new KeyConverter();

            /*
             * sub control
             */
            // set start time 
            start = (Key)kc.ConvertFromString(
                UhaSub.Properties.Settings.Default.sub_start);

            // set end time 
            end = (Key)kc.ConvertFromString(
                UhaSub.Properties.Settings.Default.sub_end);

            // set start-or-end time
            special = (Key)kc.ConvertFromString(
                UhaSub.Properties.Settings.Default.sub_time);
           
            // save sub
            save = (Key)kc.ConvertFromString(
                UhaSub.Properties.Settings.Default.sub_save);

            /*
             * video control
             */
            // go before
            before = (Key)kc.ConvertFromString(
                UhaSub.Properties.Settings.Default.go_before);
            // go after
            after = (Key)kc.ConvertFromString(
                UhaSub.Properties.Settings.Default.go_after);
            // play or pause
            pause = (Key)kc.ConvertFromString(
                UhaSub.Properties.Settings.Default.play_pause);


            // select sub
            // up and down work fine
            up = Key.Up;
            down = Key.Down;

            StartTime = UhaSub.Properties.Settings.Default.StartTime;
            EndTime = UhaSub.Properties.Settings.Default.EndTime;

            GoBeforeTime = UhaSub.Properties.Settings.Default.cfg_video_go_before * 1000;
            GoAfterTime = UhaSub.Properties.Settings.Default.cfg_video_go_after * 1000;
        }
    }
}
