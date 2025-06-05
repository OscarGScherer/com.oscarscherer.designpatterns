using UnityEngine;

namespace DesignPatterns
{
    public static class VectorExtensions
    {
        public static float Angle360(this Vector3 to, Vector3 from, Vector3 axis)
        {
            float signedAngle = Vector3.SignedAngle(from, to, axis);
            signedAngle = signedAngle < 0 ? signedAngle + 360f : signedAngle;
            return signedAngle;
        }
    }
}
