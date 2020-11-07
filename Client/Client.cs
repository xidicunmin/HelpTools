using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HelpTools.Client
{
    /// <summary>  
    /// ClassName:Client  
    /// Version:1.0  
    /// Date:2018/1/18 
    /// Author:lsy  
    /// </summary>  
    /// <remarks>  
    /// 客户端
    /// </remarks>  
    public class Client
    {

        #region 变量
        public IPEndPoint endIpInfo;//连接服务端的IP与端口号
        private Socket clientSk;     //客户端Socket
        private byte[] refBuffer;    //消息数据缓冲区
        private byte[] sendBuffer;   //发送段数据
        #endregion

        //声明委托类型     
        public delegate void ReceiveMsgHandle(Msg message);

        //定义接受到消息事件
        public event ReceiveMsgHandle ReceiveMsg;
        private bool? _connected;
        public bool Connected
        {
            get { return clientSk.Connected; }
            set { _connected = value; }
        }
        /// <summary>
        /// 构造函数用来初始化服务端数据
        /// </summary>
        /// <param name="IP">服务端IP</param>
        /// <param name="port">需要与服务端建立的端口号</param>
        public Client(string IP, int port)
        {
            endIpInfo = new IPEndPoint(IPAddress.Parse(IP), port);
            refBuffer = new byte[65535];
            sendBuffer = new byte[65535];
        }

        /// <summary>
        /// 连接服务端
        /// </summary>
        /// <returns></returns>
        public bool StartConnet(ref string errMsg)
        {
            DisConn();
            clientSk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if (GetInternetConStatus.GetNetConStatus(endIpInfo.Address.ToString()) == 3)
                {
                    clientSk.Connect(endIpInfo);//连接服务器
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                return false;
            }

            if (clientSk.Connected == true)
            {
                //通过Clientsoket发送数据
                //int receiveLength = clientSk.Receive(refBuffer);
                clientSk.BeginReceive(refBuffer, 0, refBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), null);
                return true;
            }
            else
                return false;

        }


        /// <summary>
        /// 一个 AsyncCallback 委托，它引用操作完成时要调用的方法
        /// </summary>
        /// <param name="Ar"></param>
        public void ReceiveCallBack(IAsyncResult Ar)
        {
            try
            {
                if (GetInternetConStatus.GetNetConStatus(endIpInfo.Address.ToString()) == 3)
                {
                    if (clientSk.Connected == true)
                    {
                        //clientSk.Connect(endIpInfo);//连接服务器
                        Msg msg = new Msg();
                        if (clientSk.Poll(10, SelectMode.SelectRead) == false)
                        {
                            int Rnd = clientSk.EndReceive(Ar);
                            //string a1 = GetHexadecimalValue(ToHexString(refBuffer).Substring(ToHexString(refBuffer).Length - 8, 8)).ToString();
                            string a2 = GetHexadecimalValue(ToHexString(refBuffer).Substring(4, 8)).ToString();
                            /*MODIFLY 2018/10/11 by Wang Sizhe START*/
                            msg.Message = Encoding.UTF8.GetString(refBuffer,0,Rnd); //GetHexadecimalValue(ToHexString(refBuffer)).ToString();
                            /*MODIFLY 2018/10/11 by Wang Sizhe END*/
                            msg.Messageb = a2;
                            //string msg = GetHexadecimalValue(ToHexString(refBuffer)).ToString();//GetHexadecimalValue(Encoding.Default.GetString(refBuffer, 0, Rnd)).ToString();// Encoding.Default.GetString(refBuffer, 0, Rnd);
                            if (ReceiveMsg != null && !string.IsNullOrEmpty(msg.Message))
                                ReceiveMsg(msg);//引发事件
                            clientSk.BeginReceive(refBuffer, 0, refBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), null);
                        }
                        else
                        {
                            // clientSk.Connected = false;
                            msg.Message = "服务器断开！";
                            if (ReceiveMsg != null && !string.IsNullOrEmpty(msg.Message))
                                ReceiveMsg(msg);//引发事件
                            clientSk.BeginReceive(refBuffer, 0, refBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), null);
                            clientSk.Dispose();//加入部分
                        }
                    }
                }
            }
            catch (Exception ex)
            { }

        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void DisConn()
        {
            if (clientSk != null)
            {
                if (clientSk.Connected)
                {
                    clientSk.Close();
                }
            }
        }

        /// <summary>
        /// 向目的IP发送消息
        /// </summary>
        /// <param name="Ip">目的方IP</param>
        /// <param name="Msg">消息内容</param>
        public void SendMsg(string Msg,ref string ErrMsg)
        {
            try
            {
                sendBuffer = System.Text.Encoding.Default.GetBytes(Msg);
                clientSk.Send(sendBuffer);
                ErrMsg = "成功向PLC发送："+Msg;
            }
            catch (Exception ex)
            {

                ErrMsg = "发送失败：" + ex.Message;
            }
        }
        /// <summary>
        /// byte[]转16进制格式string：
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            string[] dbId = new string[12];
            StringBuilder dds = new StringBuilder();
            if (bytes != null)
            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < 12; i++)
                {
                    // if (bytes[i] > 0)
                    // {
                    strB.Append(bytes[i].ToString("X2"));
                    // }

                    dbId[11 - i] = bytes[i].ToString("X2");


                }

                hexString = strB.ToString();

                //hexString = BitConverter.ToString(bytes).TrimEnd();
            }
            foreach (string c in dbId)
            {
                dds.Append(c);
            }
            string dd = dds.ToString();
            return dd;

        }

        /// <summary>
        /// 十六进制换算为十进制
        /// </summary>
        /// <param name="strColorValue"></param>
        /// <returns></returns>
        public static int GetHexadecimalValue(String strColorValue)
        {
            char[] nums = strColorValue.ToCharArray();
            int total = 0;
            try
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    String strNum = nums[i].ToString().ToUpper();
                    switch (strNum)
                    {
                        case "A":
                            strNum = "10";
                            break;
                        case "B":
                            strNum = "11";
                            break;
                        case "C":
                            strNum = "12";
                            break;
                        case "D":
                            strNum = "13";
                            break;
                        case "E":
                            strNum = "14";
                            break;
                        case "F":
                            strNum = "15";
                            break;
                        default:
                            break;
                    }
                    double power = Math.Pow(16, Convert.ToDouble(nums.Length - i - 1));
                    total += Convert.ToInt32(strNum) * Convert.ToInt32(power);
                }

            }
            catch (System.Exception ex)
            {
                String strErorr = ex.ToString();
                return 0;
            }


            return total;
        }
    }
    public class Msg : EventArgs
    {
        private string message;
        private string messageb;
        /// <summary>
        /// 收到的消息
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public string Messageb
        {
            get { return messageb; }
            set { messageb = value; }
        }
        private byte[] refBuffer;
        /// <summary>
        /// 收到的byte数组
        /// </summary>
        public byte[] RefBuffer
        {
            get { return refBuffer; }
            set { refBuffer = value; }
        }
    }
}
