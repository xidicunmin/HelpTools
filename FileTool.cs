using System;
using System.IO;
using System.Text;

namespace HelpTools
{
    public class FileTool
    {
       /// <summary>
       ///  复制文件/备份文件
       /// </summary>
       /// <param name="filePath">要复制的文件</param>
       /// <param name="errMsg">该方法的错误日志</param>
       /// <returns></returns>
        public static bool CopyFile(string filePath, ref string errMsg)
        {
            try
            {
                File.Copy(filePath, filePath + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 将字符串写入文件中
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">写入的内容</param>
        /// <param name="bOverride">是否覆盖文件原有的内容</param>
        public static void WriteData(string path, string content, bool bOverride)
        {
            try
            {
                Stream stream = null;
                StreamWriter sWrite;
                if (File.Exists(path))
                {
                    sWrite = new StreamWriter(path, !bOverride, Encoding.GetEncoding("UTF-8"));
                }
                else
                {
                    stream = File.Open(path, FileMode.Create);
                    sWrite = new StreamWriter(stream, Encoding.GetEncoding("UTF-8"));
                }
                byte[] data1 = Encoding.UTF8.GetBytes(content);
                string mas = System.Text.Encoding.UTF8.GetString(data1, 0, data1.Length);
                sWrite.Write(mas);
                sWrite.Flush();
                sWrite.Close();
            }
            catch (Exception) { }
        }
        /// <summary>
        /// 从文件中读取字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadData(string path)
        {
            string result = "";
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            byte[] dataTemp = new byte[fs.Length];
            fs.Read(dataTemp, 0, dataTemp.Length);
            fs.Close();
            result = Encoding.GetEncoding("gb2312").GetString(dataTemp);
            return result;
        }

        public static void CheckFile(string path, int count)
        {
            string str;
            string[] strArray;
            bool flag = count <= 100;
            if ((((uint)count) - ((uint)count)) >= 0)
            {
                goto Label_0137;
            }
        Label_0114:
            strArray = new string[5];
            strArray[0] = path.Substring(0, path.LastIndexOf('.'));

            strArray[1] = "(";
            strArray[2] = count.ToString();

            strArray[3] = ")";

            strArray[4] = path.Substring(path.LastIndexOf('.'));
        Label_009D:
            str = string.Concat(strArray);
            flag = !File.Exists(str);
            if (((uint)count) <= uint.MaxValue)
            {
                if (flag)
                {
                    File.Move(path, str);
                    if ((((uint)count) & 0) == 0)
                    {
                        if ((1) > uint.MaxValue)
                        {                              
                        }
                        goto Label_00F4;
                    }
                    goto Label_009D;
                }
            }
            else if ((((uint)count) | uint.MaxValue) == 0)
            {
                return;
            }
            CheckFile(path, count + 1);
        Label_00F4:
            if ((((uint)count) | 0x7fffffff) != 0)
            {
                return;
            }
            goto Label_0137;
     
        Label_0137:
            if (!flag)
            {
                return;
            }
        Label_013A:
            if (!File.Exists(path))
            {
                goto Label_00F4;
            }
            goto Label_0114;
        }
    }
}
