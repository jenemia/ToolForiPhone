using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using PacketNamespace;
using DBAdapterNamespace;

namespace ChattingServer
{   
    class Client
    {
        private Server mServer;
        public Socket mClient;
        private Packet mPacket;
        public int mUser;
        private DBAdapter mDBAdapter;
        
        public Client( Server server, Socket client)
        {
            this.mServer = server;
            this.mClient = client;
            this.mUser = 0;
            this.mDBAdapter = new DBAdapter();
        }

        public void Receive()
        {
            try
            {
                NetworkStream _ns = new NetworkStream(this.mClient);
                byte[] _buffer = new byte[1024*4];
                while (true)
                {
                    _ns.Read(_buffer, 0, _buffer.Length);
                    this.mPacket = (Packet)Packet.Deserialize(_buffer);

                    if (0 == this.mPacket.Player) //Client가 처음 실행 되었을 때
                    {
                        //db접속해서 정보들을 가져옴. 하지만 지금은 가져올 수 있는 것이
                        //x,y값인데(밑에 그릴 때 필요한거) 0으로 설정되어 있으니, 가져와도 사용 안함
                        //밑에 ArrayList에 브레이크 포인트 걸어서 가져온 정보들 확인 가능
                        this.mPacket.Player = Server.mCnt;
                        this.mPacket.EnemyList = this.mDBAdapter.SelectTuples("enemy", "*", "");
                        this.mPacket.TowerList = this.mDBAdapter.SelectTuples("tower", "*", "");
                        this.mPacket.User = this.mDBAdapter.SelectTuples("user", "*", "");
                        //잘 들어가는지 확인하기
                        this.mUser = Server.mCnt;
                        Server.mCnt++;

                        _buffer.Initialize();
                        _buffer = Packet.Serialize(this.mPacket);
                        Send(_buffer); // 현재 자신에게 보내기
                        continue;
                    }
                    else if( true == this.mPacket.Start) //게임 시작을 알릴 때
                    {
                        _buffer.Initialize();
                        _buffer = Packet.Serialize(this.mPacket);
                        this.mServer.Send(_buffer, 0); // 모두에게 보내기
                        continue;
                    }
                    this.mServer.Send(_buffer, this.mUser);
                    _buffer.Initialize();
                }
            }
            catch (SerializationException)
            {
                //Console.WriteLine(e.Message); 너무 빨라서 그런가..buffer에 잘못된게 들어온다 처음에
                //아마 클라이언트를 다(2명) 받고 쓰레드 시작 하면 될듯
                Receive();
            }
            catch
            {
               // this.mServer.Remove(this);
            }
        }

        public void Send( byte[] _buffer )
        {
            NetworkStream _ns = new NetworkStream(this.mClient);

            _ns.Write(_buffer, 0, _buffer.Length);
            _ns.Flush();
            _ns.Close();
        }
    }
}
