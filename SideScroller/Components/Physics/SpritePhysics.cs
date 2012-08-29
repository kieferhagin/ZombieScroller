using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller.Components.Physics
{

    public class SpritePhysics
    {

        public List<SpritePhysicsComponent> components { get; set; }
        private Sprite sprite;

        public SpritePhysics()
        {

        }

        public SpritePhysics(Sprite sprite)
        {
            this.sprite = sprite;
            this.components = new List<SpritePhysicsComponent>();
        }

        public void AddComponent(SpritePhysicsComponent c)
        {
            this.components.Add(c);
        }

        public void Apply(Point windowSize)
        {
            foreach(SpritePhysicsComponent c in components)
            {
                c.Move(sprite, windowSize);
            }
        }
    }
}
