// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering;

// namespace Oniria.Portuario
// {
//     [ExecuteInEditMode]
//     public class BoxRenderer : MonoBehaviour
//     {
//         private const int Max_Boxes = 512;
//         private readonly int _DisplacerWorldToLocal = Shader.PropertyToID("_DisplacerWorldToLocal");
//         private readonly int _DisplacerLocalToWorld = Shader.PropertyToID("_DisplacerLocalToWorld");
//         private readonly int _DisplacerCutoffNormal = Shader.PropertyToID("_DisplacerCutoffNormal");
//         private readonly int _DeltaTime = Shader.PropertyToID("_DeltaTime");
//         private readonly int _NumBoxes = Shader.PropertyToID("_NumBoxes");
//         private readonly int _ObjectToWorldBuff = Shader.PropertyToID("_ObjectToWorldBuff");
//         private readonly int _SimulationScalesBuff = Shader.PropertyToID("_SimulationScalesBuff");
//         private readonly int _OffsetBuff = Shader.PropertyToID("_OffsetBuff");
//         private readonly int _NumBoxesInDisplacerBuff = Shader.PropertyToID("_NumBoxesInDisplacerBuff");
//         private readonly int _MaxDisplacement = Shader.PropertyToID("_MaxDisplacement");
//         private readonly int _SlideForce = Shader.PropertyToID("_SlideForce");
//         private readonly int _DisplacerForce = Shader.PropertyToID("_DisplacerForce");
//         private readonly int _ReboundForce = Shader.PropertyToID("_ReboundForce");

//         // Box Sim Configuration
//         [Header("Box Sim Configuration")]
//         public Vector2 maxDisplacement = new Vector2(0.5f, 0.5f);
//         public Vector2 displacerForce = new Vector2(1f, 1f);
//         public Vector2 slideForce = new Vector2(1f, 1f);
//         public Vector2 reboundForce = new Vector2(1f, 1f);

//         // Indirect rendering
//         public Material material;
//         public Mesh mesh;
//         private GraphicsBuffer _indirectDrawCommandBuff;
//         private GraphicsBuffer.IndirectDrawIndexedArgs[] _indirectDrawCommandData;
//         private RenderParams rp;

//         // Compute shader
//         public ComputeShader computeShader;
//         int _computeShaderKernel;

//         // GPU write
//         private ComputeBuffer _offsetBuffer;
//         private readonly uint[] ZERO_COUNT = new uint[1]{ 0 };
//         private ComputeBuffer _numBoxesInsideDisplacerBuffer;

//         // CPU write
//         private ComputeBuffer _objToWorldBuffer;
//         private Matrix4x4[] _objToWorlds = new Matrix4x4[Max_Boxes];
//         private ComputeBuffer _simulationScalesBuffer;
//         private float[] _simulationScales = new float[Max_Boxes];

//         // CPU write sources
//         private static List<CeluloseBox> celuloseBoxes = new List<CeluloseBox>(Max_Boxes);
//         private static CeluloseBoxDisplacer displacer;
//         public static uint numOfBoxesInDisplacer = 0;

//         public static void SetDisplacer(CeluloseBoxDisplacer celuloseBoxDisplacer)
//         {
//             if (displacer != null)
//             {
//                 Debug.LogWarning("You are adding a second celulose box displacer, for now there can only be one.");
//                 return;
//             }
//             displacer = celuloseBoxDisplacer;
//         }

//         public static void RemoveDisplacer(CeluloseBoxDisplacer celuloseBoxDisplacer)
//         {
//             if (displacer != celuloseBoxDisplacer) return;
//             displacer = null;
//         }

//         public static void AddCeluloseBox(CeluloseBox celuloseBox)
//         {
//             if (celuloseBoxes.Count >= Max_Boxes) return;
//             celuloseBoxes.Add(celuloseBox);
//         }

//         public static void RemoveCeluloseBox(CeluloseBox celuloseBox)
//         {
//             celuloseBoxes.Remove(celuloseBox);
//         }

//         void OnEnable()
//         {
//             // Just in case
//             _indirectDrawCommandBuff?.Release();
//             _objToWorldBuffer?.Release();
//             _offsetBuffer?.Release();

//             // Strutured buffers
//             _objToWorldBuffer = new ComputeBuffer(Max_Boxes, 64, ComputeBufferType.Default);
//             _simulationScalesBuffer = new ComputeBuffer(Max_Boxes, sizeof(float), ComputeBufferType.Default);
//             _offsetBuffer = new ComputeBuffer(Max_Boxes * 8, sizeof(float) * 2, ComputeBufferType.Default);
//             Vector2[] offsets = new Vector2[Max_Boxes * 8]; // Initializing offsets with float2(0,0)
//             _offsetBuffer.SetData(offsets);
//             _numBoxesInsideDisplacerBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.Default);

//             // Compute shader
//             _computeShaderKernel = computeShader.FindKernel("UpdateBoxGroup");
//             computeShader.SetBuffer(_computeShaderKernel, _ObjectToWorldBuff, _objToWorldBuffer);
//             computeShader.SetBuffer(_computeShaderKernel, _SimulationScalesBuff, _simulationScalesBuffer);
//             computeShader.SetBuffer(_computeShaderKernel, _OffsetBuff, _offsetBuffer);
//             computeShader.SetBuffer(_computeShaderKernel, _NumBoxesInDisplacerBuff, _numBoxesInsideDisplacerBuffer);

//             // Render shader
//             rp = new RenderParams(material);
//             rp.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one); // use tighter bounds for better FOV culling
//             rp.matProps = new MaterialPropertyBlock();
//             rp.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//             rp.receiveShadows = true;
//             rp.matProps.SetBuffer(_ObjectToWorldBuff, _objToWorldBuffer);
//             rp.matProps.SetBuffer(_OffsetBuff, _offsetBuffer);
//             rp.matProps.SetBuffer(_SimulationScalesBuff, _simulationScalesBuffer);

//             _indirectDrawCommandBuff = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, Max_Boxes, GraphicsBuffer.IndirectDrawIndexedArgs.size);
//             _indirectDrawCommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[Max_Boxes];
//         }

//         void OnDisable()
//         {
//             _indirectDrawCommandBuff?.Release();
//             _objToWorldBuffer?.Release();
//             _offsetBuffer?.Release();
//             _indirectDrawCommandBuff = null;
//             _objToWorldBuffer = null;
//             _offsetBuffer = null;
//         }

//         void Update()
//         {
//             if (celuloseBoxes.Count <= 0) return;

//             for (int i = 0; i < celuloseBoxes.Count; i++)
//             {
//                 _objToWorlds[i] = celuloseBoxes[i].transform.localToWorldMatrix;
//                 _simulationScales[i] = celuloseBoxes[i].simulationScale;
//             }

//             if (displacer != null)
//             {
//                 computeShader.SetMatrix(_DisplacerLocalToWorld, displacer.transform.localToWorldMatrix);
//                 computeShader.SetMatrix(_DisplacerWorldToLocal, displacer.transform.worldToLocalMatrix);
//                 computeShader.SetVector(_DisplacerCutoffNormal, displacer.cutoff);
//             }

//             _numBoxesInsideDisplacerBuffer.SetData(ZERO_COUNT);
//             _objToWorldBuffer.SetData(_objToWorlds);
//             _simulationScalesBuffer.SetData(_simulationScales);

//             computeShader.SetVector(_MaxDisplacement, maxDisplacement);
//             computeShader.SetVector(_SlideForce, slideForce);
//             computeShader.SetVector(_DisplacerForce, displacerForce);
//             computeShader.SetVector(_ReboundForce, reboundForce);
//             computeShader.SetFloat(_DeltaTime, Time.deltaTime);
//             computeShader.Dispatch(_computeShaderKernel, celuloseBoxes.Count, 1, 1);

//             AsyncGPUReadback.Request(_numBoxesInsideDisplacerBuffer, (request) =>
//             {
//                 numOfBoxesInDisplacer = request.hasError ? 0 : request.GetData<uint>()[0];
//             });

//             rp.matProps.SetFloat(_NumBoxes, celuloseBoxes.Count);
//             for (int i = 0; i < celuloseBoxes.Count; i++)
//             {
//                 _indirectDrawCommandData[i].indexCountPerInstance = mesh.GetIndexCount(0);
//                 _indirectDrawCommandData[i].instanceCount = 8;
//             }

//             _indirectDrawCommandBuff.SetData(_indirectDrawCommandData);
//             Graphics.RenderMeshIndirect(rp, mesh, _indirectDrawCommandBuff, celuloseBoxes.Count);
//         }
//     }
// }