using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SideScroller.Components
{
    public class Level : CanSerialize
    {

        public List<Sprite> Sprites { get; set; }
        public string Name { get; set; }
        private LevelManager manager;

        public Level(String name, LevelManager manager)
        {
            this.Name = name;
            this.manager = manager;
            this.Sprites = new List<Sprite>();
        }

        public XmlElement Write(XmlDocument doc)
        {
            XmlElement level = doc.CreateElement("Level");
            XmlElement name = doc.CreateElement("LevelName");
            XmlElement spriteList = doc.CreateElement("Sprites");

            name.InnerText = this.Name;

            foreach (Sprite s in Sprites)
            {
                if (s is CanSerialize)
                    spriteList.AppendChild(((CanSerialize)s).Write(doc));
            }

            level.AppendChild(name);
            level.AppendChild(spriteList);

            return level;
        }

        public void Read(XmlElement element)
        {
            this.Name = element.GetElementsByTagName("LevelName")[0].InnerText;
            this.Sprites.Clear();

            foreach (XmlNode node in element.GetElementsByTagName("Sprites")[0].ChildNodes)
            {
                this.Sprites.Add(manager.SpriteManager.SpriteFromXML(node));
            }
        }
    }
}
