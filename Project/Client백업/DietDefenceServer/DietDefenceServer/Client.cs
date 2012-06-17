using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using PacketNamespace;

namespace ChattingServer
{   
    class Client
    {
        private Server mServer;
        public Socket mClient;
        private Packet mPacket;

        public int mPlayer;
        public int mRoom;

        public Client( Server server, Socket client)
        {
            this.mServer = server;
            this.mClient = client;
            this.mPlayer = 0;
            this.mRoom = 0;
            this.mPacket = new Packet();
        }

        ~Client()
        {
        }

        public void Receive()
        {
            try
            {
                NetworkStream _ns = new NetworkStream(this.mClient);
                byte[] _buffer = new byte[1024*4];
                while (true)
                {
                    this.mPacket.InitPacket();

                    _ns.Read(_buffer, 0, _buffer.Length);
                    this.mPacket = (Packet)Packet.Deserialize(_buffer);

                    Console.WriteLine("Play : {0} State : {1}", this.mPacket.Player, this.mPacket.State);
                    //if (null != temp)
                    {
                    }
                    //else
                    //{
                    //    continue;
                    //}

                    if ( (int)state.login == this.mPacket.State) //Client가 처음 실행 되었을 때
                    {
                        this.mPacket.Player = Server.mCnt;
                        this.SelectRoom();

                        Console.WriteLine("{0}번방 {1}번째 플레이어 입장", this.mPacket.Room, this.mPacket.Player);

                        this.mPacket.EnemyList = this.mServer.EnemyList;
                        this.mPacket.TowerList = this.mServer.TowerList;
                        this.mPacket.User = this.mServer.UserList;
                        this.mPacket.StageList = this.mServer.StageList;

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
                        Console.WriteLine("Start : {0}", this.mPlayer);
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
                    else if( (int)state.exit == this.mPacket.State)
                    {
                        if (this.mClient.Connected)
                            this.mClient.Close();
                        if (0 != Server.mRoom[this.mRoom])
                            Server.mRoom[this.mRoom]--;
                        Console.WriteLine("{0}번방 {1}번 플레이어가 종료했습니다.", this.mRoom, this.mPlayer);
                        this.mServer.Remove(this.mPlayer);

                        this.mPacket.InitPacket();
                        this.mPacket.Room = this.mRoom;
                        this.mPacket.Player = this.mPlayer;
                        this.mPacket.State = (int)state.stop;
                        this.mServer.Send(_buffer, this.mPlayer, this.mRoom);
                        break;
                    }
                    else if ((int)state.play == this.mPacket.State)
                    {
                        this.mServer.Send(_buffer, this.mPlayer, this.mRoom);
                        _buffer.Initialize();
                    }
                    else
                    {
                        Console.WriteLine("else : {0}", this.mPlayer);
                    }
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

        public void SelectRoom()
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
    }
}
