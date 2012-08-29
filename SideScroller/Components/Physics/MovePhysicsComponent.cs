using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller.Components.Physics
{
    public class MovePhysicsComponent : SpritePhysicsComponent
    {

        public override void Move(Sprite sprite, Point windowSize)
        {
            Vector2 dir = sprite.Direction;
            if (sprite.Animated)
            {
                if (!(sprite.IsMovingX() || sprite.IsMovingY()))
                {
                    sprite.Animation.CurrentFrame = new Point(1, sprite.Animation.CurrentFrame.Y);
                }
            }

            if ((sprite.Position + dir).X > 0)
                sprite.Position += dir;
        }

    }
}
