using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Runtime.Serialization;
using PacketNamespace;
using DBAdapterNamespace;
using ServerAdapterNamespace;

namespace PrototypeClient
{
    public partial class Client : Form
    {
        private const int FrameTime = 500;
        private ArrayList mLeftTower;
        private ArrayList mRightTower;
        private ArrayList mLeftEnemy;
        private ArrayList mRightEnemy;
        private ArrayList mUser;

        private int mPlayer;
        private int mRoom;
        private int mPosition;
        private bool mStart;

        private Packet mMyPacket;
        private Packet mYourPacket;

        private Thread mThreadReceive;
        private Thread mThreadStart;

        Singleton mSingleton;

        public Client()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ServerAdapter 생성
            this.mSingleton = new Singleton();
            
            
            //Server 접속 실패시 프로그램 종료
            if( false == this.mSingleton.ServerAdapter.mConnected )
            {
                MessageBox.Show("Server와 연결이 안되어 종료됩니다.");
                Close();
            }
            else
            {
                this.mLeftTower = new ArrayList();
                this.mRightTower = new ArrayList();
                this.mLeftEnemy = new ArrayList();
                this.mRightEnemy = new ArrayList();
                this.mUser = new ArrayList();

                this.mMyPacket = new Packet();
                this.mYourPacket = new Packet();

                this.mRoom = 0;
                this.mPlayer = 0;
                //현재 Client가 몇 번째 Player인지 알아내기
                SetPlayer();
                //타워,적,유저 초기화
                SetObjects();

                this.mStart = false;
                timer1.Interval = FrameTime;

                this.mThreadReceive = new Thread(new ThreadStart(StartReceive));

                //1,2Player가 모두 접속 후에 시작하기 위해 기다리는 메소드
                this.mThreadStart = new Thread(new ThreadStart(StartWait));
                this.mThreadStart.Start();
            }
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.mStart = false;
            //timer1.Stop();
            if (this.mThreadReceive != null && mThreadReceive.IsAlive)
                this.mThreadReceive.Abort();
            if (this.mThreadStart != null && this.mThreadStart.IsAlive)
                this.mThreadStart.Abort();

            this.mMyPacket.InitPacket();
            this.mMyPacket.State = (int)state.exit; //게임 종료를 알림
            this.mMyPacket.Player = this.mPlayer;
            this.mMyPacket.Room = this.mRoom;
            //서버에게 게임이 시작한다는 것을 알려줌.
            this.mSingleton.ServerAdapter.Send(this.mMyPacket);
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        //서버 접속 순서대로 플레이어 정함
        private void SetPlayer()
        {
            this.mMyPacket.State = (int)state.login;
            this.mSingleton.ServerAdapter.Send(this.mMyPacket);


            Thread.Sleep(10);

            while(true)
            {
                try
                {
                    //this.mYourPacket = this.mTestServerAdapter.Receive();
                    this.mYourPacket = this.mSingleton.ServerAdapter.Receive();

                    if (this.mYourPacket.Player != 0)
                        break;
                }
                catch( NullReferenceException )
                {
                    return;
                }
            }
            this.mPlayer = this.mYourPacket.Player;
            this.mPosition = this.mYourPacket.Position;
            this.mRoom = this.mYourPacket.Room;
            /* 
             * 리스트 초기화 하는 부분. 
            this.mLeftTower = this.mMyPacket.TowerList;
            this.mRightTower = this.mMyPacket.TowerList;
            this.mLeftEnemy = this.mMyPacket.EnemyList;
            this.mRightEnemy = this.mMyPacket.EnemyList;
            this.mUser = this.mMyPacket.User;
            */

            this.mMyPacket.InitPacket();
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

            if (this.mStart == true)
                this.StarteSend();

            SolidBrush _brush = new SolidBrush(Color.Blue);
            foreach (Tower _to in this.mLeftTower)
            {
                _g.FillRectangle(_brush, (float)_to.X, (float)_to.Y, 50, 50);
            }
            foreach (Enemy _en in this.mLeftEnemy)
            {
                _g.FillRectangle(_brush, (float)_en.X, (float)_en.Y, 25, 25);
            }

            _brush = new SolidBrush(Color.Yellow);
            foreach (Tower _to in this.mRightTower)
            {
                _g.FillRectangle(_brush, (float)_to.X, (float)_to.Y, 50, 50);
            }
            foreach (Enemy _en in this.mRightEnemy)
            {
                _g.FillRectangle(_brush, (float)_en.X, (float)_en.Y, 25, 25);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics _g = this.CreateGraphics();
            _g.Clear(Color.White);

            if (this.mPosition == (int)position.left )
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
            else if (this.mPosition == (int)position.right)
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
            while (true)
            {
                Thread.Sleep(FrameTime);
                try
                {
                    this.mYourPacket.InitPacket();
                    this.mYourPacket = this.mSingleton.ServerAdapter.Receive();

                    if ((int)state.stop == this.mYourPacket.State)
                    {
                        this.mStart = false;
                        timer1.Stop();
                        if (this.mThreadReceive != null && this.mThreadReceive.IsAlive)
                            this.mThreadReceive.Abort();
                        if (this.mThreadStart != null && this.mThreadStart.IsAlive)
                            this.mThreadStart.Abort();
                    }
                        //이 부분 다시 보기
                    else if ((int)state.start == this.mYourPacket.State)
                    {
                        this.mStart = true;
                        MethodInvoker _tmr = new MethodInvoker(timer1.Start);
                        this.Invoke(_tmr);
                        if (this.mThreadReceive != null && (this.mThreadReceive.IsAlive == false))
                            this.mThreadReceive.Start();
                    }
                    else if ((int)state.play == this.mYourPacket.State)
                    {
                        if (this.mYourPacket.Position == (int)position.left) // 받아온 패킷을 보낸 player가 1p일 때
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
                        else if (this.mYourPacket.Position == (int)position.right) // 받아온 패킷을 보낸 player가 2p일 때
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
                    }
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
            if (this.mPosition == (int)position.left)
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
            else if (this.mPosition == (int)position.right)
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
            this.mMyPacket.Position = this.mPosition;
            this.mMyPacket.Room = this.mRoom;
            this.mMyPacket.State = (int)state.play;

            this.mSingleton.ServerAdapter.Send(this.mMyPacket);
               
            this.mMyPacket.InitPacket();
        }

        /* 두 플레이어가 모두 접속할 때 까지 기다리기 위함
         * while문을 통해 기다리다가 서버를 통해 들어온 패킷에
         * bool형 Start이 true일 때 client는 시작을 하게 된다.
         * StartWait method가 쓰레드고 시작하였기 때문에 여기서 쓰레드를 생성하지 않고 시작만 한다.
         * Timer는 주 스레드에 있는 Object이기 때문에 Invoke로 실행
         */
        private void StartWait()
        {
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    this.mYourPacket = this.mSingleton.ServerAdapter.Receive();

                    if (this.mYourPacket.State == (int)state.init)
                        continue;

                    if (this.mYourPacket.State == (int)state.start) // 게임 시작이 되었다는 것을 서버에서 보낼 때 탈출
                        break;
                }
                catch
                {
                    return;
                }
            }
            this.mYourPacket.InitPacket();
            this.mMyPacket.InitPacket();

            //쓰레드 생성하고 시작
            mThreadReceive.Start();
            this.mStart = true;
            
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
            this.mMyPacket.InitPacket();
            this.mMyPacket.State = (int)state.start; //게임 시작으로 설정
            this.mMyPacket.Player = this.mPlayer;

            //서버에게 게임이 시작한다는 것을 알려줌.
            this.mSingleton.ServerAdapter.Send(this.mMyPacket);
            this.mMyPacket.InitPacket();
        }
    }
}
