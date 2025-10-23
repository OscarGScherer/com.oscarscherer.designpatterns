using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    [ExecuteInEditMode]
    public class GPUCharacter : MonoBehaviour
    {
        #region Constants

        private const int Max_Characters = 512;
        private readonly int _OTWBufferID = Shader.PropertyToID("_ObjectToWorldBuff");
        private readonly int _NumCharactersID = Shader.PropertyToID("_NumCharacters");

        #endregion

        #region Public

        public Mesh mesh;
        public List<Transform> transforms;

        #endregion

        #region Private

        private Material renderMaterial;
        private RenderParams renderParams;

        private GraphicsBuffer _indirectDrawCommandBuff;
        private GraphicsBuffer.IndirectDrawIndexedArgs[] _indirectDrawCommandArray;

        private ComputeBuffer _ObjectToWorldBuff;
        private Matrix4x4[] _ObjectToWorldArray = new Matrix4x4[Max_Characters];

        #endregion

        void OnEnable()
        {
            // Releasing buffers just in case
            _ObjectToWorldBuff?.Release();
            _indirectDrawCommandBuff?.Release();

            // Initializing buffers
            _ObjectToWorldBuff = new ComputeBuffer(Max_Characters, sizeof(float) * 16, ComputeBufferType.Default);

            // Initializing material
            renderMaterial = renderMaterial.IfNull(() => new Material(Shader.Find("IndirectDraw/GPUGuy")));

            renderParams = new RenderParams(renderMaterial);
            renderParams.worldBounds = new Bounds(Vector3.zero, 1000 * Vector3.one);
            renderParams.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderParams.receiveShadows = true;

            renderParams.matProps = new MaterialPropertyBlock();
            renderParams.matProps.SetBuffer(_OTWBufferID, _ObjectToWorldBuff);

            _indirectDrawCommandBuff = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            _indirectDrawCommandArray = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        }

        void OnDisable()
        {
            _ObjectToWorldBuff?.Release();
            _indirectDrawCommandBuff?.Release();
        }

        void Update()
        {
            if (mesh == null || transforms == null) return;

            int numCharacters = transforms.Count;
            if (numCharacters <= 0) return;

            for (int i = 0; i < numCharacters; i++)
                _ObjectToWorldArray[i] = transforms[i] == null ? Matrix4x4.identity : transforms[i].localToWorldMatrix;

            _ObjectToWorldBuff.SetData(_ObjectToWorldArray);

            renderParams.matProps.SetInteger(_NumCharactersID, numCharacters);
            _indirectDrawCommandArray[0].indexCountPerInstance = mesh.GetIndexCount(0);
            _indirectDrawCommandArray[0].instanceCount = (uint)numCharacters;

            _indirectDrawCommandBuff.SetData(_indirectDrawCommandArray);
            Graphics.RenderMeshIndirect(renderParams, mesh, _indirectDrawCommandBuff);
        }
    }
}
