using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkTest.synchronize
{
    /// <summary>
    /// TCP同步通信的客户端类
    /// </summary>
    class TCPClient
    {
        private Socket clientSocket;
        private IPEndPoint endPoint;

        public TCPClient()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            string serverIPAddress = "127.0.0.1";
            
            string serverIPPort = "8090";

            endPoint = new IPEndPoint(IPAddress.Parse(serverIPAddress), int.Parse(serverIPPort));
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        public void ConnectServer()
        {
            try
            {
                clientSocket.Connect(endPoint);
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("找不到服务器，异常退出" + "错误码:" + e.ErrorCode);
                return;
            }

            string sendString=@"Hello Server......";
            byte[] sendbuffer = new byte[1024];
            sendbuffer = Encoding.ASCII.GetBytes(sendString);
            clientSocket.Send(sendbuffer);

            byte[] buffer = new byte[1024];
            int dataLength = clientSocket.Receive(buffer);
            string data = Encoding.ASCII.GetString(buffer, 0, dataLength);
            IPEndPoint remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            System.Console.WriteLine(remoteEndPoint.Address + ":" + remoteEndPoint.Port + " say :" + data);

            sendString = @"END";
            sendbuffer = Encoding.ASCII.GetBytes(sendString);
            clientSocket.Send(sendbuffer);

            clientSocket.Disconnect(false);
            clientSocket.Dispose();
        }
		
		public static void Main()
		{
			TCPClient tcpclient=new TCPClient();
			tcpclient.ConnectServer();
		}
		
    }
}
