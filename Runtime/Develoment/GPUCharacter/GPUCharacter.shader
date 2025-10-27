Shader "IndirectDraw/GPUCharacter"
{
    HLSLINCLUDE

    #include "UnityCG.cginc"
    #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
    #include "UnityIndirect.cginc"

    #pragma target 5.0

    #define MAX_BONES 32

    struct BoneRTS
    {
        float4 pos;     // w = 0, not used
        float4 rot;     // Quaternion
        float4 scale;   // w = 0, not used
    };

    uniform int _NumCharacters;
    uniform int _NumBones;
    StructuredBuffer<int> _V2B;
    StructuredBuffer<float4x4> _PoseMatrices;
    StructuredBuffer<float4x4> _ObjectToWorldBuff;
    StructuredBuffer<float4> _BoneBindPos;

    StructuredBuffer<BoneRTS> _PoseRTSs;

    ENDHLSL

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue"="Geometry" }
        ZTest LEqual
        ZWrite On

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 debug : COLOR0;
            };

            float3 RVQ(float3 v, float4 q)
            {
                float3 t = 2.0 * cross(q.xyz, v);
                return v + q.w * t + cross(q.xyz, t);
            }

            v2f vert(appdata_base v, uint svInstanceID : SV_InstanceID, uint svVertexID : SV_VertexID)
            {
                InitIndirectDrawArgs(0);
                uint iID = GetIndirectInstanceID(svInstanceID);
                v2f o;
                BoneRTS rts = _PoseRTSs[(iID % _NumCharacters) * _NumBones + _V2B[svVertexID]];
                float4 wpos = mul(
                    _ObjectToWorldBuff[iID % _NumCharacters], 
                    v.vertex - _BoneBindPos[svVertexID] + rts.pos
                );
                // float4 wpos = mul(_ObjectToWorldBuff[iID % _NumCharacters], v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                o.uv = v.texcoord;
                float4x4 test = _PoseMatrices[(iID % _NumCharacters) * _NumBones + _V2B[svVertexID]];
                o.debug = float4(_V2B[svVertexID] == 14, 0, 0, 1);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.debug;
                return float4(i.uv, 0, 1);
            }
            ENDHLSL
        }
        
        // SHADOW CASTER PASS
        // Pass
        // {
        //     Name "ShadowCaster"
        //     Tags {"LightMode"="ShadowCaster"}

        //     HLSLPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment frag
        //     #pragma multi_compile_shadowcaster

        //     struct v2f { 
        //         V2F_SHADOW_CASTER;
        //     };

        //     float4 UnityClipSpaceShadowCasterPos2(float4 wPos, float3 normal)
        //     {
        //         // if (unity_LightShadowBias.z != 0.0)
        //         // {
        //         //     float3 wNormal = UnityObjectToWorldNormal(normal);
        //         //     float3 wLight = normalize(UnityWorldSpaceLightDir(wPos.xyz));
        //         //     float shadowCos = dot(wNormal, wLight);
        //         //     float shadowSine = sqrt(1-shadowCos*shadowCos);
        //         //     float normalBias = unity_LightShadowBias.z * shadowSine;

        //         //     wPos.xyz -= wNormal * normalBias;
        //         // }
        //         return mul(UNITY_MATRIX_VP, wPos);
        //     }

        //     v2f vert(appdata_base v, uint instanceID : SV_InstanceID)
        //     {
        //         InitIndirectDrawArgs(0);
        //         uint cmdID = GetCommandID(0);
        //         uint iID = GetIndirectInstanceID(instanceID);
        //         float2 offset = _OffsetBuff[instanceID + cmdID * 8];

        //         float3 cwp = mul(_ObjectToWorldBuff[cmdID], float4(localPositions[instanceID].xyz, 1.0)).xyz * 1;
        //         float4 randOff = float4(Noise3(cwp)*0.2 - 0.1, 0);

        //         float4 wpos = mul(
        //             _ObjectToWorldBuff[cmdID], 
        //             (v.vertex + localPositions[instanceID] + ComputeRandomOffset(cmdID, instanceID) + float4(offset.x, 0, offset.y, 0))
        //         );
        //         // o.pos = mul(UNITY_MATRIX_VP, wpos);

        //         v2f o;
        //         o.pos = UnityClipSpaceShadowCasterPos2(wpos, v.normal);
        //         o.pos = UnityApplyLinearShadowBias(o.pos);
        //         return o;
        //     }

        //     float4 frag(v2f i) : SV_Target
        //     {
        //         SHADOW_CASTER_FRAGMENT(i)
        //     }
        //     ENDHLSL
        // }
    }
}