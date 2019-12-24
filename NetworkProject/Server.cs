using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkProject
{
    class Server
    {
        public class AsyncObject
        {
            public Byte[] buffer;
            public Socket workingSocket;
            public AsyncObject(Int32 bufferSize)
            {
                this.buffer = new Byte[bufferSize];
            }
        }

        private Socket serverSocket = null;
        private AsyncCallback fnReceiveHandler;
        private AsyncCallback fnSendHandler;
        private AsyncCallback fnAcceptHandler;

        public void StartServer()
        {
            fnReceiveHandler = new AsyncCallback(handleDataReceive);
            fnSendHandler = new AsyncCallback(handleDataSend);
            fnAcceptHandler = new AsyncCallback(handleClientConnectionRequest);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1234));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(fnAcceptHandler, null);
        }

        public void StopServer()
        {
            serverSocket.Close();
        }

        public void SendMessage(String message)
        {
            AsyncObject asyncObject = new AsyncObject(1);
            asyncObject.buffer = Encoding.Unicode.GetBytes(message);
            asyncObject.workingSocket = serverSocket;
            serverSocket.BeginSend(asyncObject.buffer, 0, asyncObject.buffer.Length, SocketFlags.None, fnSendHandler, asyncObject);
        }

        private void handleClientConnectionRequest(IAsyncResult ar)
        {
            Socket sockClient = serverSocket.EndAccept(ar);
            AsyncObject asyncObject = new AsyncObject(4096);
            asyncObject.workingSocket = sockClient;
            sockClient.BeginReceive(asyncObject.buffer, 0, asyncObject.buffer.Length, SocketFlags.None, fnReceiveHandler, asyncObject);
        }

        private void handleDataReceive(IAsyncResult ar)
        {
            AsyncObject asyncObject = ar.AsyncState as AsyncObject;
            Int32 receiveBytes = asyncObject.workingSocket.EndReceive(ar);

            if (receiveBytes > 0)
                Console.WriteLine($"메세지 받음: {Encoding.Unicode.GetString(asyncObject.buffer)}");

            asyncObject.workingSocket.BeginReceive(asyncObject.buffer, 0, asyncObject.buffer.Length, SocketFlags.None, fnReceiveHandler, asyncObject);
        }

        private void handleDataSend(IAsyncResult ar)
        {
            AsyncObject asyncObject = ar.AsyncState as AsyncObject;

            Int32 sentBytes = asyncObject.workingSocket.EndSend(ar);

            if (sentBytes > 0)
                Console.WriteLine($"메세지 보냄: {Encoding.Unicode.GetString(asyncObject.buffer)}");
        }
    }
}
