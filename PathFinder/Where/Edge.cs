using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 两个关键节点之间的一条边
    /// 如：
    /// #501,90047,61.12,G4
    ///501,鄂北,14.44
    ///502,大新,14.63
    ///503,大梧,17.61
    ///90047,大悟南互通,0.00
    ///END
    /// </summary>
    public class Edge
    {
        public int StartCode { get; set; }

        public int EndCode { get; set; }

        public double Distance { get; set; }

        public string Road { get; set; }

        /// <summary>
        /// 该高速路段所包含的收费站点列表
        /// </summary>
        public List<Node> TollData { get; set; }

        public Edge()
        {
            StartCode = -1;
            EndCode = -1;
            Distance = -1;
            Road = String.Empty;
            TollData = new List<Node>();
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
                string text = $"Edge.01: {ex.Message}--{toll.ToString()}";
                Logger.WriteError(text);
            }

        }
        /// <summary>
        /// #501,90047,61.12,G4
        ///501,鄂北,14.44
        /// </summary>
        /// <param name="line">包含收费站的行</param>
        public void Parse(string line)
        {
            try
            {
                if (line.Contains("#"))
                {
                    line = line.Replace("#", "");
                    var buf = line.Split(',');
                    StartCode = Convert.ToInt32(buf[0]);
                    EndCode = Convert.ToInt32(buf[1]);
                    Distance = Convert.ToDouble(buf[2]);
                    Road = buf[3].Trim();
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
                string text = $"Edge.02: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }

        

        /// <summary>
        /// 是否包含顶点
        /// </summary>
        /// <param name="code">顶点编号</param>
        public bool ContainVertex(int code)
        {
            if (StartCode == code || EndCode == code)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 是否包含收费站
        /// </summary>
        /// <param name="code"></param>
        public bool ContainTollStation(int code)
        {
            var node = new Node() { Code = code };
            return TollData.Contains(node);
        }
        /// <summary>
        /// 以某收费站为关键节点，将一条边分裂为2条边
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public EdgeSet Split(int code)
        {
            var node = new Node() { Code = code };
            int index = TollData.IndexOf(node);
            var res = new EdgeSet();
            if (index == 0 || index == TollData.Count)
            {
                res.Add(this);
            }
            else
            {
                double distance = 0;
                var edge = new Edge();
                edge.StartCode = StartCode;
                edge.EndCode = code;
                for (int i = 0; i <= index; i++)
                {
                    edge.Add(TollData[i]);
                    distance += TollData[i].Distance;
                }
                edge.Distance = distance;
                edge.Road = Road;
                res.Add(edge);

                distance = 0;
                edge = new Edge();
                edge.StartCode = code;
                edge.EndCode = EndCode;
                for (int i = index; i <TollData.Count; i++)
                {
                    edge.Add(TollData[i]);
                    distance += TollData[i].Distance;
                }
                edge.Distance = distance;
                edge.Road = Road;
                res.Add(edge);
            }
            return res;
        }

        public static bool operator ==(Edge left, Edge right)
        {
            if ((right != null && left != null)
                && (left.StartCode == right.StartCode)
                && (left.EndCode == right.EndCode)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(Edge left, Edge right)
        {
            return ((left.StartCode != right.StartCode)
                || (left.EndCode != right.EndCode));
        }
        protected bool Equals(Edge other)
        {
            return (string.Equals(StartCode, other.StartCode) &&
                string.Equals(EndCode, other.EndCode));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            return (StartCode.GetHashCode() + EndCode.GetHashCode());
        }

        public override string ToString()
        {
            string res = $"#{StartCode},{EndCode},{Distance}\n";
            foreach (var d in TollData)
            {
                res += d.ToString() + ",";
            }
            res += "END";
            return res;
        }
    }
}
