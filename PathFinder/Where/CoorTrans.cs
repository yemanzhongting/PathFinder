using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    public class CoorTrans
    {
        private double a;
        // double f;
        private double eccSq;

        public CoorTrans()
        {
            a = 6378137.0;
            //   f = 0.335281066475e-2;
            eccSq = 6.69437999014e-3;
        }

        /// <summary>
        /// 获取卯酉圈半径
        /// </summary>
        /// <param name="B">纬度（以弧度为单位）</param>
        // <returns></returns>        
        public double GetN(double B)
        {
            double W = Math.Sqrt(1 - eccSq * Math.Pow(Math.Sin(B), 2));
            return a / W;
        }
        /// <summary>
        /// convert geodetic to cartesian (ECEF) coordinates
        /// 输入： geodetic lat(deg N), lon(deg E),
        ////       height above ellipsoid (meters)
        /// 输出： X,Y,Z in meters in cartesian (ECEF)  coordinates
        /// </summary>
        public void GeodeticToCartesian(double lat, double lon, double height,
            out double X, out double Y, out double Z)
        {
            double d2r = Math.PI / 180.0;
            lat = lat * d2r;
            lon = lon * d2r;
            double N = GetN(lat);
            double slat = Math.Sin(lat);
            double clat = Math.Cos(lat);

            X = (N + height) * clat * Math.Cos(lon);
            Y = (N + height) * clat * Math.Sin(lon);
            Z = (N * (1 - eccSq) + height) * slat;
        }
        public static double GetLength(Coordinate pt1, Coordinate pt2)
        {
            double length = 0;
            try
            {
                CoorTrans coor = new CoorTrans();
                double x1, y1, z1;
                coor.GeodeticToCartesian(pt1.Lat, pt1.Lon, pt1.Height,
                    out x1, out y1, out z1);

                double x2, y2, z2;
                coor.GeodeticToCartesian(pt2.Lat, pt2.Lon, pt2.Height,
                    out x2, out y2, out z2);
                double dx = x2 - x1;
                double dy = y2 - y1;
                double dz = z2 - z1;

                length = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            }
            catch (Exception ex)
            {
                string text = $"CoorTrans.cs: {ex.Message}";
                Logger.WriteError(text);
            }

            return length;
        }   

    }
}
