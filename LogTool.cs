using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace HelpTools
{
    public class LogTool
    {
        private static bool _needSaveLog;
        private readonly List<LogEntity> _logEntities = new List<LogEntity>();

        public LogTool()
        {
            Thread thread = new Thread(SaveThread)
            {
                IsBackground = true
            };
            thread.Start();
        }

        public void WriteLogToFile(string filePath, string content)
        {
            LogEntity item = new LogEntity
            {
                FilePath = filePath,
                Content = content
            };
            lock (this._logEntities)
            {
                this._logEntities.Add(item);
            }
        }

        public static void WriteToFile(string filePath, string Content)
        {
            if (_needSaveLog)
            {
                Thread.Sleep(50);
            }
            _needSaveLog = true;
            try
            {
                FileStream stream;
                if (!File.Exists(filePath))
                {
                    File.Create(filePath.Trim());
                }
                FileTool.CheckFile(filePath, 1);

                stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                Content = Content + "\r\n";
                byte[] bytes = new UTF8Encoding().GetBytes(Content);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                _needSaveLog = false;
            }
        }
        private void SaveThread()
        {
            while (true)
            {
                Thread.Sleep(800);
                try
                {
                    this.SaveLogInfo();
                }
                catch (Exception)
                {
                }
            }
        }

        private void SaveLogInfo()
        {
            lock (this._logEntities)
            {
                if(_logEntities.Count<1)return;
                string str = this._logEntities[0].FilePath;
                string str2 = "\r\n" + this._logEntities[0].Content;
                WriteToFile(str, str2);
                this._logEntities.RemoveAt(0);
            }
        }
    }

    internal class LogEntity
    {
        public string FilePath { get; set; }
        public string Content { get; set; }
    }
}