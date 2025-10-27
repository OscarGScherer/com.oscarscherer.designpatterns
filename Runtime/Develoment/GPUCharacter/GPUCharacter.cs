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
    }
}
