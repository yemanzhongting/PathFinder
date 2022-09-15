using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 
    /// </summary>
    public class Size
    {
        public double MinLon { get; set; }
        public double MinLat { get; set; }
        public double MaxLon { get; set; }
        public double MaxLat { get; set; }

        public Size(Coordinate start, Coordinate end, double eps)
        {
            if (start.Lat < end.Lat)
            {
                MinLat = start.Lat;
                MaxLat = end.Lat;
            }
            else
            {
                MinLat = end.Lat;
                MaxLat = start.Lat;
            }

            if (start.Lon < end.Lon)
            {
                MinLon = start.Lon;
                MaxLon = end.Lon;
            }
            else
            {
                MinLon = end.Lon;
                MaxLon = start.Lon;
            }

            MinLat = MinLat - eps;
            MaxLat = MaxLat + eps;
            MinLon = MinLon - eps;
            MaxLon = MaxLon + eps;
        }
        /// <summary>
        /// 判断某个坐标是否在范围内
        /// </summary>
        /// <param name="coord">坐标</param>
        public bool Contains(Coordinate coord)
        {
            if(coord.Lat<MaxLat && coord.Lat>MinLat &&
                coord.Lon<MaxLon && coord.Lon>MinLon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
