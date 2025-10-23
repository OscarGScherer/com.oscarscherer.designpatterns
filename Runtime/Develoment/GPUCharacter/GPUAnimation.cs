using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns
{

    [CreateAssetMenu(menuName = "GPUAnimationObject")]
    public class GPUAnimationObject : ScriptableObject
    {
        public AnimationClip animationClip;
        public Transform rig;
        public List<string> bones;

        [Button("Test")]
        void Convert()
        {
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animationClip);
            object value = bindings.GetValue(0);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, bindings[0]);
            curve.Evaluate(0);
            Debug.Log("Test");
        }

        [Button("GetBonesFromRig")]
        void GetBonesFromRig()
        {
            bones = GetAllBonePaths(rig);
        }

        public static List<string> GetAllBonePaths(Transform root)
        {
            var paths = new List<string>();
            CollectPathsRecursive(root, "", paths);
            return paths;
        }

        private static void CollectPathsRecursive(Transform current, string currentPath, List<string> paths)
        {
            string newPath = string.IsNullOrEmpty(currentPath) ? current.name : $"{currentPath}/{current.name}";
            paths.Add(newPath);
            foreach (Transform child in current) CollectPathsRecursive(child, newPath, paths);
        }
    }

    struct GPUAnimation
    {
        public GPUAnimationKeyframe[] frames;
    }

    struct GPUAnimationKeyframe
    {
        public Matrix4x4[] bones;
        public GPUAnimationKeyframe(int test) => bones = new Matrix4x4[test];
    }
}
