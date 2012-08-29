using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller.Components
{
    public abstract class HealthSprite : Sprite
    {


        public HealthSprite(Texture2D texture, Vector2 pos, Point frameDims, int collisionOffset,
            int frameDelay, Vector2 speed, float layerDepth, SpriteManager manager)
            : base(texture, pos, frameDims, collisionOffset, frameDelay, speed, layerDepth, manager)
        {
            MaxHealth = 100;
            Heal();
        }

        private int curHealth;

        public bool Invulnerable { get; set; }

        public int CurHealth
        {
            get
            {
                return curHealth;
            }
            set
            {
                curHealth = value;
            }
        }
        public int MaxHealth { get; set; }

        public void Heal(int amount)
        {
            CurHealth += amount;
        }

        public void Heal()
        {
            CurHealth = MaxHealth;
        }

        public void Damage(int amount, Sprite attacker)
        {
            if (!Invulnerable)
            {
                CurHealth -= amount;
                if (this.IsDead())
                    this.OnDeath(attacker);
            }
        }

        public void Kill()
        {
            this.Damage(this.CurHealth, null);
        }

        public virtual void OnDeath(Sprite attacker)
        {

        }

        public bool IsDead()
        {
            return this.CurHealth <= 0;
        }

    }
}
