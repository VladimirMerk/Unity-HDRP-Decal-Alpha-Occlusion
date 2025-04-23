using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[ExecuteAlways]
public class DepthCapturePass : CustomPass
{
    [Tooltip("Decal Projector")]
    public DecalProjector sourceDecal;

    [Tooltip("Depth Camera")]
    public Camera sourceCamera;

    [Tooltip("Depth Render Texture")]
    public RenderTexture depthTexture;

    static Material depthMat;
    static int _DecalDepthID = Shader.PropertyToID("_DecalDepthRT");
    static int _DecalVPID = Shader.PropertyToID("_DecalVP");
    static int _DecalZBufferParamsID = Shader.PropertyToID("_DecalZBufferParams");
    protected override void Setup(ScriptableRenderContext ctx, CommandBuffer cmd)
    {
        int w = sourceCamera.pixelWidth;
        int h = sourceCamera.pixelHeight;
        if (!depthTexture)
        {
            var descriptor = new RenderTextureDescriptor(w, h)
            {
                graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat,
                depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.None,
                dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
                useMipMap = false,
                msaaSamples = 1,
                sRGB = false,
                enableRandomWrite = false
            };

            depthTexture = new RenderTexture(descriptor);
            depthTexture.Create();
        }

        depthMat = new Material(Shader.Find("Hidden/DepthTextureWriter"));

        float n = sourceCamera.nearClipPlane;
        float f = sourceCamera.farClipPlane;
        Vector4 zParams = new Vector4(
            1f - f / n,  // x
            f / n,       // y
            (1f - f / n) / f, // z
            (f / n) / f       // w
        );

        cmd.SetGlobalVector(_DecalZBufferParamsID, zParams);
    }

    protected override void Execute(CustomPassContext ctx)
    {
        if (ctx.hdCamera.camera != sourceCamera || depthTexture == null)
            return;

        Matrix4x4 viewMatrix = sourceCamera.worldToCameraMatrix;
        Matrix4x4 projMatrix = GL.GetGPUProjectionMatrix(sourceCamera.projectionMatrix, true);
        Matrix4x4 viewProjMatrix = projMatrix * viewMatrix;
        ctx.cmd.SetGlobalMatrix(_DecalVPID, viewProjMatrix);

        ctx.cmd.Blit(null, depthTexture, depthMat);
        ctx.cmd.SetGlobalTexture(_DecalDepthID, depthTexture);
    }

    protected override void Cleanup()
    {

    }
}
