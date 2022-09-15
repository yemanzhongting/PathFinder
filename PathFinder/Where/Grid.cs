using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 根据电信区域代码分区;
    /// 服务于快速索引
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// 分区Id，16进制格式
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 收费站列表
        /// </summary>
        public List<int> TollData { get; set; }
        public Grid()
        {
            Id = string.Empty;
            TollData = new List<int>();
        }
        /// <summary>
        /// 增加一个收费站
        /// </summary>
        /// <param name="tollCode"></param>
        public void AddVertex(int tollCode)
        {
            if (!TollData.Contains(tollCode))
                TollData.Add(tollCode);
        }

        public override string ToString()
        {
            string res = $"#{Id}";
            foreach (var d in TollData)
            {
                res += $"\n{d}";
            }
            res += "\nEND";
            return res;
        }

        public static bool operator ==(Grid left, Grid right)
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

        public static bool operator !=(Grid left, Grid right)
        {
            return !(left == right);
        }
        protected bool Equals(Grid other)
        {
            return string.Equals(Id, other.Id);
        }
        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Grid)obj);
        }
    }
}
