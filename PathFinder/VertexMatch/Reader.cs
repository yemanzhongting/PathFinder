using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Where;
using System.IO;

namespace VertexMatch
{
    /// <summary>
    /// 1.读取卡片数据
    /// 2.读取收费站列表数据
    /// 3.读取分区数据
    /// 4.读取收费站缓冲区数据
    /// 5.读取道路数据库数据
    /// 
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// 读取卡片数据
        /// </summary>
        /// <param name="file">卡片数据文件路径</param>
        /// <returns>卡片数据集合</returns>
        public static List<Card> ReadCardData1(string file)
        {
            var res = new List<Card>();
            string line = string.Empty;
            try
            {
                var reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line) && !line.Contains("#"))
                    {
                        var card = new Card();
                        card.Parse1(line);
                        res.Add(card);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }
            return res;
        }
        public static List<Card> ReadCardData2(string file)
        {
            var res = new List<Card>();
            string line = string.Empty;
            try
            {
                var reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line) && !line.Contains("#"))
                    {
                        var card = new Card();
                        card.Parse2(line);
                        res.Add(card);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }
            return res;
        }

        public static List<Card> ReadCardData3(string file)
        {
            var res = new List<Card>();
            string line = string.Empty;
            try
            {
                var reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line) && !line.Contains("#"))
                    {
                        var card = new Card();
                        card.Parse3(line);
                        res.Add(card);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }
            return res;
        }

        /// <summary>
        /// 读取收费站列表数据
        /// #ID,lon,lat,height,weight
        ///08601小军山,114.13476,30.39420,10.89,791
        ///00513军山,114.10338,30.39757,16.93,48
        ///00510武汉西,114.04929,30.47383,15.61,174
        ///00509蔡甸,114.04674,30.57062,20.05,313
        ///00508武汉北,113.90512,30.82043,26.45,234
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public static TollStationSet ReadTollStationData(string filename)
        {
            var res = new TollStationSet();
            string line = string.Empty;
            try
            {
                var reader = new StreamReader(filename);
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line != null && !line.Contains("#"))
                    {
                        TollStation v = new TollStation();
                        v.Parse(line);
                        res.Add(v);
                    }
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }
            return res;
        }

        /// <summary>
        /// 读取分区文件
        /// </summary>
        /// <param name="file">分区文件名称</param>
        /// <returns>分区数据集合</returns>
        public static GridSet ReadGridData(string file)
        {
            var res = new GridSet();
            string line=string.Empty;
            try
            {
                var reader = new StreamReader(file);
                var grid = new Grid();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.Contains("#"))
                    {
                        line = line.Replace("#", "");
                        grid.Id = line;
                    }
                    else if (line.Contains("END"))
                    {
                        res.Data.Add(grid);
                        grid = new Grid();
                    }
                    else if (line.Length > 5)
                    {
                        // var code = Convert.ToInt32(line.Substring(0, 5));
                        var buf = line.Split(',');
                        var code = Convert.ToInt32(buf[0]);
                        grid.AddVertex(code);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }

            return res;
        }

        /// <summary>
        /// 读取收费站数据
        /// 该数据是计算道路还原的基础数据
        /// </summary>
        /// <param name="file">收费站列表数据文件（包含移动基站信息）</param>
        /// <returns></returns>
        public static TollBufferSet ReadTollData(string file)
        {
            var res = new TollBufferSet();
            string line = string.Empty;
            try
            {
                var reader = new StreamReader(file);
                var toll = new TollBuffer();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line != null && line.Contains("#"))
                    {
                        line = line.Replace("#", "");
                        var buf = line.Split(',');

                        toll.Code = Convert.ToInt32(buf[0]);
                    }
                    else if (line != null && line.Contains("END"))
                    {
                        res.Add(toll);
                        toll = new TollBuffer();
                    }
                    else
                    {
                        if (line != null)
                        {
                            string cellbase = line;
                            toll.Add(cellbase);
                        }

                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }
            return res;
        }

        /// <summary>
        /// 读取路网数据
        /// </summary>
        /// <param name="filename">路网数据名称</param>
        public static RoadSet ReadRoadData(string filename)
        {
            var res = new RoadSet();
            string line = string.Empty;
            try
            {
                var reader = new StreamReader(filename);
                var road = new Road();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line != null && line.Contains("#"))
                    {
                        road.Parse(line);
                    }
                    else if (line != null && line.Contains("END"))
                    {
                        res.Add(road);
                        road = new Road();
                    }
                    else
                    {
                        road.Parse(line);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }
            return res;
        }
        /// <summary>
        /// 读取边数据
        /// </summary>
        /// <param name="filename">边数据文件</param>
        /// <returns></returns>
        internal static EdgeSet ReadEdgeData(string filename)
        {
            var res = new EdgeSet();
            string line = string.Empty;
            try
            {
                var reader = new StreamReader(filename);
                var edge = new Edge();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line != null && line.Contains("#"))
                    {
                        edge.Parse(line);
                    }
                    else if (line != null && line.Contains("END"))
                    {
                        res.Add(edge);
                        edge = new Edge();
                    }
                    else
                    {
                        edge.Parse(line);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string text = $"Reader.cs: {ex.Message} +{line}";
                Logger.WriteError(text);
            }
            return res;
        }
    }
}
