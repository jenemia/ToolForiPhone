using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Collections;

namespace ToolForiPhone
{
    class XMLAdapter
    {
        string mName;

        public XMLAdapter()
        {
            this.mName = "ToolXML.xml";

            XmlDocument NewXmlDoc = new XmlDocument();
            NewXmlDoc.AppendChild(NewXmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            //최상위 노드를 만든다
            XmlNode root = NewXmlDoc.CreateElement("", "Root", "");
            NewXmlDoc.AppendChild(root);

            //지정된 XML문서로 만들고 저장
            NewXmlDoc.Save(this.mName);
        }

        private XmlNode CreateNode(XmlDocument xmlDoc, string name, string innerXml)
        {
            XmlNode node = xmlDoc.CreateElement(string.Empty, name, string.Empty);
            node.InnerXml = innerXml;

            return node;
        }

        public void NodeSetting(ArrayList list)
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(this.mName);

            /*
            XmlNode FirstNode = XmlDoc.DocumentElement;
            foreach ( PictureComponents picture in list)
            {
                XmlElement root = XmlDoc.CreateElement(picture.mName);
                root.SetAttribute("index", Convert.ToString(picture.mIndexNumber) );
                root.SetAttribute("ViewController", Convert.ToString(picture.mControllerNumber));

                Point location= picture.mSaveLocation;
                Size size = picture.mSaveSizeImage;

                XmlElement child = XmlDoc.CreateElement("rect");
                child.SetAttribute("width", Convert.ToString(size.Width) );
                child.SetAttribute("height", Convert.ToString(size.Height));
                child.SetAttribute("x", Convert.ToString(location.X));
                child.SetAttribute("y", Convert.ToString(location.Y));
                root.AppendChild(child);

                FirstNode.AppendChild(root);
            }
             */

            XmlNode FirstNode = XmlDoc.DocumentElement;
            foreach( PictureComponents picture in list )
            {
                if( picture.mControllerNumber == -1) //UIViewController
                {
                    XmlElement root = XmlDoc.CreateElement(picture.mName);
                    root.SetAttribute("index", Convert.ToString(picture.mIndexNumber));
                    root.SetAttribute("ViewController", Convert.ToString(picture.mControllerNumber));
                    Point location = picture.mSaveLocation;
                    Size size = picture.mSaveSizeImage;

                    XmlElement child = XmlDoc.CreateElement("rect");
                    child.SetAttribute("width", Convert.ToString(size.Width));
                    child.SetAttribute("height", Convert.ToString(size.Height));
                    child.SetAttribute("x", Convert.ToString(location.X));
                    child.SetAttribute("y", Convert.ToString(location.Y));
                    root.AppendChild(child);

                    FirstNode.AppendChild(root);
                }
            }

            foreach (PictureComponents picture in list)
            {
                XmlNodeList elemList = XmlDoc.GetElementsByTagName("UIViewController");
                foreach (XmlNode node in elemList)
                {
                    if( Convert.ToInt32(node.Attributes["index"].Value) == picture.mControllerNumber )
                    {
                        //UIViewController찾아서 child로 넣는다.
                        Console.WriteLine("right");
                        XmlElement root = XmlDoc.CreateElement(picture.mName);
                        root.SetAttribute("index", Convert.ToString(picture.mIndexNumber));
                        root.SetAttribute("ViewController", Convert.ToString(picture.mControllerNumber));
                        Point location = picture.mSaveLocation;
                        Size size = picture.mSaveSizeImage;

                        XmlElement child = XmlDoc.CreateElement("rect");
                        child.SetAttribute("width", Convert.ToString(size.Width));
                        child.SetAttribute("height", Convert.ToString(size.Height));
                        child.SetAttribute("x", Convert.ToString(location.X));
                        child.SetAttribute("y", Convert.ToString(location.Y));
                        root.AppendChild(child);

                        node.AppendChild(root);
                    }
                }
            }

            XmlDoc.Save(this.mName);
        }
    }
}
