using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UhaSub
{
    public class Ass
    {
        public int ID { get; set; }
        public Time Start { get; set; }
        public Time End { get; set; }
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
                Start = 0,
                End = 1,
                Text = ""
            });

            list.Add(new Ass
            {
                ID = 2,
                Start = 0,
                End = 0,
                Text = ""
            });

            return list;
        }

        /* 
         * load txt file 
         */
        static public List<Ass> LoadTxt(string file_name)
        {
            List<Ass> list = new List<Ass>();

            StreamReader sr = new StreamReader(file_name);
            // file should existed

            // read file
            for(int i=1;!sr.EndOfStream;i++)
            {
                // read a line from file
                string s = sr.ReadLine();

                list.Add(new Ass
                {
                    ID = i,
                    Start = 0,
                    End = 0,
                    Text = s
                });
            }

            sr.Close();

           
            return list;
        }

        /* 
         * load ass file 
         */
        static public List<Ass> LoadAss(string file_name,ref string header)
        {
            /*
             * ass general format
             * [Events]
                Format: Layer, Start, End, Style, Actor, MarginL, MarginR, MarginV, Effect, Text
             */
            List<Ass> list = new List<Ass>();


            return list;
        }
        

    }

    
}
