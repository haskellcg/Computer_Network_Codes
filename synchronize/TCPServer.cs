using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkTest.synchronize
{
    /// <summary>
    /// TCP同步通信的服务类
    /// </summary>
    class TCPServer
    {

        private Socket serverSocket;
        private IPEndPoint endPoint;
        private int listenNum; 

        public TCPServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            string serverIPAddress="127.0.0.1";
            
            string serverIPPort="8090";
            

            endPoint = new IPEndPoint(IPAddress.Parse(serverIPAddress),int.Parse(serverIPPort));

            listenNum = 20;
        }

        /// <summary>
        /// 开启服务器
        /// </summary>
        public void StartServer()
        {
            serverSocket.Bind(endPoint);
            serverSocket.Listen(listenNum);
            System.Console.WriteLine("服务器启动，等待客户端的连接...........");
            Socket clientSocket = serverSocket.Accept();

            DateTime startTime = DateTime.Now;

            byte[] buffer = new byte[1024];
            int dataLength=clientSocket.Receive(buffer);
            string data = Encoding.ASCII.GetString(buffer,0,dataLength);
            IPEndPoint remoteEndPoint=(IPEndPoint)clientSocket.RemoteEndPoint;
            System.Console.WriteLine(remoteEndPoint.Address+":"+remoteEndPoint.Port+" say :"+data);

            string sendData = @"I Have Got It!";
            byte[] sendbuffer = new byte[1024];
            sendbuffer = Encoding.ASCII.GetBytes(sendData);
            clientSocket.Send(sendbuffer);

            dataLength = clientSocket.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, dataLength);
            System.Console.WriteLine(remoteEndPoint.Address + ":" + remoteEndPoint.Port + " say :" + data);

            System.Console.WriteLine("通信结束..........");

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime - startTime;

            System.Console.WriteLine("使用时间（微秒）:"+span.TotalMilliseconds);

            clientSocket.Dispose();
        }
		
		public static void Main()
		{
			TCPServer tcpserver=new TCPServer();
			tcpserver.StartServer();
		}
    }
}
