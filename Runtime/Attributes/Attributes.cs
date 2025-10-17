using UnityEngine;

namespace DesignPatterns
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DisplayInterfacesAttribute : PropertyAttribute { }

    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ColorAttribute : PropertyAttribute
    {
        public readonly string color;
        public ColorAttribute(string color) => this.color = color;
    }
}