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

        private String Style;

        private int Layer;
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
                Text = "",
                Style="Default",
                Layer=0
            });

            list.Add(new Ass
            {
                ID = 2,
                Start = 0,
                End = 0,
                Text = "",
                Style = "Default",
                Layer = 0
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
                    Text = s,
                    Style = "Default",
                    Layer = 0
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
             *  Dialogue: 0,0:00:56.84,0:00:58.40,Default,,0,0,0,,text
             */
            List<Ass> list = new List<Ass>();

            StreamReader sr = new StreamReader(file_name);

            StringBuilder s = new StringBuilder();

            /* 
             * load header
             */
            while(true)
            {
                string l = sr.ReadLine();
                s.Append(l+"\n");

                if (l == "[Events]")
                {
                    s.Append(sr.ReadLine() + "\n");    // read next line
                    break;
                }
            }

            header = s.ToString();

            /*
             * load content
             */
            for (int i = 1; !sr.EndOfStream; i++)
            {
                // read a line from file
                string l = sr.ReadLine();

                string[] ls = l.Split(',');

                /*
                 * compute the txt field
                 */
                StringBuilder t = new StringBuilder();
                for (int j = 9; j < ls.Length; j++)
                    t.Append(ls[j] + ",");
                t.Length -= 1; // skip the finaly ','

                /*
                 * compute layer id
                 */
                int start = l.IndexOf(':');
                int end = l.IndexOf(',');
                string sl = l.Substring(start + 1,end - start -1 );

                    /*
                     * add to list
                     */
                    list.Add(new Ass
                    {
                        ID = i,
                        Start = Time.Parse(ls[1]),
                        End = Time.Parse(ls[2]),
                        Text = t.ToString(),
                        Style = ls[3],
                        Layer = int.Parse(sl)
                    });
            }

            sr.Close();
            return list;
        }

        public static async Task<bool> Save(List<Ass> list,string head,string path)
        {
            StreamWriter sw = new StreamWriter(path);

            // write header
            await sw.WriteAsync(head);


            /*
             * write content
             *  Dialogue: 0,0:00:56.84,0:00:58.40,Default,,0,0,0,,text
             */
            StringBuilder s = new StringBuilder();
            foreach (Ass ass in list)
            {
                /*
                 * build current line
                 */
                s.Append("Dialogue: ");
                s.Append(ass.Layer.ToString() + ",");
                s.Append(ass.Start.ToString() + ",");
                s.Append(ass.End.ToString() + ",");
                s.Append(ass.Style + ",,0,0,0,,");
                s.Append(ass.Text);

                // write
                await sw.WriteLineAsync(s.ToString());

                s.Clear();
            }

            await sw.FlushAsync();
            sw.Close();
            return true;
        }
        

    }

    
}
