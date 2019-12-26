using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkProject
{
    class Client
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

        private Boolean connected;
        private Socket clientSocket = null;
        private AsyncCallback fnReceiveHandler;
        private AsyncCallback fnSendHandler;

        public Client()
        {
            fnReceiveHandler = new AsyncCallback(handleDataReceive);
            fnSendHandler = new AsyncCallback(handleDataSend);
        }

        public Boolean Connected => connected;

        public void ConnectToServer(String hostName,UInt16 hostPort)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Boolean isConnected = false;
            try
            {
                clientSocket.Connect(hostName, hostPort);
                isConnected = true;
            }
            catch
            {
                isConnected = false;
            }

            if(isConnected)
            {
                AsyncObject asyncObject = new AsyncObject(4096);
                asyncObject.workingSocket = clientSocket;
                clientSocket.BeginReceive(asyncObject.buffer, 0, asyncObject.buffer.Length, SocketFlags.None, fnReceiveHandler, asyncObject);

                Console.WriteLine("연결 성공");
            }
            else
            {
                Console.WriteLine("연결 실패");
            }
        }

        public void StopClient()
        {
            clientSocket.Close();
        }

        public void SendMessage(String message)
        {
            AsyncObject asyncObject = new AsyncObject(1);
            asyncObject.buffer = Encoding.Unicode.GetBytes(message);
            asyncObject.workingSocket = clientSocket;

            try
            {
                clientSocket.BeginSend(asyncObject.buffer, 0, asyncObject.buffer.Length, SocketFlags.None, fnSendHandler, asyncObject);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"전송 중 오류 발생: {ex.Message}");
            }
        }

        private void handleDataReceive(IAsyncResult ar)
        {
            AsyncObject asyncObject = ar.AsyncState as AsyncObject;
            Int32 receiveBytes;

            try
            {
                receiveBytes = asyncObject.workingSocket.EndReceive(ar);
            }
            catch
            {
                return;
            }

            if(receiveBytes>0)
            {
                Byte[] msgByte = new Byte[receiveBytes];
                Array.Copy(asyncObject.buffer, msgByte, receiveBytes);

                Console.WriteLine($"메시지 받음: {Encoding.Unicode.GetString(msgByte)}");
            }

            try
            {
                asyncObject.workingSocket.BeginReceive(asyncObject.buffer, 0, asyncObject.buffer.Length, SocketFlags.None, fnReceiveHandler, asyncObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"자료 수신 대기 도중 오류 발생! {ex.Message}");
                return;
            }
        }

        private void handleDataSend(IAsyncResult ar)
        {
            var asyncObject = ar.AsyncState as AsyncObject;
            Int32 sentBytes;

            try
            {
                sentBytes = asyncObject.workingSocket.EndSend(ar);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"자료 송신 도중 오류: {ex.Message}");
                return;
            }

            if (sentBytes > 0)
            {
                Byte[] msgByte = new Byte[sentBytes];
                Array.Copy(asyncObject.buffer, msgByte, sentBytes);

                Console.WriteLine("메세지 보냄: {0}", Encoding.Unicode.GetString(msgByte));
            }
        }
    }
}
