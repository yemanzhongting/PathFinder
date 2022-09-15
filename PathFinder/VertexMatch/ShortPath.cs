using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Where;
namespace VertexMatch
{
    /// <summary>
    /// 根据起点和终点，查找最短路径
    /// 主要步骤：
    /// 1. 根据起点和终点，圈定搜索区间（向外拓展1度）
    /// 2. 以起点为顶点，将该边打断，形成1~2条边；以终点为顶点，将该边打断，形成1-2条边
    /// 3. 顶点向量准备
    /// 4. 边矩阵准备
    /// 5. 搜索最短路径
    /// 6. 串联从起点到终点的最短路径
    /// </summary>

    public class ShortPath
    {

        private TollStationSet V;
        private EdgeSet E;
        private Size Limit;

        public double ExtendWidth = 0.5;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="extendWidth">向外扩展搜索的半径，以度（°）为单位</param>
        public ShortPath( double extendWidth)
        {
            V = new TollStationSet();
            E = new EdgeSet();
            ExtendWidth = extendWidth;
        }


        /// <summary>
        /// 根据一次记录数据，搜索其所经过的收费站
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Trip FindTrip(Card card)
        {

            var trip = FindPath(card.StartCode, card.EndCode);
            try
            {
                trip.Type = "最短路径";
                trip.Id = card.CarNumber;

                trip.TimeLength = card.TimeLength;
                trip.RecordNum = card.TotalRecord;
                trip.Valid = false;
                //  trip.Length = CoorTrans.GetLength(trip.Start.Position, trip.End.Position);
            }
            catch (Exception ex)
            {
                string text = $"ShortPath.01: {ex.Message} +{card.ToString()}";
                Logger.WriteError(text);
            }
            return trip;
        }


        /// <summary>
        /// 查找从起点收费站到终点收费站的最短路径
        /// </summary>
        /// <param name="startCode">起点收费站编号</param>
        /// <param name="endCode">终点收费站编号</param>
        public Trip FindPath(int startCode, int endCode)
        {
            Trip trip = new Trip();
            try
            {
                PrepareVerties(startCode, endCode);
                PrepareEdges(startCode, endCode);

                int count = V.Count;
                var graph = new Dijkstra(count);
                foreach (var v in V.Data)
                {
                    graph.AddVertex(v.Code);
                }

                foreach (var e in E.Data)
                {
                    graph.AddEdge(e.StartCode, e.EndCode, e.Distance);
                }

                var nodes = graph.FindPath(startCode, endCode);

                // var res = graph.DisplayPaths();

                trip = GetTrip(nodes, startCode, endCode);

            }
            catch (Exception ex)
            {
                string text = $"ShortPath.02: {ex.Message} ;{startCode}-{endCode}";
                Logger.WriteError(text);
            }


            return trip;
        }
        /// <summary>
        /// 串联成为Trip
        /// </summary>
        /// <param name="startCode">起点收费站编号</param>
        /// <param name="endCode">终点收费站编号</param>
        private Trip GetTrip(List<Node> nodes, int startCode, int endCode)
        {
            var res = new Trip();
            try
            {
                res.Start = V[startCode.ToString()];
                res.End = V[endCode.ToString()];
                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    var edge = FindEdge(nodes[i].Code, nodes[i + 1].Code);
                    foreach (var d in edge.TollData)
                    {
                        var toll = DataLib.StationData[d.Code.ToString()];
                        res.Data.Add(toll);
                    }

                }
            }
            catch (Exception ex)
            {
                string text = $"ShortPath.03: {ex.Message} ;{startCode}-{endCode}";
                Logger.WriteError(text);
            }

            return res;
        }
        Edge FindEdge(int startCode, int endCode)
        {
            var edge = new Edge
            {
                StartCode = startCode,
                EndCode = endCode
            };
            try
            {
                for (int i = 0; i < E.Count; i++)
                {
                    if (E.Data[i].StartCode == startCode && E.Data[i].EndCode == endCode)
                    {
                        edge.TollData = E.Data[i].TollData;
                        break;
                    }
                    else if (E.Data[i].StartCode == endCode && E.Data[i].EndCode == startCode)
                    {
                        var tolls = E.Data[i].TollData;
                        tolls.Reverse();
                        edge.TollData = tolls;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string text = $"ShortPath.04: {ex.Message} ;{startCode}-{endCode}";
                Logger.WriteError(text);
            }

            return edge;
        }


        /// <summary>
        /// 获取相关的顶点
        /// </summary>
        /// <param name="startCode">起点收费站编码</param>
        /// <param name="endCode">终点收费站编码</param>
        void PrepareVerties(int startCode, int endCode)
        {
            try
            {
                V = new TollStationSet();
                var start = DataLib.StationData.FindTollStation(startCode);
                var end = DataLib.StationData.FindTollStation(endCode);
               
                Limit = new Size(start.Position, end.Position, ExtendWidth);

                V.Add(start);
                foreach (var d in DataLib.Vdata.Data)
                {
                    if (Limit.Contains(d.Position))
                    {
                        V.Add(d);
                    }
                }
                V.Add(end);
            }
            catch (Exception ex)
            {
                string text = $"ShortPath.05: {ex.Message} ;{startCode}-{endCode}";
                Logger.WriteError(text);
            }

        }

        /// <summary>
        ///获取相关的边
        /// </summary>
        /// <param name="startCode">起点收费站编码</param>
        /// <param name="endCode">终点收费站编码</param>
        void PrepareEdges(int startCode, int endCode)
        {
            try
            {
                E = new EdgeSet();

                foreach (var d in DataLib.Edata.Data)
                {
                    if (d.ContainTollStation(startCode))
                    {
                        var es = d.Split(startCode);
                        //边的端点在搜索范围内容，则加入，否则舍弃
                        foreach (var e in es.Data)
                        {
                            if (CheckEdge(e))
                            {
                                E.Add(e);
                            }
                        }
                    }
                    else if (d.ContainTollStation(endCode))
                    {
                        var es = d.Split(endCode);
                        foreach (var e in es.Data)
                        {
                            if (CheckEdge(e))
                            {
                                E.Add(e);
                            }
                        }

                    }
                    else if (V.Contains(d.StartCode) && V.Contains(d.EndCode))
                    {
                        E.Add(d);
                    }
                }

            }
            catch (Exception ex)
            {
                string text = $"ShortPath.06: {ex.Message} ;{startCode}-{endCode}";
                Logger.WriteError(text);
            }

        }
        /// <summary>
        /// 当边的两个端点都在搜索空间内，返回整，否则返回假
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        bool CheckEdge(Edge edge)
        {
            var start = DataLib.StationData[edge.StartCode.ToString()];
            var end = DataLib.StationData[edge.EndCode.ToString()];
            if (Limit.Contains(start.Position) && Limit.Contains(end.Position))
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
