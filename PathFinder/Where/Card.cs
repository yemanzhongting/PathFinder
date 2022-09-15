using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 卡片数据
    /// </summary>
    public class Card
    {
        /// <summary>
        /// 卡片编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 入口收费站编码
        /// </summary>
        public int StartCode { get; set; }
        /// <summary>
        /// 入站时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 出口收费站编码
        /// </summary>
        public int EndCode { get; set; }
        /// <summary>
        /// 出站时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 时间长度，以分钟为单位
        /// </summary>
        public double TimeLength { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string CarNumber { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalRecord { get; set; }

        public List<string> CellBaseList { get; set; }
        /// <summary>
        /// 如果时间与记录数目相符合，则为true，否则为false
        /// </summary>
        public bool Valid { get; set; }
        public Card()
        {
            Id = string.Empty;
            StartCode = -1;
            StartTime = new DateTime(2000, 1, 1);
            EndCode = -1;
            EndTime = new DateTime(2000, 1, 1);
            TimeLength = 0;
            Valid = true;
            CarNumber = string.Empty;
            TotalRecord = 0;
            CellBaseList = new List<string>();

        }
        /// <summary>
        /// 根据文件格式，进行数据解析
        /// #卡号,入口收费站编码,入站时间,出口收费站编码,出站时间,基站数量（十六进制）,基站列表
        ///2040006244,10563,2015-01-28 09:13:46.000,10560,2015-01-28 10:57:52.000,蓝鄂Q23H27,0A,71DA8590704F52BC704F84D071DD269871DD218F71DD848A71DD213F71D910C271DE25D2704D21A40000
        ///2040002131,510,2015-01-28 08:48:19.000,10536,2015-01-28 11:08:19.000,黄鄂AJA663,0F,70368EBE702BA30E702B886070AF9E2D70C8C81570F29CE270C6C8DE70F5C82070F5C8F270F6D7E870F6E82870C09C9B70C0D8B170C0D7FD705253B80000
        /// </summary>
        /// <param name="line">包含一张卡片的所有信息</param>
        public void Parse2(string line)
        {
            try
            {
                var buf = line.Split(',');
                Id = buf[0];
                //StartCode = Convert.ToInt32(buf[1]);
                //StartTime = ParseTime(buf[2]);
                //EndCode = Convert.ToInt32(buf[3]);
                //EndTime = ParseTime(buf[4]);

                //TimeSpan tv = EndTime - StartTime;
                //TimeLength = tv.Days * 24 * 60 + tv.Hours * 60 + tv.Minutes;

                //CarNumber = buf[5];
                CarNumber = buf[1];

                StartCode = Convert.ToInt32(buf[2]);
                StartTime = ParseTime(buf[3]);
                EndCode = Convert.ToInt32(buf[4]);
                EndTime = ParseTime(buf[5]);
                TimeSpan tv = EndTime - StartTime;
                TimeLength = tv.Days * 24 * 60 + tv.Hours * 60 + tv.Minutes;


                TotalRecord = Convert.ToInt32(buf[6], 16);

                if (Math.Abs(TimeLength / 10 - TotalRecord) < 2)
                {
                    Valid = true;
                }
                else
                {
                    Valid = false;
                }

                //有几种错误情况
                //第1种情况，文件没有正常结束
                //第2种情况，记录中有错误内容，黄鄂EB3569,A2,FFFFFFFFF01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2A002A298909C754_0100000000FF001C25000009014200002F2C54B6000020150127114414C607C69C2A29>
                if (buf[7].Contains("FFFFF") || buf[7].Contains(">"))
                {
                    Valid = false;
                    //return;
                }
                int count = TotalRecord;
                if (buf[7].Length / 8 < TotalRecord)
                {
                    Valid = false;
                    count = Convert.ToInt32(Math.Floor(buf[7].Length / 8.0));
                    TotalRecord = count;
                }
                for (int i = 0; i < count; i++)
                {
                    var cellId = buf[7].Substring(i * 8, 8);
                    CellBaseList.Add(cellId);
                }
            }
            catch (Exception ex)
            {
                string text = $"Card.02-2: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }

        /// <summary>
        /// 根据文件格式，进行数据解析
        /// #卡号,入口收费站编码,入站时间,出口收费站编码,出站时间,基站数量（十六进制）,基站列表
        ///    'card_id','vehicle_license','entry_station_id','entry_time','exit_station_id','exit_time','c0020','c0021'
        ///'2017001888','黄鄂H39968','11512','2018/1/1 00:00:40','11510','2018/1/1 00:30:10','0103F82C000061E1F82C00030000000071AC85DF7118864F711896CB',''
        ///'2017006535','黄皖M56799','2003','2018/1/1 00:01:57','10401','2018/1/1 13:13:37','011DD307000128B7D3071304000000007113D14E7020AB4B713CAB3871209C4D72192C8B71102DB6711285A471123A40711239F0711287E771125DF47073653770733D9C70733B6C707A3F37707A3F35707A4027707A65F6707D6AA57216448F721643F0707F698E707F6C03707F6C2B707F69BF707F90F9707F69C072176B8C72179288',''
        ///'2017006535','黄皖M56799','2003','2018/1/1 00:01:57','10401','2018/1/1 13:13:37','011DD307000128B7D3071304000000007113D14E7020AB4B713CAB3871209C4D72192C8B71102DB6711285A471123A40711239F0711287E771125DF47073653770733D9C70733B6C707A3F37707A3F35707A4027707A65F6707D6AA57216448F721643F0707F698E707F6C03707F6C2B707F69BF707F90F9707F69C072176B8C72179288',''
        ///'2017004038','黄鄂AJC579','8703','2018/1/1 00:02:51','8702','2018/1/1 00:11:12','0100FF2100025DE2F22C234500000000',''
        /// </summary>
        /// <param name="line">包含一张卡片的所有信息</param>
        public void Parse3(string line)
        {
            try
            {
                string prime = "\'";
                line=line.Replace(prime,"");
                var buf = line.Split(',');
                Id = buf[0];
                CarNumber = buf[1];
                StartCode = Convert.ToInt32(buf[2]);
                StartTime = ParseTime(buf[3]);
                EndCode = Convert.ToInt32(buf[4]);
                EndTime = ParseTime(buf[5]);
                TimeSpan tv = EndTime - StartTime;
                TimeLength = tv.Days * 24 * 60 + tv.Hours * 60 + tv.Minutes;

                if (buf[6].Length > 36)
                {
                    string total = buf[6].Substring(2, 2);
                    TotalRecord = Convert.ToInt32(total, 16);

                    if (Math.Abs(TimeLength / 10 - TotalRecord) < 2)
                    {
                        Valid = true;
                    }
                    else
                    {
                        Valid = false;
                    }

                    var record = buf[6].Substring(32, buf[6].Length - 32);
                    //有几种错误情况
                    //第1种情况，文件没有正常结束
                    //第2种情况，记录中有错误内容，黄鄂EB3569,A2,FFFFFFFFF01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2A002A298909C754_0100000000FF001C25000009014200002F2C54B6000020150127114414C607C69C2A29>
                    if (record.Contains("FFFFF") || record.Contains(">"))
                    {
                        Valid = false;
                        //return;
                    }
                    int count = TotalRecord;
                    if (record.Length / 8 < TotalRecord)
                    {
                        Valid = false;
                        count = Convert.ToInt32(Math.Floor(buf[7].Length / 8.0));
                        TotalRecord = count;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        var cellId = record.Substring(i * 8, 8);
                        CellBaseList.Add(cellId);
                    }

                }
              
            }
            catch (Exception ex)
            {
                string text = $"Card.02-3: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }
        public void Parse1(string line)
        {
            try
            {
                var buf = line.Split(',');
                Id = buf[0];
                StartCode = Convert.ToInt32(buf[1]);
                StartTime = ParseTime(buf[2]);
                EndCode = Convert.ToInt32(buf[3]);
                EndTime = ParseTime(buf[4]);

                TimeSpan tv = EndTime - StartTime;
                TimeLength = tv.Days * 24 * 60 + tv.Hours * 60 + tv.Minutes;

                CarNumber = buf[5];

                TotalRecord = Convert.ToInt32(buf[6], 16);

                if (Math.Abs(TimeLength / 10 - TotalRecord) < 2)
                {
                    Valid = true;
                }
                else
                {
                    Valid = false;
                }

                //有几种错误情况
                //第1种情况，文件没有正常结束
                //第2种情况，记录中有错误内容，黄鄂EB3569,A2,FFFFFFFFF01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2A002A298909C754_0100000000FF001C25000009014200002F2C54B6000020150127114414C607C69C2A29>
                if (buf[7].Contains("FFFFF") || buf[7].Contains(">"))
                {
                    Valid = false;
                    //return;
                }
                int count = TotalRecord;
                if (buf[7].Length / 8 < TotalRecord)
                {
                    Valid = false;
                    count = Convert.ToInt32(Math.Floor(buf[7].Length / 8.0));
                    TotalRecord = count;
                }
                for (int i = 0; i < count; i++)
                {
                    var cellId = buf[7].Substring(i * 8, 8);
                    CellBaseList.Add(cellId);
                }
            }
            catch (Exception ex)
            {
                string text = $"Card.02-1: {ex.Message}--{line}";
                Logger.WriteError(text);
            }
        }
        /// <summary>
        /// 将字符串转为时间
        /// </summary>
        private DateTime ParseTime(string v)
        {
            var dt = Convert.ToDateTime(v);
            return dt;
        }
        /// <summary>
        /// 头文件字符串
        /// </summary>
        /// <returns></returns>
        public string HeaderString()
        {
            string res =
                $"#{Id},{StartCode},{StartTime.ToLongTimeString()},{EndCode},{EndTime.ToLongTimeString()},{TimeLength},{Valid}, {CarNumber},{TotalRecord}";
            return res;
        }
    }
}
