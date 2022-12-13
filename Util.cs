using GameEngine.Maths;
using GameEngine.Maths.Vectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RopePhysics
{
    public static class Util
    {
        public static byte[] ReadResource(string name)
        {
            byte[] bytes;
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] s= asm.GetManifestResourceNames();
            string resourceName = "RopePhysics." + name;
            using (Stream stream = asm.GetManifestResourceStream(resourceName)) {
                bytes = new byte[(int)stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
            }

            return bytes;
        }

        public static float LineToPointDistance(Vector2F A, Vector2F B, Vector2F C)
        {
            // Compute vectors AC and AB
            Vector2F AC = C - A;
            Vector2F AB = B - A;

            // Get point D by taking the projection of AC onto AB then adding the offset of A
            Vector2F D = proj(AC, AB) + A;

            Vector2F AD = D - A;
            // D might not be on AB so calculate k of D down AB (aka solve AD = k * AB)
            // We can use either component, but choose larger value to reduce the chance of dividing by zero
            float k = Math.Abs(AB.X) > Math.Abs(AB.Y) ? AD.X / AB.X : AD.Y / AB.Y;

            // Check if D is off either end of the line segment
            if (k <= 0.0) {
                return (float)Math.Sqrt(hypot2(C, A));
            }
            else if (k >= 1.0) {
                return (float)Math.Sqrt(hypot2(C, B));
            }

            return (float)Math.Sqrt(hypot2(C, D));
        }

        private static float hypot2(Vector2F a, Vector2F b) => (a - b).Dot(a - b);

        private static Vector2F proj(Vector2F a, Vector2F b)
        {
            float k = a.Dot(b) / b.Dot(b);
            return new Vector2F(k * b.X, k * b.Y);
        }

        public static Vector2I Project(Vector2I vi) => (Vector2I)Project((Vector2F)vi);

        public static Vector2F Project(Vector2F vf)
        {
            // 0 - 2
            vf.X /= (Game.WindowWidth / 2f);
            vf.Y /= (Game.WindowHeight / 2f);
            // -1 - 1
            vf -= 1f;
            // zoom
            vf *= Game.Zoom;
            // 0 - 2
            vf += 1f;
            vf.X *= (Game.WindowWidth / 2f);
            vf.Y *= (Game.WindowHeight / 2f);
            vf += Game.Pos;
            return vf;
        }

        public static Vector2I UnProject(Vector2I vi) => (Vector2I)UnProject((Vector2F)vi);

        public static Vector2F UnProject(Vector2F vf)
        {
            vf -= Game.Pos;
            // 0 - 2
            vf.X /= (Game.WindowWidth / 2f);
            vf.Y /= (Game.WindowHeight / 2f);
            // -1 - 1
            vf -= 1f;
            // zoom
            vf /= Game.Zoom;
            // 0 - 2
            vf += 1f;
            vf.X *= (Game.WindowWidth / 2f);
            vf.Y *= (Game.WindowHeight / 2f);
            return vf;
        }

        public static Vector2F Zoom(Vector2F vf)
        {
            // 0 - 2
            vf.X /= (Game.WindowWidth / 2f);
            vf.Y /= (Game.WindowHeight / 2f);
            // -1 - 1
            vf -= 1f;
            // zoom
            vf *= Game.Zoom;
            // 0 - 2
            vf += 1f;
            vf.X *= (Game.WindowWidth / 2f);
            vf.Y *= (Game.WindowHeight / 2f);
            return vf;
        }
    }
}
