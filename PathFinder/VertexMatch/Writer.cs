using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Where;

namespace VertexMatch
{
    public class Writer
    {
        
        /// <summary>
        /// 将一次行车记录结果，写出到文件中
        /// </summary>
        /// <param name="trip">一次行车记录</param>
        /// <param name="fileName">输出文件</param>
        public static void WriteTrip(Trip trip, string fileName)
        {
            try
            {
                var writer = new StreamWriter(fileName);
                writer.WriteLine(trip.ToString());
                writer.Close();

            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);
            }
        }

        public static void WriteAllTrip(List<Trip> tripList, string fileName)
        {
            try
            {
                var writer = new StreamWriter(fileName);
                foreach (var trip in tripList)
                {
                    writer.WriteLine(trip.ToString());
                }
                writer.Close();

            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);
            }
        }
        /// <summary>
        /// 将所有的收费站信息写出到文件之中
        /// </summary>
        /// <param name="tollSet"></param>
        /// <param name="fileName"></param>
        public static void WrtieTollStation(TollStationSet tollSet, string fileName)
        {
            string line = string.Empty;
            try
            {
                var writer = new StreamWriter(fileName);
                foreach (var toll in tollSet.Data)
                {
                    line = toll.ToString();
                    writer.WriteLine(line);
                    //Logger.WriteLog(toll.ToString());
                }
                writer.Close();

            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message+line);
            }
        }
    }
}
