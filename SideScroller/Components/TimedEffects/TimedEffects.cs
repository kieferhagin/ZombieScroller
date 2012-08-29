using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace SideScroller.Components.TimedEffects
{
    public class TimedEffects
    {
        private Hashtable effects;

        public TimedEffects()
        {
            effects = new Hashtable();
        }

        public void AddEffect(string name, TimedEffect effect)
        {
            this.effects.Add(name, effect);
        }

        public void Update(GameTime time)
        {
            foreach (TimedEffect t in effects.Values)
            {
                t.Update(time);
            }
        }

        public void Enable(string name)
        {
            ((TimedEffect)this.effects[name]).Initiate();
        }

    }
}
