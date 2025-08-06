using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Takkhal_Panel.Networking
{
    public class TcpClientHandler : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;
        private string serverIp;
        private int serverPort;

        public bool IsConnected => client != null && client.Connected;

        public TcpClientHandler(string ip, int port)
        {
            serverIp = ip;
            serverPort = port;
        }

        public bool Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(serverIp, serverPort);
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                stream = client.GetStream();
                stream.ReadTimeout = 3000;
                DebugLog("Connected to server.");
                return true;
            }
            catch (SocketException ex)
            {
                DebugLog($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                DebugLog($"Exception: {ex.Message}");
            }

            return false;
        }

        public bool Send(string message)
        {
            if (!IsConnected || stream == null)
            {
                DebugLog("Send failed: Not connected.");
                return false;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                DebugLog($"Sent: {message}");
                return true;
            }
            catch (Exception ex)
            {
                DebugLog($"Send error: {ex.Message}");
                return false;
            }
        }

        public string Receive(int bufferSize = 1024)
        {
            if (!IsConnected || stream == null)
            {
                DebugLog("Receive failed: Not connected.");
                return null;
            }

            try
            {
                byte[] buffer = new byte[bufferSize];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                DebugLog($"Received: {response}");
                return response;
            }
            catch (Exception ex)
            {
                DebugLog($"Receive error: {ex.Message}");
                return null;
            }
        }

        public void Close()
        {
            if (stream != null)
                stream.Close();

            if (client != null)
                client.Close();

            stream = null;
            client = null;

            DebugLog("Connection closed.");
        }

        private void DebugLog(string msg)
        {
          
            Console.WriteLine("[TcpClientHandler] " + msg);
          
        }

        public void Dispose()
        {
            Close();
        }
    }
}
