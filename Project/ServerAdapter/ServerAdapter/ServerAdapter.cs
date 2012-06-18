using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using PacketNamespace;

namespace ServerAdapterNamespace
{
    public class ServerAdapter
    {
        private Socket mClient;

        private Packet mYourPacket;
        private Packet mMyPacket;

        private Thread mThreadReceive;

        public bool mConnected = false;

        public ServerAdapter()
        {
            try
            {
                TcpClient _tcp = new TcpClient();
                _tcp.Connect("127.0.0.1", 8080);
                this.mClient = _tcp.Client;

                this.mConnected = true;

                this.mYourPacket = new Packet();
                this.mMyPacket = new Packet();

                this.mThreadReceive = new Thread(new ThreadStart(StartReceive));
                this.mThreadReceive.Start();
            }
            catch
            {
                this.mConnected = false;
            }
        }

        ~ServerAdapter()
        {
            try
            {
                if (this.mThreadReceive.IsAlive)
                    this.mThreadReceive.Abort();
                if (this.mClient.Connected)
                    this.mClient.Close();
            }
            catch (NullReferenceException)
            {
            }
        }

        public void Send(Packet packet)
        {
            this.mMyPacket = packet;
            this.StarteSend();
        }

        public Packet Receive()
        {
            return this.mYourPacket;
        }

        /* 서버에게 패킷을 받아 오는 부분
         * 받은 패킷의 player가 1player라면 왼쪽, 
         * plyaer가 2라면 오른쪽으로 뿌려준다.
         * 1번 client 2번 client 둘다 왼쪽은 1plyaer , 오른쪽은 2plyaer
         */
        private void StartReceive()
        {
            try
            {
                byte[] _buffer = new byte[1024 * 8];
                NetworkStream _ns = new NetworkStream(this.mClient);

                while (true)
                {
                    if (this.mConnected == false)
                        break;
                    Thread.Sleep(30);

                    try
                    {
                        _ns.Read(_buffer, 0, _buffer.Length);
                        this.mYourPacket.InitPacket();
                        Packet temp = (Packet)Packet.Deserialize(_buffer);
                        if (null != temp)
                            this.mYourPacket = temp;

                        _buffer.Initialize();
                    }
                    catch (SerializationException)
                    {

                    }
                    catch (NullReferenceException)
                    {
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }
                _ns.Close();
            }
            catch
            {

            }
        }

        /*패킷을 서버에게 보내는 부분.
         * 객체의 데이터가 바뀌었을 때 다른 플레이어게 보여줘야 하는 부분을 
         * 이 method를 통하여 서버에 보내면, 다른 플레이어게도 
         * 객체의 정보가 넘어가서 화면에 그려진다.
         */
        private void StarteSend()
        {
            try
            {
                byte[] _buffer = new byte[1024 * 8];
                NetworkStream _ns = new NetworkStream(this.mClient);

                try
                {
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
                _ns.Close();
            }
            catch
            {

            }
        }

        public void ExitServer()
        {
            this.mConnected = false;
        }
    }
}
