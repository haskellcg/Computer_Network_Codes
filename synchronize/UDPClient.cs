using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkTest.synchronize
{
    /// <summary>
    /// UDP同步通信的客户端
    /// </summary>
    class UDPClient
    {
        private Socket clientSocket;
        private IPEndPoint endPoint;

        public UDPClient()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
            string serverIPAddress = "127.0.0.1";

            string serverIPPort ="8090";

            endPoint = new IPEndPoint(IPAddress.Parse(serverIPAddress),int.Parse(serverIPPort));
        }

        public void ConnectServer()
        {
            string sendData = @"Hello Server......";
            byte[] sendbuffer = new byte[1024];
            sendbuffer = Encoding.ASCII.GetBytes(sendData);
            clientSocket.SendTo(sendbuffer,endPoint);

            byte[] buffer = new byte[1024];
            EndPoint UdpEP = (EndPoint)endPoint;
            int dataLength = clientSocket.ReceiveFrom(buffer, ref UdpEP);
            string data = Encoding.ASCII.GetString(buffer, 0, dataLength);
            System.Console.WriteLine(UdpEP.ToString() + " say :" + data);

            sendData = @"END";
            sendbuffer = Encoding.ASCII.GetBytes(sendData);
            clientSocket.SendTo(sendbuffer, UdpEP);

            clientSocket.Close();
        }
		
		public static void Main()
		{
			UDPClient udpclient=new UDPClient();
			udpclient.ConnectServer();
		}
    }
}
