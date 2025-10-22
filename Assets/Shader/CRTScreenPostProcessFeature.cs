// CRTScreenPostProcessFeature.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTScreenPostProcessFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CRTScreenSettings
    {
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public string textureId = "_MainTex";
    }

    public CRTScreenSettings settings = new CRTScreenSettings();
    CRTScreenPostProcessPass CRTScreenPass;

    public override void Create()
    {
        CRTScreenPass = new CRTScreenPostProcessPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
        {
            Debug.LogWarningFormat("Missing Material", GetType().Name);
            return;
        }

        CRTScreenPass.Setup(renderer, renderingData);
        renderer.EnqueuePass(CRTScreenPass);
    }
}

// CRTScreenPostProcessPass.cs
public class CRTScreenPostProcessPass : ScriptableRenderPass
{
    private Material material;
    private RenderTargetIdentifier source;
    private RenderTargetHandle destination;
    private string profilerTag;
    private ScriptableRenderer renderer;
    private RenderingData renderingData;

    public CRTScreenPostProcessPass(CRTScreenPostProcessFeature.CRTScreenSettings settings)
    {
        this.material = settings.material;
        this.renderPassEvent = settings.renderPassEvent;
        this.profilerTag = "CRTScreenPostProcess";
    }

    public void Setup(ScriptableRenderer renderer, RenderingData renderingData)
    {
        this.renderer = renderer;
        this.renderingData = renderingData;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

        // Get the camera color target from within the pass
        source = renderer.cameraColorTarget;

        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;

        cmd.GetTemporaryRT(destination.id, opaqueDesc);

        // 执行后处理
        cmd.Blit(source, destination.Identifier(), material);
        cmd.Blit(destination.Identifier(), source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(destination.id);
    }
}