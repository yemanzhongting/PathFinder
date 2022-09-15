using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 收费站、服务区、互通点等信息
    /// </summary>
    public class TollStation
    {
        //11006洛阳店
        public string Name { get; set; }
        //11006
        public int Code { get; set; }

        /// <summary>
        /// 该收费所在的高速路名
        /// 有时候一个收费站，可能有一条以上的高速关联
        /// 格式如：G59呼北高速
        /// </summary>
        public List<string> RoadId { get; set; }
        //{8,113.43850985,31.4618675,90.19999694824219}
        public Coordinate Position { get; set; }


        /// <summary>
        /// 该收费站的权重
        /// 1.起点站、终点站的权重为10
        /// 2.利用移动基站得到唯一的返回值为5
        /// 3.利用移动基站得到多个收费站，每个收费站的权重为1
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// 是否为有效的站点
        /// </summary>
        public bool Valid { get; set; }
        public TollStation()
        {
            Name = string.Empty;
            Code = -1;
            Position = new Coordinate();
            RoadId = new List<string>();
            Weight = 0;
            Valid = false;
        }

        /// <summary>
        /// 8201,武昌,114.2528223,30.43043415,S11
        /// </summary>
        /// <param name="line">输入信息</param>
        public void Parse(string line)
        {
            try
            {
                var buf = line.Split(',');
                if (buf.Length >= 4)
                {
                    Code = Convert.ToInt32(buf[0]);
                    Name = $"{Code:00000}{buf[1]}";
                    Position.Lon = Convert.ToDouble(buf[2]);
                    Position.Lat = Convert.ToDouble(buf[3]);
                    Position.Height = Convert.ToDouble(buf[4]);
                    for (int i = 5; i < buf.Length; i++)
                    {
                        if (buf[i].Length > 0)
                            RoadId.Add(buf[i]);
                    }
                    Valid = true;
                }
                else if (buf.Length == 1)
                {
                    Name = buf[0];
                    Position.Lon = 0;
                    Position.Lat = 0;
                    Position.Height = 0;
                }

            }
            catch (Exception ex)
            {
                string text = $"TollStation.cs: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }


        public static bool operator ==(TollStation left, TollStation right)
        {
            if (right != null && (left != null && left.Code == right.Code))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(TollStation left, TollStation right)
        {
            return !(left == right);
        }
        protected bool Equals(TollStation other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TollStation)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public string ToStringCheck()
        {
            string check = "N";
            if (Valid)
            {
                check = "Y";
            }

            string res = $"{Name}({check})";
            //if (res.Contains("(互通)"))
            //{
            //    res = res.Replace("(互通)", "");
            //}
            return res;
        }
        public override string ToString()
        {
            string res = Code + ",";
            try
            {
                if (Name.Length > 5)
                    res += Name.Substring(5, Name.Length - 5) + "," + Position.ToString();
                else
                {
                    res += $"{Name},{Position.ToString()},{Weight}";
                }
                foreach (var road in RoadId)
                {
                    res += $",{road}";
                }
            }
            catch (Exception ex)
            {
                string text = $"TollStation.cs: {ex.Message}--{res}";
                Logger.WriteError(text);
            }

            return res;
        }
    }
}
