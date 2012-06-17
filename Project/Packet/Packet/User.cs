using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketNamespace
{
    [Serializable]
    public class User    
    {
        private int mHp;
        private int mMoney;

        public User( int hp=0, int money=0)
        {
            this.mHp = hp;
            this.mMoney = money;
        }

        public int Hp
        {
            set { mHp = value;}
            get { return mHp; }
        }

        public int Money
        {
            set { mMoney = value;}
            get { return mMoney; }
        }

        public void Clear()
        {
            this.mHp = 0;
            this.mMoney = 0;
        }
    }
}
