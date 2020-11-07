using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpTools
{
    public class StringTool
    {
        /// <summary>
        /// 合并两个syte[]
        /// </summary>
        /// <param name="valueMain">第一个byte[]</param>
        /// <param name="valueAdd">第二个byte[]</param>
        /// <param name="length">第二个的长度</param>
        /// <returns></returns>
        public static byte[] CombineByte(byte[] valueMain, byte[] valueAdd, int length)
        {
            byte[] buffer = new byte[valueMain.Length + length];
            for (int i = 0; i < valueMain.Length; i++)
            {
                buffer[i] = valueMain[i];
            }
            for (int i = 0; i < valueAdd.Length; i++)
            {
                buffer[valueMain.Length + i] = valueAdd[i];
            }
            return buffer;
        }
    }
}
