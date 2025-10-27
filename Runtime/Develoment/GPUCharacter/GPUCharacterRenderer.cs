using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    [ExecuteInEditMode]
    public class GPUCharacterRenderer : MonoBehaviour
    {
        #region Constants
        private static ComputeShader _GPUAnimatorCS;
        private int _GPUAnimCSKernel;

        private const int Max_Characters = 1;
        private readonly int _NumCharactersID = Shader.PropertyToID("_NumCharacters");
        private readonly int _OTWBufferID = Shader.PropertyToID("_ObjectToWorldBuff");
        
        // Static animation data
        private readonly int _NumBonesID = Shader.PropertyToID("_NumBones");
        private readonly int _V2BID = Shader.PropertyToID("_V2B");  // Rig
        private readonly int _AMetasID = Shader.PropertyToID("_AMetas");
        private readonly int _FrameTimesID = Shader.PropertyToID("_FrameTimes");
        private readonly int _BoneRTSsID = Shader.PropertyToID("_BoneRTSs"); // Actual animations

        // Variable animation data
        private readonly int _C2AID = Shader.PropertyToID("_C2A");  // Which animation each character is playing
        private readonly int _C2TID = Shader.PropertyToID("_C2T");  // At what point of the animation each character is in
        private readonly int _PoseMatricesID = Shader.PropertyToID("_PoseMatrices"); // Managed entirely by the GPU
        private readonly int _BoneBindPosID = Shader.PropertyToID("_BoneBindPos");
        private readonly int _PoseRTSsID = Shader.PropertyToID("_PoseRTSs");

        #endregion

        #region Public

        [Range(0,1)] public float animTime;
        public CPUCharacter cpuCharacter;
        public List<Transform> transforms;

        #endregion

        #region Private

        private ComputeShader _gpuAnimatorCSInstance;
        private Material _renderMaterial;
        private RenderParams _renderParams;

        // Buffers
        private GraphicsBuffer _indirectDrawCommandBuff;
        private GraphicsBuffer.IndirectDrawIndexedArgs[] _indirectDrawCommandArray;
        private ComputeBuffer _ObjectToWorldBuff;
        // Static animation buffers
        private ComputeBuffer _AMetasBuff, _FrameTimesBuff, _BoneRTSsBuff, _V2BBuff, _BoneBindPosBuff, _PoseRTSsBuff;
        // Variable buffers
        private ComputeBuffer _C2ABuff, _C2TBuff;
        private ComputeBuffer _PoseMatricesBuff;

        private Matrix4x4[] _ObjectToWorldArray;

        #endregion

        // Debug
        public Matrix4x4[] test;
        public float[] debug;
        private ComputeBuffer _Debug;

        private void ReleaseBuffers()
        {
            _ObjectToWorldBuff?.Release();
            _indirectDrawCommandBuff?.Release();
            _AMetasBuff?.Release();
            _FrameTimesBuff?.Release();
            _BoneRTSsBuff?.Release();
            _V2BBuff?.Release();
            _C2ABuff?.Release();
            _C2TBuff?.Release();
            _PoseMatricesBuff?.Release();
            _Debug?.Release();
            _BoneBindPosBuff?.Release();
            _PoseRTSsBuff?.Release();
            _gpuAnimatorCSInstance = null;
        }

        void OnEnable()
        {
            // Releasing buffers just in case
            ReleaseBuffers();

            if (cpuCharacter?.gpuAnimatorData == null) return;

            _ObjectToWorldArray = new Matrix4x4[Max_Characters];

            if (_GPUAnimatorCS == null)
            {
                _GPUAnimatorCS = Resources.Load<ComputeShader>("GPUAnimator");
            }

            _gpuAnimatorCSInstance = _gpuAnimatorCSInstance == null ? Instantiate(_GPUAnimatorCS) : _gpuAnimatorCSInstance;
            _GPUAnimCSKernel = _gpuAnimatorCSInstance.FindKernel("EvaluatePoses");

            // Initializing buffers
            _ObjectToWorldBuff = new ComputeBuffer(Max_Characters, sizeof(float) * 16, ComputeBufferType.Default);
            // Animation buffers
            GPUAnimatorData animData = cpuCharacter.gpuAnimatorData;
            // Static buffers
            _AMetasBuff = new ComputeBuffer(animData.numAnimations, sizeof(int) * 2, ComputeBufferType.Default);
            _FrameTimesBuff = new ComputeBuffer(animData.frameTimes.Length, sizeof(float), ComputeBufferType.Default);
            _BoneRTSsBuff = new ComputeBuffer(animData.boneRTSs.Length, GPUBoneTRS.SizeInBytes(), ComputeBufferType.Default);
            _V2BBuff = new ComputeBuffer(animData.v2b.Length, sizeof(int), ComputeBufferType.Default);
            _BoneBindPosBuff = new ComputeBuffer(animData.tPoseBonePositions.Length, sizeof(float) * 4, ComputeBufferType.Default);

            // Variable buffers
            _C2ABuff = new ComputeBuffer(Max_Characters, sizeof(int), ComputeBufferType.Default);
            _C2TBuff = new ComputeBuffer(Max_Characters, sizeof(float), ComputeBufferType.Default);
            _PoseMatricesBuff = new ComputeBuffer(Max_Characters * animData.numBones, sizeof(float) * 16, ComputeBufferType.Default);
            _PoseRTSsBuff = new ComputeBuffer(Max_Characters * animData.numBones, sizeof(float) * 12, ComputeBufferType.Default);

            // Initializing compute shader
            _gpuAnimatorCSInstance.SetInt(_NumBonesID, animData.numBones);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, _AMetasID, _AMetasBuff);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, _FrameTimesID, _FrameTimesBuff);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, _BoneRTSsID, _BoneRTSsBuff);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, _C2AID, _C2ABuff);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, _C2TID, _C2TBuff);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, _PoseMatricesID, _PoseMatricesBuff);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, _PoseRTSsID, _PoseRTSsBuff);

            // Initializing material
            _renderMaterial = _renderMaterial.IfNull(() => new Material(Shader.Find("IndirectDraw/GPUCharacter")));

            _renderParams = new RenderParams(_renderMaterial);
            _renderParams.worldBounds = new Bounds(Vector3.zero, 1000 * Vector3.one);
            _renderParams.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _renderParams.receiveShadows = false;

            _renderParams.matProps = new MaterialPropertyBlock();
            _renderParams.matProps.SetInt(_NumBonesID, animData.numBones);
            _renderParams.material.SetBuffer(_V2BID, _V2BBuff);
            _renderParams.material.SetBuffer(_PoseMatricesID, _PoseMatricesBuff);
            _renderParams.matProps.SetBuffer(_OTWBufferID, _ObjectToWorldBuff);
            _renderParams.matProps.SetBuffer(_BoneBindPosID, _BoneBindPosBuff);
            _renderParams.matProps.SetBuffer(_PoseRTSsID, _PoseRTSsBuff);

            // Initializing buffers
            _AMetasBuff.SetData(animData.animationMetas);
            _FrameTimesBuff.SetData(animData.frameTimes);
            _BoneRTSsBuff.SetData(animData.boneRTSs);
            _V2BBuff.SetData(animData.v2b);
            Matrix4x4[] posesInit = Matrix4x4.identity.RepeatForArray(Max_Characters * animData.numBones);
            _PoseMatricesBuff.SetData(posesInit);
            _BoneBindPosBuff.SetData(animData.tPoseBonePositions);
            _PoseRTSsBuff.SetData(new GPUBoneTRS().RepeatForArray(Max_Characters * animData.numBones));

            _indirectDrawCommandBuff = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            _indirectDrawCommandArray = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

            test = new Matrix4x4[Max_Characters * animData.numBones];
            debug = new float[Max_Characters * animData.numBones];
            _Debug = new ComputeBuffer(Max_Characters * animData.numBones, sizeof(float), ComputeBufferType.Default);
            _gpuAnimatorCSInstance.SetBuffer(_GPUAnimCSKernel, Shader.PropertyToID("_Debug"), _Debug);
        }

        void OnDisable()
        {
            ReleaseBuffers();
        }

        void Update()
        {
            if (cpuCharacter == null || transforms == null || _gpuAnimatorCSInstance == null) return;

            int numCharacters = transforms.Count;
            if (numCharacters <= 0) return;

            for (int i = 0; i < numCharacters && i < Max_Characters; i++)
                _ObjectToWorldArray[i] = transforms[i] == null ? Matrix4x4.identity : transforms[i].localToWorldMatrix;

            // Below is WIP

            // Update variable buffers
            _ObjectToWorldBuff.SetData(_ObjectToWorldArray);
            _C2ABuff.SetData(0.RepeatForArray(Max_Characters));
            _C2TBuff.SetData(animTime.RepeatForArray(Max_Characters));
            _gpuAnimatorCSInstance.SetInt(_NumCharactersID, numCharacters);
            _gpuAnimatorCSInstance.Dispatch(_GPUAnimCSKernel, numCharacters, 1, 1);

            _PoseMatricesBuff.GetData(test);
            _Debug.GetData(debug);

            _renderParams.matProps.SetInteger(_NumCharactersID, numCharacters);

            Mesh mesh = cpuCharacter.smr.sharedMesh;

            _indirectDrawCommandArray[0].indexCountPerInstance = mesh.GetIndexCount(0);
            _indirectDrawCommandArray[0].instanceCount = (uint)numCharacters;

            _indirectDrawCommandBuff.SetData(_indirectDrawCommandArray);
            Graphics.RenderMeshIndirect(_renderParams, mesh, _indirectDrawCommandBuff);
        }
    }
}
