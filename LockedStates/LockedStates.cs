using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics.LockedStates
{
    public static class LockedStates
    {
        public static ILockedState[] States = new ILockedState[]
        {
            new StateUnlocked(),
            new StateLocked(),
            new StateUnlockedX(),
            new StateUnlockedY(),
        };
    }
}
