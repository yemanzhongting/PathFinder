using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Where;

namespace VertexMatch
{
    /// <summary>
    /// 1.根据行程Trip入站、出站和经过的收费站，查询高速路段
    /// 2.入站和出站的赋予较大的权值（5），其他站点权值为1
    /// 3.搜索完成之后，将权值为1的路段移除（这是误判路段）
    /// </summary>
    class RoadSearch
    {
        /// <summary>
        /// 存储候选路段名、权值
        /// </summary>
        public RoadSet RoadCandinate { get; set; }

        public RoadSearch()
        {
            RoadCandinate = new RoadSet();
        }
        /// <summary>
        /// 根据行程，查找可能的道路
        /// </summary>
        /// <param name="trip"></param>
        public void FindRoad(Trip trip)
        {
            try
            {

                foreach (var toll in trip.Data.Data)
                {
                    foreach (var id in toll.RoadId)
                    {
                        var road = DataLib.RoadData[id];
                        RoadCandinate.Add2(road, toll.Weight, 1);
                    }
                }

            }
            catch (Exception ex)
            {
                string text = $"RoadSearch.cs: {ex.Message} +{trip.ToString()}";
                Logger.WriteError(text);
            }

        }
        
                
    }
}
