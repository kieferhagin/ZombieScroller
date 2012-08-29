using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller.Components
{
    public abstract class Camera2D
    {
        public Rectangle Window { get; protected set; }

        public Sprite Actor { get; protected set; }

        public Camera2D(Sprite actor, Point windowBounds)
        {
            this.Actor = actor;
            this.Window = new Rectangle(0, 0, windowBounds.X, windowBounds.Y);
        }

        public abstract void Update();

    }
}
