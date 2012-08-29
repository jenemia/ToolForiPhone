using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace ToolForiPhone
{
    public partial class Form1 : Form
    {
        ArrayList mListImage;
        static public int mSelectedTag;
        static public int mTagCount;
        PictureComponents mSelectedControl;
        PictureComponents mPicturePickUp;

        public Form1()
        {
            InitializeComponent();
            this.mListImage = new ArrayList();
            Form1.mSelectedTag = 0;
            Form1.mTagCount = 0;
            this.mSelectedControl = null;
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

        //PictureComponents에서 호출 되는 함수
        public void PictureBoxMouseDown()
        {
            panel1.Invalidate();
            PictureComponents sender = ((PictureComponents)this.mListImage[Form1.mSelectedTag]);

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
            if (0 == Convert.ToInt32(item.Tag))
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
