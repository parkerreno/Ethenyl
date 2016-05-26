using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Networking.Sockets;

namespace SoftServe.PCL
{
    /// <summary>
    /// This is mainly a workaround for the screwy behavior of WPF, ClickOnce and WinRT APIS - expect it to change
    /// </summary>
    public class SocketListener
    {
        private StreamSocketListener listener;
        public event EventHandler<Queue<string>> ConnectionReceived;
        public SocketListener(string socket)
        {
            SetupListener(socket);
        }

        /// <summary>
        /// New connection received
        /// </summary>
        private async void Listener_ConnectionReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            using (var recvStr = args.Socket.InputStream.AsStreamForRead())
            {
                using (var reader = new StreamReader(recvStr))
                {
                    try
                    {
                        Queue<string> receivedStuff = new Queue<string>();

                        bool go = true;

                        do
                        {
                            var line = await reader.ReadLineAsync();
                            if (line.ToUpper().Equals("ENDTRANSMISSION") || string.IsNullOrWhiteSpace(line))
                            {
                                go = false;
                                continue;
                            }
                            receivedStuff.Enqueue(line);
                        } while (go);
                        ConnectionReceived?.Invoke(sender, receivedStuff);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                       
                    }
                }
            }
        }

        private async void SetupListener(string endpointName)
        {
            listener = new StreamSocketListener();
            listener.ConnectionReceived += Listener_ConnectionReceived;
            await listener.BindServiceNameAsync(endpointName);
        }
    }
}
