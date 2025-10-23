#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace DesignPatterns
{
    [CustomEditor(typeof(ScriptableObject), true)]
    public class ScriptableObjectEditor : EditorAttributes
    {

    }
}
#endif
