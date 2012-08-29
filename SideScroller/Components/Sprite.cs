using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using SideScroller.Components.Physics;
using SideScroller.Components.TimedEffects;
using System.Xml;

namespace SideScroller.Components
{
    public abstract class Sprite
    {
        //sprite's texture
        public Texture2D TextureImage { get; set; }
        //current position in the world
        public Vector2 Position { get; set; }
        public Vector2 StartPosition { get; set; }
        //Dimension of a frame of animation
        public Point FrameDims { get; set; }
        //amount to offset collision rectangle bounds
        public int CollisionOffset { get; set; }
        public Boolean Animated { get; set; }
        
        //speed in x and y
        public Vector2 Speed { get;  set; }

        public float LayerDepth { get; set; }

        public abstract Vector2 Direction { get; }

        public Boolean Blocking { get; set; }

        public Boolean CanCollide { get; set; }

        private int alpha;

        public TimedEffects.TimedEffects TimedEffects { get; set; }

        public int Alpha
        {

            get
            {
                return alpha;
            }

            set
            {
                if (value > 255)
                    alpha = 255;
                else if (value < 0)
                    alpha = 0;
                else
                    alpha = value;
            }
        }

        public Boolean IsJumping
        {
            get
            {
                return JumpVelocity > 0f;
            }
        }

        private float jumpV = 0f;

        public float JumpVelocity
        {
            get { return jumpV ; }
            set
            {
                if (value < 0f)
                    jumpV = 0f;
                else
                    jumpV = value;
            }

        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, FrameDims.X, FrameDims.Y);
            }
        }

        public SpritePhysics Physics { get; set; }

        public SpriteManager Manager { get; set; }

        private static int jumpWait = 100;
        private int timeSinceJump;

        public SpriteAnimation Animation { get; set; }

        public Sprite(Texture2D texture, Vector2 pos, Point frameDims, int collisionOffset,
            int frameDelay, Vector2 speed, float layerDepth, SpriteManager manager)
        {
            this.CanCollide = true;
            this.TextureImage = texture;
            this.Position = pos;
            this.StartPosition = pos;
            this.FrameDims = frameDims;
            this.CollisionOffset = collisionOffset;
            this.Alpha = 255;
            
            this.Animation = new SpriteAnimation(this,frameDelay);
            this.Speed = speed;
            this.LayerDepth = layerDepth;
            this.Physics = new SpritePhysics(this);
            this.Animated = true;
            this.Blocking = false;
            this.Manager = manager;
            this.TimedEffects = new TimedEffects.TimedEffects();
        }

        public void Jump(GameTime gameTime)
        {

            this.timeSinceJump += gameTime.ElapsedGameTime.Milliseconds;
            if (this.timeSinceJump >= jumpWait)
            {
                if (!this.IsJumping && this.IsOnGround)
                {
                    this.JumpVelocity = 150f;
                    this.timeSinceJump -= jumpWait;

                }
            }
            
        }

        public Boolean IsOnGround
        {
            get
            {

                
                Rectangle nRect = this.Bounds;
                nRect.Y += 1;
                return Manager.isBlocked(nRect) || (this.Bounds.Bottom >= Manager.Camera.Window.Height);
            }
        }

        public Boolean IsMovingX()
        {
            return this.Direction.X != 0;
        }

        public Boolean IsMovingY()
        {
            return this.Direction.Y != 0;
        }

        private void Animate(GameTime gameTime)
        {
            if (Animated)
            {
                Animation.Animate(gameTime);
            }
        }


        private void Move(Point windowSize)
        {
            float lastX = this.Position.X;
            float lastY = this.Position.Y;
            Physics.Apply(windowSize);

            Vector2 newPos = this.Position;

            this.Position = new Vector2(newPos.X, lastY);
            List<Sprite> xCollide = Manager.checkCollisions(this);
            this.Position = new Vector2(lastX, newPos.Y);
            List<Sprite> yCollide = Manager.checkCollisions(this);
            this.Position = newPos;
            foreach (Sprite s in xCollide)
            {
                if (s.Blocking)
                {
                    this.Position = new Vector2(lastX, newPos.Y);
                    break;
                }
            }

            foreach (Sprite s in yCollide)
            {
                if (s.Blocking)
                {
                    this.Position = new Vector2(this.Position.X, lastY);
                    break;
                }
            }
            Vector2 finalPos = this.Position;
            this.Position = newPos;
            this.OnCollision(Manager.checkCollisions(this));
            this.Position = finalPos;
        }

        public virtual void Update(GameTime gameTime, Point windowSize)
        {
            this.Move(windowSize);
            this.Animate(gameTime);
            this.TimedEffects.Update(gameTime);
        }

        public virtual void OnCollision(List<Sprite> collide)
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch batch, Camera2D camera, Vector2 scale)
        {
            if (camera.Window.Intersects(this.Bounds))
            {
                Vector2 offset = new Vector2(Position.X - camera.Window.X, Position.Y - camera.Window.Y);
                /*           batch.Draw(this.TextureImage, new Rectangle((int)offset.X, (int)offset.Y, FrameDims.X, FrameDims.Y),
                               new Rectangle(this.CurrentFrame.X * this.FrameDims.X, this.CurrentFrame.Y * this.FrameDims.Y,
                               this.FrameDims.X, this.FrameDims.Y), Color.White, 0, Vector2.Zero, SpriteEffects.None, this.LayerDepth); */

                batch.Draw(this.TextureImage, offset, new Rectangle(this.Animation.CurrentFrame.X * this.FrameDims.X, this.Animation.CurrentFrame.Y * this.FrameDims.Y,
                    this.FrameDims.X, this.FrameDims.Y), new Color(255, 255, 255, alpha), 0, Vector2.Zero, scale, SpriteEffects.None, this.LayerDepth);
            }
        }

    }
}
