using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    // <summary>
    /// 路线
    /// 【例】
    ///#G7011
    ///90058,十房十漫交界(互通),9.028
    ///11031,张湾,13.935
    ///11032,黄龙,21.337
    ///11033,鲍峡,13.999
    ///11034,鄂陕,0
    ///END
    /// </summary>
    public class Road
    {
        /// <summary>
        /// 高速路段名称
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// 该高速路段所包含的收费站点列表
        /// </summary>
        public List<Node> TollData { get; set; }
        /// <summary>
        /// 道路是否有效
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// 收费站样本权值之和
        /// </summary>
        public int Weight;
        
        //采样的收费站数目
        public int SampleNumber;
        public Road()
        {
            Id = string.Empty;
            TollData = new List<Node>();
            Valid = false;
            Weight = 0;
            SampleNumber = 0;
        }
        /// <summary>
        /// 向数据集增加一个关键点（收费站等）
        ///  1. 如果不存在，则直接增加
        ///  2. 如果已经存在，则忽略
        /// </summary>
        public void Add(Node toll)
        {
            try
            {
                if (!TollData.Contains(toll))
                {
                    TollData.Add(toll);
                }

            }
            catch (Exception ex)
            {
                string text = $"Road.cs: {ex.Message}--{toll.ToString()}";
                Logger.WriteError(text);
            }
        }
        /// <summary>
        ///#G7011
        ///90058,十房十漫交界(互通),0,9.028
        /// </summary>
        /// <param name="line">包含收费站的行</param>
        public void Parse(string line)
        {
            try
            {
                if (line.Contains("#"))
                {
                    line = line.Replace("#", "");
                    Id = line;
                }
                else
                {
                    var node = new Node();
                    node.Parse(line);
                    Add(node);
                }

            }
            catch (Exception ex)
            {
                string text = $"Road.cs: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }
        /// <summary>
        /// 该路段是否包含某一个收费站？
        /// </summary>
        /// <param name="toll">收费站</param>
        /// <returns></returns>
        public bool Contains(TollStation toll)
        {
            Node node = new Node { Code = toll.Code };
            return TollData.Contains(node);
        }

        /// <summary>
        /// 将收费站点列表转换为收费站集合
        /// </summary>
        /// <param name="tollSetLib">收费站集合数据库</param>
        /// <returns></returns>
        public TollStationSet ToTollStationSet(TollStationSet tollSetLib)
        {
            var res = new TollStationSet();
            var toll = new TollStation();
            try
            {
                foreach (var d in TollData)
                {
                    toll = tollSetLib[d.Code.ToString()];
                    res.Add(toll);
                }
            }
            catch (Exception ex)
            {
                string text = $"Road.cs: {ex.Message}--{toll.ToString()}";
                Logger.WriteError(text);
            }

            return res;
        }

        public override string ToString()
        {
            string res = $"#{Id}\n";
            foreach (var d in TollData)
            {
                res += d.ToString() + "\n";
            }
            res += "END";
            return res;
        }
        public static bool operator ==(Road left, Road right)
        {
            if (right != null && (left != null && left.Id == right.Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(Road left, Road right)
        {
            return !(left.Id == right.Id);
        }
        protected bool Equals(Road other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Road)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

    }
}
