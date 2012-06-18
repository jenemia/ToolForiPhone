using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using PacketNamespace;
using System.Collections;

namespace ChattingServer
{   
    class Client
    {
        private Server mServer;
        public Socket mClient;
        private Packet mPacket;
        private JoinPacket mJoinPacket;

        public int mPlayer;
        public int mRoom;
        public string mID;
        public string mPW;
        
        public Client( Server server, Socket client)
        {
            this.mServer = server;
            this.mClient = client;
            this.mPlayer = 0;
            this.mRoom = 0;
        }

        ~Client()
        {
        }

        /*
         * Clinet가 보낸 Packet을 처리 하는 부분.
         * Packetd에 포함된 State에 따라 처리 하는 Routine이 다르다.
         * 1.join   : 회원가입 - 성공, 실패 패킷 보냄
         * 2.lgoin  : 로그인 - 성공, 실패 패킷 보냄
         * 3.setting: 처음 client가 접속했을 때 - list 초기화 data 및 player, room 보냄
         * 4.start  : Clinet가 2명 접속해서 게임 시작을 알릴 때
         * 5.exit   : Clinet종료를 알릴 때
         * 6.play   : Game을 진행할 때
         */ 
        public void Receive()
        {
            try
            {
                NetworkStream _ns = new NetworkStream(this.mClient);
                byte[] _buffer = new byte[1024*8];
                while (true)
                {
                    _ns.Read(_buffer, 0, _buffer.Length);
                    Packet temp = (Packet)Packet.Deserialize(_buffer);
                    if (null != temp)
                        this.mPacket = temp;

                    if ((int)accountState.join == this.mPacket.State) 
                    {//join 신청할 때
                        if( !this.JoinToDB(_buffer) )
                        {//회원가입 실패 했을 때
                            this.mPacket.State = (int)accountState.error;
                            _buffer = Packet.Serialize(this.mPacket);
                            Send(_buffer);
                        }
                        else
                        {//성공했을 때
                            this.mPacket.State = (int)accountState.join;
                            _buffer = Packet.Serialize(this.mPacket);
                            Send(_buffer);
                        }
                    }
                    else if ((int)accountState.login == this.mPacket.State)
                    {//로그인 시도
                        if (!this.LoginToDB(_buffer))
                        {//로그인실패 했을 때
                            this.mPacket.State = (int)accountState.error;
                            _buffer = Packet.Serialize(this.mPacket);
                            Send(_buffer);
                        }
                        else if( !this.mServer.CheckClinetsID(this.mID, this))
                        {//이미 로그인한 id가 있을 때
                            this.mPacket.State = (int)accountState.error;
                            _buffer = Packet.Serialize(this.mPacket);
                            Send(_buffer);
                        }
                        else 
                        {//성공했을 때
                            this.mPacket.State = (int)accountState.login;
                            _buffer = Packet.Serialize(this.mPacket);
                            Send(_buffer);
                        }
                     }
                    else if ( (int)state.setting == this.mPacket.State) //Client가 처음 실행 되었을 때
                    {
                        this.mPacket.Player = Server.mCnt;
                        this.SelectRoom();

                        Console.WriteLine("{0}번방 {1}번째 플레이어 입장", this.mPacket.Room, this.mPacket.Player);
                        //DB에 있던 초기화 정보들을 패킷에 넣는다.
                        this.mPacket.EnemyList = this.mServer.EnemyList;
                        this.mPacket.TowerList = this.mServer.TowerList;
                        this.mPacket.User = this.mServer.UserList;
                        this.mPacket.StageList = this.mServer.StageList;
                        this.mPacket.ID = this.mID;

                        this.mPlayer = this.mPacket.Player;
                        this.mRoom = this.mPacket.Room;
                        
                        Server.mCnt++;
                        _buffer.Initialize();
                        _buffer = Packet.Serialize(this.mPacket);
                        Send(_buffer); // 현재 자신에게 보내기
                        continue;
                    }
                    else if( (int)state.start == this.mPacket.State) //게임 시작을 알릴 때
                    {
                        //해당 패킷의 room의 플레이어 수를 체크하여 2명이면 state = start 하고 보낼 것.
                        if (Server.mRoom[this.mRoom] == 2)
                            this.mPacket.State = (int)state.start;
                        else
                            this.mPacket.State = (int)state.init;

                        _buffer.Initialize();
                        _buffer = Packet.Serialize(this.mPacket);
                        this.mServer.Send(_buffer, 0, this.mRoom); // 모두에게 보내기
                        continue;
                    }
                    else if( (int)state.stop == this.mPacket.State ) //멈출 때
                    {//게임끝날때, 
                        this.mPacket.InitPacket();
                        this.mPacket.State = (int)state.stop;
                        _buffer.Initialize();
                        _buffer = Packet.Serialize(this.mPacket);
                        this.mServer.Send(_buffer, this.mPlayer, this.mRoom);
                    }
                    else if( (int)state.exit == this.mPacket.State) //종료할 때
                    {
                        this.mPacket.InitPacket();
                        this.mPacket.State = (int)state.exit;
                        _buffer.Initialize();
                        _buffer = Packet.Serialize(this.mPacket);
                        this.mServer.Send(_buffer, this.mPlayer, this.mRoom);

                        if (this.mClient.Connected)
                            this.mClient.Close();
                        if (0 != Server.mRoom[this.mRoom])
                            Server.mRoom[this.mRoom]--;

                        Console.WriteLine("{0}번방 {1}번 플레이어가 종료했습니다.", this.mRoom, this.mPlayer);
                        this.mServer.Remove(this.mPlayer);
                        break;
                    }
                    else if( (int)state.play == this.mPacket.State) //일반 게임 플레이 중
                    {
                        this.mServer.Send(_buffer, this.mPlayer, this.mRoom);              
                    }
                    _buffer = new byte[1024 * 8];
                }
            }
            catch (SerializationException)
            {
                Receive();
            }
            catch
            {
            }
            return;
        }

        public void Send(byte[] _buffer)
        {
            NetworkStream _ns = new NetworkStream(this.mClient);

            _ns.Write(_buffer, 0, _buffer.Length);
            _ns.Flush();
            _ns.Close();
        }

        public void SelectRoom() //접속한 클라이언트에게 방 배정
        {
            for (int i = 0; i < Server.mRoom.Length; i++ )
            {
                if( 0 == Server.mRoom[i] )
                {
                    this.mRoom = i;
                    this.mPacket.Room = i;
                    this.mPacket.Position = (int)position.left;
                    Server.mRoom[i] = 1;
                    break;
                }
                else if( 1 == Server.mRoom[i] )
                {
                    this.mRoom = i;
                    this.mPacket.Room = i;
                    this.mPacket.Position = (int)position.right;
                    Server.mRoom[i] = 2;
                    break;
                }
            }
        }

        /*
         * table : "joinUser"
         * attributes : " name,passwd "
         * values : " "soohyun" , "1234" "
         */
        private bool JoinToDB( byte[] buffer ) //회원가입
        {
            this.mJoinPacket = (JoinPacket)Packet.Deserialize(buffer);
            this.mID= this.mJoinPacket.JoinID;
            this.mPW = this.mJoinPacket.JoinPW;

            try
            {
                if (this.mServer.mDBAdapter.InsertUserJoinTuple("joinUser", "id,passwd", this.mID, this.mPW))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool LoginToDB(byte[] buffer) //로그인
        {
            this.mJoinPacket = (JoinPacket)Packet.Deserialize(buffer);
            this.mID = this.mJoinPacket.JoinID;
            this.mPW = this.mJoinPacket.JoinPW;

            try
            {
                if (this.mServer.mDBAdapter.CheckIDandPW(this.mID, this.mPW))
                    return true;//일치 한게 있을 때
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
