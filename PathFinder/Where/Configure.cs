using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Where
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class Configure
    {
        public static string Root { get; set; }
        public static double MinLat { get; set; }
        public static double MaxLat { get; set; }
        public static double MinLon { get; set; }
        public static double MaxLon { get; set; }
        public static double MinHeight { get; set; }
        public static double MaxHeight { get; set; }

        /// <summary>
        /// 基站覆盖区域的最大距离
        /// </summary>
        public static double MaxCoverDistance { get; set; }
        /// <summary>
        /// 汽车10分钟行驶的最远距离
        /// </summary>
        public static double MaxDistance10Miniter { get; set; }
        static Configure()
        {
            ReadAllSettings();
        }
        static void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    Logger.WriteError("AppSettings is empty.");
                }
                else
                {
                    Root = ReadSetting("Root");
                    MinLat = Convert.ToDouble(ReadSetting("MinLat"));
                    MaxLat = Convert.ToDouble(ReadSetting("MaxLat"));
                    MinLon = Convert.ToDouble(ReadSetting("MinLon"));
                    MaxLon = Convert.ToDouble(ReadSetting("MaxLon"));
                    MinHeight = Convert.ToDouble(ReadSetting("MinHeight"));
                    MaxHeight = Convert.ToDouble(ReadSetting("MaxHeight"));

                    MaxCoverDistance = Convert.ToDouble(ReadSetting("MaxCoverDistance"));
                    MaxDistance10Miniter = Convert.ToDouble(ReadSetting("MaxDistance10Miniter"));
                }
            }
            catch (ConfigurationErrorsException)
            {
                Logger.WriteError("Error reading app settings");
            }
        }

        static string ReadSetting(string key)
        {
            string result = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";
                Console.WriteLine(result);
            }
            catch (ConfigurationErrorsException)
            {
                Logger.WriteError("Error reading app settings");
            }
            return result;
        }

    }
}
