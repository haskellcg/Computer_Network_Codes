using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkTest.asynchronize
{
    /// <summary>
    /// TCP异步通信的服务器端
    /// </summary>
    public class TCPServer
    {
        private Socket serverSocket;
        private IPEndPoint endPoint;
        private int listenNum;
        private ManualResetEvent allDone;
        private DateTime startTime;
        private DateTime endTime;

        public TCPServer()
        {
            allDone = new ManualResetEvent(false);
            serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            string serverIPAddress = "127.0.0.1";

            string serverIPPort = "8090";


            endPoint = new IPEndPoint(IPAddress.Parse(serverIPAddress), int.Parse(serverIPPort));

            listenNum = 20;

        }

        public void StartServer()
        {
            serverSocket.Bind(endPoint);
            serverSocket.Listen(listenNum);

			while(true)
            {
				allDone.Reset();
				System.Console.WriteLine("服务器开启，等待连接..........");
				serverSocket.BeginAccept(new AsyncCallback(ATCPAccept),serverSocket);
				allDone.WaitOne();
			}
			
            
        }

        private void ATCPAccept(IAsyncResult iar)
        {
		
            Socket mySocket = (Socket)iar.AsyncState;
            Socket clientSocket = mySocket.EndAccept(iar);

            IPEndPoint endPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            System.Console.WriteLine(endPoint.Address + ":" + endPoint.Port + " 连接到服务器");
            startTime = DateTime.Now;


            StateObject state = new StateObject();
            state.workSocket = clientSocket;
            clientSocket.BeginReceive(state.buffer,0,state.BufferSize(),0,new AsyncCallback(ATCPRecieve),state);
		}

        private void ATCPRecieve(IAsyncResult iar)
        {
            StateObject state = (StateObject)iar.AsyncState;
            Socket clientSocket = state.workSocket;

            int dataLength = clientSocket.EndReceive(iar);
            state.sb.Clear();
            state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,dataLength));

            IPEndPoint remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            System.Console.WriteLine(remoteEndPoint.Address+":"+remoteEndPoint.Port+" say :"+state.sb);	
            if (state.sb.ToString().Equals("END"))
            {
				clientSocket.Disconnect(false);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                endTime = DateTime.Now;

                TimeSpan span = endTime - startTime;
                System.Console.WriteLine("通信结束.........");
                System.Console.WriteLine("使用时间（微秒）:"+span.TotalMilliseconds);
				
				allDone.Set();
            }
            else
            {
                string sendData = "I Have Got It!";
                byte[] sendBuffer = new byte[1024];
                sendBuffer=Encoding.ASCII.GetBytes(sendData);
                clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(ATCPSend), clientSocket);
			}
			
        }

        private void ATCPSend(IAsyncResult iar)
        {
            Socket clientSocket = (Socket)iar.AsyncState;
            int dataLength = clientSocket.EndSend(iar);
            StateObject state = new StateObject();
            state.workSocket = clientSocket;

            clientSocket.BeginReceive(state.buffer, 0, state.BufferSize(), 0, new AsyncCallback(ATCPRecieve), state);
        }
		
		public static void Main()
		{
			TCPServer tcpserver=new TCPServer();
			tcpserver.StartServer();
		}
    }

    /// <summary>
    /// 建立一个构造体，便于数据的传递
    /// </summary>
    public class StateObject
    {
        // Client socket.     
        public Socket workSocket = null;
        // Size of receive buffer.     
        public const int bufferSize = 1024;
        // Receive buffer.     
        public byte[] buffer = new byte[bufferSize];
        // Received data string.     
        public StringBuilder sb = new StringBuilder();

        public int BufferSize()
        {
            return bufferSize;
        }
    }
}
