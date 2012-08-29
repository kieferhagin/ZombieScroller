using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller.Components
{
    public class ContentManager {

        private Game game;

        public ContentManager(Game game)
        {
            this.game = game;
        }

        public Texture2D GetPlatformTexture(int type, int width, int height)
        {
            String name = "platform_" + type + "_" + width + "x" + height;
            return GetTexture(name);
        }

        public Texture2D GetZombieTexture(int type)
        {
            return GetTexture("zombie_" + type);
        }

        public Texture2D GetTexture(String name)
        {
            Texture2D tex = game.Content.Load<Texture2D>(@"Images/" + name);
            tex.Name = name;
            return tex;
        }

    }
}
