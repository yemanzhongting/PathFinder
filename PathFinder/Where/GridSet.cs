using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 区域划分数据集
    /// </summary>
    public class GridSet
    {
        public List<Grid> Data { set; get; }

        public GridSet()
        {
            Data = new List<Grid>();
        }
        /// <summary>
        /// 1. 如果该区域已经存在，则将该顶点加入
        /// 2. 如果该区域不存在，则创建区域，然后放入点
        /// </summary>
        /// <param name="gridId"></param>
        /// <param name="tollCode"></param>
        public void AddGrid(string gridId, int tollCode)
        {
            try
            {
                var grid = new Grid { Id = gridId };
                if (Data.Contains(grid))
                {
                    int index = Data.IndexOf(grid);
                    Data[index].AddVertex(tollCode);

                }
                else
                {
                    grid.AddVertex(tollCode);
                    Data.Add(grid);
                }

            }
            catch (Exception ex)
            {
                string text = $"GridSet.cs: {ex.Message}--{gridId}--{tollCode}";
                Logger.WriteError(ex.Message);
            }
        }
        /// <summary>
        /// 根据移动基站Id，搜索相关的收费站列表 
        /// </summary>
        /// <param name="cellbaseId">移动基站Id</param>
        /// <returns>收费站列表</returns>
        public List<int> FindTollSet(string cellbaseId)
        {
            var res = new List<int>();
            try
            {
                var lac = cellbaseId.Substring(0, 4);
                var grid = FindGrid(lac);
                res = grid.TollData;
            }
            catch (Exception ex)
            {
                string text = $"GridSet.cs: {ex.Message}--{cellbaseId}";
                Logger.WriteError(text);
            }
            return res;
        }

        /// <summary>
        /// 根据电信的Lac码，返回格网
        /// </summary>
        /// <param name="lac"></param>
        /// <returns></returns>
        public Grid FindGrid(string lac)
        {
            var res = new Grid();
            try
            {
                var grid = new Grid { Id = lac };
                int index = Data.IndexOf(grid);
                if (index != -1)
                    res = Data[index];
            }
            catch (Exception ex)
            {
                string text = $"GridSet.cs: {ex.Message}--{lac}";
                Logger.WriteError(text);
            }
            return res;
        }

    }
}
