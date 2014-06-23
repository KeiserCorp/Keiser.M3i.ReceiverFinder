using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Keiser.M3i.ReceiverFinder
{
    public partial class MainForm : Form
    {
        private Listener listener;
        private Lister lister;

        public MainForm()
        {
            InitializeComponent();
            listener = new Listener();
            lister = new Lister(listener, mainPanel);
        }
    }
}
