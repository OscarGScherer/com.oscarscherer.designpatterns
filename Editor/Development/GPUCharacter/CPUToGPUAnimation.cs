using UnityEngine;

namespace DesignPatterns
{
    public class CPUToGPUAnimation : ScriptableObject
    {
        public SkinnedMeshRenderer smr;
        public Transform[] bones;
        public BoneWeight[] boneWeights;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
