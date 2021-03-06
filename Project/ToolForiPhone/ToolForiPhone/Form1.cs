﻿using System;
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
        public int mWheelCnt;
        PictureComponents mSelectedControl;
        XMLAdapter mXMLAdapter;

        public Form1()
        {
            InitializeComponent();
            this.mListObjects = new ArrayList();
            this.mIndexNumber = 0;
            this.mSelectedNumber = 0;
            this.mSelectedControl = null;
            this.mXMLAdapter = new XMLAdapter();

            this.mWheelCnt = 0;
            panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseWheel);
            panel1.PreviewKeyDown += new PreviewKeyDownEventHandler(this.panel1_KeyDown);
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
            int sign = 0;
            if (e.Delta / 120 > 0)
            {
                if (this.mWheelCnt >= 0) //원본보다 확대를 하진 않는다.
                     return;
                this.mWheelCnt += 1;
                sign = 1;
            }
            else
            {
                this.mWheelCnt -= 1;
                sign = -1;
            }

            foreach (PictureComponents picture in this.mListObjects) //picutrebox 크기를 늘인다.
                picture.ChangeSize(sign);

            panel1.Invalidate();
            propertyGrid1.Refresh();
        }

        private void panel1_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.mSelectedControl != null)
            {
                Point newLocation = new Point(this.mSelectedControl.Location.X, this.mSelectedControl.Location.Y);
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        this.mSelectedControl.Location = new Point(newLocation.X - 1, newLocation.Y);
                        break;
                    case Keys.Right:
                        this.mSelectedControl.Location = new Point(newLocation.X + 1, newLocation.Y);
                        break;
                    case Keys.Up:
                        this.mSelectedControl.Location = new Point(newLocation.X, newLocation.Y - 1);
                        break;
                    case Keys.Down:
                        this.mSelectedControl.Location = new Point(newLocation.X, newLocation.Y + 1);
                        break;
                }
                this.mSelectedControl.ReSettingProperty();
                propertyGrid1.Refresh();
            }
        }

        //PictureComponents에서 호출 되는 함수
        public void PictureBoxMouseDown(int num)
        {
            this.mSelectedNumber = num;

            panel1.Invalidate();
            PictureComponents sender = ((PictureComponents)this.mListObjects[num]);
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

        //PictureComponents에서 호출 되는 함수
        public void PictureBoxMouseUp()
        {
            this.propertyGrid1.Invalidate();
            this.Focus();
            try
            {
                if (0 != this.mSelectedControl.mTag)
                    this.mSelectedControl.mControllerNumber = this.WhereViewController(this.mSelectedControl);
            }
            catch( NullReferenceException )
            {

            }
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
            sender.ReSettingProperty();
            sender.Invalidate();
            propertyGrid1.Refresh();
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListViewItem item = e.Item as ListViewItem;

            PictureComponents picture = new PictureComponents(imageList1, item.ImageIndex, item.Text, this.mIndexNumber, Convert.ToInt32(item.Tag), this);
            picture.Show();
            picture.Focus();

            this.mListObjects.Add(picture);
            panel1.Controls.Add(picture);

            if (0 == Convert.ToInt32(item.Tag)) //UIViewController
                picture.SendToBack();
            else
                picture.BringToFront();

            this.mIndexNumber++;
            propertyGrid1.Refresh();
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
            this.mXMLAdapter.NodeSetting(this.mListObjects);
        }
        #endregion
    }
}
