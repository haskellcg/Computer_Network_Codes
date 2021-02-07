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
    /// TCP异步通信的客户端类
    /// </summary>
    class TCPClient
    {
        private Socket clientSocket;
        private IPEndPoint endPoint;
        private ManualResetEvent connectDone;
        private ManualResetEvent sendDone;
        private ManualResetEvent recieveDone;

        public TCPClient()
        {
            connectDone = new ManualResetEvent(false);
            sendDone = new ManualResetEvent(false);
            recieveDone = new ManualResetEvent(false);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string serverIPAddress = "127.0.0.1";

            string serverIPPort ="8090";


            endPoint = new IPEndPoint(IPAddress.Parse(serverIPAddress), int.Parse(serverIPPort));
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        public void ConnectServer()
        {
            clientSocket.BeginConnect(endPoint,new AsyncCallback(ATCPConnect),clientSocket);
            connectDone.WaitOne();

            string data = "Hello Server......";
            byte[] sendBuffer = new byte[1024];
            sendBuffer = Encoding.ASCII.GetBytes(data);
            clientSocket.BeginSend(sendBuffer,0,sendBuffer.Length,0,new AsyncCallback(ATCPSend),clientSocket);
            sendDone.WaitOne();

            StateObject state = new StateObject();
            state.workSocket = clientSocket;
            clientSocket.BeginReceive(state.buffer, 0, state.BufferSize(), 0, new AsyncCallback(ATCPRecieve),state);
            recieveDone.WaitOne();

            data = "END";
            sendBuffer = Encoding.ASCII.GetBytes(data);
            clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(ATCPSend), clientSocket);
            sendDone.WaitOne();
			
			clientSocket.Disconnect(false);
			clientSocket.Dispose();
			clientSocket.Close();
        }

        private void ATCPConnect(IAsyncResult iar)
        {
            Socket mySocket = (Socket)iar.AsyncState;
            try
            {
                mySocket.EndConnect(iar);
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("错误码:" + e.ErrorCode);
            }

            connectDone.Set();
        }

        private void ATCPSend(IAsyncResult iar)
        {
            Socket clientSocket = (Socket)iar.AsyncState;
            clientSocket.EndSend(iar);
            sendDone.Set();
        }

        private void ATCPRecieve(IAsyncResult iar)
        {
            StateObject state = (StateObject)iar.AsyncState;
            Socket clientSocket = state.workSocket;
            int dataLength = clientSocket.EndReceive(iar);
            state.sb.Clear();
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, dataLength));

            
            System.Console.WriteLine(endPoint.Address + ":" + endPoint.Port + " say :" + state.sb);
            recieveDone.Set();
        }
		
		public static void Main()
		{
			TCPClient tcpclient=new TCPClient();
			tcpclient.ConnectServer();
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
