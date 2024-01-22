using GameEngine.Maths.Vectors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics.LockedStates
{
    public class StateLocked : ILockedState
    {
        public int Color { get; } = System.Drawing.Color.Red.ToArgb();

        public Vector2F Add(Vector2F og, Vector2F toAdd)
            => og;

        public Vector2F Set(Vector2F og, Vector2F value)
            => og;
    }
}
