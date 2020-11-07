using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;

namespace HelpTools
{
   public class ConfigTool
    {
       public static string ConfigPath = (AppDomain.CurrentDomain.BaseDirectory + @"\Config\System.ini");

        [DllImport("kernel32")]
        private static extern double GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
       /// <summary>
       /// 获取参数
       /// </summary>
        /// <param name="section">[部分]</param>
        /// <param name="key">参数名</param>
        /// <returns>参数值</returns>
       public static string GetValue(string section, string key)
        {
            StringBuilder retVal = new StringBuilder(0x7d0);
            GetPrivateProfileString(section, key, "", retVal, 0x7d0, ConfigPath);
            bool flag = retVal.ToString().Length >= 1;
            if (!flag)
            {
                SetValue(section, key, "");
            }
            return retVal.ToString().Trim();
        }
         /// <summary>
         /// 写入参数
         /// </summary>
         /// <param name="section">[部分]</param>
         /// <param name="key">参数名</param>
         /// <param name="value">参数值</param>
        public static void SetValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, ConfigPath);
        }

        [DllImport("kernel32")]
        private static extern void WritePrivateProfileString(string section, string key, string value, string filePath);
    }
}
