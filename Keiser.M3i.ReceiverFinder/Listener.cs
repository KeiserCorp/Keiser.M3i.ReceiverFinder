using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Keiser.M3i.ReceiverFinder
{
    class Listener
    {
        public List<IPAddress> ipAddresses = new List<IPAddress>();
        private Thread thread;
        private bool stop = false;
        public string ipAddress = "239.10.10.10";
        public UInt16 ipPort = 35680;

        public Listener() {
            thread = new Thread(worker);
            thread.Start();
        }

        public void Dispose() {
            stop = true;
        }

        private void worker()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, ipPort);
            EndPoint remoteEndPoint = (EndPoint)ipEndPoint;
            socket.Bind(ipEndPoint);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(ipAddress)));
            byte[] receivedData = new byte[1024];
            while(!stop)
            {
                if (socket.Poll(200000, SelectMode.SelectRead))
                {
                    socket.ReceiveFrom(receivedData, ref remoteEndPoint);
                    if (!ipAddresses.Contains(((IPEndPoint)remoteEndPoint).Address))
                        ipAddresses.Add(((IPEndPoint)remoteEndPoint).Address);
                    receivedData = new byte[1024];
                }
            }
        }

    }
}
