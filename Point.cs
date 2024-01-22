using GameEngine.Maths.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics
{
    public class Point
    {
        public Vector2F Pos;
        public Vector2F PrevPos;
        public int State;
        public float Mass;

        public Point(Vector2F _pos)
        {
            Pos = _pos;
            PrevPos = _pos;
            State = 0;
            Mass = 1f;
        }
    }
}
