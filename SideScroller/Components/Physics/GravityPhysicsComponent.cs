using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller.Components.Physics
{
    public class GravityPhysicsComponent : SpritePhysicsComponent
    {

        private const int GRAVITY_SPEED = 7;

        public override void Move(Sprite sprite, Microsoft.Xna.Framework.Point windowSize)
        {

            Vector2 dir = Vector2.Zero;

            //if (sprite.IsOnGround)
               // return;
            
            //if we arent moving in the y by some other force other than gravity
            if (sprite.IsJumping)
            {
                sprite.JumpVelocity -= GRAVITY_SPEED;
            }
            else
            {


                if (!sprite.IsMovingY())
                {
                    //apply gravitational pull
                    Rectangle nRect = sprite.Bounds;
                    nRect.Y += GRAVITY_SPEED;
                    
                    bool blocked = false;
                    foreach (Sprite s in sprite.Manager.checkCollisions(nRect))
                    {
                        if (s.Blocking)
                        {
                            blocked = true;
                            dir.Y += s.Bounds.Y - (sprite.Bounds.Bottom);
                            break;
                        }
                        
                    }
                    
                    if (sprite.Bounds.Bottom < windowSize.Y && !blocked)
                        dir.Y += GRAVITY_SPEED;
                }
            }

            sprite.Position += dir;
        }
    }
}
