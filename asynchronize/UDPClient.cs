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
    /// UDP异步通信的客户端
    /// </summary>
    class UDPClient
    {
        private Socket clientSocket;
        private IPEndPoint endPoint;

        private ManualResetEvent sendDone;
        private ManualResetEvent recieveDone;

        public UDPClient()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8090);

            sendDone = new ManualResetEvent(false);
            recieveDone = new ManualResetEvent(false);
        }

        public void ConnectServer()
        {
            string sendData = "Hello Server......";
            byte[] sendBuffer = new byte[1024];
            sendBuffer = Encoding.ASCII.GetBytes(sendData);
            clientSocket.BeginSendTo(sendBuffer, 0, sendBuffer.Length, 0,(EndPoint)endPoint, new AsyncCallback(AUDPSend), clientSocket);
            sendDone.WaitOne();

            UDPState state = new UDPState();
            state.workSocket = clientSocket;
            state.endPoint = (EndPoint)(new IPEndPoint(IPAddress.Parse("0.0.0.0"),0));
            clientSocket.BeginReceiveFrom(state.buffer, 0, state.BufferSize(), 0, ref state.endPoint, new AsyncCallback(AUDPRecieve), state);
            recieveDone.WaitOne();

			sendDone.Reset();
            string sendData2 = "END";
            byte[] sendBuffer2 = new byte[1024];
            sendBuffer2 = Encoding.ASCII.GetBytes(sendData2);
            clientSocket.BeginSendTo(sendBuffer2, 0, sendBuffer2.Length, 0,(EndPoint)endPoint, new AsyncCallback(AUDPSend), clientSocket);
            sendDone.WaitOne();
        }

        private void AUDPSend(IAsyncResult iar)
        {
            Socket mySocket = (Socket)iar.AsyncState;
            mySocket.EndSendTo(iar);
            sendDone.Set();
        }

        private void AUDPRecieve(IAsyncResult iar)
        {
            UDPState state = (UDPState)iar.AsyncState;
            Socket mySocket = state.workSocket;
            int dataLength = mySocket.EndReceiveFrom(iar, ref state.endPoint);
            state.data = Encoding.ASCII.GetString(state.buffer,0,dataLength);

            System.Console.WriteLine(state.endPoint.ToString() + " say :" + state.data);



            recieveDone.Set();
        }
		
		
		public static void Main()
		{
			UDPClient udpclient=new UDPClient();
			udpclient.ConnectServer();
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
