using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Where
{
    /// <summary>
    /// 记录日志
    /// </summary>
    public class Logger
    {
        static string file;

        static Logger()
        {
            file = "log.txt";
            if (File.Exists(file))
                File.Delete(file);
        }

        public static void WriteLog(string txt)
        {
            var writer = new StreamWriter(file, true);
            writer.WriteLine($"Log: {txt}");
            writer.Close();
        }
        public static void WriteError(string txt)
        {
            var writer = new StreamWriter(file, true);
            writer.WriteLine($"Error: {txt}");
            writer.Close();
        }
    }
}
