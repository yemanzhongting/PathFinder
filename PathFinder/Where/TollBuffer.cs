using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 收费站及其该站周围的移动基站
    /// </summary>
    public class TollBuffer
    {
        /// <summary>
        /// 收费站Id
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 收费站缓冲区内的移动基站
        /// </summary>
        public List<string> CellbaseData { get; set; }

        public TollBuffer()
        {
            Code = -1;
            CellbaseData = new List<string>();
        }

        /// <summary>
        /// 向数据集增加一个移动基站
        ///  1. 如果不存在，则直接增加
        ///  2. 如果已经存在，则更新坐标
        /// </summary>
        public void Add(string cellbase)
        {
            if (!CellbaseData.Contains(cellbase))
            {
                CellbaseData.Add(cellbase);
            }
        }
        /// <summary>
        /// 判断该移动基站数组中是否存在
        ///  </summary>
        /// <param name="cellbase">移动基站的8位十六进制编码</param>
        /// <returns>如果存在，则返回true，否则为false</returns>
        public bool Contains(string cellbase)
        {
            return CellbaseData.Contains(cellbase);
        }
        public static bool operator ==(TollBuffer left, TollBuffer right)
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
        public static bool operator !=(TollBuffer left, TollBuffer right)
        {
            return !(left == right);
        }
        protected bool Equals(TollBuffer other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TollBuffer)obj);
        }
        public override int GetHashCode()
        {
            return (Code.GetHashCode());
        }
        public override string ToString()
        {
            string res = $"#{Code}";
            foreach (var d in CellbaseData)
            {
                res += $"\n{d}";
            }
            res += "\nEND";
            return res;
        }

        public void Update(TollBuffer station)
        {
            try
            {
                foreach (var cb in station.CellbaseData)
                {
                    Add(cb);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);
            }
        }
    }
}
