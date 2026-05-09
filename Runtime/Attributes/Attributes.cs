using System;
using System.Reflection;
using UnityEngine;

namespace DesignPatterns
{
    // [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    // public class DisplayInterfacesAttribute : PropertyAttribute { }

    // [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    // public class ColorAttribute : PropertyAttribute
    // {
    //     public readonly string color;
    //     public ColorAttribute(string color) => this.color = color;
    // }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public readonly bool fromTop;
        public readonly int editorOrder;
        public readonly string label;
        public ButtonAttribute(int editorOrder = 0, string label = null, bool fromTop = true)
        {
            this.fromTop = fromTop;
            this.editorOrder = editorOrder;
            this.label = label;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyAttribute : PropertyAttribute {}
}