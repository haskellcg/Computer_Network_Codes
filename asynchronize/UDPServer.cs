using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace NetworkTest.asynchronize
{
    /// <summary>
    /// UDP异步通信的服务器端
    /// </summary>
    class UDPServer
    {
        private Socket serverSocket;
        private IPEndPoint endPoint;
        private IPEndPoint remoteEndPoint;

        private ManualResetEvent sendDone;
        private ManualResetEvent recieveDone;

        private DateTime startTime;
        private DateTime endTime;

        public UDPServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),8090);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0);

            sendDone = new ManualResetEvent(false);
            recieveDone = new ManualResetEvent(false);
        }

        public void StartServer()
        {
            serverSocket.Bind(endPoint);
			System.Console.WriteLine("服务器开启，等待数据.........");

            startTime = DateTime.Now;

            UDPState state=new UDPState();
            state.workSocket = serverSocket;
            state.endPoint=(EndPoint)remoteEndPoint;
            serverSocket.BeginReceiveFrom(state.buffer,0,state.BufferSize(),0,ref state.endPoint,new AsyncCallback(AUDPRecieve),state);
            recieveDone.WaitOne();

            string sendData = "I Have Got It!";
            byte[] sendBuffer = new byte[1024];
            sendBuffer = Encoding.ASCII.GetBytes(sendData);
            serverSocket.BeginSendTo(sendBuffer,0,sendBuffer.Length,0,state.endPoint,new AsyncCallback(AUDPSend),serverSocket);
            sendDone.WaitOne();

            state = new UDPState();
			recieveDone.Reset();
            state.workSocket = serverSocket;
            state.endPoint = (EndPoint)remoteEndPoint;
            serverSocket.BeginReceiveFrom(state.buffer, 0, state.BufferSize(), 0, ref state.endPoint, new AsyncCallback(AUDPRecieve), state);
            recieveDone.WaitOne();

            endTime = DateTime.Now;

            TimeSpan span = endTime - startTime;

            System.Console.WriteLine("通信结束..........");

            System.Console.WriteLine("使用时间（微秒）:"+span.TotalMilliseconds);
        }

        private void AUDPRecieve(IAsyncResult iar)
        {
            UDPState state = (UDPState)iar.AsyncState;
            Socket mySocket = state.workSocket;
            int dataLength=mySocket.EndReceiveFrom(iar,ref state.endPoint);
            state.data = Encoding.ASCII.GetString(state.buffer,0,dataLength);

			
			
            System.Console.WriteLine(state.endPoint.ToString()+ " say :" + state.data);



            recieveDone.Set();
        }

        private void AUDPSend(IAsyncResult iar)
        {
            Socket mySocket = (Socket)iar.AsyncState;
            mySocket.EndSendTo(iar);
            sendDone.Set();
        }
		
		public static void Main()
		{
			UDPServer udpserver=new UDPServer();
			udpserver.StartServer();
		}
    }

    class UDPState
    {
        public Socket workSocket=null;
        public EndPoint endPoint=null;
        public const int bufferSize = 1024;
        public byte[] buffer = new byte[bufferSize];
        public string data="";

        public int BufferSize()
        {
            return bufferSize;
        }
    }
}
