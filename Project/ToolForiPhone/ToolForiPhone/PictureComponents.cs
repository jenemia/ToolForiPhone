﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Threading;

namespace ToolForiPhone
{
    class PictureComponents : PictureBox
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

        Point mPointMouse;
        Image mImage;
        Form1 mForm;
        PropertyGridCustom mPropertyGrid;
        Direction mDirection = Direction.None;
        int mTag;

        bool isBorder = false;
        bool isMouseLeft = false;

        const int DRAG_HANDLE_SIZE = 7;

        public PictureComponents(ImageList list, int index, string name, int tag, Form1 form)
        {
            Image temp = list.Images[index];

            ResourceManager manager = Properties.Resources.ResourceManager;
            this.Image = (Bitmap)manager.GetObject(name);

            this.mImage = this.Image;
            this.Size = this.ChoiceImageSize(name);
            this.mTag = tag;
            this.mForm = form;
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Location = new Point(50, 50);

            //pictureBox의 propertyGrid를 Custom하여 보여준다.
            this.mPropertyGrid = new PropertyGridCustom();
            this.mPropertyGrid.Add(new CustomProperty("x", this.Location.X, false, true));
            this.mPropertyGrid.Add(new CustomProperty("y", this.Location.Y, false, true));
            this.mPropertyGrid.Add(new CustomProperty("Width", this.Size.Width, false, true));
            this.mPropertyGrid.Add(new CustomProperty("Height", this.Size.Height, false, true));
        }

        public PictureComponents(string resourcesName)
        {
            ResourceManager manager = Properties.Resources.ResourceManager;

            Bitmap bm = (Bitmap)manager.GetObject(resourcesName);

            this.Image = bm;
            this.mImage = bm;
            this.Width = bm.Width;
            this.Height = bm.Height;
            this.Size = this.Image.Size;
            this.SizeMode = PictureBoxSizeMode.StretchImage;

            this.mPointMouse = Point.Empty;
        }

        public Size GetSize
        {
            get { return this.Size; }
        }

        public Point GetLocation
        {
            get { return this.Location; }
        }

        public void CancelBorder()
        {
            this.isBorder = false;
        }

        public PropertyGridCustom PropertyGrid
        {
            get { return this.mPropertyGrid; }
        }

        private Size ChoiceImageSize(string name)
        {
            Size result = new Size(100, 30);
            if (name.Equals("UIViewController"))
            {
                result = new Size(365, 700);
            }
            else if (name.Equals("UIButton"))
            {
                result = new Size(100, 48);
            }
            else if (name.Equals("UIPickerView"))
            {
                result = new Size(320, 150);
            }

            return result;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                Form1.mSelectedTag = (int)this.mTag;
                this.mPointMouse = e.Location;
                this.mForm.PictureBoxMouseDown();

                this.isBorder = true;
                this.isMouseLeft = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.mPointMouse = Point.Empty;

            Form1.mSelectedTag = (int)this.mTag;
            //이동된 좌표값으로 수정.                        
            this.mPropertyGrid.Replace("x", this.Location.X);
            this.mPropertyGrid.Replace("y", this.Location.Y);
            this.DrawControlBorder();
            this.isMouseLeft = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ( this.isBorder ) //테두리가 있을 때
            {
                //클릭 지점부터 이동하는 좌표를 계산.
                Point pos = new Point(e.Location.X - this.mPointMouse.X, e.Location.Y - this.mPointMouse.Y);
                Point newLocation = this.Location;
                Size newSize = this.Size;
                if (this.isMouseLeft && this.mPointMouse != Point.Empty ) //왼쪽 마우스 누르고 있는 상태일 때
                {
                    #region resize the control in 8 directions
                    if (mDirection == Direction.NW)
                    {
                        //north west, location and width, height change
                        newLocation = new Point(this.Location.X + pos.X, this.Location.Y + pos.Y);
                        newSize = new Size(this.Size.Width - pos.X, this.Size.Height - pos.Y );

                        this.Location = newLocation;
                        this.Size = newSize;
                        Console.WriteLine("location : " + e.Location); 

                        Console.WriteLine("point : " + this.mPointMouse);                        
                    }
                    else if (mDirection == Direction.N)
                    {
                        //north, location and height change
                        newLocation = new Point(this.Location.X, this.Location.Y + pos.Y);
                        newSize = new Size(this.Size.Width, this.Size.Height - pos.Y);

                        this.Location = newLocation;
                        this.Size = newSize;
                    }
                    else if (mDirection == Direction.NE)
                    {
                        //north east, location, width and height change
                        newLocation = new Point(this.Location.X, this.Location.Y + pos.Y);
                        newSize = new Size(this.Size.Width + pos.X, this.Size.Height - pos.Y);

                        this.Location = newLocation;
                        this.Size = newSize;
                        //pictureBox크기가 변함에 따라 mPointMouse(클릭 지점)도 변해야 한다.
                        this.mPointMouse = new Point( this.mPointMouse.X + pos.X , this.mPointMouse.Y);

                        Console.WriteLine("location : " + e.Location);
                        Console.WriteLine("point : " + this.mPointMouse);
                    }
                    else if (mDirection == Direction.W)
                    {
                        //west, location and width will change
                        newLocation = new Point(this.Location.X + pos.X, this.Location.Y);
                        newSize = new Size(this.Size.Width - pos.X , this.Size.Height);

                        this.Location = newLocation;
                        this.Size = newSize;
                    }
                    else if (mDirection == Direction.E)
                    {
                        //east, only width changes
                        newLocation = new Point(this.Location.X, this.Location.Y);
                        newSize = new Size(this.Size.Width + pos.X, this.Size.Height);
                        this.mPointMouse = new Point(this.mPointMouse.X + pos.X, this.mPointMouse.Y);

                        this.Location = newLocation;
                        this.Size = newSize;
                    }
                    else if (mDirection == Direction.SW)
                    {
                        //south west, location, width and height change
                        newLocation = new Point(this.Location.X + pos.X, this.Location.Y);
                        newSize = new Size(this.Size.Width - pos.X, this.Size.Height + pos.Y);
                        this.mPointMouse = new Point(this.mPointMouse.X, this.mPointMouse.Y + pos.Y);

                        this.Location = newLocation;
                        this.Size = newSize;
                    }
                    else if (mDirection == Direction.S)
                    {
                        //south, only the height changes
                        newLocation = new Point(newLocation.X, newLocation.Y);
                        newSize = new Size(this.Size.Width, this.Size.Height + pos.Y);
                        this.mPointMouse = new Point(this.mPointMouse.X, this.mPointMouse.Y + pos.Y);

                        this.Size = newSize;
                    }
                    else if (mDirection == Direction.SE)
                    {
                        //south east, width and height change
                        newLocation = new Point(newLocation.X, newLocation.Y);
                        newSize = new Size(this.Size.Width + pos.X, this.Size.Height + pos.Y);
                        this.mPointMouse = new Point(this.mPointMouse.X + pos.X, this.mPointMouse.Y + pos.Y);

                        this.Size = newSize;
                    }
                    else // 크기 변환 부분이 아닐 때는 그냥 이동.
                    {
                        Rectangle rect = this.Parent.RectangleToScreen(this.Parent.Bounds);

                        Point mouse = e.Location;
                        mouse.X += rect.X;
                        mouse.Y += rect.Y;

                        //pictureBox 범위 안에 있을 때 이동
                        if (rect.Contains(mouse))
                        {
                            Point pt = new Point(e.Location.X - mPointMouse.X, e.Location.Y - mPointMouse.Y);
                            this.Location = new Point(this.Location.X + pt.X, this.Location.Y + pt.Y);
                        }
                        else
                            mPointMouse = Point.Empty;
                    }
                    #endregion

                    this.ResizeProperty(newLocation, newSize);
                }
                else
                {//왼쪽 클릭이 아닐 때에는 마우스 커서 변환만.
                    this.mDirection = Direction.None;
                    //테두리 방향에 따라 마우스 커서 모양 변하게 하기
                    #region Get the direction and display correct cursor
                    //check if the mouse cursor is within the drag handle
                    if ((pos.X >= 0 && pos.X <= DRAG_HANDLE_SIZE) &&
                        (pos.Y >= 0 && pos.Y <= DRAG_HANDLE_SIZE))
                    {
                        this.mDirection = Direction.NW;
                        Cursor = Cursors.SizeNWSE;
                    }
                    else if ((pos.X >= this.Size.Width / 2 - DRAG_HANDLE_SIZE / 2) &&
                        pos.X <= this.Size.Width / 2 + DRAG_HANDLE_SIZE / 2 &&
                        pos.Y >= 0 && pos.Y <= DRAG_HANDLE_SIZE)
                    {
                        this.mDirection = Direction.N;
                        Cursor = Cursors.SizeNS;
                    }
                    else if ((pos.X >= this.Size.Width - DRAG_HANDLE_SIZE &&
                        pos.X <= this.Size.Width) &&
                        (pos.Y >= 0 && pos.Y <= DRAG_HANDLE_SIZE))
                    {
                        this.mDirection = Direction.NE;
                        Cursor = Cursors.SizeNESW;
                    }
                    else if ((pos.X >= 0 && pos.X <= DRAG_HANDLE_SIZE &&
                        pos.Y >= this.Size.Height / 2 - DRAG_HANDLE_SIZE / 2 &&
                        pos.Y <= this.Size.Height / 2 + DRAG_HANDLE_SIZE / 2))
                    {
                        this.mDirection = Direction.W;
                        Cursor = Cursors.SizeWE;
                    }
                    else if ((pos.X >= this.Size.Width - DRAG_HANDLE_SIZE &&
                        pos.X <= this.Size.Width &&
                        pos.Y >= this.Size.Height / 2 - DRAG_HANDLE_SIZE / 2 &&
                        pos.Y <= this.Size.Height / 2 + DRAG_HANDLE_SIZE / 2))
                    {
                        this.mDirection = Direction.E;
                        Cursor = Cursors.SizeWE;
                    }
                    else if ((pos.X >= 0 && pos.X <= DRAG_HANDLE_SIZE) &&
                        (pos.Y >= 0 + this.Size.Height - DRAG_HANDLE_SIZE &&
                        pos.Y <= 0 + this.Size.Height + DRAG_HANDLE_SIZE))
                    {
                        this.mDirection = Direction.SW;
                        Cursor = Cursors.SizeNESW;
                    }
                    else if ((pos.X >= this.Size.Width / 2 - DRAG_HANDLE_SIZE / 2) &&
                        pos.X <= this.Size.Width / 2 + DRAG_HANDLE_SIZE / 2 &&
                        pos.Y >= this.Size.Height - DRAG_HANDLE_SIZE &&
                        pos.Y <= 0 + this.Size.Height)
                    {
                        this.mDirection = Direction.S;
                        Cursor = Cursors.SizeNS;
                    }
                    else if ((pos.X >= this.Size.Width - DRAG_HANDLE_SIZE &&
                        pos.X <= this.Size.Width &&
                        pos.Y >= this.Size.Height - DRAG_HANDLE_SIZE &&
                        pos.Y <= this.Size.Height))
                    {
                        this.mDirection = Direction.SE;
                        Cursor = Cursors.SizeNWSE;
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                        this.mDirection = Direction.None;
                    }
                    #endregion
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.SizeAll;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Default;
        }

        public void ResizeImage(int desWidth, int desHeight)
        {
            Image image = this.mImage;

            var bmp = new Bitmap(desWidth, desHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, desWidth, desHeight);
            }

            this.Image = bmp;
        }

        public void ResizeProperty(Point location, Size size)
        {
            this.mPropertyGrid.Replace("x", location.X);
            this.mPropertyGrid.Replace("y", location.Y);

            this.mPropertyGrid.Replace("Width", size.Width);
            this.mPropertyGrid.Replace("Height", size.Height);
        }

        //pictureBox를 클릭하면 주위에 크기를 변경할 수 있는 모서리들이 생긴다.
        public void DrawControlBorder()
        {
            //define the border to be drawn, it will be offset by DRAG_HANDLE_SIZE / 2
            //around the this, so when the drag handles are drawn they will seem to be
            //connected in the middle.
            Rectangle Border = new Rectangle(
                new Point(0, 0),
                new Size(this.Size.Width,
                    this.Size.Height));
            //define the 8 drag handles, that has the size of DRAG_HANDLE_SIZE
            Rectangle NW = new Rectangle(
                new Point(0, 0),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle N = new Rectangle(
                new Point(0 + this.Size.Width / 2 - DRAG_HANDLE_SIZE / 2, 0),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle NE = new Rectangle(
                new Point(this.Size.Width - DRAG_HANDLE_SIZE, 0),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle W = new Rectangle(
                new Point(0, this.Size.Height / 2 - DRAG_HANDLE_SIZE / 2),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle E = new Rectangle(
                new Point(this.Size.Width - DRAG_HANDLE_SIZE, this.Size.Height / 2 - DRAG_HANDLE_SIZE / 2),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle SW = new Rectangle(
                new Point(0, this.Size.Height - DRAG_HANDLE_SIZE),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle S = new Rectangle(
                new Point(this.Size.Width / 2 - DRAG_HANDLE_SIZE / 2, this.Size.Height - DRAG_HANDLE_SIZE),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            Rectangle SE = new Rectangle(
                new Point(this.Size.Width - DRAG_HANDLE_SIZE, this.Size.Height - DRAG_HANDLE_SIZE),
                new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
            //get the form graphic
            Graphics g = this.CreateGraphics();
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
    }
}
