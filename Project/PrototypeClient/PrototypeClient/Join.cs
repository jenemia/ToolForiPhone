using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PacketNamespace;
using DBAdapterNamespace;
using ServerAdapterNamespace;

namespace PrototypeClient
{
    public partial class Join : Form
    {
        public Join()
        {
            InitializeComponent();
        }

        private void buttonJoin_Click(object sender, EventArgs e)
        {
            String _id = textBoxID.Text;
            String _pw = textBoxPasswd.Text;
            if (_id.Equals("") || _pw.Equals(""))
                MessageBox.Show("ID or Password 입력하세요");

        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {

        }
    }
    public class Singleton
    {
        static public ServerAdapter mServerAdapter;
        public Singleton()
        {
            mServerAdapter = new ServerAdapter();
        }
        public ServerAdapter ServerAdapter
        {
            get { return mServerAdapter; }
        }
    }
}
