using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics
{
    public class Stick
    {
        public Point PointA;
        public Point PointB;
        public float Length;

        public static bool operator ==(Stick a, Stick b)
            => a.PointA == b.PointA && a.PointB == b.PointB;
        public static bool operator !=(Stick a, Stick b)
            => a.PointA != b.PointA || a.PointB != b.PointB;

        public override bool Equals(object obj)
        {
            if (obj is Stick s)
                return this == s;
            else
                return false;
        }
    }
}
