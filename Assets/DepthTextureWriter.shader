Shader "Hidden/DepthTextureWriter"
{
    SubShader
    {
        Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Opaque" }

        Pass
        {
            Name "DepthTextureWriter"
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 4.5

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings Vert(uint id : SV_VertexID)
            {
                float2 ndc[3] = { float2(-1,-1), float2(3,-1), float2(-1,3) };
                Varyings o;
                o.positionCS = float4(ndc[id], 0, 1);
                o.uv         = ndc[id] * 0.5 + 0.5;
                return o;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float rawDepth = SampleCameraDepth(i.uv);
                float linearEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                return linearEyeDepth;
            }
            ENDHLSL
        }
    }
}
