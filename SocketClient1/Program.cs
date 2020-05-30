using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient1
{
    class Program
    {

        static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();
            Socket socket = tcpClient.Client;
            string str = "Hello world!";
            try
            { // sends the text with timeout 10s
                Send(socket, Encoding.UTF8.GetBytes(str), 0, str.Length, 10000);
            }
            catch (Exception ex) { /* ... */ }

            //TcpClient tcpClient = new TcpClient();
            //Socket socket = tcpClient.Client;
            //byte[] buffer = new byte[12];  // length of the text "Hello world!"
            //try
            //{ // receive data with timeout 10s
            //    Receive(socket, buffer, 0, buffer.Length, 10000);
            //    string str = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            //}
            //catch (Exception ex) { /* ... */ }

        }

        public static void Receive(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            int startTickCount = Environment.TickCount;
            int received = 0;  // how many bytes is already received
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (received < size);
        }


        public static void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            int startTickCount = Environment.TickCount;
            int sent = 0;  // how many bytes is already sent
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (sent < size);
        }
    }
}