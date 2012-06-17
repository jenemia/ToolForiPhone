using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Runtime.Serialization;
using PacketNamespace;
using DBAdapterNamespace;

namespace PrototypeClient
{
    public partial class Client : Form
    {
        private Socket mClient;

        private ArrayList mLeftTower;
        private ArrayList mRightTower;
        private ArrayList mLeftEnemy;
        private ArrayList mRightEnemy;
        private ArrayList mUser;
        private int mPlayer;

        private Packet mMyPacket;
        private Packet mYourPacket;

        private Thread mThreadSend;
        private Thread mThreadReceive;
        private Thread mThreadStart;

        public Client()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.mLeftTower = new ArrayList();
            this.mRightTower = new ArrayList();
            this.mLeftEnemy = new ArrayList();
            this.mRightEnemy = new ArrayList();
            this.mUser = new ArrayList();

            this.mMyPacket = new Packet();
            this.mYourPacket = new Packet();
            //Server에 접속
            TcpClient _tcp = new TcpClient();
            _tcp.Connect("127.0.0.1", 8080);
            this.mClient = _tcp.Client;

            //현재 Client가 몇 번째 Player인지 알아내기
            SetPlayer();
            //타워,적,유저 초기화
            SetObjects();

            timer1.Interval = 1000;

            this.mThreadReceive = new Thread(new ThreadStart(StartReceive));
            this.mThreadSend = new Thread(new ThreadStart(StarteSend));

            //1,2Player가 모두 접속 후에 시작하기 위해 기다리는 메소드
            this.mThreadStart = new Thread(new ThreadStart(StartWait));
            this.mThreadStart.Start();
        }

        //서버 접속 순서대로 플레이어 정함
        private void SetPlayer()
        {
            NetworkStream _ns = new NetworkStream(this.mClient);
            byte[] _buffer = new byte[1024 * 4];
            _buffer = Packet.Serialize(this.mMyPacket);
            _ns.Write(_buffer, 0, _buffer.Length);
            _buffer.Initialize();
            this.mMyPacket.InitPacket();

            Thread.Sleep(10);

            _buffer = new byte[1024 * 4];
            _ns.Read(_buffer, 0, _buffer.Length);
            this.mMyPacket = (Packet)Packet.Deserialize(_buffer);
            this.mPlayer = this.mMyPacket.Player;
            /* 
             * 리스트 초기화 하는 부분. 
            this.mLeftTower = this.mMyPacket.TowerList;
            this.mRightTower = this.mMyPacket.TowerList;
            this.mLeftEnemy = this.mMyPacket.EnemyList;
            this.mRightEnemy = this.mMyPacket.EnemyList;
            this.mUser = this.mMyPacket.User;
            */

            this.mMyPacket.InitPacket();
            _ns.Close();
            _buffer.Initialize();
        }

        //초기화
        //위에서 리스트 초기화 하면 이 부분은 필요 없음
        private void SetObjects()
        {
            Tower _tower = new Tower(1, 40, 200);
            this.mLeftTower.Add(_tower);
            Enemy _enemy = new Enemy(1, 100, 200);
            this.mLeftEnemy.Add(_enemy);

            _tower = new Tower(1, 190, 200);
            this.mRightTower.Add(_tower);
            _enemy = new Enemy(1, 250, 200);
            this.mRightEnemy.Add(_enemy);

            User _user = new User(1000, 1000);
            this.mUser.Add(_user);
            _user = new User(1000, 2000);
            this.mUser.Add(_user);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics _g = e.Graphics;

            SolidBrush _brush = new SolidBrush(Color.Blue);
            foreach (Tower _to in this.mLeftTower)
            {
                _g.FillRectangle(_brush, _to.X, _to.Y, 50, 50);
            }
            foreach (Enemy _en in this.mLeftEnemy)
            {
                _g.FillRectangle(_brush, _en.X, _en.Y, 25, 25);
            }

            _brush = new SolidBrush(Color.Yellow);
            foreach (Tower _to in this.mRightTower)
            {
                _g.FillRectangle(_brush, _to.X, _to.Y, 50, 50);
            }
            foreach (Enemy _en in this.mRightEnemy)
            {
                _g.FillRectangle(_brush, _en.X, _en.Y, 25, 25);
            }
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.mThreadReceive.IsAlive)
                this.mThreadReceive.Abort();
            if (this.mThreadSend.IsAlive)
                this.mThreadSend.Abort();
            if (this.mThreadSend.IsAlive)
                this.mThreadStart.Abort();
            this.mClient.Close();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics _g = this.CreateGraphics();
            _g.Clear(Color.White);

            if (this.mPlayer == 1)
            {
                for (int i = 0; i < this.mLeftTower.Count; i++)
                {
                    ((Tower)this.mLeftTower[i]).Y += 10;
                }
                for (int i = 0; i < this.mLeftEnemy.Count; i++)
                {
                    ((Enemy)this.mLeftEnemy[i]).Y += 10;
                }
            }
            else if (this.mPlayer == 2)
            {
                for (int i = 0; i < this.mRightTower.Count; i++)
                {
                    ((Tower)this.mRightTower[i]).Y -= 10;
                }
                for (int i = 0; i < this.mRightEnemy.Count; i++)
                {
                    ((Enemy)this.mRightEnemy[i]).Y -= 10;
                }
            }
            Invalidate();
        }

        /* 서버에게 패킷을 받아 오는 부분
         * 받은 패킷의 player가 1player라면 왼쪽, 
         * plyaer가 2라면 오른쪽으로 뿌려준다.
         * 1번 client 2번 client 둘다 왼쪽은 1plyaer , 오른쪽은 2plyaer
         */
        private void StartReceive()
        {
            byte[] _buffer = new byte[1024 * 4];
            NetworkStream _ns = new NetworkStream(this.mClient);
            while (true)
            {
                try
                {
                    _ns.Read(_buffer, 0, _buffer.Length);
                    this.mYourPacket.InitPacket();
                    this.mYourPacket = (Packet)Packet.Deserialize(_buffer);

                    if (this.mYourPacket.Player == 1) // 받아온 패킷을 보낸 player가 1p일 때
                    {
                        for (int i = 0; i < this.mYourPacket.TowerList.Count; i++)
                        {
                            ((Tower)this.mLeftTower[i]).Y = ((Tower)this.mYourPacket.TowerList[i]).Y;
                        }
                        for (int i = 0; i < this.mYourPacket.EnemyList.Count; i++)
                        {
                            ((Enemy)this.mLeftEnemy[i]).Y = ((Enemy)this.mYourPacket.EnemyList[i]).Y;
                        }
                    }
                    else if (this.mYourPacket.Player == 2) // 받아온 패킷을 보낸 player가 2p일 때
                    {
                        for (int i = 0; i < this.mYourPacket.TowerList.Count; i++)
                        {
                            ((Tower)this.mRightTower[i]).Y = ((Tower)this.mYourPacket.TowerList[i]).Y;
                        }
                        for (int i = 0; i < this.mYourPacket.EnemyList.Count; i++)
                        {
                            ((Enemy)this.mRightEnemy[i]).Y = ((Enemy)this.mYourPacket.EnemyList[i]).Y;
                        }
                    }
                    _buffer.Initialize();
                    _ns.Flush();
                    this.mYourPacket.InitPacket();
                }
                catch (SerializationException)
                {

                }
                catch (NullReferenceException)
                {
                }
                catch (IOException)
                {
                }
            }
        }

        /*패킷을 서버에게 보내는 부분.
         * 객체의 데이터가 바뀌었을 때 다른 플레이어게 보여줘야 하는 부분을 
         * 이 method를 통하여 서버에 보내면, 다른 플레이어게도 
         * 객체의 정보가 넘어가서 화면에 그려진다.
         */
        private void StarteSend()
        {
             byte[] _buffer = new byte[1024 * 4];
            NetworkStream _ns = new NetworkStream(this.mClient);
            while (true)
            {
                Thread.Sleep(10);
                this.mMyPacket.InitPacket();
                try
                {
                    if (this.mPlayer == 1)
                    {
                        for (int i = 0; i < this.mLeftTower.Count; i++)
                        {
                            this.mMyPacket.TowerList.Add(new Tower());
                            ((Tower)this.mMyPacket.TowerList[i]).Y = ((Tower)this.mLeftTower[i]).Y;
                        }
                        for (int i = 0; i < this.mLeftEnemy.Count; i++)
                        {
                            this.mMyPacket.EnemyList.Add(new Enemy());
                            ((Enemy)this.mMyPacket.EnemyList[i]).Y = ((Enemy)this.mLeftEnemy[i]).Y;
                        }
                    }
                    else if (this.mPlayer == 2)
                    {
                        for (int i = 0; i < this.mRightTower.Count; i++)
                        {
                            this.mMyPacket.TowerList.Add(new Tower());
                            ((Tower)this.mMyPacket.TowerList[i]).Y = ((Tower)this.mRightTower[i]).Y;
                        }
                        for (int i = 0; i < this.mRightEnemy.Count; i++)
                        {
                            this.mMyPacket.EnemyList.Add(new Enemy());
                            ((Enemy)this.mMyPacket.EnemyList[i]).Y = ((Enemy)this.mRightEnemy[i]).Y;
                        }
                    }
                    this.mMyPacket.Player = this.mPlayer;

                    _buffer = Packet.Serialize(this.mMyPacket);
                    _ns.Write(_buffer, 0, _buffer.Length);
                    _ns.Flush();
                    this.mMyPacket.InitPacket();
                }
                catch (NullReferenceException)
                {
                }
                catch (InvalidOperationException)
                {
                    //   NetworkStream _netStream = this.mClient.GetStream(); 에러
                }
                catch (IOException)
                {
                }
            }
        }

        /* 두 플레이어가 모두 접속할 때 까지 기다리기 위함
         * while문을 통해 기다리다가 서버를 통해 들어온 패킷에
         * bool형 Start이 true일 때 client는 시작을 하게 된다.
         * StartWait method가 쓰레드고 시작하였기 때문에 여기서 쓰레드를 생성하지 않고 시작만 한다.
         * Timer는 주 스레드에 있는 Object이기 때문에 Invoke로 실행
         */
        private void StartWait()
        {
            byte[] _buffer = new byte[1024 * 4];
            NetworkStream _ns = new NetworkStream(this.mClient);

            while (true)
            {
                Thread.Sleep(10);

                _ns.Read(_buffer, 0, _buffer.Length);
                this.mYourPacket.InitPacket();
                this.mYourPacket = (Packet)Packet.Deserialize(_buffer);

                if (this.mYourPacket.Start) // 게임 시작이 되었다는 것을 서버에서 보낼 때 탈출
                    break;
                
            }
            this.mYourPacket.InitPacket();
            this.mMyPacket.InitPacket();
            
            //쓰레드 생성하고 시작
            mThreadReceive.Start();
            mThreadSend.Start();

            //timer는 thread.Start와 다르게 주 Thread에서 관리되는 Control이기 때문에
            //Invoke를 사용 하여야 조종 가능.
            MethodInvoker _tmr = new MethodInvoker(timer1.Start);
            this.Invoke(_tmr);
           
        }

        /*client가 게임 시작을 하고 싶을때 누르는 버튼 이벤트
         * 향후 현재 게임에 client가 몇명 접속했는지 보여져야 한다.
         * 현재는 2명 뿐이니 1/2 정도로 표시해도 될듯.
         */
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] _buffer = new byte[1024 * 4];
            NetworkStream _ns = new NetworkStream(this.mClient);

            this.mMyPacket.InitPacket();
            this.mMyPacket.Start = true; //게임 시작으로 설정
            this.mMyPacket.Player = this.mPlayer;

            //서버에게 게임이 시작한다는 것을 알려줌.
            _buffer = Packet.Serialize(this.mMyPacket);
            _ns.Write(_buffer, 0, _buffer.Length); 
            _ns.Flush();
            this.mMyPacket.InitPacket();
        }
    }
}
