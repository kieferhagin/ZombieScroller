using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller.Components.TimedEffects
{
    public abstract class TimedEffect
    {

        private int TimeSinceLast;
        private int startTime;
        private bool enabled;
        private bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (value != enabled)
                    this.OnStateChange(value);
                enabled = value;

                if (!value)
                {
                    this.startTime = 0;
                }
            }

        }
        public int TotalDuration { get; set; }
        public int Delay { get; set; }

        public void Initiate()
        {
            Enabled = true;
            this.startTime = 0;
        }

        public void Update(GameTime time)
        {
            if (Enabled)
            {
                if (this.startTime == 0)
                    this.startTime = (int)time.TotalGameTime.TotalMilliseconds;
                int elapsed = (int)time.TotalGameTime.TotalMilliseconds - this.startTime;

                if (elapsed >= TotalDuration)
                    Enabled = false;
                   
                this.TimeSinceLast += time.ElapsedGameTime.Milliseconds;
                if (this.TimeSinceLast >= this.Delay)
                {
                    this.TimeSinceLast -= this.Delay;

                    this.Effect(time);
                }
            }
        }

        protected abstract void Effect(GameTime time);

        protected abstract void OnStateChange(Boolean state);

    }
}
