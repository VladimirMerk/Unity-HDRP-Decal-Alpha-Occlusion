
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

void AlphaDepthMask_float(
    float3 worldPos,
    float  uvScale,
    float bias1,
    float bias2,
    out   float alpha,
    out   float4 debug)
{
    // 1) UV
    float4 clip = mul(_DecalVP, float4(worldPos, 1));
    float2 uv = clip.xy / clip.w * 0.5 + 0.5;
    uv = saturate((uv - 0.5) * uvScale + 0.5);

    // 2) Sample
    float rawDepth = SAMPLE_TEXTURE2D(_DecalDepthRT, sampler_DecalDepthRT, uv).r;
    float mapDepth = LinearEyeDepth(rawDepth, _DecalZBufferParams);

    // float lower = mapDepth + bias1;
    // float upper = mapDepth + bias2;
    // alpha = step(upper, lower, worldPos.y);
    debug = float4(rawDepth, 0, 0, 1);
    alpha = step(mapDepth + bias1, worldPos.y + bias2);
}

