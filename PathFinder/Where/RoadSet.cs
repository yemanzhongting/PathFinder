using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 路网数据库
    /// 1. 构建路网数据库，通过文件读入
    /// 2. 快速检索一个收费站在那条道路上，例如【房县】在【谷竹高速】
    /// 3. 快速检索若干收费站在那条（那几条）道路上；
    /// 4. 如果存在多条道路，互通点在哪里？
    /// </summary>
    public class RoadSet
    {
        /// <summary>
        /// 道路列表，
        /// </summary>
        public List<Road> RoadData { get; set; }

        public RoadSet()
        {
            RoadData = new List<Road>();
        }

        public Road this[int i]
        {
            get
            {
                return RoadData[i];
            }
            set
            {
                RoadData[i] = value;
            }
        }

        public Road this[string roadId]
        {
            get
            {
                Road r = new Road()
                { Id = roadId };
                int i = IndexOf(r);
                r.Weight = RoadData[i].Weight;
                r.SampleNumber = RoadData[i].SampleNumber;
                r.Valid = RoadData[i].Valid;
                foreach (var toll in RoadData[i].TollData)
                {
                    r.TollData.Add(toll);
                }
                return r;
            }

        }
        /// <summary>
        /// 更新收费站列表数据集合
        /// 主要更新其中收费站所属的道路名称内容
        /// </summary>
        /// <param name="tollSet">收费站数据集合</param>
        public void UpdateTollStationSet(ref TollStationSet tollSet)
        {
            try
            {
                foreach (var road in RoadData)
                {
                    foreach (var toll in road.TollData)
                    {
                        tollSet.UpdateRoadName(road.Id, toll.Name);
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.WriteError(ex.Message);
            }
        }

        public int Count => RoadData.Count;

        public int IndexOf(Road road) => RoadData.IndexOf(road);
        /// <summary>
        /// 增加一条路段
        /// 1. 如果该路段不存在，则增加
        /// 2. 如果存在，并且存在的位置距离尾端很近（小于4），则更新道路权重
        /// </summary>
        /// <param name="road">道路</param>
        public void Add(Road road)
        {
            if (!RoadData.Contains(road))
            {
                RoadData.Add(road);
            }
            else
            {
                int index = RoadData.LastIndexOf(road);
                if (Math.Abs(RoadData.Count - index) < 2)
                {
                    RoadData[index].Weight += road.Weight;
                    RoadData[index].SampleNumber++;
                }
                else
                {
                    RoadData.Add(road);
                }
            }
        }

        public void Add2(Road road, int weight, int sampleNumble)
        {
         
            if (!RoadData.Contains(road))
            {
                road.Weight = weight;
                road.SampleNumber = sampleNumble;
                RoadData.Add(road);
            }
            else
            {
                int index = RoadData.LastIndexOf(road);
                if (Math.Abs(RoadData.Count - index) < 4)
                {
                    RoadData[index].Weight += weight;
                    RoadData[index].SampleNumber+=sampleNumble;
                }
                else
                {
                    road.Weight = weight;
                    road.SampleNumber = sampleNumble;
                    RoadData.Add(road);
                }
            }
        }
    }
}
