using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller.Components
{
    public abstract class PowerupSprite : Sprite
    {


        public PowerupSprite(Texture2D texture, Vector2 pos, Point frameDims, int collisionOffset,
            int frameDelay, Vector2 speed, float layerDepth, SpriteManager manager)
            : base(texture, pos, frameDims, collisionOffset, frameDelay, speed, layerDepth, manager)
        {
            this.Animation.Continuous = true;
        }

        public abstract void OnPowerup(UserControlledSprite sprite);

    }
}
