using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UhaSub
{
    public class Ass
    {
        public int ID { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public String   Text{ get; set; }

        /* 
         * create a default ass list
         */
        static public List<Ass> Load()
        {
            List<Ass> list = new List<Ass>();

            list.Add(new Ass
            {
                ID = 1,
                Start = TimeSpan.FromSeconds(0),
                End = TimeSpan.FromSeconds(0),
                Text = ""
            });

            list.Add(new Ass
            {
                ID = 2,
                Start = TimeSpan.FromSeconds(0),
                End = TimeSpan.FromSeconds(0),
                Text = ""
            });

            return list;
        }

        /*
         * load ass from file
         */
        static public List<Ass> Load(string file_name)
        {
            List<Ass> list = new List<Ass>();


            return list;
        }
    }
}
