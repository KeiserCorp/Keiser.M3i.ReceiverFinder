using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace Keiser.M3i.ReceiverFinder
{
    class Lister
    {
        private Thread thread;
        private bool stop = false;
        private Listener listener;
        private List<IPAddress> ipAddresses = new List<IPAddress>();
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private Panel mainPanel;

        public Lister(Listener listener, Panel mainPanel)
        {
            this.listener = listener;
            this.mainPanel = mainPanel;
            thread = new Thread(worker);
            thread.Start();
        }

        public void Dispose()
        {
            stop = true;
        }

        public void worker()
        {
            while (!stop)
            {
                IEnumerable<IPAddress> diffList = listener.ipAddresses.Except(ipAddresses);
                foreach(IPAddress ip in diffList)
                {
                    ipAddresses.Add(ip);
                    addIp(ip);
                }
            }
        }

        private void addIp(IPAddress ipAddress)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate() { addIp(ipAddress); });
            }
            else
            {
                Debug.WriteLine(ipAddress);
                Button button = new Button();
                button.Text = ipAddress.ToString();
                int offset = ipAddresses.Count;
                button.Location = new System.Drawing.Point(1, (offset - 1) * 26);
                button.Size = new System.Drawing.Size(124, 25);
                button.Click += new System.EventHandler(ipAddressButton_Click);
                mainPanel.Controls.Add(button);
            }
        }

        public void ipAddressButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ProcessStartInfo sInfo = new ProcessStartInfo("http://" + button.Text + ":80");
            Process.Start(sInfo);
        }

    }
}
