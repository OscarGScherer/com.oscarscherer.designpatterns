#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class AnimationClipUtility
{
    public class KeyframeData
    {
        public string path;             // e.g. "Armature/Hips/Spine"
        public string propertyName;     // e.g. "m_LocalPosition.x"
        public float normalizedTime;    // Time [0 - 1]
        public float value;             // Curve value
    }

    /// <summary>
    /// Extracts all keyframes from an AnimationClip into a flat list.
    /// </summary>
    public static List<KeyframeData> ExtractAllKeyframes(AnimationClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("AnimationClip is null!");
            return null;
        }

        var keyframeList = new List<KeyframeData>();

        // Get all the curve bindings in this clip
        var curveBindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var binding in curveBindings)
        {
            // Get the curve for each binding
            var curve = AnimationUtility.GetEditorCurve(clip, binding);

            foreach (Keyframe key in curve.keys)
            {
                keyframeList.Add(new KeyframeData
                {
                    path = binding.path,
                    propertyName = binding.propertyName,
                    normalizedTime = key.time / clip.length,
                    value = key.value
                });
            }
        }

        return keyframeList;
    }
}
#endif
