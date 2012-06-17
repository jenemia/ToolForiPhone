using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using DBAdapterNamespace;
using PacketNamespace;

namespace ChattingServer
{
    static class Constants
    {
        public const int RoomCnt = 10;
    }

    class Server
    {
        private TcpListener mServer;
        private Socket mClient;

        ArrayList mArrayClient;
        Hashtable mHashThreadForClient;
        public static int[] mRoom;

        public static int mCnt;

        public bool mExit = false;

        private DBAdapter mDBAdapter;
        private ArrayList mEnemyList;
        private ArrayList mTowerList;
        private ArrayList mUserList;
        private ArrayList mStageList;

        static int Main(string[] args)
        {
            Server server = new Server();
            if( server.mExit != true) //정상적으로 실행 되었을 때
                  server.StartListen();
            return 0;
        }

        public Server()
        {
            try
            {
                Console.WriteLine("Server Loading.......");
                //DataBase에서 가져온 초기 값으로 List 초기화
                this.mDBAdapter = new DBAdapter();
                this.SetArrayListByDB();

                //Server 설정 및 시작
                IPAddress _addr = IPAddress.Parse("127.0.0.1");
                this.mServer = new TcpListener(_addr, 8080);
                this.mServer.Start();
                Console.WriteLine("Server Start!.");

                this.mArrayClient = new ArrayList();
                this.mHashThreadForClient = new Hashtable();
                mCnt = 1;
                mRoom = new int[Constants.RoomCnt];
            }
            catch( SocketException e) //각 아이피에 포트는 하나씩 있어야 한다.
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ErrorCode);
                //서버가 생성되지 않을 exception이 발생 했을 때 
                //main에서 종료 되도록,
                this.mExit = true;
            }
        }
        ~Server()
        {
            this.mServer.Stop(); //서버 멈추기
        }

        public ArrayList EnemyList
        {
            get { return mEnemyList; }
        }

        public ArrayList TowerList
        {
            get { return mTowerList; }
        }

        public ArrayList UserList
        {
            get { return mUserList; }
        }

        public ArrayList StageList
        {
            get { return mStageList; }
        }

        public void Remove( int player)
        {
            Client ct = new Client(null,null);

            foreach ( Client temp in this.mArrayClient)
            {
                if( temp.mPlayer == player)
                {
                    ct = temp;
                    break;
                }
            }

            Thread _th = (Thread)this.mHashThreadForClient[ct];
            this.mArrayClient.Remove(ct);
            if (_th != null && _th.IsAlive)
                _th.Abort();
        }

        private void SetArrayListByDB()
        {
            this.mEnemyList = this.mDBAdapter.SelectTuples("enemy", "*", "");
            this.mTowerList = this.mDBAdapter.SelectTuples("tower", "*", "");
            this.mUserList = this.mDBAdapter.SelectTuples("user", "*", "");
            this.mStageList = this.mDBAdapter.SelectTuples("stage", "*", "");
        }

        public void StartListen()
        {
            //Client 소켓 연결
            while (true)
            {
                Thread.Sleep(10);
                Socket _client = this.mServer.AcceptSocket();

                if (_client.Connected)
                {
                    this.mClient = _client;

                    Client csClient = new Client(this, _client);

                    mArrayClient.Add(csClient);
                    Thread _th = new Thread(new ThreadStart(csClient.Receive));

                    this.mHashThreadForClient.Add(csClient, _th);

                    _th.Start();
                }
            }
        }

        //브로드캐스팅
        //user를 제외한 모든 client에게 메세지 보내기
        public void Send( byte[] _buffer, int user, int room)
        {
            foreach( Client _client in this.mArrayClient)
            {
                if( room == _client.mRoom)
                {
                    if (user != _client.mPlayer)
                    {
                        Console.WriteLine("{0}번방의 {1}에게 보내짐", _client.mRoom, _client.mPlayer);
                        _client.Send(_buffer);
                    }
                }
            }
        }
    }
}
