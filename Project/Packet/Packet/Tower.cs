using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketNamespace
{
    [Serializable]
    public class Tower
    {
        private int mType;
        private double mX;
        private double mY;
        private string mName;
        private int mLevel;
        private int mRange;
        private int mDamage;
        private int mSpeed;
        private int mPrice;
        private int mRemove;
        private int mIncreaseDamage;
        private int mIncreaseMax;
        private int mIncreasePrice;
        private double mMissileX;
        private double mMissileY;

        public Tower(int type = 0, double x = 0, double y = 0)
        {
            this.mType = type;
            this.mX = x;
            this.mY = y;
            this.mName = null;
            this.mLevel = 0;
            this.mRange = 0;
            this.mDamage = 0;
            this.mSpeed = 0;
            this.mPrice = 0;
            this.mRemove = 0;
            this.mIncreaseDamage = 0;
            this.mIncreaseMax = 0;
            this.mIncreasePrice = 0;
            this.mMissileX = 0;
            this.mMissileY = 0;
        }

        public double X
        {
            set { mX = value;}
            get { return mX;}
        }

        public double Y
        {
            set { mY = value;}
            get { return mY;}
        }

        public int Type
        {
            set { mType = value;}
            get { return mType;}
        }

        public string Name
        {
            set { mName = value;}
            get { return mName; }
        }

        public int Level
        {
            set { mLevel = value;}
            get { return mLevel;}
        }

        public int Range
        {
            set { mRange = value;}
            get { return mRange; }
        }

        public int Damage
        {
            set { mDamage = value;}
            get { return mDamage;}
        }

        public int Speed
        {
            set { mSpeed = value;}
            get { return mSpeed;}
        }

        public int Price
        {
            set { mPrice = value; }
            get { return mPrice; }
        }

        public int Remove
        {
            set { mRemove = value;}
            get { return mRemove;}
        }

        public int IncreaseDamage
        {
            set { mIncreaseDamage = value;}
            get { return mIncreaseDamage;}
        }

        public int IncreaseMax
        {
            set { mIncreaseMax = value;}
            get { return mIncreaseMax;}
        }

        public int IncreasePrice
        {
            set { mIncreasePrice = value;}
            get { return mIncreasePrice;}
        }

        public double MissileX
        {
            set { mMissileX = value;}
            get { return mMissileX; }
        }

        public double MissileY
        {
            set { mMissileY = value;}
            get { return mMissileY; }
        }
    }
}
