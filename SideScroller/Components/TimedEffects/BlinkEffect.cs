using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller.Components.TimedEffects
{
    public class BlinkEffect : TimedEffect
    {

        private Sprite sprite;

        public int MinAlpha { get; set; }
        public int MaxAlpha { get; set; }

        public BlinkEffect(int totalDuration, int delay, int minAlpha, int maxAlpha, Sprite sprite)
        {
            this.sprite = sprite;
            this.TotalDuration = totalDuration;
            this.Delay = delay;
            this.MinAlpha = minAlpha;
            this.MaxAlpha = maxAlpha;
        }

        protected override void Effect(GameTime time)
        {
            if (sprite.Alpha == MaxAlpha)
                sprite.Alpha = MinAlpha;
            else
                sprite.Alpha = MaxAlpha;
        }

        protected override void OnStateChange(Boolean state)
        {
            if (!state)
            {
                sprite.Alpha = 255;
                if (sprite is HealthSprite)
                    ((HealthSprite)sprite).Invulnerable = false;
            }
            else
            {
                if (sprite is HealthSprite)
                    ((HealthSprite)sprite).Invulnerable = true;
            }
        }
    }
}
