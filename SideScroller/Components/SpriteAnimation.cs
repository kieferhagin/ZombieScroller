using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller.Components
{
    public class SpriteAnimation
    {

        private Sprite sprite;

        public SpriteAnimation() { }

        public SpriteAnimation(Sprite sprite, int frameDelay)
        {
            this.FrameDelay = frameDelay;
            this.SheetSize = new Point(sprite.TextureImage.Bounds.Width / sprite.Bounds.Width,
                sprite.TextureImage.Bounds.Height / sprite.Bounds.Height);
            this.sprite = sprite;
            this.CurrentFrame = new Point(0, 0);
        }

        public void Animate(GameTime gameTime)
        {

            if (sprite.IsMovingX() || sprite.IsMovingY() || this.Continuous)
            {
                this.TimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (this.TimeSinceLastFrame >= this.FrameDelay)
                {
                    this.TimeSinceLastFrame -= this.FrameDelay;
                    int y = 0;
                    int x = this.CurrentFrame.X + 1;

                    if (sprite.Direction.X < 0)
                        y = 1;
                    if (x >= this.SheetSize.X)
                        x = 0;
                    this.CurrentFrame = new Point(x, y);

                }
            }
        }

        public int TimeSinceLastFrame { get; set; }
        public int FrameDelay { get; set; }
        public Boolean Continuous { get; set; }
        public Point CurrentFrame { get; set; }
        public Point SheetSize { get; set; }
    }
}
