using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 车辆行程
    /// 最后解析的目标
    /// </summary>
    public class Trip
    {
        /// <summary>
        /// 车辆牌照
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 起点
        /// </summary>
        public TollStation Start { get; set; }
        /// <summary>
        /// 终点
        /// </summary>
        public TollStation End { get; set; }
        /// <summary>
        /// 直线距离
        /// </summary>
        public double Length { get; set; }

        //记录数值
        public int RecordNum { get; set; }

        /// <summary>
        /// 时间长度，以分钟为单位
        /// </summary>
        public double TimeLength { get; set; }
        /// <summary>
        /// 方位角：终点相对于起点之间的关系
        /// N：北向
        /// NE：东北向
        /// E：东
        /// SE：东南
        /// S：南
        /// SW：西南
        /// W：西
        /// NW：西北
        /// </summary>
        public string Azimuth { get; set; }

        /// <summary>
        /// 该高速路段所包含的收费站点列表
        /// </summary>
        public TollStationSet Data { get; set; }

        public bool Valid { get; set; }

        public string Type { get; set; }
        public Trip()
        {
            Id = string.Empty;
            Data = new TollStationSet();

            Start = new TollStation();
            End = new TollStation();
            Length = 0;
            TimeLength = 0;
            Azimuth = string.Empty;
            Valid = true;
            Type = "道路还原";
        }
        /// <summary>
        /// 向数据集增加一个关键点（收费站等）
        ///  1. 如果不存在，则直接增加
        ///  2. 如果已经存在，则更新坐标
        /// </summary>
        public void Add(TollStation keyPoint)
        {
            try
            {
                if (!Data.Contains(keyPoint))
                {
                    Data.Add(keyPoint);
                }

            }
            catch (Exception ex)
            {
                string text = $"Trip.cs: {ex.Message}--{keyPoint.ToString()}";
                Logger.WriteError(text);
            }

        }
        public void Add2(TollStation keyPoint)
        {
            try
            {
                if (!Data.Contains(keyPoint))
                {
                    Data.Add(keyPoint);
                }

                else
                {
                    int index = Data.Data.LastIndexOf(keyPoint);
                    if (Math.Abs(Data.Count - index) > 1)
                    {
                        Data.Data.Add(keyPoint);
                    }

                }
            }
            catch (Exception ex)
            {
                string text = $"Trip.cs: {ex.Message}--{keyPoint.ToString()}";
                Logger.WriteError(text);
            }

        }
        /// <summary>
        /// 合并行程：将收费站串联到尾端
        /// </summary>
        /// <param name="other"></param>
        public void MergeTrip(Trip other)
        {
            foreach (var toll in other.Data.Data)
            {
                Add2(toll);
            }
        }

        public void Parse(string line)
        {
            try
            {
                TollStation keyPoint = new TollStation();
                keyPoint.Parse(line);
                Add(keyPoint);
            }
            catch (Exception ex)
            {
                string text = $"Trip.cs: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }

        public override string ToString()
        {
            //string check = "√";
            //if (!Valid) check = "×";
            //string res = $"#{Id},{RecordNum}个记录,{TimeLength}分钟,直线距离{Length / 1000:f2}公里,{check},类型：{Type}\n";
            string res = $"#{Id},{RecordNum}个记录,{TimeLength}分钟,类型：{Type}\n";
            //string res = $"#{Id},{RecordNum}个记录,{TimeLength}分钟,{check},类型：{Type}\n";
            res += $"{Start.ToStringCheck()}\n";
            try
            {
                if (Data.Count < 1)
                {
                    res += "!!!数据不完整，无法进行路径还原，非常抱歉。\n";
                }
                foreach (var d in Data.Data)
                {
                    if (!(d.Code == Start.Code || d.Code == End.Code))
                    {
                        res += d.ToStringCheck() + "\n";
                    }

                }
                res += End.ToStringCheck();

            }
            catch (Exception ex)
            {
                string text = $"Trip.cs: {ex.Message}--{res}";
                Logger.WriteError(text);
            }
            return res;
        }
    }
}
