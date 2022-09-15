using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 坐标
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// 观测点数目
        /// </summary>
        
        public double Lon { get; set; }

        public double Lat { get; set; }

        public double Height { get; set; }

        public Coordinate()
        {
            Lon = 0;
            Lat = 0;
            Height = 0;
        }

        public bool Valid()
        {
            return Valid(Lon, Lat, Height);
        }

        bool Valid(double lon, double lat, double height)
        {
            if (lon < Configure.MinLon || lon > Configure.MaxLon)
            {
                return false;
            }
            if (lat < Configure.MinLat || lat > Configure.MaxLat)
            {
                return false;
            }
            if (height < Configure.MinHeight || height > Configure.MaxHeight)
            {
                return false;
            }
           return true;
        }
       

        public override string ToString()
        {
            string line = $"{Lon:f6},{Lat:f6},{Height:f2}";
            return line;
        }
    }
}
