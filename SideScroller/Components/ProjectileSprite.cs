using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller.Components.Physics;

namespace SideScroller.Components
{
    class ProjectileSprite : Sprite
    {

        private Vector2 dir = Vector2.Zero;
        private Vector2 startPos;
        private int range;
        public int BaseDamage { get; set; }
        private Sprite owner;

        public ProjectileSprite(Sprite owner, Texture2D texture, Vector2 pos, Point frameDims, int collisionOffset
            , int frameDelay, Vector2 speed, float layerDepth, int range, SpriteManager manager)
            : base(texture, pos, frameDims, collisionOffset, frameDelay, speed, layerDepth, manager)
        {
            this.owner = owner;
            this.Animated = false;
            this.Physics.AddComponent(new MovePhysicsComponent());
            this.range = range;
            this.startPos = pos;
            this.BaseDamage = 10;
        }

        public override Vector2 Direction
        {
            get { return dir * this.Speed; }
        }

        public void launch(Vector2 dir)
        {
            this.dir = dir;
        }

        public override void OnCollision(List<Sprite> collide)
        {
            collide.Remove(owner);
            List<Sprite> removals = new List<Sprite>();
            foreach (Sprite c in collide)
            {

                if (c is HealthSprite)
                {
                    int damage = this.BaseDamage;
                    if (this.owner is UserControlledSprite)
                        damage += ((UserControlledSprite)this.owner).Upgrades.DamageUpgrade;
                    ((HealthSprite)c).Damage(damage, owner);
                }
                if (c is ProjectileSprite)
                    removals.Add(c);
            }

            foreach (Sprite c in removals)
                collide.Remove(c);

            if (!(collide.Count == 0))
                Manager.removeSprite(this);
        }

        public override void Update(GameTime gameTime, Point windowSize)
        {
            if (this.Position.X <= 10 || MathHelper.Distance(startPos.X, this.Position.X) >= range)
                this.Manager.removeSprite(this);
            base.Update(gameTime, windowSize);
        }
    }
}
