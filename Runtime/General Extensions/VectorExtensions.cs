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
        // V2 INT
        public static Vector2Int Swizzle(this Vector3Int self, int x, int y) => new Vector2Int(self[x], self[y]);
        public static Vector2Int Swizzle(this Vector2Int self, int x, int y) => new Vector2Int(self[x], self[y]);
        // V3 INT
        public static Vector3Int Swizzle(this Vector3Int self, int x, int y, int z) => new Vector3Int(self[x], self[y], self[z]);
        public static Vector3Int Swizzle(this Vector2Int self, int x, int y, int z) => new Vector3Int(self[x], self[y], self[z]);
        #endregion
    }
}
