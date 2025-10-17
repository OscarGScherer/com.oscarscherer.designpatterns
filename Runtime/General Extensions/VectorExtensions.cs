using System;
using UnityEngine;

namespace DesignPatterns
{
    public static class VectorExtensions
    {
        #region Angles
        // ==================================================================
        // Anglles
        // ==================================================================
        public static float Angle360(Vector3 from, Vector3 to, Vector3 axis)
        {
            float signedAngle = Vector3.SignedAngle(from, to, axis);
            signedAngle = signedAngle < 0 ? signedAngle + 360f : signedAngle;
            return signedAngle;
        }
        #endregion

        #region Clamping
        // ==================================================================
        // CLAMPING
        // ==================================================================
        // INT Vectors
        public static Vector2Int Clamp(this Vector2Int self,
            int xMin = int.MinValue, int yMin = int.MinValue,
            int xMax = int.MaxValue, int yMax = int.MaxValue
        ) => new Vector2Int(Mathf.Clamp(self.x, xMin, xMax), Mathf.Clamp(self.y, yMin, yMax));
        public static Vector3Int Clamp(this Vector3Int self,
            int xMin = int.MinValue, int yMin = int.MinValue, int zMin = int.MinValue,
            int xMax = int.MaxValue, int yMax = int.MaxValue, int zMax = int.MaxValue
        ) => new Vector3Int(Mathf.Clamp(self.x, xMin, xMax), Mathf.Clamp(self.y, yMin, yMax), Mathf.Clamp(self.z, zMin, zMax));
        // FLOAT Vectors
        public static Vector2 Clamp(this Vector2 self,
            float xMin = float.MinValue, float yMin = float.MinValue,
            float xMax = float.MaxValue, float yMax = float.MaxValue
        ) => new Vector2(Mathf.Clamp(self.x, xMin, xMax), Mathf.Clamp(self.y, yMin, yMax));
        public static Vector3 Clamp(this Vector3 self,
            float xMin = float.MinValue, float yMin = float.MinValue, float zMin = float.MinValue,
            float xMax = float.MaxValue, float yMax = float.MaxValue, float zMax = float.MaxValue
        ) => new Vector3(Mathf.Clamp(self.x, xMin, xMax), Mathf.Clamp(self.y, yMin, yMax), Mathf.Clamp(self.z, zMin, zMax));
        public static Vector4 Clamp(this Vector4 self,
            float xMin = float.MinValue, float yMin = float.MinValue, float zMin = float.MinValue, float wMin = float.MinValue,
            float xMax = float.MaxValue, float yMax = float.MaxValue, float zMax = float.MaxValue, float wMax = float.MaxValue
        ) => new Vector4(Mathf.Clamp(self.x, xMin, xMax), Mathf.Clamp(self.y, yMin, yMax), Mathf.Clamp(self.z, zMin, zMax), Mathf.Clamp(self.w, wMin, wMax));
        #endregion

        #region Swizzling
        // ==================================================================
        // SWIZZLING
        // ==================================================================
        // V2
        public static Vector2 ToV2(this Vector4 v4, string xy) => new Vector2(v4.Get(xy[0]), v4.Get(xy[1]));
        public static Vector2 ToV2(this Vector3 v3, string xy) => new Vector2(v3.Get(xy[0]), v3.Get(xy[1]));
        public static Vector2 ToV2(this Vector2 vw, string xy) => new Vector2(vw.Get(xy[0]), vw.Get(xy[1]));
        // V3
        public static Vector3 ToV3(this Vector4 v4, string xyz) => new Vector3(v4.Get(xyz[0]), v4.Get(xyz[1]), v4.Get(xyz[2]));
        public static Vector3 ToV3(this Vector3 v3, string xyz) => new Vector3(v3.Get(xyz[0]), v3.Get(xyz[1]), v3.Get(xyz[2]));
        public static Vector3 ToV3(this Vector2 v2, string xyz) => new Vector3(v2.Get(xyz[0]), v2.Get(xyz[1]), v2.Get(xyz[2]));
        // V4
        public static Vector4 ToV4(this Vector4 v4, string xyzw) => new Vector4(v4.Get(xyzw[0]), v4.Get(xyzw[1]), v4.Get(xyzw[2]), v4.Get(xyzw[3]));
        public static Vector4 ToV4(this Vector3 v3, string xyzw) => new Vector4(v3.Get(xyzw[0]), v3.Get(xyzw[1]), v3.Get(xyzw[2]), v3.Get(xyzw[3]));
        public static Vector4 ToV4(this Vector2 v2, string xyzw) => new Vector4(v2.Get(xyzw[0]), v2.Get(xyzw[1]), v2.Get(xyzw[2]), v2.Get(xyzw[3]));
        // V2 INT
        public static Vector2Int ToV2Int(this Vector3Int v3, string xy) => new Vector2Int(v3.Get(xy[0]), v3.Get(xy[1]));
        public static Vector2Int ToV2Int(this Vector2Int v2, string xy) => new Vector2Int(v2.Get(xy[0]), v2.Get(xy[1]));
        // V3 INT
        public static Vector3Int ToV3Int(this Vector3Int v3, string xyz) => new Vector3Int(v3.Get(xyz[0]), v3.Get(xyz[1]), v3.Get(xyz[1]));
        public static Vector3Int ToV3Int(this Vector2Int v2, string xyz) => new Vector3Int(v2.Get(xyz[0]), v2.Get(xyz[1]), v2.Get(xyz[1]));
        // Helpers
        private static float Get(this Vector2 v, char c)
        {
            switch (c)
            {
                case 'x': case 'r': return v[0];
                case 'y': case 'g': return v[1];
                default: return c - '0';
            }
        }
        private static float Get(this Vector3 v, char c)
        {
            switch (c)
            {
                case 'x': case 'r': return v[0];
                case 'y': case 'g': return v[1];
                case 'z': case 'b': return v[2];
                default: return c - '0';
            }
        }
        private static float Get(this Vector4 v, char c)
        {
            switch (c)
            {
                case 'x': case 'r': return v[0];
                case 'y': case 'g': return v[1];
                case 'z': case 'b': return v[2];
                case 'w': case 'a': return v[3];
                default: return c - '0';
            }
        }
        private static int Get(this Vector2Int v, char c)
        {
            switch (c)
            {
                case 'x': case 'r': return v[0];
                case 'y': case 'g': return v[1];
                default: return c - '0';
            }
        }
        private static int Get(this Vector3Int v, char c)
        {
            switch (c)
            {
                case 'x': case 'r': return v[0];
                case 'y': case 'g': return v[1];
                case 'z': case 'b': return v[2];
                default: return c - '0';
            }
        }
        #endregion
    }
}
