using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketNamespace
{
    [Serializable]
    public class Enemy
    {
        private int mType;
        private double mX;
        private double mY;
        private string mName;
        private int mSpeed;
        private int mHp;
        private int mDamage;
        private int mPrice;

        public Enemy(int type = 0, double x = 0, double y = 0)
        {
            this.mType = type;
            this.mX = x;
            this.mY = y;
            this.mName = null;
            this.mSpeed = 0;
            this.mHp = 0;
            this.mDamage = 0;
            this.mPrice = 0;
        }

        public int Type
        {
            set { mType = value;}
            get { return mType; }
        }

        public double X
        {
            set { mX = value;}
            get { return mX; }
        }

        public double Y
        {
            set { mY = value;}
            get { return mY;}
        }

        public string Name
        {
            set { mName = value;}
            get { return mName; }
        }

        public int Speed
        {
            set { mSpeed = value;}
            get { return mSpeed; }
        }

        public int Hp
        {
            set { mHp = value; }
            get { return mHp; }
        }

        public int Damage
        {
            set { mDamage = value; }
            get { return mDamage; }
        }

        public int Price
        {
            set { mPrice = value;}
            get { return mPrice; }
        }
    }
}
