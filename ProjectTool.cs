using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpTools
{
    public class ProjectTool
    {
        public static bool SetComputerStartRun(string path,bool isStart)
        {
            bool start = false;
            string name = System.IO.Path.GetFileName(path);
            try
            {
                Microsoft.Win32.RegistryKey HKLM = Microsoft.Win32.Registry.CurrentUser;
                Microsoft.Win32.RegistryKey Run = HKLM.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                try
                {
                    object Get = Run.GetValue(name);
                    start = (Get == null);
                }
                catch
                {
                    start = false;
                }
                if (start && isStart)
                {
                    try
                    {
                        Run.SetValue(name, path);
                        HKLM.Close();
                    }
                    catch
                    {
                        start = false;
                    }
                }
                else if (!start && !isStart)
                {
                    try
                    {
                        Run.DeleteValue(name);
                        HKLM.Close();
                        start = true;
                    }
                    catch
                    {
                        start = false;
                    }
                }
                else
                {
                    start = true;
                }
            }
            catch (System.Exception ex_74)
            {
            }
            return start;
        }
    }
}
