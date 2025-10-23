using System;
using UnityEngine;

namespace DesignPatterns
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DisplayInterfacesAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ColorAttribute : PropertyAttribute
    {
        public readonly string color;
        public ColorAttribute(string color) => this.color = color;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : PropertyAttribute
    {
        public readonly string Label;
        public ButtonAttribute(string label = null) => Label = label;
    }
}