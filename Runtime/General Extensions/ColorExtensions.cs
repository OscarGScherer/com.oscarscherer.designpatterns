using UnityEngine;

public static class ColorExtensions
{
    public static Color Hex(this Color self, string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out self);
        return self;
    }
}