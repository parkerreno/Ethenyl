using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Networking.Sockets;

namespace SoftServe.PCL
{
    public class SocketListener
    {
        private StreamSocketListener listener;
        public event EventHandler<Queue<string>> ConnectionReceived;
        public SocketListener(string endpointName)
        {
            SetupListener("5452");
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            using (var recvStr = args.Socket.InputStream.AsStreamForRead())
            {
                using (var reader = new StreamReader(recvStr))
                {
                    Queue<string> receivedStuff = new Queue<string>();

                    bool go = true;

                    do
                    {
                        var line = reader.ReadLine();
                        if (line.ToUpper().Equals("ENDTRANSMISSION") || string.IsNullOrWhiteSpace(line))
                        {
                            go = false;
                            continue;
                        }
                        receivedStuff.Enqueue(line);
                    } while (go);

                    //while (reader != null && !reader.EndOfStream)
                    //{
                    //    receivedStuff.Enqueue(reader.ReadLine());
                    //}

                    ConnectionReceived?.Invoke(sender, receivedStuff);
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
