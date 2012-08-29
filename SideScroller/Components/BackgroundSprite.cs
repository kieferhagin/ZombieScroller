using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller.Components.Physics;

namespace SideScroller.Components
{
    public class BackgroundSprite : Sprite
    {

        public static float SPEED_FACTOR = 0.1f;
        private Sprite actor;

        public BackgroundSprite(Texture2D texture, Point frameDims, Sprite actor, SpriteManager manager)
            : base(texture, new Vector2(0,0), frameDims, 0, 1, actor.Speed * SPEED_FACTOR, 1, manager)
        {
            this.CanCollide = false;
            this.actor = actor;
            this.Physics.AddComponent(new MovePhysicsComponent());
        }

        public override Vector2 Direction
        {
            get
            {
                return Vector2.Zero;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera2D camera, Vector2 scale)
        {
            this.Position = new Vector2(camera.Window.X, camera.Window.Y);
            base.Draw(gameTime, batch, camera, scale);
        }

        
    }
}
