using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SideScroller.Components
{
    public class HealthPackSprite : PowerupSprite
    {


        public HealthPackSprite(Texture2D texture, Vector2 pos, SpriteManager manager)
            : base(texture, pos, new Point(30, 20), 0, 100, Vector2.Zero, 0, manager)
        {

        }

        public override Vector2 Direction
        {
            get { return Vector2.Zero;  }
        }

        public override void OnPowerup(UserControlledSprite sprite)
        {
            if (sprite.CurHealth < sprite.MaxHealth)
            {
                sprite.Heal();
                Manager.removeSprite(this);
            }
        }
    }
}
