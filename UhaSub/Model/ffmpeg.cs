using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UhaSub.Model
{
    public class FFmpeg
    {

        /*
         * start ffmpeg
         * refer:https://jasonjano.wordpress.com/2010/02/09/a-simple-c-wrapper-for-ffmpeg/
         */
        public static void Run(string path,string param)
        {
            //create a process info object so we can run our app
            ProcessStartInfo oInfo = new ProcessStartInfo(
                path + "\\ffmpeg.exe",
                param);

            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;

            //so we are going to redirect the output and error so that we can parse the return
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            //Create the output and streamreader to get the output
            string output = null; StreamReader srOutput = null;

            //try the process
            try
            {
                //run the process
                Process proc = System.Diagnostics.Process.Start(oInfo);

                proc.WaitForExit();

                //get the output
                srOutput = proc.StandardError;

                //now put it in a string
                output = srOutput.ReadToEnd();

                proc.Close();
            }
            catch (Exception)
            {
                output = string.Empty;
                throw new FileNotFoundException();
            }
            finally
            {
                //now, if we succeded, close out the streamreader
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }
        }

    }
}
