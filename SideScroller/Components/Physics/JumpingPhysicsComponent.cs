using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SideScroller.Components.Physics
{
    public class JumpingPhysicsComponent : SpritePhysicsComponent
    {
        public override void Move(Sprite sprite, Microsoft.Xna.Framework.Point windowSize)
        {
            if (sprite.IsJumping)
            {
                sprite.Position -= new Microsoft.Xna.Framework.Vector2(0, sprite.Speed.Y);
            }
        }
    }
}
