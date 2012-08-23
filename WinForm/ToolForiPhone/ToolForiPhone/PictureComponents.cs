using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;

namespace ToolForiPhone
{
    class PictureComponents : PictureBox
    {
        Point mPointMouse;
        Image mImage;
        Form1 mForm;
        PropertyGridCustom mPropertyGrid;

        public PictureComponents( ImageList list, int index, int tag, Form1 form)
        {
            this.Image = list.Images[index];
            this.mImage = this.Image;
            this.Size = this.Image.Size;
            this.Tag = tag;
            this.mForm = form;
            this.SizeMode =  PictureBoxSizeMode.StretchImage;
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

        public PropertyGridCustom PropertyGrid
        {
            get { return this.mPropertyGrid; }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Left)
            {
                Form1.mSelectedTag = (int)this.Tag;
                this.mPointMouse = e.Location;
                this.mForm.PictureBoxMouseDown();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.mPointMouse = Point.Empty;

            Form1.mSelectedTag = (int)this.Tag;
            //이동된 좌표값으로 수정.                        
            this.mPropertyGrid.Replace("x", this.Location.X);
            this.mPropertyGrid.Replace("y", this.Location.Y);

            this.mForm.PictureBoxMouseUp();
            this.mForm.panel1_MouseUp(null, null);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if( e.Button == MouseButtons.Left )
            {
                if( mPointMouse != Point.Empty )
                {
                    Rectangle rect = this.Parent.RectangleToScreen(this.Parent.Bounds);

                    Point mouse = e.Location;
                    mouse.X += rect.X;
                    mouse.Y += rect.Y;

                    if (rect.Contains(mouse))
                    {
                        Point pt2 = e.Location;
                        Point pt = new Point(pt2.X - mPointMouse.X, pt2.Y - mPointMouse.Y);
                        this.Location = new Point(this.Location.X + pt.X, this.Location.Y + pt.Y);
                        mPointMouse = new Point(pt2.X - pt.X, pt2.Y - pt.Y);
                    }
                    else
                        mPointMouse = Point.Empty;
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

        public void ResizeProperty( Point location, Size size )
        {
            this.mPropertyGrid.Replace("x", location.X);
            this.mPropertyGrid.Replace("y", location.Y);

            this.mPropertyGrid.Replace("Width", size.Width);
            this.mPropertyGrid.Replace("Height", size.Height);
        }
    }
}
