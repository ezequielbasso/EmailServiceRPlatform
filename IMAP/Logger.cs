using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace IMAP
{
   public class Logger
    {
        public string  LogPath { get; set; }
        public Logger(string path)
        {
            LogPath = path;
        }
        public void ToFile(string Message, Exception ex=null)
        {
            string name = "LOG_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".log";
            using (System.IO.StreamWriter writer = new StreamWriter(string.Format("{0}{1}",LogPath, name), true))
            {
                string Line = string.Empty;
               Line = string.Format("[{0}] [{1}] -> {2} ", DateTime.Now.ToLongDateString(), "INFO", Message);
                writer.WriteLine(Line);

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
    }
}
