using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HelpTools.Server
{
    /// <summary>  
    /// ClassName:Server  
    /// Version:1.0  
    /// Date:2018/1/18 
    /// Author:lsy  
    /// </summary>  
    /// <remarks>  
    /// 服务器端
    /// </remarks>  
    public class Server
    {
        #region 变量
        private static byte[] result = new byte[1024];
        private int myProt = 8885;   //端口
        private string myIp = "";
        private IPEndPoint ipendpoint;
        static Socket serverSocket;
        #endregion

        public Server(string ip, int port)
        {
            myIp = ip;
            myProt = port;
            ipendpoint = new IPEndPoint(IPAddress.Parse(ip), port);

        }
        //服务开启
        public void StartServer()
        {
            try
            {
                //服务器IP地址
                //IPAddress ip = IPAddress.Parse("127.0.0.1");
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (GetInternetConStatus.GetNetConStatus(ipendpoint.Address.ToString()) == 3)
                {
                    serverSocket.Bind(ipendpoint);//new IPEndPoint(ip, myProt));  //绑定IP地址：端口
                }
                else
                {
                    return;
                }
                serverSocket.Listen(10);    //设定最多10个排队连接请求
                Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
                //通过Clientsoket发送数据
                Thread myThread = new Thread(ListenClientConnect);
                myThread.IsBackground = true;
                myThread.Name = "StartServer";
                myThread.Start();
                Console.ReadLine();
            }
            catch (Exception ex)
            {

            }

        }
        /// <summary>
        /// 监听客户端连接
        /// </summary>
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                // clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="clientSocket">客户端</param>
        /// <param name="msg">发送信息内容</param>
        public void SendMsg(Socket clientSocket, string msg)
        {
            try
            {
                if (clientSocket == null) return;
                clientSocket.Send(Encoding.ASCII.GetBytes(msg));

            }
            catch (Exception ex)
            {
                Msg messaage = new Msg();
                messaage.clientSK = clientSocket;
                messaage.Message = "服务器关闭：" + ex.Message;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="clientSocket"></param>
        public static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据
                    int receiveNumber = myClientSocket.Receive(result);
                    // Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                    string remsg = Encoding.ASCII.GetString(result, 0, receiveNumber);
                    Msg messaage = new Msg();
                    messaage.clientSK = myClientSocket;
                    messaage.Message = remsg;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    Msg messaage = new Msg();
                    messaage.clientSK = myClientSocket;
                    messaage.Message = ex.Message;
                    break;
                }
            }
        }
        /// <summary>
        /// 关掉服务器
        /// </summary>
        public void DisServer()
        {
            serverSocket.Close();
        }

        /// <summary>
        /// 获得主机IPAddress
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetIPAddress()
        {
            string HostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(HostName);
            foreach (IPAddress ip in iphostentry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }
        /// <summary>
        /// 获得客户端IP地址、端口号
        /// </summary>
        /// <param name="client"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void GetAddressPort(Socket client, ref string address, ref string port)
        {
            if (client != null)
            {
                IPEndPoint clientIpEndPoint = (IPEndPoint)client.RemoteEndPoint;
                address = clientIpEndPoint.Address.ToString();
                port = clientIpEndPoint.Port.ToString();
            }
        }

        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        private bool IPCheck(string IP)
        {
            string num = "(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)";
            return Regex.IsMatch(IP, ("^" + num + "\\." + num + "\\." + num + "\\." + num + "$"));
        }


    }
    public class Msg : EventArgs
    {
        private string message;
        /// <summary>
        /// 收到的消息
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
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

        private Socket clientsk;
        /// <summary>
        /// 客户端Socket
        /// </summary>
        public Socket clientSK
        {
            get { return clientsk; }
            set { clientsk = value; }
        }
    }
}
