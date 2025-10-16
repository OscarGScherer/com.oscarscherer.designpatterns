using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DesignPatterns
{
    [Serializable] public class ObservedCameraEvent : ObservedField<CameraEvent>
    {
        public ObservedCameraEvent() { }
        public ObservedCameraEvent(CameraEvent initialValue) : base(initialValue) { }
        public static implicit operator CameraEvent(ObservedCameraEvent of) => of.value;
    }
    [Serializable] public class ObservedVector2Int : ObservedField<Vector2Int>
    {
        public ObservedVector2Int() { }
        public ObservedVector2Int(Vector2Int initialValue) : base(initialValue) { }
        public static implicit operator Vector2Int(ObservedVector2Int of) => of.value;
    }
    [Serializable] public class ObservedInt : ObservedField<int>
    {
        public ObservedInt() { }
        public ObservedInt(int initialValue) : base(initialValue) { }
        public static implicit operator int(ObservedInt of) => of.value;
    }
    [Serializable] public class ObservedFloat : ObservedField<float>
    {
        public ObservedFloat() { }
        public ObservedFloat(float initialValue) : base(initialValue) { }
        public static implicit operator float(ObservedFloat of) => of.value;
    }
    [Serializable] public class ObservedBool : ObservedField<bool>
    {
        public ObservedBool() { }
        public ObservedBool(bool initialValue) : base(initialValue) { }
        public static implicit operator bool(ObservedBool of) => of.value;
    }
    [Serializable] public class ObservedMaterial : ObservedField<Material>
    {
        public ObservedMaterial() { }
        public ObservedMaterial(Material initialValue) : base(initialValue) { }
        public static implicit operator Material(ObservedMaterial of) => of.value;
    }
    [Serializable] public class ObservedFilterMode : ObservedField<FilterMode>
    {
        public ObservedFilterMode() { }
        public ObservedFilterMode(FilterMode initialValue) : base(initialValue) { }
        public static implicit operator FilterMode(ObservedFilterMode of) => of.value;
    }
}