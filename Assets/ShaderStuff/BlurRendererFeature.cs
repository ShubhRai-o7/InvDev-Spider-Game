using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRendererFeature : ScriptableRendererFeature
{
    class BlurPass : ScriptableRenderPass
    {
        private Material blurMaterial;
        private RTHandle source;
        private RTHandle destination;
        private float blurSize;

        public BlurPass(Material material, float size)
        {
            blurMaterial = material;
            blurSize = size;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Allocate the RTHandle for the destination render target
            destination = RTHandles.Alloc(
                cameraTextureDescriptor.width,
                cameraTextureDescriptor.height,
                depthBufferBits: 0,
                dimension: TextureDimension.Tex2D,
                colorFormat: GraphicsFormatUtility.GetGraphicsFormat(cameraTextureDescriptor.colorFormat, true), // Fix: Convert RenderTextureFormat to GraphicsFormat
                filterMode: FilterMode.Bilinear,
                name: "_BlurDestination"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("BlurPass");

            // Fix: Use cameraColorTargetHandle instead of cameraColorTarget
            source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Fix: Use RTHandle in Blit calls
            Blit(cmd, source, destination, blurMaterial);
            Blit(cmd, destination, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Release the RTHandle to clean up memory
            destination.Release();
        }
    }

    [SerializeField] private Material blurMaterial;
    [SerializeField] private float blurSize = 1.0f;

    private BlurPass blurPass;

    public override void Create()
    {
        // Create the blur material
        if (blurMaterial == null)
        {
            Shader blurShader = Shader.Find("Custom/BlurShader");
            blurMaterial = new Material(blurShader);
        }

        blurPass = new BlurPass(blurMaterial, blurSize);
        blurPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(blurPass);
    }
}
