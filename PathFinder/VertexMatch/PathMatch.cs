using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Where;
namespace VertexMatch
{
    /// <summary>
    /// 1. 根据收费卡的内容，搜索相应的顶点
    /// 2. 搜索过程中补充时空信息的约束
    /// 3. 补全缺失的数据
    /// 4. 质量检核与标定
    /// 5. 信息预警
    /// 7.方位角约束
    /// </summary>
    public class PathMatch
    {

        /// <summary>
        /// 根据一次记录数据，搜索其所经过的收费站
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Trip FindTrip(Card card)
        {
            var trip = new Trip();
            try
            {
                //准确信息
                trip.Id = card.CarNumber;
                trip.Start = DataLib.StationData.FindTollStation(card.StartCode);
                // res.Start.Valid = true;
                trip.End = DataLib.StationData.FindTollStation(card.EndCode);
                // res.End.Valid = true;
                trip.TimeLength = card.TimeLength;
                trip.RecordNum = card.TotalRecord;
                trip.Valid = card.Valid;
                //计算数据，比较准确
                //trip.Length = CoorTrans.GetLength(trip.Start.Position, trip.End.Position);
                // res.Azimuth = CoorTrans.GetAzimuth(res.Start.Position, res.End.Position);

                //存在较多可能的搜索结果
                trip.Data = FindTollStations(card, trip);

                //对在同一条路上的收费站按照先后顺序排序
                SortStation(ref trip);

                //调整权值
                AdjustWeight(ref trip);

                //利用关键点进行路径还原
                SolutionWithKeyPoint(ref trip);

                //利用交叉点进行路径还原
                //SolutionWithIntersection(ref trip);
            }
            catch (Exception ex)
            {

                string text = $"PathMatch.01: {ex.Message} +{trip.ToString()}+ {card.ToString()}";
                Logger.WriteError(text);
            }

            return trip;
        }
        /// <summary>
        /// 同一条路上，出现顺序的错乱会导致道路冗余；
        /// 因此，需要进行先后顺序排序
        /// 1. 将在（1）同一条道路上、（2）权值小于5的收费站提取出来，形成一个道路片段；
        /// 2. 对道路片段进行排序
        ///    （1）确定前进的方向[增加、或减少，或掉头]
        ///    （2) 将方向错乱的收费站标识为-5
        /// </summary>
        /// <param name="trip">行程</param>
        private void SortStation(ref Trip trip)
        {
            int start = 0;
            int end = 0;
            for (int i = start; i < trip.Data.Count - 1; i++)
            {
                RoadSegment(trip, start, out end);
                if (end - start >= 3)
                {
                    int start0 = start;
                    string roadId = SameRoadSegment(trip, ref start0, ref end);
                    if (roadId != "NONE")
                    {

                        StationFilter(trip, ref start0, ref end, roadId);
                    }
                }
                start = end;
                i = end - 1;
            }

        }

        private void StationFilter(Trip trip, ref int start, ref int end, string roadId)
        {
            int direction = RoadDirection(trip, ref start, ref end, roadId);
            Road r = DataLib.RoadData[roadId];
            var tss = r.ToTollStationSet(DataLib.StationData);
            for (int k = start; k < end; k++)
            {
                int i = tss.IndexOf(trip.Data.Data[k].Code.ToString());
                int j = tss.IndexOf(trip.Data.Data[k + 1].Code.ToString());
                if (direction > 0)
                {
                    if (i > j)
                    {
                        trip.Data.Data[k + 1].Weight = -5;
                    }
                }
                else
                {
                    if (i < j)
                    {
                        trip.Data.Data[k + 1].Weight = -5;
                    }

                }

            }
        }

        /// <summary>
        /// 道路方向统计
        /// </summary>
        /// <param name="trip">行程</param>
        /// <param name="start">起点param>
        /// <param name="end">终点</param>
        /// <param name="roadId">道路名称</param>
        /// <returns></returns>
        private int RoadDirection(Trip trip, ref int start, ref int end, string roadId)
        {
            int res = 0;
            Road r = DataLib.RoadData[roadId];
            var tss = r.ToTollStationSet(DataLib.StationData);
            for (int k = start; k < end; k++)
            {
                int i = tss.IndexOf(trip.Data.Data[k].Code.ToString());
                int j = tss.IndexOf(trip.Data.Data[k + 1].Code.ToString());
                if (i < j)
                {
                    res++;
                }
                else
                {
                    res--;
                }
            }
            return res;

        }

        /// <summary>
        /// 提取收费站之间2个高权点（>=4）之间的道路片段
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void RoadSegment(Trip trip, int start, out int end)
        {
            end = start;
            for (int j = start + 1; j < trip.Data.Count; j++)
            {
                if (trip.Data.Data[j].Weight >= 4)
                {
                    end = j;
                    break;
                }
            }
        }

        /// <summary>
        /// 当有连续3个或者3个以上点在共同道路时，获取该道路片段中的共同道路部分
        /// </summary>
        /// <param name="trip">行程</param>
        /// <param name="start">片段开始的收费站</param>
        /// <param name="end">片段结束的收费站</param>
        private string SameRoadSegment(Trip trip, ref int start, ref int end)
        {
            string res = "NONE";
            int end0 = end;
            for (int i = start + 1; i < end0; i++)
            {
                string id = SameRoad2(trip.Data.Data[i - 1], trip.Data.Data[i], trip.Data.Data[i + 1]);
                if (id != "NONE")
                {
                    if (res == "NONE")
                    {
                        res = id;
                        start = i - 1;
                        end = i + 1;
                    }
                    else if (res == id)
                    {
                        end = i + 1;
                    }
                }

            }
            return res;
        }

        private string SameRoad2(TollStation d0, TollStation d1, TollStation d2)
        {
            string res = "NONE";
            foreach (var id1 in d0.RoadId)
            {
                foreach (var id2 in d1.RoadId)
                {
                    foreach (var id3 in d2.RoadId)
                    {
                        if (id1 == id2 && id2 == id3)
                        {
                            res = id1;
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 连续3个点有共同道路时，则各个点的权值+1
        /// </summary>
        /// <param name="trip">行程</param>
        private void AdjustWeight(ref Trip trip)
        {
            for (int i = 1; i < trip.Data.Count - 2; i++)
            {
                var d0 = trip.Data.Data[i - 1];
                var d1 = trip.Data.Data[i];
                var d2 = trip.Data.Data[i + 1];


                if (SameRoad(d0, d1, d2))
                {
                    //获得d0，d1，d2在道路上的索引，根据索引取，
                    //对中间的收费站的权重+1

                    //todo: Sort(d0,d1,d2);

                    //trip.Data.Data[i - 1].Weight++;
                    trip.Data.Data[i].Weight++;
                    //trip.Data.Data[i].Weight++;
                }
            }
        }

        private bool SameRoad(TollStation d0, TollStation d1, TollStation d2)
        {
            bool res = false;
            foreach (var id1 in d0.RoadId)
            {
                foreach (var id2 in d1.RoadId)
                {
                    foreach (var id3 in d2.RoadId)
                    {
                        if (id1 == id2 && id2 == id3)
                        {
                            return true;
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 根据收费卡的内容，搜索相应的顶点
        /// </summary>
        /// <param name="card">收费中所读取的内容</param>
        /// <returns></returns>
        private TollStationSet FindTollStations(Card card, Trip trip)
        {
            var res = new TollStationSet();
            try
            {
                //起点站
                //trip.Start.Weight = 10;
                int weight = 10;
                res.Add2(trip.Start, weight);
                //移动基站反演收费站
                foreach (var cellbase in card.CellBaseList)
                {
                    var ids = DataLib.BufferData.FindTollCodeByCellbase(cellbase, DataLib.GridData);

                    foreach (var id in ids)
                    {
                        var toll = DataLib.StationData[id.ToString()];
                        if (ids.Count > 1)
                        {
                            weight = 1;
                        }
                        else
                        {
                            weight = 4;
                        }
                        // if ((toll.Code != trip.Start.Code) && (toll.Code != trip.End.Code))
                        res.Add2(toll, weight);
                    }
                }
                //终点站
                weight = 10;
                res.Add2(trip.End, weight);
            }

            catch (Exception ex)
            {
                string text = $"PathMatch.02: {ex.Message} +{trip.ToString()}+ {card.ToString()}";
                Logger.WriteError(text);
            }
            return res;
        }

        /// <summary>
        /// 利用关键点的方式进行搜索
        /// 1. 搜索关键点
        /// 2. 根据关键点，依次进行路径搜搜
        ///    方法一： 如果连续的2个点在同一条道路上，则直接获取
        ///    方法二： 如果连续的2个点不在同一条道路上，则通过最短路径获取
        /// </summary>
        /// <param name="trip">收费站集合</param>
        private void SolutionWithKeyPoint(ref Trip trip)
        {
            try
            {
                TollStationSet tollSet = trip.Data;
                var keyPt = FindKeyPointSet(tollSet);
                var res = new Trip();
                for (int i = 1; i < keyPt.Count; i++)
                {
                    Trip subTrip = new Trip();
                    var roadId = CheckRoadName(keyPt.Data[i - 1], keyPt.Data[i]);
                    if (roadId != "NONE")
                    {
                        subTrip = GetPathFromRoad(roadId, keyPt.Data[i - 1], keyPt.Data[i]);
                    }
                    else
                    {
                        double extendWidth = 0.5;
                        ShortPath path = new ShortPath(extendWidth);
                        subTrip = path.FindPath(keyPt.Data[i - 1].Code, keyPt.Data[i].Code);
                    }
                    //合并路径
                    res.MergeTrip(subTrip);
                }

                //标记匹配结果
                CheckRoadMathch(ref trip, res.Data);

                trip.Type = "道路还原：利用高可靠点";
            }
            catch (Exception ex)
            {
                string text = $"PathMatch.03: {ex.Message} +{trip.ToString()}";
                Logger.WriteError(text);
            }

        }

        /// <summary>
        /// 从已有道路上获取路径片段
        /// </summary>
        /// <param name="roadId">道路Id</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private Trip GetPathFromRoad(string roadId, TollStation start, TollStation end)
        {
            Road r = DataLib.RoadData[roadId];
            var tss = r.ToTollStationSet(DataLib.StationData);
            int i = tss.IndexOf(start.Code.ToString());
            int j = tss.IndexOf(end.Code.ToString());

            var res = new Trip();
            if (i < j)
            {
                for (int k = i; k <= j; k++)
                {
                    res.Add(tss.Data[k]);
                }
            }
            else
            {
                for (int k = i; k >= j; k--)
                {
                    res.Add(tss.Data[k]);
                }
            }
            return res;
        }

        /// <summary>
        /// 检测2个收费站是否在同一条路上
        /// </summary>
        private string CheckRoadName(TollStation start, TollStation end)
        {
            string res = "NONE";
            foreach (var id in start.RoadId)
            {
                if (end.RoadId.Contains(id))
                {
                    res = id;
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// 通过设定阈值方式，选出可靠点作为关键点
        /// </summary>
        /// <param name="tollSet">收费站集合</param>
        /// <returns></returns>
        private TollStationSet FindKeyPointSet(TollStationSet tollSet)
        {
            int threshlod = 2;
            var res = new TollStationSet();
            foreach (var d in tollSet.Data)
            {
                if (d.Weight >= threshlod)
                {
                    res.Add2(d);
                }
            }
            return res;
        }
        void CheckRoadMathch(ref Trip trip, TollStationSet tollPass)
        {
            //tollPass.Add(trip.End);
            //途经点检查，如果是候选收费站中的点，则标示为真
            for (int i = 0; i < tollPass.Data.Count; i++)
            {
                //if (tollPass.Data[i].Code == trip.Start.Code)
                //{
                //    tollPass.Data[i].Valid = true;
                //}
                if (trip.Data.Contains(tollPass.Data[i]))
                {
                    tollPass.Data[i].Valid = true;
                }
                else
                {
                    tollPass.Data[i].Valid = false;
                }
                //if (tollPass.Data[i].Code == trip.End.Code)
                //{
                //    tollPass.Data[i].Valid = true;
                //}

            }
            trip.Data = tollPass;
            trip.Start.Valid = true;
            trip.End.Valid = true;
        }

        ///// <summary>
        ///// 基于交叉点的方式进行搜索
        ///// </summary>
        ///// <param name="trip"></param>
        //private void SolutionWithIntersection(ref Trip trip)
        //{
        //    //候选道路
        //    RoadSearch roads = new RoadSearch();
        //    roads.FindRoad(trip);

        //    //道路清洗；
        //    //有效道路条件： 存在2个及其以上的关键点，才是有效道路
        //    //关键点包括： 起点、终点、互通点
        //    CleanRoad1(ref roads);

        //    //将候选道路中不存在收费站移除
        //    CleanTollStation(ref trip, roads);


        //    CleanRoad2(trip, ref roads);

        //    //候选道路排序
        //    //SortRoad(trip, ref roads);

        //    //U形路检查
        //    // CheckUturn(ref trip, road);

        //    //针对候选道路，检核所通过的收费站
        //    RoadMatched(ref trip, roads);

        //    //检查是否存在部分缺失，如果存在，则启动最短路径搜索补全
        //    CheckLackRoad(ref trip);

        //    trip.Type = "道路还原：求取互通点法";
        //}

        ///// <summary>
        ///// 样例数据
        ///// 2040002185,510,2015-01-28 09:05:32.000,13011,2015-01-28 12:34:19.000,蓝鄂A507J9,12,70369635703687F37015188B702A9667702A7F1570147F1F7014A1CE71D206CD71D2072970D223C970D22ED870D234B570D5111370D5221171D5264A71D5262171D127E471D1088F0000
        ///// </summary>
        ///// <param name="trip"></param>
        //private void CheckLackRoad(ref Trip trip)
        //{

        //    int count = trip.Data.Count;

        //    for (int i = 1; i < count - 1; i++)
        //    {
        //        if (trip.Data.Data[i].Code == -1)
        //        {
        //            var path = new ShortPath(0.1);
        //            var subPath = path.FindPath(trip.Data.Data[i - 1].Code, trip.Data.Data[i + 1].Code);

        //            //结果串接
        //            var data = new TollStationSet();
        //            for (int j = 0; j < i; j++)
        //            {
        //                data.Add(trip.Data.Data[j]);
        //            }
        //            foreach (var d in subPath.Data.Data)
        //            {
        //                data.Add(d);
        //            }
        //            for (int j = i + 2; j < count; j++)
        //            {
        //                data.Add(trip.Data.Data[j]);
        //            }
        //            trip.Data = data;
        //            trip.Type = $"部分最短路径搜索:{subPath.Start.Name}--{subPath.End.Name}";
        //            break;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 候选道路排序
        ///// 1.起始站排在第一
        ///// 2.终点站排在最后
        ///// 3.从第一条道路开始，与中间道路必须有互通点
        ///// </summary>
        ///// <param name="trip">行程</param>
        ///// <param name="roads">候选道路</param>
        //private void SortRoad(Trip trip, ref RoadSearch roads)
        //{
        //    try
        //    {
        //        if (roads.RoadCandinate.Count < 2)
        //            return;

        //        RoadSet res1 = new RoadSet();

        //        roads.RoadCandinate[0].Valid = true;

        //        Road road0 = roads.RoadCandinate[0];
        //        res1.Add(road0);
        //        for (int i = 1; i < roads.RoadCandinate.Count; i++)
        //        {
        //            roads.RoadCandinate[i].Valid = false;
        //        }
        //        for (int i = 0; i < roads.RoadCandinate.Count; i++)
        //        {


        //            for (int j = 1; j < roads.RoadCandinate.Count; j++)
        //            {
        //                if (roads.RoadCandinate[j].Valid)
        //                {
        //                    continue;

        //                }
        //                else
        //                {
        //                    var road1 = roads.RoadCandinate[j];
        //                    var intersection = FindIntersection(road0, road1);
        //                    if (intersection.Code != -1)
        //                    {
        //                        road1.Valid = true;
        //                        roads.RoadCandinate[j] = road1;
        //                        res1.Add(road1);
        //                        road0 = road1;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        RoadSet res = new RoadSet();
        //        bool getEnd = false;
        //        //如果最后一条不包含终点站，
        //        for (int i = res1.RoadData.Count - 1; i >= 0; i--)
        //        {
        //            if (getEnd)
        //            {
        //                res.Add(res1.RoadData[i]);
        //            }
        //            else
        //            {
        //                if (res1.RoadData[i].Contains(trip.End))
        //                {
        //                    res.Add(res1.RoadData[i]);
        //                    getEnd = true;
        //                }
        //            }

        //        }

        //        roads.RoadCandinate = new RoadSet();
        //        for (int i = res.RoadData.Count - 1; i >= 0; i--)
        //        {
        //            roads.RoadCandinate.Add(res.RoadData[i]);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string text = $"PathMatch.01: {ex.Message} +{trip.ToString()}+ {roads.ToString()}";
        //        Logger.WriteError(text);
        //    }

        //}

        ///// <summary>
        ///// 第1遍道路清洗；
        /////  除开起点、终点外，其余道路中只有采样点，并且权值比5小的道路
        ///// </summary>
        ///// <param name="roads">候选道路</param>
        //private void CleanRoad1(ref RoadSearch roads)
        //{
        //    try
        //    {
        //        var data = new RoadSet();

        //        foreach (var d in roads.RoadCandinate.RoadData)
        //        {

        //            if (!(d.SampleNumber < 2 && d.Weight < 10))
        //            {
        //                data.Add(d);
        //            }

        //        }
        //        roads.RoadCandinate.RoadData = data.RoadData;

        //    }
        //    catch (Exception ex)
        //    {
        //        string text = $"PathMatch.02-1: {ex.Message} + {roads.ToString()}";
        //        Logger.WriteError(text);
        //    }


        //}

        ///// <summary>
        ///// 道路清洗；
        ///// 有效道路条件： 存在2个及其以上的关键点，才是有效道路
        ///// 关键点包括： 起点、终点、互通点
        ///// </summary>
        ///// <param name="trip">行程</param>
        ///// <param name="roads">候选道路</param>
        //private void CleanRoad2(Trip trip, ref RoadSearch roads)
        //{
        //    try
        //    {
        //        var data = new RoadSet();

        //        //检查每条道路的关键点：开始点、结束点、交叉点
        //        //有2个关键点，才是有效点
        //        data = CheckKeyPoint(trip, roads.RoadCandinate);
        //        //再排除一遍
        //        //data = CheckKeyPoint(trip, data);

        //        roads.RoadCandinate = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        string text = $"PathMatch.02-2: {ex.Message} +{trip.ToString()}+ {roads.ToString()}";
        //        Logger.WriteError(text);
        //    }


        //}

        //private RoadSet CheckKeyPoint(Trip trip, RoadSet data)
        //{
        //    var res = new RoadSet();
        //    try
        //    {
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            int count = 0;
        //            if (data[i].Contains(trip.Start))
        //            {
        //                count++;
        //            }
        //            if (data[i].Contains(trip.End))
        //            {
        //                count++;
        //            }
        //            for (int j = i - 1; j < data.Count; j++)
        //            {
        //                if (j == i || j < 0)
        //                {
        //                    continue;

        //                }
        //                //如果是同一条路，则返回这条路
        //                if (data[i].Id == data[j].Id)
        //                {
        //                    res.Add(data[i]);
        //                    i = j - 1;
        //                    break;
        //                }
        //                else
        //                {
        //                    var intersection = FindIntersection(data[i], data[j]);
        //                    if (intersection.Code != -1)
        //                    {
        //                        count++;
        //                    }
        //                }

        //                if (count > 1)
        //                {
        //                    break;
        //                    i = j - 1;
        //                }

        //            }
        //            if (count > 1)
        //            {
        //                res.Add(data[i]);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string text = $"PathMatch.03: {ex.Message} +{trip.ToString()}+ {data.ToString()}";
        //        Logger.WriteError(text);
        //    }

        //    return res;
        //}

        ///// <summary>
        ///// 对trip中收费站进行排查，将没有出现在roads中的收费站清除
        ///// </summary>
        ///// <param name="roads">候选道路</param>
        ///// <param name="trip">行程数据</param>
        //private void CleanTollStation(ref Trip trip, RoadSearch roads)
        //{
        //    try
        //    {
        //        List<int> tollSet = new List<int>();
        //        foreach (var road in roads.RoadCandinate.RoadData)
        //        {
        //            foreach (var t in road.TollData)
        //            {
        //                tollSet.Add(t.Code);
        //            }
        //        }
        //        var data = new TollStationSet();
        //        foreach (var d in trip.Data.Data)
        //        {
        //            if (tollSet.Contains(d.Code))
        //            {
        //                data.Add2(d, d.Weight);
        //            }
        //        }
        //        trip.Data = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        string text = $"PathMatch.04: {ex.Message} +{trip.ToString()}+ {roads.ToString()}";
        //        Logger.WriteError(text);
        //    }

        //}

        ///// <summary>
        ///// 针对候选道路，检核所通过的收费站
        ///// </summary>
        ///// <param name="trip">行程</param>
        ///// <param name="roads">候选道路信息</param>
        //private void RoadMatched(ref Trip trip, RoadSearch roads)
        //{
        //    try
        //    {
        //        var tollPass = new TollStationSet();
        //        // tollPass.Add(trip.Start);
        //        //如果只有1条路径，则根据开始收费站、截止收费站，根据路段先后顺序，写出结果
        //        if (roads.RoadCandinate.Count == 1)
        //        {


        //            var road = roads.RoadCandinate[0];
        //            var tolls = RoadMatch(road, trip.Start, trip.End);
        //            foreach (var toll in tolls.Data)
        //            {
        //                tollPass.Add(toll);
        //            }
        //            //OneRoadMatch(ref trip, road);


        //        }
        //        ////有2条路
        //        ////1.判断互通点
        //        ////2.收费站搜索
        //        //else if (roads.RoadCandinate.Count == 2)
        //        //{
        //        //    //搜索互通点
        //        //    var road0 = roads.RoadCandinate[0];
        //        //    var road1 = roads.RoadCandinate[1];

        //        //    TwoRoadMatch(ref trip, road0, road1);
        //        //}
        //        //超过1条路
        //        else if (roads.RoadCandinate.Count > 1)
        //        {
        //            //计算交叉点
        //            List<TollStation> keyPt = FindKeyPoints(trip, roads);

        //            for (int i = 0; i < roads.RoadCandinate.Count; i++)
        //            {
        //                var road = roads.RoadCandinate[i];
        //                var tolls = RoadMatch(road, keyPt[i], keyPt[i + 1]);
        //                foreach (var toll in tolls.Data)
        //                {
        //                    tollPass.Add(toll);
        //                }
        //                tollPass.Add(keyPt[i + 1]);
        //            }

        //            CheckRoadMathch(ref trip, tollPass);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string text = $"PathMatch.05: {ex.Message} +{trip.ToString()}";
        //        Logger.WriteError(text);
        //    }

        //}


        ///// <summary>
        ///// 计算出搜索路径中的关键节点
        ///// 起点、交叉点、终点
        /////  </summary>
        ///// <param name="trip">行程</param>
        ///// <param name="roads">候选道路</param>
        ///// <returns>关键节点集合</returns>
        //private List<TollStation> FindKeyPoints(Trip trip, RoadSearch roads)
        //{
        //    var res = new List<TollStation>();
        //    res.Add(trip.Start);
        //    for (int i = 0; i < roads.RoadCandinate.Count - 1; i++)
        //    {
        //        var road0 = roads.RoadCandinate[i];
        //        var road1 = roads.RoadCandinate[i + 1];
        //        var intersection = FindIntersection(road0, road1);
        //        res.Add(intersection);
        //    }
        //    res.Add(trip.End);
        //    return res;
        //}


        ///// <summary>
        ///// 找到2条路的交叉点
        ///// </summary>
        ///// <param name="road0">第1条路</param>
        ///// <param name="road1">第2条路</param>
        ///// <returns></returns>
        //private TollStation FindIntersection(Road road0, Road road1)
        //{
        //    var toll = new TollStation();

        //    var ts1 = road0.ToTollStationSet(DataLib.StationData);
        //    var ts2 = road1.ToTollStationSet(DataLib.StationData);
        //    foreach (var t in ts1.Data)
        //    {
        //        if (DataLib.Vdata.Contains(t))
        //        {
        //            if (ts2.Contains(t))
        //            {
        //                toll = t;
        //                break;
        //            }
        //        }
        //    }
        //    return toll;
        //}
        ///// <summary>
        ///// 道路匹配
        ///// </summary>
        ///// <param name="road">候选道路</param>
        ///// <param name="start">起点站</param>
        ///// <param name="end">终点站</param>
        ///// <returns>经过的收费站</returns>
        //private TollStationSet RoadMatch(Road road, TollStation start, TollStation end)
        //{
        //    var tollRef = road.ToTollStationSet(DataLib.StationData);
        //    var tolls = new TollStationSet();
        //    try
        //    {
        //        int startId = start.Code;
        //        int iStart = tollRef.IndexOf(startId.ToString());
        //        int endId = end.Code;
        //        int iEnd = tollRef.IndexOf(endId.ToString());
        //        if (iStart == -1 || iEnd == -1)
        //        {
        //            return tolls;
        //        }
        //        if (iStart < iEnd)
        //        {
        //            for (int i = iStart + 1; i < iEnd; i++)
        //            {
        //                var t = tollRef.Data[i];
        //                t.Valid = false;
        //                tolls.Add(t);
        //            }
        //        }
        //        else
        //        {
        //            for (int i = iStart - 1; i >= iEnd; i--)
        //            {
        //                var t = tollRef.Data[i];
        //                tolls.Add(t);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string text = $"PathMatch.06: {ex.Message} --{road.ToString()}--{start.ToString()}--{end.ToString()}";
        //        Logger.WriteError(text);
        //    }

        //    return tolls;
        //}



    }
}
