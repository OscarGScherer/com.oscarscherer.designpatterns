#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DesignPatterns;
using System.Linq;

[CustomEditor(typeof(CPUCharacter), true)]
public class CPUCharacterEditor : EditorAttributes
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("CPU to GPU animations"))
        {
            CPUCharacter cpuCharacter = (CPUCharacter)target;
            GPUAnimatorData animData = cpuCharacter.gpuAnimatorData;

            // Translate rig
            animData.numBones = cpuCharacter.smr.bones.Length;
            animData.v2b = cpuCharacter.smr.sharedMesh.boneWeights.Select(bw => bw.boneIndex0).ToArray();

            // Translate animations
            cpuCharacter.animations = cpuCharacter.animations.Where(a => a != null).ToList();
            animData.numAnimations = cpuCharacter.animations.Count;

            // Calculating tPose postions for every bone in object space
            List<GPUBoneTRS> tPose = new List<GPUBoneTRS>();
            foreach (Transform bone in cpuCharacter.smr.bones) tPose.Add(GetRelativeTRS(bone, cpuCharacter.animator.transform));

            Transform[] bones = cpuCharacter.smr.bones;
            animData.tPoseBonePositions = bones.Select(b => (Vector4) cpuCharacter.animator.transform.InverseTransformPoint(b.position)).ToArray();

            List<GPUAnimationMeta> animMetas = new();
            List<GPUBoneTRS> boneTRSs = new();
            List<float> frameTimes = new();
            Matrix4x4 worldToObject = cpuCharacter.animator.transform.worldToLocalMatrix;
            int frameCount = 0;
            foreach (AnimationClip clip in cpuCharacter.animations)
            {
                // Gets all times where there is a user defined keyframe in the clip
                List<float> kfTimes = GetAllKeyframeTimes(clip);
                animMetas.Add(new GPUAnimationMeta() { startFrame = frameCount, numberOfFrames = kfTimes.Count, });
                frameCount += kfTimes.Count;
                foreach (float time in kfTimes)
                {
                    frameTimes.Add(time / clip.length);
                    clip.SampleAnimation(cpuCharacter.animator.gameObject, time);
                    for (int bInd = 0; bInd < animData.numBones; bInd++)
                    {
                        GPUBoneTRS boneTRS = GetRelativeTRS(bones[bInd], cpuCharacter.animator.transform);
                        boneTRSs.Add(new GPUBoneTRS()
                        {
                            position = boneTRS.position,
                            rotation = QToV4(V4ToQ(boneTRS.rotation) * Quaternion.Inverse(V4ToQ(tPose[bInd].rotation))),
                            // rotation = QToV4(bones[bInd].localRotation),
                            scale = new Vector4(
                                boneTRS.scale.x / tPose[bInd].scale.x,
                                boneTRS.scale.y / tPose[bInd].scale.y,
                                boneTRS.scale.z / tPose[bInd].scale.z,
                                0
                            )
                        });
                    }
                }
            }
            animData.frameTimes = frameTimes.ToArray();
            animData.animationMetas = animMetas.ToArray();
            animData.boneRTSs = boneTRSs.ToArray();
        }
    }

    GPUBoneTRS GetRelativeTRS(Transform target, Transform root)
    {
        return new GPUBoneTRS
        {
            position = root.InverseTransformPoint(target.position),
            rotation = QToV4(Quaternion.Inverse(root.rotation) * target.rotation),
            scale = GetRelativeScale(target, root)
        };
    }

    Vector4 QToV4(Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);
    Quaternion V4ToQ(Vector4 v4) => new Quaternion(v4.x, v4.y, v4.z, v4.w);

    Vector4 GetRelativeScale(Transform target, Transform root)
    {
        Vector3 worldScaleTarget = target.lossyScale;
        Vector3 worldScaleRoot = root.lossyScale;
        return new Vector4(
            worldScaleTarget.x / worldScaleRoot.x,
            worldScaleTarget.y / worldScaleRoot.y,
            worldScaleTarget.z / worldScaleRoot.z,
            0
        );
    }
    
    private static void Decompose(Matrix4x4 matrix, out Vector3 pos, out Quaternion rot, out Vector3 scale)
    {
        pos = matrix.GetColumn(3);

        // Extract scale
        scale = new Vector3(
            matrix.GetColumn(0).magnitude,
            matrix.GetColumn(1).magnitude,
            matrix.GetColumn(2).magnitude
        );

        // Normalize the rotation axes
        Vector3 up = matrix.GetColumn(1) / scale.y;
        Vector3 forward = matrix.GetColumn(2) / scale.z;

        rot = Quaternion.LookRotation(forward, up);
    }

    /// <summary>
    /// Returns a sorted list of all unique keyframe times (in seconds)
    /// found in the given AnimationClip.
    /// </summary>
    private static List<float> GetAllKeyframeTimes(AnimationClip clip)
    {
        var times = new HashSet<float>();

        if (clip == null)
        {
            Debug.LogError("GetAllKeyframeTimes: clip is null");
            return new List<float>();
        }

        // Get all curve bindings in the clip (position, rotation, etc.)
        var curveBindings = AnimationUtility.GetCurveBindings(clip);
        foreach (var binding in curveBindings)
        {
            // Retrieve the actual animation curve
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve == null) continue;

            foreach (var key in curve.keys)
                times.Add(key.time);
        }

        // Convert to list and sort in ascending order
        var sortedTimes = new List<float>(times);
        sortedTimes.Sort();
        return sortedTimes;
    }
}
#endif
