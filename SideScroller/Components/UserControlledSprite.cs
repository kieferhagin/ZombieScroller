using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SideScroller.Components.Physics;
using SideScroller.Components.TimedEffects;

namespace SideScroller.Components
{
    public class UserControlledSprite : HealthSprite
    {
        private Texture2D bulletTexture;
        private int timeSinceLastBullet;
        private static int bulletCooldownTime = 200;
        public int TotalXP { get; set; }
        public int TotalCola { get; set; }

        public Components.Upgrades Upgrades { get; set; }

        public UserControlledSprite(Texture2D texture, Texture2D bulletTexture, Vector2 pos, Point frameDims, int collisionOffset
            , int frameDelay, Vector2 speed, float layerDepth, SpriteManager manager)
            : base(texture, pos, frameDims, collisionOffset, frameDelay, speed, layerDepth, manager)
        {
            this.Physics.AddComponent(new GravityPhysicsComponent());
            this.Physics.AddComponent(new MovePhysicsComponent());

            this.Physics.AddComponent(new JumpingPhysicsComponent());
            this.bulletTexture = bulletTexture;

            this.TimedEffects.AddEffect("Blink", new BlinkEffect(3000, 100, 1, 255, this));
            this.Upgrades = new Upgrades();
        }

        public override void Update(GameTime gameTime, Point windowSize)
        {
            if (this.Position.Y > 700)
            {
                this.Kill();
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Space))
                this.Jump(gameTime);
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                this.timeSinceLastBullet += gameTime.ElapsedGameTime.Milliseconds;
                this.fireBullet();
            }
            base.Update(gameTime, windowSize);
        }

        public bool fireBullet()
        {

            if (this.timeSinceLastBullet >= bulletCooldownTime)
            {
                this.timeSinceLastBullet -= bulletCooldownTime;

                ProjectileSprite bullet = new ProjectileSprite(this, bulletTexture,
                    this.Position, this.FrameDims, 10, 0, this.Speed * 4f, 0f, 400, this.Manager);
                Vector2 dir = Vector2.Zero;

                if (Animation.CurrentFrame.Y == 0)
                    dir.X = 1;
                else
                    dir.X = -1;
                bullet.Animation.CurrentFrame = this.Animation.CurrentFrame;
                bullet.launch(dir);
                this.Manager.addSprite(bullet);
                this.Manager.soundBank.PlayCue("gun_shot");
                return true;
            }

            return false;

        }

        public override void OnCollision(List<Sprite> collide)
        {

            foreach (Sprite s in collide)
            {
                if (s is ZombieSprite)
                {
                    if (!this.Invulnerable)
                    {
                        this.Damage(50, s);
                        this.TimedEffects.Enable("Blink");
                    }
                }
                else if (s is PowerupSprite)
                {
                    ((PowerupSprite)s).OnPowerup(this);
                }
            }
            base.OnCollision(collide);
        }

        public override Vector2 Direction
        {
            get
            {
                Vector2 dir = Vector2.Zero;
                KeyboardState ks = Keyboard.GetState();

                //left
                if (ks.IsKeyDown(Keys.A))
                    dir.X -= 1;
                //right
                if (ks.IsKeyDown(Keys.D))
                    dir.X += 1;

                return dir * Speed;
            }
        }

        public override void OnDeath(Sprite attacker)
        {
            base.OnDeath(attacker);
            Manager.PlayerDied();
            this.TimedEffects.Enable("Blink");
        }

    }
}
