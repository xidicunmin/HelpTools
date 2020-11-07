using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace HelpTools
{
    public class ComHelper
    {
        struct ComParams
        {
            public string ComName;
            public int BaudRate;
            public int DataBits;
            public string StopBits;
            public string Parity;
        };
        public SerialPort ComPort = new SerialPort();
        public delegate void OnConnectStatusChange(bool bConnected);

        public delegate void OnErrorHandle(string errorMessage);

        public event OnErrorHandle OnError;

        public event OnConnectStatusChange ConnectStatusChange;

        public bool Connected
        {
            get
            {
                return this.ComPort != null && this.ComPort.IsOpen;
            }
        }

        public bool Connect()
        {
            if (this.ComPort == null)
            {
                this.ComPort = new SerialPort();
            }
            bool result;
            if (this.ComPort.IsOpen)
            {
                try
                {
                    this.ComPort.Close();
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    if (this.OnError != null)
                    {
                        this.OnError("关闭端口出现异常：" + msg);
                    }
                    return false;
                }
            }
            ComParams comParams=new ComParams();
            try
            {
                this.ComPort.PortName = comParams.ComName;
                this.ComPort.BaudRate = comParams.BaudRate;
                this.ComPort.DataBits = comParams.DataBits;
                this.ComPort.StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), comParams.StopBits);
                this.ComPort.Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), comParams.Parity);
                this.ComPort.ReadBufferSize = (this.ComPort.WriteBufferSize = 100);
                this.ComPort.ReadTimeout = (this.ComPort.WriteTimeout = 500);
                this.ComPort.RtsEnable = true;
                this.ComPort.DtrEnable = true;
                this.ComPort.DataReceived += new SerialDataReceivedEventHandler(this.ComPort_DataReceived);
                this.ComPort.Open();
                if (ConnectStatusChange != null)
                {
                    ConnectStatusChange(true);
                }
                System.Threading.Thread threadCheck = new System.Threading.Thread(CheckLive)
                {
                    IsBackground = true
                };
                threadCheck.Start();
                result = true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (this.OnError != null)
                {
                    this.OnError("打开串口异常：" + msg);
                }
                result = false;
            }
            return result;
        }

        private void CheckLive()
        {
            while (true)
            {
                if (DateTime.Now > _lastReceiveTime.AddSeconds(10))
                {//10秒未接收到PLC的数据，判定为通信丢失
                    if (false)
                    {
                        if (ConnectStatusChange != null)
                        {
                            ConnectStatusChange(false);
                        }
                    }
                    break;
                }
            }
        }

        public void DisConnect()
        {
            if (this.ComPort != null)
            {
                this.ComPort.DataReceived -= new SerialDataReceivedEventHandler(this.ComPort_DataReceived);
                if (this.ComPort.IsOpen)
                {
                    try
                    {
                        this.ComPort.Close();
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        if (this.OnError != null)
                        {
                            this.OnError("关闭串口异常：" + msg);
                        }
                    }
                }
            }
        }

        private readonly List<byte> _readtemp = new List<byte>();
        /// <summary>
        /// 最后接收到串口发送数据的时间点
        /// </summary>
        private DateTime _lastReceiveTime = DateTime.Now;
        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //byte[] Readtemp = new byte[this.ComPort.BytesToRead];
                //this.ComPort.Read(Readtemp, 0, this.ComPort.BytesToRead);
                //for (int i = 0; i < Readtemp.Length; i++)
                //{
                //    this._Readtemp.Add(Readtemp[i]);
                //}
                //LastReceiveTime = DateTime.Now;

                //    string s = "";
                //    s = Encoding.Default.GetString(_Readtemp.ToArray(), 0, _Readtemp.Count);
                byte[] readtemp = new byte[this.ComPort.BytesToRead];
                this.ComPort.Read(readtemp, 0, this.ComPort.BytesToRead);
                foreach (byte t in readtemp)
                {
                    this._readtemp.Add(t);
                }
                _lastReceiveTime = DateTime.Now;
                string s = Encoding.GetEncoding("GBK").GetString(_readtemp.ToArray(), 0, _readtemp.Count);
                OnError("接收到串口数据" + s);
                this._readtemp.Clear();

            }
            catch (Exception ex)
            {
                this.OnError("串口接收数据异常：" + ex.Message);
            }
        }


    }
}
