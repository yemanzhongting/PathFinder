using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 一个节点
    /// 内容包括Id，
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 节点编码
        /// </summary>
        public int Code;
        /// <summary>
        /// 编码+名称
        /// </summary>
        public string Name { set; get; }

        public double Distance;
        public Node()
        {
            Code = -1;
            Name = string.Empty;
            Distance = -1;
        }
        /// <summary>
        //01025,荆州南,26.664
        //01026,公安,20.506
        //90065,陡兴场互通,10.462
        /// </summary>
        /// <param name="line">输入信息</param>
        public void Parse(string line)
        {
            try
            {
                var buf = line.Split(',');
                if (buf.Length > 2)
                {
                    Code = Convert.ToInt32(buf[0]);
                    Name = buf[1];
                    Distance = Convert.ToDouble(buf[2]);
                }
                //Name = line.Trim();
                //Code = Convert.ToInt32(Name.Substring(0, 5));
            }
            catch (Exception ex)
            {
                string text = $"Node.cs: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }
        public static bool operator ==(Node left, Node right)
        {
            if (right != null && (left != null && left.Code == right.Code))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(Node left, Node right)
        {
            return !(left == right);
        }
        protected bool Equals(Node other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            return (Code.GetHashCode());
        }

        public override string ToString()
        {
            string res = $"{Code}";
            return res;
        }

        public TollStation ToTollStation()
        {
            var toll = new TollStation();
            toll.Code = Code;
            toll.Name = Name;
            return toll;
        }
    }
}
