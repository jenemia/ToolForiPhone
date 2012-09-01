using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace ToolForiPhone
{
    public partial class 
        
        Form1 : Form
    {
        ArrayList mListObjects; //Object Manage List
        int mIndexNumber; //PictureBox Couonting 
        int mSelectedNumber;
        int mWheelCnt;
        PictureComponents mSelectedControl; 

        public Form1()
        {
            InitializeComponent();
            this.mListObjects = new ArrayList();
            this.mIndexNumber = 0;
            this.mSelectedNumber = 0;
            this.mSelectedControl = null;

            this.mWheelCnt = 0;
            panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseWheel);
        }

        #region form1 and in form1 controls event
        private void Form1_Load(object sender, EventArgs e)
        {
            ConsoleLib.ConsoleLib.CreateConsole();
        }
        #endregion

        #region panel1 and in panel1 controls event
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Invalidate();
            panel1.Focus();
            if (this.mSelectedControl != null)
            {
                this.mSelectedControl.Invalidate();
                this.mSelectedControl.CancelBorder();
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mSelectedControl != null)
            {
                Point pos = this.PointToClient(MousePosition);
                
            }
        }

        private void panel1_MouseWheel( object sender, MouseEventArgs e)
        {
            if (e.Delta / 120 > 0)//위로
            {
                this.mWheelCnt += 10;
                foreach( PictureComponents picture in this.mListObjects ) //picutrebox 크기를 늘인다.
                {
                    picture.Size = new Size(picture.Size.Width + 10, picture.Size.Height + 10);
                    picture.mIncrease++;
                }
                panel1.Invalidate();

                Console.WriteLine("Wheel UP : {0}" , this.mWheelCnt );
            }
            else //아래로
            {
                this.mWheelCnt -= 10;
                foreach (PictureComponents picture in this.mListObjects)
                {
                    picture.Size = new Size(picture.Size.Width - 10, picture.Size.Height - 10);
                    picture.mIncrease--;
                }
                panel1.Invalidate();
                Console.WriteLine("Wheel Down : {0} ", this.mWheelCnt );
            }
        }

        //PictureComponents에서 호출 되는 함수
        public void PictureBoxMouseDown(int num)
        {
            this.mSelectedNumber = num;

            panel1.Invalidate();
            PictureComponents sender = ((PictureComponents)this.mListObjects[num]);

            propertyGrid1.Refresh();
            propertyGrid1.SelectedObject = sender.PropertyGrid;

            if (this.mSelectedControl == null)
                this.mSelectedControl = sender;
            else if( this.mSelectedControl != sender )
            {
                this.mSelectedControl.Invalidate();
                this.mSelectedControl.CancelBorder();
                this.mSelectedControl = sender;
            }
        }

        public void PictureBoxMouseUp()
        {
            if (0 != this.mSelectedControl.mTag)
                this.mSelectedControl.mControllerNumber = this.WhereViewController(this.mSelectedControl);
        }

        #endregion

        #region listview1, propertygrid

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PictureComponents sender = ((PictureComponents)this.mListObjects[this.mSelectedNumber]);
            Point newPosition = new Point((int)sender.PropertyGrid.GetProperty("x"), (int)sender.PropertyGrid.GetProperty("y"));
            Size newSize = new Size((int)sender.PropertyGrid.GetProperty("Width"), (int)sender.PropertyGrid.GetProperty("Height"));

            sender.Size = newSize;
            sender.Location = newPosition;
            sender.ResizeProperty(newPosition, newSize);
            sender.Invalidate();
            propertyGrid1.Refresh();
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListViewItem item = e.Item as ListViewItem;

            PictureComponents picture = new PictureComponents(imageList1, item.ImageIndex, item.Text, this.mIndexNumber, Convert.ToInt32(item.Tag), this);
            picture.Show();
            Point newPosition = new Point(1, 30); //초기위치
            picture.Location = newPosition;
            picture.Focus();

            this.mListObjects.Add(picture);
            panel1.Controls.Add(picture);

            if (0 == Convert.ToInt32(item.Tag)) //UIViewController
                picture.SendToBack();
            else
                picture.BringToFront();

            this.mIndexNumber++;
        }

        #endregion

        #region ArrayList
        private int WhereViewController( PictureComponents sender )
        {
            foreach( PictureComponents picture in this.mListObjects )
            {
                if (0 == picture.mTag) //UIViewController
                {
                    if( (picture.Location.X <= sender.Location.X) && ( picture.Location.X + picture.Size.Width >= sender.Location.X ) && 
                        (picture.Location.Y <= sender.Location.Y) && ( picture.Location.Y + picture.Size.Height >= sender.Location.Y ) ) //ViewController 안에 있을 때
                    {
                        Console.WriteLine(picture.mIndexNumber);
                        return picture.mIndexNumber;
                    }
                }
            }
            return 0;
        }
        #endregion

        #region Menu
        private void xML로저장SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            foreach( PictureComponents picture in this.mListObjects )
            {
             * 
            picture.mName;
            picture.Location.X;
            picture.Location.Y;
            picture.Size.Width;
            picture.SIze.Height;
            }
             */
        }
        #endregion
    }
}
