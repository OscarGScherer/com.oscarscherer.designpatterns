using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    public class CPUCharacter : MonoBehaviour
    {
        public SkinnedMeshRenderer smr;
        public Animator animator;
        public List<AnimationClip> animations;
        public List<float> test;
        public GPUAnimatorData gpuAnimatorData = new();
        public float t;

        public Transform[] bones;

        [Button("Test")]
        void Test()
        {
            bones = smr.bones;
        }

        [Button("Sample Animation")]
        void SampleAnimation()
        {
            animations[0].SampleAnimation(animator.gameObject, t);
        }
    }

    [Serializable]
    public class GPUAnimatorData
    {
        public int numBones;
        public int numAnimations;
        public int[] v2b; // rig
        public float[] frameTimes;
        public GPUAnimationMeta[] animationMetas;
        public GPUBoneTRS[] boneRTSs;
    }

    [Serializable]
    public struct GPUAnimationMeta
    {
        public int startFrame;
        public int numberOfFrames;
    }

    [Serializable]
    public struct GPUBoneTRS
    {
        public Vector4 position; // w = time of keyframe
        public Vector4 rotation; // Quaternion, uses xyzw
        public Vector4 scale;    // w = free

        public static int SizeInBytes() => sizeof(float) * 4 * 3;
    }
}