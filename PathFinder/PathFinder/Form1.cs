using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Where;
using VertexMatch;

namespace PathFinder
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolFindPath_Click(object sender, EventArgs e)
        {

            //string cardFile = $"{Configure.Root}/测试/03.交易数据_10000.csv";
            //var cardList = Reader.ReadCardData1(cardFile);

            //string cardFile = $"{Configure.Root}/测试/路线数据（处理后）.txt";
            //var cardList = Reader.ReadCardData2(cardFile);

            //string cardFile = $"{Configure.Root}/测试/2018年5月.txt";
            string cardFile = $"{Configure.Root}/测试/11.txt";
            var cardList = Reader.ReadCardData3(cardFile);

            var dt1 = DateTime.Now;

            var tr = new TripRoad();
            var triplist = tr.FindAllTrip(cardList);
            Writer.WriteAllTrip(triplist, "trip.txt");

            var dt2 = DateTime.Now;
            var dt = new TimeSpan();
            dt = dt2 - dt1;
            Logger.WriteLog(dt.Milliseconds.ToString());

        }

        private void toolStripTest_Click(object sender, EventArgs e)
        {
            var res = new TollStationSet();
            foreach (var road in DataLib.RoadData.RoadData)
            {
                foreach (var node in road.TollData)
                {
                    var toll = node.ToTollStation();
                    toll.RoadId.Add(road.Id);
                    res.Add(toll);
                }
            }
            //更新坐标
            foreach (var toll in DataLib.StationData.Data)
            {
                if (res.Data.Contains(toll))
                {
                    int index = res.Data.IndexOf(toll);
                    res.Data[index].Name = toll.Name;
                    res.Data[index].Position = toll.Position;
                }
            }
            Writer.WrtieTollStation(res, "收费站.txt");
        }

        private void toolShortPath_Click(object sender, EventArgs e)             
        {
             var path = new ShortPath(0.5);
            // path.FindPath(502, 11521);
            path.FindPath(8103, 90011);
        }
    }
}
