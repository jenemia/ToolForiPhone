using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace ChattingServer
{
    class Server
    {
        private TcpListener mServer;
        private Socket mClient;

        ArrayList mArrayClient;
        public static int mCnt;

        static int Main(string[] args)
        {
            Server server = new Server();
            server.StartListen();
            return 0;
        }

        public Server()
        {
            IPAddress _addr = IPAddress.Parse("127.0.0.1");
            this.mServer = new TcpListener(_addr, 8080);
            this.mServer.Start();
            Console.WriteLine("Server Start!.");

            mArrayClient = new ArrayList();
            mCnt = 1;
        }
        ~Server()
        {
            this.mServer.Stop(); //서버 멈추기
        }

        public void Remove( Client ct)
        {
            this.mArrayClient.Remove(ct);
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
                    Console.WriteLine("Clinet Connect!");

                    Client csClient = new Client(this, _client);

                    mArrayClient.Add(csClient);
                    Thread _tr = new Thread( new ThreadStart( csClient.Receive ) );
                    _tr.Start();
                }
            }
        }

        //브로드캐스팅
        //user를 제외한 모든 client에게 메세지 보내기
        public void Send( byte[] _buffer, int user)
        {
            foreach( Client _client in this.mArrayClient)
            {
                if (user != _client.mUser)
                    _client.Send(_buffer);
                else
                    Console.WriteLine("{0}에게 안보내짐",user);
            }
        }
    }
}
