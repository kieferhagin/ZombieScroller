using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SideScroller.Components.Physics;
using System.Xml;

namespace SideScroller.Components
{
    public class ZombieSprite : HealthSprite, CanSerialize
    {

        private Vector2 lastDir = Vector2.Zero;
        public static Texture2D healthBarEnemy;
        public int KillXp { get; set; }


        public ZombieSprite(Texture2D texture, Vector2 pos, Point frameDims, int collisionOffset,
            int frameDelay, Vector2 speed, float layerDepth, SpriteManager manager)
            : base(texture, pos, frameDims, collisionOffset, frameDelay, speed, layerDepth, manager)
        {
            this.Physics.AddComponent(new GravityPhysicsComponent());
            this.Physics.AddComponent(new MovePhysicsComponent());
            this.KillXp = 100;
        }

        public override void OnDeath(Sprite attacker)
        {
            Manager.removeSprite(this);
            if (attacker is UserControlledSprite)
                ((UserControlledSprite)attacker).TotalXP += this.KillXp;
            Manager.soundBank.PlayCue("splat");
            base.OnDeath(attacker);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera2D camera, Vector2 scale)
        {
            base.Draw(gameTime, batch, camera, scale);
            float health = ((float)this.CurHealth) / ((float)this.MaxHealth);
            Vector2 offset = new Vector2(this.Position.X - camera.Window.X, this.Position.Y - camera.Window.Y - 16);
            offset.X -= (healthBarEnemy.Bounds.Width / 2) - (this.Bounds.Width / 2);
            batch.Draw(healthBarEnemy, new Rectangle((int)offset.X, (int)offset.Y, (int)(((float)healthBarEnemy.Width) * health),
                healthBarEnemy.Height), new Rectangle(0, 0, (int)(((float)healthBarEnemy.Width) * health), healthBarEnemy.Height),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.09f);
        }


        public override Vector2 Direction
        {
            get {
                Vector2 playerPos = Manager.Player.Position;
                if (Calculations.GetDistance(playerPos, this.Position) <= 400f)
                {
                    float diff = this.Position.X - playerPos.X;

                    if (diff > 0)
                        return new Vector2(-1, 0) * this.Speed;
                    else
                        return new Vector2(1, 0) * this.Speed;

                }
                else
                {
                    Random rnd = new Random();
                    int changeRoll = rnd.Next(0, 500);

                    if (changeRoll < 10)
                    {
                        lastDir = new Vector2(rnd.Next(-1, 1), 0);
                    }

                    return lastDir * Speed;

                }
            
            }
        }

        public XmlElement Write(XmlDocument doc)
        {
            XmlElement x = doc.CreateElement("ZombieSprite");
            XmlElement pos = doc.CreateElement("Pos");
            XmlElement posX = doc.CreateElement("X");
            XmlElement posY = doc.CreateElement("Y");
            XmlElement startPos = doc.CreateElement("StartPos");
            XmlElement startPosX = doc.CreateElement("X");
            XmlElement startPosY = doc.CreateElement("Y");

            posX.InnerText = this.Position.X.ToString();
            posY.InnerText = this.Position.Y.ToString();
            pos.AppendChild(posX);
            pos.AppendChild(posY);

            startPosX.InnerText = this.StartPosition.X.ToString();
            startPosY.InnerText = this.StartPosition.Y.ToString();
            startPos.AppendChild(startPosX);
            startPos.AppendChild(startPosY);

            x.AppendChild(pos);
            x.AppendChild(startPos);

            return x;
        }

        public void Read(XmlElement element)
        {
            XmlNode pos = element.GetElementsByTagName("Pos")[0];
            XmlNode startPos = element.GetElementsByTagName("StartPos")[0];
            this.Position = new Vector2(float.Parse(pos.FirstChild.InnerText), float.Parse(pos.LastChild.InnerText));
            this.StartPosition = new Vector2(float.Parse(startPos.FirstChild.InnerText), float.Parse(startPos.LastChild.InnerText));
        }
    }
}
