using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkTest.synchronize
{
    /// <summary>
    /// UCP同步通信的服务器类
    /// </summary>
    class UDPServer
    {
        private Socket serverSocket;
        private IPEndPoint endPoint;
        private IPEndPoint remoteEndPoint;

        public UDPServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);

            string serverIPAddress = "127.0.0.1";

            string serverIPPort = "8090";
            
            string clientIPAddress = "0.0.0.0";

            string clientIPPort = "0";

            remoteEndPoint = new IPEndPoint(IPAddress.Parse(clientIPAddress), int.Parse(clientIPPort));
            endPoint = new IPEndPoint(IPAddress.Parse(serverIPAddress),int.Parse(serverIPPort));
        }

        public void StartServer()
        {
            serverSocket.Bind(endPoint);

            System.Console.WriteLine("服务器开启，等待通信..........");

            DateTime startTime = DateTime.Now;

            byte[] buffer = new byte[1024];
            EndPoint UdpEP=(EndPoint)remoteEndPoint;
            int dataLength = serverSocket.ReceiveFrom(buffer,ref UdpEP);
            string data = Encoding.ASCII.GetString(buffer,0,dataLength);

            System.Console.WriteLine(UdpEP.ToString() + " say :" + data);

            string sendData = @"I Have Got It!";
            byte[] sendbuffer = new byte[1024];
            sendbuffer = Encoding.ASCII.GetBytes(sendData);
            serverSocket.SendTo(sendbuffer,UdpEP);

            dataLength = serverSocket.ReceiveFrom(buffer, ref UdpEP);
            data = Encoding.ASCII.GetString(buffer, 0, dataLength);

            System.Console.WriteLine(UdpEP.ToString() + " say :" + data);

            System.Console.WriteLine("通信结束........");

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime - startTime;

            System.Console.WriteLine("使用时间（微秒）:" + span.TotalMilliseconds);
        }
		
		public static void Main()
		{
			UDPServer udpserver=new UDPServer();
			udpserver.StartServer();
		}
    }
}
