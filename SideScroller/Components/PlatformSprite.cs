using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace SideScroller.Components
{
    public class PlatformSprite : Sprite, CanSerialize
    {

        public PlatformSprite(Texture2D texture, Vector2 pos, Point frameDims, int collisionOffset, int frameDelay, Vector2 speed, SpriteManager manager)
            : base(texture, pos, frameDims, collisionOffset, frameDelay, speed, 0.9f, manager)
        {
            this.Blocking = true;
        }

        public override Vector2 Direction
        {
            get { return Vector2.Zero; }
        }

        public XmlElement Write(XmlDocument doc)
        {
            XmlElement platform = doc.CreateElement("Platform");

            XmlElement startPos = doc.CreateElement("StartPos");
            XmlElement pos = doc.CreateElement("Pos");
            XmlElement posX = doc.CreateElement("X");
            XmlElement posY = doc.CreateElement("Y");

            XmlElement dims = doc.CreateElement("Dims");
            XmlElement type = doc.CreateElement("Type");
            XmlElement width = doc.CreateElement("Width");
            XmlElement height = doc.CreateElement("Height");

            XmlElement startPosX = doc.CreateElement("X");
            XmlElement startPosY = doc.CreateElement("Y");

            posX.InnerText = this.Position.X.ToString();
            posY.InnerText = this.Position.Y.ToString();

            startPosX.InnerText = this.StartPosition.X.ToString();
            startPosY.InnerText = this.StartPosition.Y.ToString();

            string[] texName = this.TextureImage.Name.Split(new char[] { '_' });
            type.InnerText = texName[1];
            width.InnerText = texName[2].Split(new char[] { 'x' })[0];
            height.InnerText = texName[2].Split(new char[] { 'x' })[1];

            dims.AppendChild(type);
            dims.AppendChild(width);
            dims.AppendChild(height);

            startPos.AppendChild(startPosX);
            startPos.AppendChild(startPosY);

            pos.AppendChild(posX);
            pos.AppendChild(posY);

            platform.AppendChild(pos);
            platform.AppendChild(startPos);
            platform.AppendChild(dims);

            return platform;
        }

        public void Read(XmlElement element)
        {
            XmlNode dims = element.GetElementsByTagName("Dims")[0];
            XmlNode pos = element.GetElementsByTagName("Pos")[0];
            XmlNode startPos = element.GetElementsByTagName("StartPos")[0];
            this.TextureImage = Manager.contentManager.GetPlatformTexture(int.Parse(dims.ChildNodes[0].InnerText),
                int.Parse(dims.ChildNodes[1].InnerText), int.Parse(dims.ChildNodes[2].InnerText));
            this.Position = new Vector2(float.Parse(pos.FirstChild.InnerText), float.Parse(pos.LastChild.InnerText));
            this.StartPosition = new Vector2(float.Parse(startPos.FirstChild.InnerText), float.Parse(startPos.LastChild.InnerText));
            this.FrameDims = new Point(this.TextureImage.Width, this.TextureImage.Height);
        }
    }
}
