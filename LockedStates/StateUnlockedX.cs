using GameEngine.Maths.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics.LockedStates
{
    public class StateUnlockedX : ILockedState
    {
        public int Color { get; } = System.Drawing.Color.Green.ToArgb();

        public Vector2F Add(Vector2F og, Vector2F toAdd)
            => new Vector2F(og.X + toAdd.X, og.Y);

        public Vector2F Set(Vector2F og, Vector2F value)
            => new Vector2F(value.X, og.Y);
    }
}
