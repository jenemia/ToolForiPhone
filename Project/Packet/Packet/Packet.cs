﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;


namespace PacketNamespace
{
    public enum position
    {
        left = 0,
        right
    }

    public enum state
    {
        init = 0,
        login,
        start,
        stop,
        play,
        exit,
        join
    }

    [Serializable]
    public class Packet
    {
        private ArrayList mTowerList;
        private ArrayList mEnemyList;
        private ArrayList mUserList;
        private ArrayList mStageList;

        private int mPlayer;
        private int mAllPlayer;
        private int mRoom;
        private int mPosition;

        private int mState;

        public Packet()
        {
            this.mTowerList = new ArrayList();
            this.mEnemyList = new ArrayList();
            this.mUserList = new ArrayList();
            this.mState = (int)state.init;

            this.mPlayer = 0;
            this.mAllPlayer = 0;
            this.mPosition = 0;
            this.mRoom = 0;
        }

        public ArrayList TowerList
        {
            set{mTowerList = value;}
            get { return mTowerList; }
        }

        public ArrayList EnemyList
        {
            set { mEnemyList = value; }
            
            get { return mEnemyList; }
        }

        public ArrayList StageList
        {
            set { mStageList = value;}
            get { return mStageList;}
        }

        public ArrayList User
        {
            set { mUserList = value;}
            get { return mUserList; }
        }

        public int Player
        {
            set { mPlayer = value;}
            get { return mPlayer; }
        }

        public int AllPlayer
        {
            set { mAllPlayer = value;}
            get { return mAllPlayer;}
        }

        public int Position
        {
            set { mPosition = value; }
            get { return mPosition; }
        }

        public int Room
        {
            set { mRoom = value; }
            get { return mRoom; }
        }

        public int State
        {
            set { mState = value; }
            get { return mState; }
        }

        public void AddTower( Tower to )
        {
            this.mTowerList.Add(to);
        }

        public void AddEnemy( Enemy en )
        {
            this.mEnemyList.Add(en);
        }

        public void InitPacket()
        {
            this.mTowerList.Clear();
            this.mEnemyList.Clear();
            this.mUserList.Clear();
            this.mPlayer = 0;
            this.mState = (int)state.init;
            this.mPosition = 0;
            this.mRoom = 0;
        }

        public static byte[] Serialize(Object o)	// 패킷 객체를 byte[] 로 바꿀때 사용. 패킷 보낼때
        {
            MemoryStream ms = new MemoryStream(1024*8);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }

        public static Object Deserialize(byte[] bt)	// byte[] 을 패킷 객체로 만들때 사용. 패킷 수신했을 때
        {
            //try
            {
                MemoryStream ms = new MemoryStream(1024 * 8);

                foreach (byte b in bt)
                    ms.WriteByte(b);

                ms.Seek(0, SeekOrigin.Begin);
                //ms.Position = 0;
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }/*
            catch( System.Runtime.Serialization.SerializationException)
            {
                return null;
            }*/
        }
    }

    [Serializable]
    public class JoinPacket : Packet
    {
        private string mID;
        private string mPW;

        public JoinPacket()
        {
            this.mID = "";
            this.mPW = "";
        }

        public string ID
        {
            set { mID = value; }
            get { return mID; }
        }

        public string PW
        {
            set { mPW = value; }
            get { return mPW; }
        }
    }
}
