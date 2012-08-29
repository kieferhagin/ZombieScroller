using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SideScroller.Components
{
    public class LevelManager
    {

        private List<Level> levels;

        public SpriteManager SpriteManager { get; set; }

        public LevelManager(SpriteManager manager)
        {
            this.levels = new List<Level>();
            this.levels.Add(new Level("TestLevel", this));
            this.SpriteManager = manager;
        }

        private int curLevelIndex;

        public int CurLevelIndex
        {
            get
            {
                return curLevelIndex;
            }

            set
            {
                if (value >= 0 && value < levels.Count)
                {
                    curLevelIndex = value;
                    SpriteManager.ClearSprites();
                    foreach (Sprite s in levels[curLevelIndex].Sprites)
                    {
                        SpriteManager.addSprite(s);
                    }
                }
            }
        }

        public Level CurLevel
        {
            get
            {
                return levels[curLevelIndex];
            }

        }

        public void LoadAll()
        {
            this.levels.Clear();
            try
            {
                int i = 0;
                while (true)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@"Content\Levels\Level" + i + ".xml");
                    Level l = new Level("", this);
                    l.Read(doc.DocumentElement);
                    this.levels.Add(l);
                    i++;
                }
            }
            catch
            {

            }
        }

        public void Save()
        {
            CurLevel.Sprites = SpriteManager.spriteList;

            for (int i = 0; i < levels.Count; i++)
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(levels[i].Write(doc));
                doc.Save(@"Content\Levels\Level" + i + ".xml");
            }
        }
    }
}
