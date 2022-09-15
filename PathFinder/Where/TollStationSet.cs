using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 顶点集合
    /// </summary>
    public class TollStationSet
    {
        public List<TollStation> Data { get; set; }

        public TollStationSet()
        {
            Data = new List<TollStation>();
        }
        public bool Valid()
        {
            if (Data.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int Count => Data.Count;

        public bool Contains(TollStation toll)
        {
            return Data.Contains(toll);
        }

        public bool Contains(int code)
        {
            var toll = new TollStation() { Code = code };
            return Contains(toll);
        }
        public int IndexOf(TollStation toll)
        {
            return Data.IndexOf(toll);
        }
        //根据收费站编号获取或者设置值
        public TollStation this[string id]
        {
            get
            {
                int code = Convert.ToInt32(id);
                var toll = new TollStation() { Code = code };
                try
                {
                    if (Data.Contains(toll))
                    {
                        int index = IndexOf(id);
                        toll = Data[index];
                    }
                }
                catch (Exception ex)
                {
                    string text = $"TollStationSet.cs: {ex.Message}--{toll.ToString()}";
                    Logger.WriteError(text);
                }
                return toll;

            }
        }

        /// <summary>
        /// 根据收费站Id，返回其索引
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int IndexOf(string id)
        {
            int code = Convert.ToInt32(id);
            var toll = new TollStation() { Code = code };
            int index = Data.IndexOf(toll);
            return index;

        }
        /// <summary>
        /// 向数据集增加一个关键点（收费站等）
        ///  1. 如果不存在，则直接增加
        ///  2. 如果已经存在,则更新道路列表
        /// </summary>
        public void Add(TollStation toll)
        {
            try
            {

                if (!Data.Contains(toll))
                {
                    Data.Add(toll);
                }
                else
                {
                    int index = Data.IndexOf(toll);
                    foreach (var id in toll.RoadId)
                    {
                        if (!Data[index].RoadId.Contains(id))
                        {
                            Data[index].RoadId.Add(id);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string text = $"TollStationSet.01: {ex.Message}--{toll.ToString()}";
                Logger.WriteError(text);

            }

        }
        /// <summary>
        /// 向数据集增加一个关键点（收费站等）
        ///  1. 如果不存在，则直接增加
        ///  2. 如果已经存在,并且存在的位置距离尾端很近（小于5），则更新道路权重
        /// </summary>
        public void Add2(TollStation toll, int weight)
        {
            try
            {
                if (!Data.Contains(toll))
                {
                    toll.Weight = weight;
                    Data.Add(toll);
                }
                else
                {
                    int index = Data.LastIndexOf(toll);
                    if (Math.Abs(Data.Count - index) < 4)
                    {
                        Data[index].Weight += weight;
                    }
                    else
                    {
                       // toll.Weight = weight;
                        Data.Add(toll);
                    }

                }
            }
            catch (Exception ex)
            {
                string text = $"TollStationSet.02: {ex.Message}--{toll.ToString()}";
                Logger.WriteError(text);

            }

        }

        public void Add2(TollStation toll)
        {
            try
            {
                if (!Data.Contains(toll))
                {
                    Data.Add(toll);
                }
                else
                {
                    int index = Data.LastIndexOf(toll);
                    if (Math.Abs(Data.Count - index) < 3)
                    {
                        Data[index].Weight += toll.Weight;
                    }
                    else
                    {
                        Data.Add(toll);
                    }

                }
            }
            catch (Exception ex)
            {
                string text = $"TollStationSet.03: {ex.Message}--{toll.ToString()}";
                Logger.WriteError(text);

            }

        }

        /// <summary>
        /// 根据收费站编码，找出对应的测站名字
        /// </summary>
        /// <param name="tollCode">收费站编码，如10101</param>
        /// <returns></returns>
        public string FindTollName(int tollCode)
        {

            var res = string.Empty;
            foreach (TollStation t in Data)
            {
                if (t.Code == tollCode)
                {
                    res = t.Name;
                    break;
                }
            }
            return res;

        }

        /// <summary>
        /// 根据收费站编码，找出对应的收费站
        /// </summary>
        /// <param name="tollCode">收费站编码，如10101</param>
        /// <returns>收费站</returns>
        public TollStation FindTollStation(int tollCode)
        {

            var res = new TollStation();
            foreach (TollStation t in Data)
            {
                if (t.Code == tollCode)
                {
                    res = t;
                    break;
                }
            }
            return res;

        }

        public override string ToString()
        {
            string line = string.Empty;
            foreach (var d in Data)
            {
                line += $"{d.ToString()}\n";

            }
            return line;
        }

        /// <summary>
        /// 将道路Id分配到相关收费站
        /// </summary>
        /// <param name="roadId">道路Id</param>
        /// <param name="tollName">收费站名字</param>
        public void UpdateRoadName(string roadId, string tollName)
        {
            try
            {
                var ts = new TollStation();
                ts.Name = tollName;
                ts.Code = Convert.ToInt32(tollName.Substring(0, 5));
                if (Data.Contains(ts))
                {
                    int index = Data.IndexOf(ts);
                    if (!Data[index].RoadId.Contains(roadId))
                    {
                        Data[index].RoadId.Add(roadId);
                    }
                }
            }
            catch (Exception ex)
            {
                string text = $"TollStationSet.cs: {ex.Message}--{roadId}--{tollName}";
                Logger.WriteError(text);
            }

        }
    }
}
