using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace ToolForiPhone
{
    public partial class Form1 : Form
    {
        enum Direction
        {
            NW,
            N,
            NE,
            W,
            E,
            SW,
            S,
            SE,
            None
        }

        ArrayList mListImage;
        static public int mSelectedTag;
        static public int mTagCount;
        Direction mDirection;
        Control mSelectedControl;
        PictureComponents mPicturePickUp;
        bool mBoolListViewDrag;

        const int DRAG_HANDLE_SIZE = 7;

        public Form1()
        {
            InitializeComponent();
            this.mListImage = new ArrayList();
            Form1.mSelectedTag = 0;
            Form1.mTagCount = 0;
            this.mDirection = Direction.None;
            this.mSelectedControl = null;
            this.mBoolListViewDrag = false;
            this.mPicturePickUp = null;

        }

        #region form1 and in form1 controls event
        private void Form1_Load(object sender, EventArgs e)
        {
            ConsoleLib.ConsoleLib.CreateConsole();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Console.WriteLine("paint");
        }
        #endregion

        #region panel1 and in panel1 controls event
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Invalidate();
            if (this.mDirection == Direction.None)
                this.mSelectedControl = null;
        }

        //pictureBox를 클릭하면 주위에 크기를 변경할 수 있는 모서리들이 생긴다.
        public void DrawControlBorder(object sender)
        {
            Control control = (Control)sender;
            //define the border to be drawn, it will be offset by DRAG_HANDLE_SIZE / 2
            //around the control, so when the drag handles are drawn they will seem to be
            //connected in the middle.
            Rectangle Border = new Rectangle(
                new Point(control.Location.X - DRAG_HANDLE_SIZE / 2,
                    control.Location.Y - DRAG_HANDLE_SIZE / 2),
                new Size(control.Size.Width + DRAG_HANDLE_SIZE,
                    control.Size.Height + DRAG_HANDLE_SIZE));
            //define the 8 drag handles, that has the size of DRAG_HANDLE_SIZE
            Rectangle NW = new Rectangle(
                new Point(control.Location.X - DRAG_HANDLE_SIZE,
                    control.Location.Y - DRAG_HANDLE_SIZE),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle N = new Rectangle(
                new Point(control.Location.X + control.Width / 2 - DRAG_HANDLE_SIZE / 2,
                    control.Location.Y - DRAG_HANDLE_SIZE),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle NE = new Rectangle(
                new Point(control.Location.X + control.Width,
                    control.Location.Y - DRAG_HANDLE_SIZE),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle W = new Rectangle(
                new Point(control.Location.X - DRAG_HANDLE_SIZE,
                    control.Location.Y + control.Height / 2 - DRAG_HANDLE_SIZE / 2),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle E = new Rectangle(
                new Point(control.Location.X + control.Width,
                    control.Location.Y + control.Height / 2 - DRAG_HANDLE_SIZE / 2),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle SW = new Rectangle(
                new Point(control.Location.X - DRAG_HANDLE_SIZE,
                    control.Location.Y + control.Height),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle S = new Rectangle(
                new Point(control.Location.X + control.Width / 2 - DRAG_HANDLE_SIZE / 2,
                    control.Location.Y + control.Height),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle SE = new Rectangle(
                new Point(control.Location.X + control.Width,
                    control.Location.Y + control.Height),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));

            //get the form graphic
            Graphics g = panel1.CreateGraphics();
            //draw the border and drag handles
            ControlPaint.DrawBorder(g, Border, Color.Gray, ButtonBorderStyle.Dotted);
            ControlPaint.DrawGrabHandle(g, NW, true, true);
            ControlPaint.DrawGrabHandle(g, N, true, true);
            ControlPaint.DrawGrabHandle(g, NE, true, true);
            ControlPaint.DrawGrabHandle(g, W, true, true);
            ControlPaint.DrawGrabHandle(g, E, true, true);
            ControlPaint.DrawGrabHandle(g, SW, true, true);
            ControlPaint.DrawGrabHandle(g, S, true, true);
            ControlPaint.DrawGrabHandle(g, SE, true, true);
            g.Dispose();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mSelectedControl != null)
            {
                Point newLocation = this.mSelectedControl.Location;
                Size newSize = this.mSelectedControl.Size;
                Point pos = this.PointToClient(MousePosition);
                if (e.Button == MouseButtons.Left)
                {
                    #region resize the control in 8 directions
                    if (mDirection == Direction.NW)
                    {
                        //north west, location and width, height change
                        newLocation = new Point(pos.X, pos.Y);
                        newSize = new Size(mSelectedControl.Size.Width - (newLocation.X - mSelectedControl.Location.X),
                            mSelectedControl.Size.Height - (newLocation.Y - mSelectedControl.Location.Y));
                        mSelectedControl.Location = newLocation;
                        mSelectedControl.Size = newSize;
                    }
                    else if (mDirection == Direction.SE)
                    {
                        //south east, width and height change
                        newLocation = new Point(newLocation.X, pos.Y);
                        newSize = new Size(pos.X - mSelectedControl.Location.X,
                            mSelectedControl.Height + (newLocation.Y - mSelectedControl.Height - mSelectedControl.Location.Y));
                        mSelectedControl.Size = newSize;
                    }
                    else if (mDirection == Direction.N)
                    {
                        //north, location and height change
                        newLocation = new Point(mSelectedControl.Location.X, pos.Y);
                        newSize = new Size(mSelectedControl.Width,
                            mSelectedControl.Height - (pos.Y - mSelectedControl.Location.Y));
                        mSelectedControl.Location = newLocation;
                        mSelectedControl.Size = newSize;
                    }
                    else if (mDirection == Direction.S)
                    {
                        //south, only the height changes
                        newLocation = new Point(newLocation.X, newLocation.Y);
                        newSize = new Size(mSelectedControl.Width,
                            pos.Y - mSelectedControl.Location.Y);
                        mSelectedControl.Size = newSize;
                    }
                    else if (mDirection == Direction.W)
                    {
                        //west, location and width will change
                        newLocation = new Point(pos.X, mSelectedControl.Location.Y);
                        newSize = new Size(mSelectedControl.Width - (pos.X - mSelectedControl.Location.X),
                            mSelectedControl.Height);
                        mSelectedControl.Location = newLocation;
                        mSelectedControl.Size = newSize;
                    }
                    else if (mDirection == Direction.E)
                    {
                        //east, only width changes
                        newLocation = new Point(newLocation.X, newLocation.Y);
                        newSize = new Size(pos.X - mSelectedControl.Location.X,
                            mSelectedControl.Height);
                        mSelectedControl.Size = newSize;
                    }
                    else if (mDirection == Direction.SW)
                    {
                        //south west, location, width and height change
                        newLocation = new Point(pos.X, mSelectedControl.Location.Y);
                        newSize = new Size(mSelectedControl.Width - (pos.X - mSelectedControl.Location.X),
                            pos.Y - mSelectedControl.Location.Y);
                        mSelectedControl.Location = newLocation;
                        mSelectedControl.Size = newSize;
                    }
                    else if (mDirection == Direction.NE)
                    {
                        //north east, location, width and height change
                        newLocation = new Point(mSelectedControl.Location.X, pos.Y);
                        newSize = new Size(pos.X - mSelectedControl.Location.X,
                            mSelectedControl.Height - (pos.Y - mSelectedControl.Location.Y));
                        mSelectedControl.Location = newLocation;
                        mSelectedControl.Size = newSize;
                    }
                    #endregion

                    ((PictureComponents)this.mSelectedControl).ResizeProperty(newLocation, newSize);

                }
                else
                {
                    //테두리 방향에 따라 마우스 커서 모양 변하게 하기
                    #region Get the direction and display correct cursor
                    if (mSelectedControl != null)
                    {
                        //check if the mouse cursor is within the drag handle
                        if ((pos.X >= mSelectedControl.Location.X - DRAG_HANDLE_SIZE &&
                            pos.X <= mSelectedControl.Location.X) &&
                            (pos.Y >= mSelectedControl.Location.Y - DRAG_HANDLE_SIZE &&
                            pos.Y <= mSelectedControl.Location.Y))
                        {
                            mDirection = Direction.NW;
                            Cursor = Cursors.SizeNWSE;
                        }
                        else if ((pos.X >= mSelectedControl.Location.X + mSelectedControl.Width &&
                            pos.X <= mSelectedControl.Location.X + mSelectedControl.Width + DRAG_HANDLE_SIZE &&
                            pos.Y >= mSelectedControl.Location.Y + mSelectedControl.Height &&
                            pos.Y <= mSelectedControl.Location.Y + mSelectedControl.Height + DRAG_HANDLE_SIZE))
                        {
                            mDirection = Direction.SE;
                            Cursor = Cursors.SizeNWSE;
                        }
                        else if ((pos.X >= mSelectedControl.Location.X + mSelectedControl.Width / 2 - DRAG_HANDLE_SIZE / 2) &&
                            pos.X <= mSelectedControl.Location.X + mSelectedControl.Width / 2 + DRAG_HANDLE_SIZE / 2 &&
                            pos.Y >= mSelectedControl.Location.Y - DRAG_HANDLE_SIZE &&
                            pos.Y <= mSelectedControl.Location.Y)
                        {
                            mDirection = Direction.N;
                            Cursor = Cursors.SizeNS;
                        }
                        else if ((pos.X >= mSelectedControl.Location.X + mSelectedControl.Width / 2 - DRAG_HANDLE_SIZE / 2) &&
                            pos.X <= mSelectedControl.Location.X + mSelectedControl.Width / 2 + DRAG_HANDLE_SIZE / 2 &&
                            pos.Y >= mSelectedControl.Location.Y + mSelectedControl.Height &&
                            pos.Y <= mSelectedControl.Location.Y + mSelectedControl.Height + DRAG_HANDLE_SIZE)
                        {
                            mDirection = Direction.S;
                            Cursor = Cursors.SizeNS;
                        }
                        else if ((pos.X >= mSelectedControl.Location.X - DRAG_HANDLE_SIZE &&
                            pos.X <= mSelectedControl.Location.X &&
                            pos.Y >= mSelectedControl.Location.Y + mSelectedControl.Height / 2 - DRAG_HANDLE_SIZE / 2 &&
                            pos.Y <= mSelectedControl.Location.Y + mSelectedControl.Height / 2 + DRAG_HANDLE_SIZE / 2))
                        {
                            mDirection = Direction.W;
                            Cursor = Cursors.SizeWE;
                        }
                        else if ((pos.X >= mSelectedControl.Location.X + mSelectedControl.Width &&
                            pos.X <= mSelectedControl.Location.X + mSelectedControl.Width + DRAG_HANDLE_SIZE &&
                            pos.Y >= mSelectedControl.Location.Y + mSelectedControl.Height / 2 - DRAG_HANDLE_SIZE / 2 &&
                            pos.Y <= mSelectedControl.Location.Y + mSelectedControl.Height / 2 + DRAG_HANDLE_SIZE / 2))
                        {
                            mDirection = Direction.E;
                            Cursor = Cursors.SizeWE;
                        }
                        else if ((pos.X >= mSelectedControl.Location.X + mSelectedControl.Width &&
                            pos.X <= mSelectedControl.Location.X + mSelectedControl.Width + DRAG_HANDLE_SIZE) &&
                            (pos.Y >= mSelectedControl.Location.Y - DRAG_HANDLE_SIZE &&
                            pos.Y <= mSelectedControl.Location.Y))
                        {
                            mDirection = Direction.NE;
                            Cursor = Cursors.SizeNESW;
                        }
                        else if ((pos.X >= mSelectedControl.Location.X - DRAG_HANDLE_SIZE &&
                            pos.X <= mSelectedControl.Location.X + DRAG_HANDLE_SIZE) &&
                            (pos.Y >= mSelectedControl.Location.Y + mSelectedControl.Height - DRAG_HANDLE_SIZE &&
                            pos.Y <= mSelectedControl.Location.Y + mSelectedControl.Height + DRAG_HANDLE_SIZE))
                        {
                            mDirection = Direction.SW;
                            Cursor = Cursors.SizeNESW;
                        }
                        else
                        {
                            Cursor = Cursors.Default;
                            mDirection = Direction.None;
                        }
                    }
                    else
                    {
                        mDirection = Direction.None;
                        Cursor = Cursors.Default;
                    }
                    #endregion

                }
            }
        }

        public void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            propertyGrid1.Refresh();
        }

        //PictureComponents에서 호출 되는 함수
        public void PictureBoxMouseDown()
        {
            panel1.Invalidate();
            PictureComponents sender = ((PictureComponents)this.mListImage[Form1.mSelectedTag]);
            sender.Invalidate();

            propertyGrid1.Refresh();
            propertyGrid1.SelectedObject = sender.PropertyGrid;

            this.mSelectedControl = sender;
            this.DrawControlBorder(sender);
        }

        //PictureComponents에서 호출 되는 함수
        public void PictureBoxMouseUp()
        {
            PictureComponents sender = ((PictureComponents)this.mListImage[Form1.mSelectedTag]);
            sender.Invalidate();
            propertyGrid1.Refresh();
            this.DrawControlBorder(sender);
        }
        #endregion

        #region listview1, propertygrid

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PictureComponents sender = ((PictureComponents)this.mListImage[Form1.mSelectedTag]);
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

            PictureComponents picture = new PictureComponents(imageList1, item.ImageIndex, item.Text, Form1.mTagCount, this);
            picture.Show();

            this.mPicturePickUp = picture;
           // Point newPosition = new Point(listView1.Location.X + item.Position.X, listView1.Location.Y + item.Position.Y);
            Point newPosition = new Point(1, 1);
            this.mPicturePickUp.Location = newPosition;
            this.mPicturePickUp.Focus();

            panel1.Controls.Add(this.mPicturePickUp);
            if (0 == Convert.ToInt32(item.Tag) )
                this.mPicturePickUp.SendToBack();
            else
                this.mPicturePickUp.BringToFront();
            this.mListImage.Add(this.mPicturePickUp);
   
            Form1.mTagCount++;
            //((ListView)sender).DoDragDrop(item, DragDropEffects.Copy | DragDropEffects.Move);

        }

        #endregion
    }
}
