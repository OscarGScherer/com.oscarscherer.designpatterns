using System;
using UnityEngine;

namespace DesignPatterns
{
    /// <summary>
    /// This class was made with chat gpt since it is pretty much all boiler plate, it might have errors.
    /// </summary>
    public static class TypeConverter
    {
        // Main generic method
        public static T ConvertObjectToType<T>(object obj)
        {
            if (obj == null) return default;

            Type targetType = typeof(T);

            // Handle direct cast if possible
            if (targetType.IsAssignableFrom(obj.GetType()))
                return (T)obj;

            return obj switch
            {
                Vector2 v2 => (T)ConvertVector2ToType<T>(v2),
                Vector3 v3 => (T)ConvertVector3ToType<T>(v3),
                string s => (T)ConvertStringToType<T>(s),
                bool b => (T)ConvertBoolToType<T>(b),
                byte bt => (T)ConvertNumericToType<T>(bt),
                short sh => (T)ConvertNumericToType<T>(sh),
                int i => (T)ConvertNumericToType<T>(i),
                long l => (T)ConvertNumericToType<T>(l),
                float f => (T)ConvertNumericToType<T>(f),
                double d => (T)ConvertNumericToType<T>(d),
                decimal dec => (T)ConvertNumericToType<T>(dec),
                _ => default,
            };
        }

        // Vector2 conversions
        public static object ConvertVector2ToType<T>(Vector2 v2)
        {
            Type t = typeof(T);
            if (t == typeof(float)) return v2.x;
            if (t == typeof(double)) return (double)v2.x;
            if (t == typeof(decimal)) return (decimal)v2.x;
            if (t == typeof(int)) return (int)v2.x;
            if (t == typeof(long)) return (long)v2.x;
            if (t == typeof(short)) return (short)v2.x;
            if (t == typeof(byte)) return (byte)v2.x;
            if (t == typeof(Vector3)) return new Vector3(v2.x, v2.y, 0);
            if (t == typeof(string)) return v2.ToString();
            if (t == typeof(bool)) return v2.x != 0 || v2.y != 0;

            return default;
        }

        // Vector3 conversions
        public static object ConvertVector3ToType<T>(Vector3 v3)
        {
            Type t = typeof(T);
            if (t == typeof(float)) return v3.x;
            if (t == typeof(double)) return (double)v3.x;
            if (t == typeof(decimal)) return (decimal)v3.x;
            if (t == typeof(int)) return (int)v3.x;
            if (t == typeof(long)) return (long)v3.x;
            if (t == typeof(short)) return (short)v3.x;
            if (t == typeof(byte)) return (byte)v3.x;
            if (t == typeof(Vector2)) return new Vector2(v3.x, v3.y);
            if (t == typeof(string)) return v3.ToString();
            if (t == typeof(bool)) return v3.x != 0 || v3.y != 0 || v3.z != 0;

            return default;
        }

        // String conversions
        public static object ConvertStringToType<T>(string s)
        {
            Type t = typeof(T);

            if (t == typeof(string)) return s;

            if (t == typeof(bool))
            {
                if (bool.TryParse(s, out var boolResult)) return boolResult;
                // Try numeric parse to bool
                if (double.TryParse(s, out var num)) return num != 0;
                return false;
            }

            if (t == typeof(Vector2))
            {
                // Parse something like "x,y"
                var parts = s.Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out var x) &&
                    float.TryParse(parts[1], out var y))
                {
                    return new Vector2(x, y);
                }
                return default(Vector2);
            }

            if (t == typeof(Vector3))
            {
                // Parse something like "x,y,Z"
                var parts = s.Split(',');
                if (parts.Length == 3 &&
                    float.TryParse(parts[0], out var x) &&
                    float.TryParse(parts[1], out var y) &&
                    float.TryParse(parts[2], out var z))
                {
                    return new Vector3(x, y, z);
                }
                return default(Vector3);
            }

            // Numeric types
            if (TryParseNumeric<T>(s, out var numResult))
                return numResult;

            return default;
        }

        // Bool conversions
        public static object ConvertBoolToType<T>(bool b)
        {
            Type t = typeof(T);

            if (t == typeof(bool)) return b;
            if (IsNumericType(t)) return b ? Convert.ChangeType(1, t) : Convert.ChangeType(0, t);
            if (t == typeof(string)) return b.ToString();
            if (t == typeof(Vector2)) return new Vector2(b ? 1 : 0, 0);
            if (t == typeof(Vector3)) return new Vector3(b ? 1 : 0, 0, 0);

            return default;
        }

        // Numeric conversions
        public static object ConvertNumericToType<T>(object numeric)
        {
            Type t = typeof(T);

            if (t == typeof(string)) return numeric.ToString();
            if (t == typeof(bool))
            {
                // Non-zero = true
                return Convert.ToSingle(numeric) != 0;
            }

            if (IsNumericType(t))
            {
                // Use Convert.ChangeType for numeric conversion
                return Convert.ChangeType(numeric, t);
            }

            if (t == typeof(Vector2))
            {
                float val = Convert.ToSingle(numeric);
                return new Vector2(val, 0);
            }

            if (t == typeof(Vector3))
            {
                float val = Convert.ToSingle(numeric);
                return new Vector3(val, 0, 0);
            }

            return default;
        }

        // Helper to detect numeric types
        private static bool IsNumericType(Type t)
        {
            return t == typeof(byte) ||
                t == typeof(short) ||
                t == typeof(int) ||
                t == typeof(long) ||
                t == typeof(float) ||
                t == typeof(double) ||
                t == typeof(decimal);
        }

        // Helper to parse numeric from string
        private static bool TryParseNumeric<T>(string s, out object result)
        {
            Type t = typeof(T);
            result = null;

            try
            {
                if (t == typeof(byte) && byte.TryParse(s, out var b))
                {
                    result = b; return true;
                }
                if (t == typeof(short) && short.TryParse(s, out var sh))
                {
                    result = sh; return true;
                }
                if (t == typeof(int) && int.TryParse(s, out var i))
                {
                    result = i; return true;
                }
                if (t == typeof(long) && long.TryParse(s, out var l))
                {
                    result = l; return true;
                }
                if (t == typeof(float) && float.TryParse(s, out var f))
                {
                    result = f; return true;
                }
                if (t == typeof(double) && double.TryParse(s, out var d))
                {
                    result = d; return true;
                }
                if (t == typeof(decimal) && decimal.TryParse(s, out var dec))
                {
                    result = dec; return true;
                }
            }
            catch
            {
                // Ignore parse exceptions
            }

            return false;
        }
    }
}