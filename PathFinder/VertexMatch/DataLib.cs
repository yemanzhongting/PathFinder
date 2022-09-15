using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Where;
namespace VertexMatch
{
    /// <summary>
    /// 参考数据库
    /// </summary>
    public class DataLib
    {
        /// <summary>
        /// 分区数据集合，用于快速分辨缓冲区
        /// </summary>
        public static GridSet GridData { get; set; }
        /// <summary>
        /// 收费站缓冲区集合
        /// 包含缓冲区的
        /// </summary>
        public static TollBufferSet BufferData { get; set; }

        /// <summary>
        /// 收费站信息文件
        /// </summary>
        public static TollStationSet StationData;

        public static RoadSet RoadData { get; set; }

        /// <summary>
        /// 顶点数据（交叉点或路线起止点）
        /// </summary>
        public static TollStationSet Vdata;
        /// <summary>
        /// 边数据（用于最短路径搜索）
        /// </summary>
        public static EdgeSet Edata;
        static DataLib()
        {
            try
            {
                string tollFile = $"{Configure.Root}/Lib/收费站-V3.txt";
                StationData = Reader.ReadTollStationData(tollFile);

                string gridFile = $"{Configure.Root}/Lib/分区-V4.txt";
                GridData = Reader.ReadGridData(gridFile);

                string bufferFile = $"{Configure.Root}/Lib/缓冲区-V4.txt";
                BufferData = Reader.ReadTollData(bufferFile);

                var roadFile = $"{Configure.Root}/Lib/路网-V4.txt";
                RoadData = Reader.ReadRoadData(roadFile);

                //string vertexFile = $"{Configure.Root}/Lib/顶点列表-V3.txt";
                string vertexFile = $"{Configure.Root}/Lib/keypoint.txt";
                Vdata = Reader.ReadTollStationData(vertexFile);

                //string edgeFile = $"{Configure.Root}/Lib/边-V3.txt";
                string edgeFile = $"{Configure.Root}/Lib/edge.txt";
                Edata = Reader.ReadEdgeData(edgeFile);

            }
            catch (Exception ex)
            {
                string text = $"DataLib.cs: {ex.Message}";
                Logger.WriteError(text);
            }

        }

    }
}
