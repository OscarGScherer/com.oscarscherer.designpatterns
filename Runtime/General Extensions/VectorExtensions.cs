using UnityEngine;

namespace DesignPatterns
{
    public static class VectorExtensions
    {
        public static float Angle360(Vector3 from, Vector3 to, Vector3 axis)
        {
            float signedAngle = Vector3.SignedAngle(from, to, axis);
            signedAngle = signedAngle < 0 ? signedAngle + 360f : signedAngle;
            return signedAngle;
        }

        // Swizzling
        // V2
        public static Vector2 Swizzle(this Vector4 self, int x, int y) => new Vector4(self[x], self[y]);
        public static Vector2 Swizzle(this Vector3 self, int x, int y) => new Vector2(self[x], self[y]);
        public static Vector2 Swizzle(this Vector2 self, int x, int y) => new Vector2(self[x], self[y]);
        // V3
        public static Vector3 Swizzle(this Vector4 self, int x, int y, int z) => new Vector3(self[x], self[y], self[z]);
        public static Vector3 Swizzle(this Vector3 self, int x, int y, int z) => new Vector3(self[x], self[y], self[z]);
        public static Vector3 Swizzle(this Vector2 self, int x, int y, int z) => new Vector3(self[x], self[y], self[z]);
        // V4
        public static Vector4 Swizzle(this Vector4 self, int x, int y, int z, int w) => new Vector4(self[x], self[y], self[z], self[w]);
        public static Vector4 Swizzle(this Vector3 self, int x, int y, int z, int w) => new Vector4(self[x], self[y], self[z], self[w]);
        public static Vector4 Swizzle(this Vector2 self, int x, int y, int z, int w) => new Vector4(self[x], self[y], self[z], self[w]);
    }
}
