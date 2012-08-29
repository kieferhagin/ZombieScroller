using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace SideScroller.Components
{
    public class ZombieColaSprite : PowerupSprite, CanSerialize
    {


        public ZombieColaSprite(Texture2D texture, Vector2 pos, SpriteManager manager)
            : base(texture, pos, new Point(21, 40), 0, 100, Vector2.Zero, 0, manager)
        { }

        public override void OnPowerup(UserControlledSprite sprite)
        {
            sprite.Heal();
            sprite.TotalCola++;
            Manager.removeSprite(this);
        }

        public override Vector2 Direction
        {
            get { return Vector2.Zero; }
        }


        public XmlElement Write(XmlDocument doc)
        {
            XmlElement cola = doc.CreateElement("ZombieCola");
            XmlElement startPos = doc.CreateElement("StartPos");
            XmlElement startPosX = doc.CreateElement("X");
            XmlElement startPosY = doc.CreateElement("Y");

            startPosX.InnerText = this.Position.X.ToString();
            startPosY.InnerText = this.Position.Y.ToString();

            startPos.AppendChild(startPosX);
            startPos.AppendChild(startPosY);

            cola.AppendChild(startPos);

            return cola;
        }

        public void Read(XmlElement element)
        {
            XmlNode startPos = element.GetElementsByTagName("StartPos")[0];
            this.StartPosition = new Vector2(float.Parse(startPos.FirstChild.InnerText), float.Parse(startPos.LastChild.InnerText));
            this.Position = this.StartPosition;
        }
    }
}
