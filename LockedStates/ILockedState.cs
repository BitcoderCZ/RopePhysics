using GameEngine.Maths.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics
{
    public interface ILockedState
    {
        int Color { get; }

        Vector2F Add(Vector2F og, Vector2F toAdd);
        Vector2F Set(Vector2F og, Vector2F value);
    }
}
