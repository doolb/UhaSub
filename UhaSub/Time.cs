using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UhaSub
{
    /*
     * custom time
     * as 1ms
     */
    public class Time
    {
        // time as ms
        private long time;

        public override string ToString()
        {
            return TimeSpan.FromMilliseconds(time).
                ToString(@"h\:mm\:ss\.ff");
        }

        public Time()
        {
            time = 0;
        }

        public Time(long ms)
        {
            time = ms;
        }

        /*
         * override compare
         */
        public static bool operator ==(Time t1, Time t2)
        {
            if (t1.time == t2.time)
                return true;
            return false;
        }
        public static bool operator !=(Time t1, Time t2)
        {
            if (t1.time != t2.time)
                return true;
            return false;
        }

        /* 
         * implicit convert
         * refer:http://stackoverflow.com/questions/4537803/overloading-assignment-operator-in-c-sharp
         */
        public static implicit operator Time(long value)
        {
            return new Time(value);
        }

        public static implicit operator long(Time value)
        {
            if (value.Equals(null))
                return 0;
            return value.time; 
        }

        public static Time Parse(string s)
        {
            TimeSpan t = TimeSpan.Parse(s);
            Time tm = (long)t.TotalMilliseconds;

            return tm;
        }
    }
}
