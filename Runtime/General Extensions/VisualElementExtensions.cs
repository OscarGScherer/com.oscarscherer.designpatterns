using UnityEngine.UIElements;

namespace DesignPatterns
{
    public static class VisualElementExtensions
    {
        public static T WithClasses<T>(this T self, params string[] classes) where T : VisualElement
        {
            foreach(var ussClass in classes) self.AddToClassList(ussClass);
            return self;
        }

        public static T WithParent<T>(this T self, VisualElement parent) where T : VisualElement
        {
            parent.Add(self);
            return self;
        }

        public static T WithFlex<T>(this T self, float flexGrow = -1, float flexShrink = -1) where T : VisualElement
        {
            if (flexGrow != -1) self.style.flexGrow = flexGrow;
            if (flexShrink != -1) self.style.flexShrink = flexShrink;
            return self;
        }
    }
}