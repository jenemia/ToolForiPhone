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
        Singleton mSingleton;
        public Join()
        {
            InitializeComponent();
            this.mSingleton = new Singleton();
        }

        private void buttonJoin_Click(object sender, EventArgs e)
        {
            String _id = textBoxID.Text;
            String _pw = textBoxPasswd.Text;
            if (_id.Equals("") || _pw.Equals(""))
                MessageBox.Show("ID or Password 입력하세요");


            JoinPacket _join = new JoinPacket();
            _join.State = (int)accountState.join;
            _join.JoinID = _id;
            _join.JoinPW = _pw;

            this.mSingleton.ServerAdapter.Send(_join);

            Packet _result;
            while(true)
            {
                _result = (Packet)this.mSingleton.ServerAdapter.Receive();
                if ((int)accountState.join == _result.State || (int)accountState.error == _result.State)
                    break;
            }

            if( (int)state.error == _result.State )
            {
                MessageBox.Show("회원 가입 실패, 다시 입력하세요");
            }
            else
            {
                ActiveForm.Visible = false;
                Client dlg = new Client();
                dlg.ShowDialog();
                this.Close();
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            String _id = textBoxID.Text;
            String _pw = textBoxPasswd.Text;
            if (_id.Equals("") || _pw.Equals(""))
                MessageBox.Show("ID or Password 입력하세요");


            JoinPacket _login = new JoinPacket();
            _login.State = (int)accountState.login;
            _login.JoinID = _id;
            _login.JoinPW = _pw;

            this.mSingleton.ServerAdapter.Send(_login);

            Packet _result;
            while (true)
            {
                _result = (Packet)this.mSingleton.ServerAdapter.Receive();
                if ((int)accountState.login == _result.State || (int)state.error == _result.State)
                    break;
            }

            if ((int)accountState.error == _result.State)
            {
                MessageBox.Show("id,passwd 확인 또는 이미 로그인한 id가 존재");
            }
            else
            {
                ActiveForm.Visible = false;
                Client dlg = new Client();
                dlg.ShowDialog();
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Join_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.mSingleton.ServerAdapter.ExitServer();
        }
    }
    public class Singleton
    {
        static public ServerAdapter mServerAdapter;
        public Singleton()
        {
            mServerAdapter = new ServerAdapter();
        }

        ~Singleton()
        {
        }
        public ServerAdapter ServerAdapter
        {
            get { return mServerAdapter; }
        }
    }
}
