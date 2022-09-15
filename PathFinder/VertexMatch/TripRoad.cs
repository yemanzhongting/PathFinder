using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Where;
namespace VertexMatch
{
    public class TripRoad
    {
        /// <summary>
        /// 找出所有卡片对应的行程
        /// </summary>
        /// <param name="cardList">卡数据集合</param>
        /// <returns>行程集合</returns>
        public List<Trip> FindAllTrip(List<Card> cardList)
        {
            var res = new List<Trip>();
            var trip = new Trip();


            try
            {

                for (int i =0; i < cardList.Count; i++)
                {
                   // var dt1 = DateTime.Now;
                    trip = FindTrip(cardList[i]);

                    //var dt2 = DateTime.Now;
                    //var dt = new TimeSpan();
                    //dt = dt2 - dt1;
                    //Logger.WriteLog(dt.Milliseconds.ToString());
                   
                    //两者对比
                    //var card = cardList[i];
                    //PathMatch pm = new PathMatch();
                    //trip = pm.FindTrip(card);
                    // res.Add(trip);

                    //var path = new ShortPath();
                    //trip = path.FindTrip(card);

                    //加入列表
                    res.Add(trip);
                }
            }
            catch (Exception ex)
            {
                string text = $"TripRoad.cs: {ex.Message} +{trip.ToString()}";
                Logger.WriteError(text);
            }
            return res;
        }
        /// <summary>
        /// 根据一次记录数据，搜索其所经过的收费站
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Trip FindTrip(Card card)
        {
            PathMatch pm = new PathMatch();
            var trip = pm.FindTrip(card);
            if (trip.Data.Count < 1)
            {
                var path = new ShortPath(0.5);
                trip = path.FindTrip(card);
            }
            return trip;
        }
    }
}
