using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller.Components
{
    public class PanningCamera : Camera2D
    {

        public PanningCamera(Sprite actor, Point windowBounds)
            : base(actor, windowBounds)
        {
        }

        public override void Update()
        {
            Point p = centerOnActor();
            this.Window = new Rectangle(p.X, p.Y, Window.Width, Window.Height);
        }

        private Point centerOnActor()
        {
            //offset half screen from actors center
            int x = (int)(Actor.Position.X + (Actor.FrameDims.X / 2)) - (Window.Width / 2);
            int y = (int)(Actor.Position.Y + (Actor.FrameDims.Y / 2)) - (Window.Height / 2);

            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            return new Point(x, y);
        }
    }
}
