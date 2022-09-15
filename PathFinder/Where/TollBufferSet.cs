using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 收费站缓冲区集合
    /// </summary>
    public class TollBufferSet
    {
        public List<TollBuffer> Data { get; set; }

        public TollBufferSet()
        {
            Data = new List<TollBuffer>();
        }

        public int Count => Data.Count;

        public int IndexOf(int tollCode)
        {
            int i = -1;
            try
            {
                var toll = new TollBuffer() { Code = tollCode };
                i = Data.IndexOf(toll);

            }
            catch (Exception ex)
            {
                string text = $"TollBufferSet.cs: {ex.Message}--{tollCode}";
                Logger.WriteError(ex.Message);
            }

            return i;
        }

        /// <summary>
        /// 增加收费站
        /// 1.如果该收费不站存在，则直接增加
        /// 2.如果收费存在，则更新相关数据
        /// </summary>
        /// <param name="station"></param>
        public void Add(TollBuffer station)
        {
            if (!Data.Contains(station))
            {
                Data.Add(station);
            }
            else
            {
                int index = Data.IndexOf(station);
                Data[index].Update(station);
            }
        }

        /// <summary>
        /// 根据移动基站Id，查找包含该基站Id的所有收费站
        /// </summary>
        /// <param name="cellbase">移动基站Id</param>
        /// <param name="gridSet">分区数据集合</param>
        /// <returns>包含该移动基站的收费站ID列表</returns>
        public List<int> FindTollCodeByCellbase(string cellbase, GridSet gridSet)
        {
            var res = new List<int>();
            var tollIds = gridSet.FindTollSet(cellbase);
            try
            {
                foreach (var t in tollIds)
                {
                    var toll = new TollBuffer() { Code = t };
                    if (!Data.Contains(toll)) continue;

                    int i = Data.IndexOf(toll);
                    if (Data[i].Contains(cellbase))
                    {
                        res.Add(Data[i].Code);
                    }
                }
            }
            catch (Exception ex)
            {
                string text = $"TollBufferSet.cs: {ex.Message}--{tollIds}";
                Logger.WriteError(ex.Message);
            }

            return res;
        }
    }
}
