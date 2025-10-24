using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns
{

    [CreateAssetMenu(menuName = "GPUCharacter")]
    public class GPUCharacter : ScriptableObject
    {
        [Header("CPU Data")]
        public CPUCharacter cpuCharacter;
        public List<AnimationClip> animations;

        [Header("Intermediate Data")]
        public List<string> bonePaths;

        [Header("GPU Data")]
        public uint numBones;
        public uint[] vertexToBones;


        [Button("Test")]
        void Convert()
        {
            if (cpuCharacter == null) return;

            numBones = (uint)cpuCharacter.smr.bones.Length;
            vertexToBones = cpuCharacter.smr.sharedMesh.boneWeights.Select(bw => (uint)bw.boneIndex0).ToArray();
            bonePaths = GetBonePaths(cpuCharacter.smr);
            // EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animationClip);
            // object value = bindings.GetValue(0);
            // AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, bindings[0]);
            // curve.Evaluate(0);
            // Debug.Log("Test");
        }

        /// <summary>
        /// Returns a list of bone transform paths (relative to the root bone or model root)
        /// for a given SkinnedMeshRenderer.
        /// </summary>
        public static List<string> GetBonePaths(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("SkinnedMeshRenderer is null!");
                return null;
            }

            var bonePaths = new List<string>();
            Transform[] bones = skinnedMeshRenderer.bones;

            // Use the root bone as the path root if available,
            // otherwise use the renderer's transform
            Transform root = skinnedMeshRenderer.rootBone != null
                ? skinnedMeshRenderer.rootBone
                : skinnedMeshRenderer.transform.root;

            foreach (Transform bone in bones)
            {
                string path = GetTransformPath(bone, root);
                bonePaths.Add(path);
            }

            return bonePaths;
        }

        /// <summary>
        /// Recursively constructs a path from the root to the target transform.
        /// Example result: "Armature/Hips/Spine/Chest/RightShoulder"
        /// </summary>
        private static string GetTransformPath(Transform target, Transform root)
        {
            if (target == null)
                return string.Empty;
            string path = target.name;
            Transform current = target.parent;
            while (current != null && current != root)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            return path;
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

    public struct GPUAnimationRig
    {
        public BoneWeight[] boneWeights;
    }

    public struct GPUCharacterAnimationData
    {
        public GPUAnimationRig rig;
        public GPUAnimation[] animations;
    }

    public struct GPUAnimation
    {
        public GPUAnimationKeyframe[] frames;
    }

    public struct GPUAnimationKeyframe
    {
        public float duration;
        public GPUBoneTransform[] bones;
    }

    public struct GPUBoneTransform
    {
        public Vector4 position;
        public Vector4 rotation;
        public Vector4 scale;
    }
}
