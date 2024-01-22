using GameEngine.Maths.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics.LockedStates
{
    public class StateUnlocked : ILockedState
    {
        public int Color { get; } = System.Drawing.Color.White.ToArgb();

        public Vector2F Add(Vector2F og, Vector2F toAdd)
            => og + toAdd;

        public Vector2F Set(Vector2F og, Vector2F value)
            => value;
    }
}
