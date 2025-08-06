using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Takkhal_Panel.Networking
{
    public class NetworkManager
    {
        private static TcpClient persistentClient;
        private static NetworkStream persistentStream;
        private static readonly object streamLock = new object();
        private static readonly int reconnectDelayMs = 3000;
        private static string lastKnownIp = "5.42.223.48";
        private static int lastKnownPort = 5000;
        private static bool isReconnecting = false;

        public static async Task<bool> ConnectPesistentClientAsync(string serverIp = "5.42.223.48", int port = 5000)
        {
            lastKnownIp = serverIp;
            lastKnownPort = port;

            try
            {
                if (persistentClient != null && persistentClient.Connected)
                    return true;

                persistentClient = new TcpClient();
                await persistentClient.ConnectAsync(serverIp, port);
                persistentStream = persistentClient.GetStream();

                Logger.Log("Connected to server.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Connection failed: " + ex.Message);
                return false;
            }
        }

        private static async Task<bool> TryReconnectAsync()
        {
            if (isReconnecting) return false;
            isReconnecting = true;

            Logger.Log("Trying to reconnect...");

            Disconnect();

            while (true)
            {
                bool result = await ConnectPesistentClientAsync(lastKnownIp, lastKnownPort);
                if (result)
                {
                    Logger.Log("Reconnected successfully.");
                    isReconnecting = false;
                    return true;
                }

                Logger.Log("Reconnect attempt failed. Retrying in 3 seconds...");
                await Task.Delay(reconnectDelayMs);
            }
        }

        public static async Task<string> SendReceiveAsync(string message, string expectedPrefix = null, int timeoutMs = 3000)
        {
            if (!IsConnected())
            {
                await TryReconnectAsync();
                if (!IsConnected())
                    return null;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");

                lock (streamLock)
                {
                    persistentStream.Write(data, 0, data.Length);
                    persistentStream.Flush();
                }

                var buffer = new byte[4096];
                var received = new StringBuilder();
                var timeoutTask = Task.Delay(timeoutMs);

                while (true)
                {
                    var readTask = persistentStream.ReadAsync(buffer, 0, buffer.Length);
                    var completed = await Task.WhenAny(readTask, timeoutTask);

                    if (completed == timeoutTask)
                    {
                        Logger.Log("Response timed out.");
                        return null;
                    }

                    int bytesRead = readTask.Result;
                    if (bytesRead <= 0) break;

                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    received.Append(chunk);

                    string[] lines = received.ToString().Split('\n', (char)StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        string trimmed = line.Trim();

                        if (string.IsNullOrWhiteSpace(expectedPrefix) || trimmed.StartsWith(expectedPrefix))
                        {
                            return trimmed;
                        }
                    }

                    // otherwise keep looping and collecting more
                }
            }
            catch (IOException ioex)
            {
                Logger.Log("IO error: " + ioex.Message);
                await TryReconnectAsync();
            }
            catch (Exception ex)
            {
                Logger.Log("Send/Receive exception: " + ex.Message);
            }

            return null;
        }


        public static bool IsConnected()
        {
            return persistentClient != null && persistentClient.Connected;
        }

        public static void Disconnect()
        {
            try
            {
                persistentStream?.Close();
                persistentClient?.Close();
                persistentStream = null;
                persistentClient = null;
            }
            catch (Exception ex)
            {
                Logger.Log("Disconnect error: " + ex.Message);
            }
        }

    
        public static async Task<int> GetConnectedClientCountAsync()
        {
            string response = await SendReceiveAsync("GET_COUNT");

            if (!string.IsNullOrEmpty(response) && response.StartsWith("CLIENT_COUNT:"))
            {
                if (int.TryParse(response.Substring("CLIENT_COUNT:".Length).Trim(), out int count))
                {
                    return count;
                }
            }

            return -1;
        }
    }
}
