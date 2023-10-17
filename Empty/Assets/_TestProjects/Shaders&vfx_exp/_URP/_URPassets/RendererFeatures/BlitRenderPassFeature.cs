using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitRenderPassFeature : ScriptableRendererFeature
{
    private class CustomRenderPass : ScriptableRenderPass
    {
        
        private readonly BlitSettings settings;
        
        private RTHandle _source;
        private RTHandle _destination;
        private RTHandle _temp;
        
        //private RTHandle srcTextureId;
        private readonly RTHandle srcTextureObject;
        private RTHandle dstTextureId;
        private readonly RTHandle dstTextureObject;

        private readonly string m_ProfilerTag;
        
        private readonly Material _material;
        private readonly int _passIndex;
        
        /*
        //Obsolete
        //public RenderTargetIdentifier Source;
        //private RenderTargetHandle _tempRenderTargetHandle; //texture buffer
        */


        public CustomRenderPass(BlitSettings blitSettings, string name)
        {
            settings = blitSettings;
            // Configures where the render pass should be injected. BeforeRenderingPostProcessing;
            renderPassEvent = blitSettings.Event;
            
            _material = blitSettings.material;
            _passIndex = blitSettings.blitMaterialPassIndex;
            
            m_ProfilerTag = name;
            if (settings.srcType == Target.RenderTextureObject && settings.srcTextureObject)
                srcTextureObject = RTHandles.Alloc(settings.srcTextureObject);
            if (settings.dstType == Target.RenderTextureObject && settings.dstTextureObject)
                dstTextureObject = RTHandles.Alloc(settings.dstTextureObject);
            
            //obsolete
            //_tempRenderTargetHandle.Init("_TempColorTexture");
        }
        
        public void Setup(ScriptableRenderer renderer) 
        {
            if (settings.requireDepthNormals)
                ConfigureInput(ScriptableRenderPassInput.Normal);
        }
        
        public void Dispose() 
        {
            _temp?.Release();
            dstTextureId?.Release();
        }
        
        
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd) 
        {
            _source = null;
            _destination = null;
        }
        
        /*
         This method is called before executing the render pass.
        It can be used to configure render targets and their clear state. Also to create
        temporary render target textures. When empty this render pass will render to the 
        active camera render target. You should never call CommandBuffer.SetRenderTarget. 
        Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>. The render pipeline 
        will ensure target setup and clearing happens in a performant manner.
        */
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0; // Color and depth cannot be combined in RTHandles
            
            /*RenderingUtils.ReAllocateIfNeeded(ref rth_temp, desc, FilterMode.Point,
                TextureWrapMode.Clamp, name: "_TempColorTexture");*/
            RenderingUtils.ReAllocateIfNeeded(ref _temp, desc, name: "_TemporaryColorTexture");
            
            var renderer = renderingData.cameraData.renderer;
            
            
            if (settings.srcType == Target.CameraColor) 
            {
                _source = renderer.cameraColorTargetHandle;
            } 
            else if (settings.srcType == Target.TextureID) 
            {
                /*RenderingUtils.ReAllocateIfNeeded(ref srcTextureId, Vector2.one,
                desc, name: settings.srcTextureId);
                source = srcTextureId;*/
                /*
                Doesn't seem to be a good way to get an existing target with this new RTHandle system.
                The above would work but means I'd need fields to set the desc too, which is just messy. 
                If they don't match completely we get a new target. Previously we could use a 
                RenderTargetIdentifier... but the Blitter class doesn't have support for those in 2022.1 -_-
                Instead, I guess we'll have to rely on the shader sampling the global textureID
                #1#*/
                
                _source = _temp;
            }
            else if (settings.srcType == Target.RenderTextureObject) 
            {
                _source = srcTextureObject;
            }
            
            
            if (settings.dstType == Target.CameraColor) 
            {
                _destination = renderer.cameraColorTargetHandle;
            } 
            else if (settings.dstType == Target.TextureID) 
            {
                desc.graphicsFormat = settings.graphicsFormat;
                RenderingUtils.ReAllocateIfNeeded(ref dstTextureId,
                    Vector2.one, desc, name: settings.dstTextureId);
                _destination = dstTextureId;
            } 
            else if (settings.dstType == Target.RenderTextureObject) 
            {
                _destination = dstTextureObject;
            }
        }

        
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;
            
            CommandBuffer commandBuffer = CommandBufferPool.Get(m_ProfilerTag);

            if (settings.setInverseViewMatrix) 
            {
                commandBuffer.SetGlobalMatrix("_InverseView",
                    renderingData.cameraData.camera.cameraToWorldMatrix);
            }
            
            if (_source == _destination)
            {
                Blitter.BlitCameraTexture(commandBuffer, _source, _temp, _material, _passIndex);
                Blitter.BlitCameraTexture(commandBuffer, _temp, _destination, Vector2.one);
            }
            else
            {
                Blitter.BlitCameraTexture(commandBuffer, _source, _destination,
                    _material, settings.blitMaterialPassIndex);
            }

            /*
            //Obsolete
            //Blit(commandBuffer, Source, Source, _material); //works without texture buffer
            // commandBuffer.GetTemporaryRT(_tempRenderTargetHandle.id,
            // renderingData.cameraData.cameraTargetDescriptor);
            // Blit(commandBuffer, Source, _tempRenderTargetHandle.Identifier(), _material);
            // Blit(commandBuffer, _tempRenderTargetHandle.Identifier(), Source, _material);
            */

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }
    }

    
    
    [System.Serializable]
    public class BlitSettings 
    {
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
        
        public Material material;
        public int blitMaterialPassIndex;
        
        public bool setInverseViewMatrix;
        public bool requireDepthNormals;

        public Target srcType = Target.CameraColor;
        //public string srcTextureId = "_CameraColorTexture";
        public RenderTexture srcTextureObject;

        public Target dstType = Target.CameraColor;
        public string dstTextureId = "_BlitPassTexture";
        public RenderTexture dstTextureObject;

        public bool overrideGraphicsFormat;
        public UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat;
    }
    
    public enum Target 
    {
        CameraColor,
        TextureID,
        RenderTextureObject
    }
    

    public BlitSettings settings = new BlitSettings();
    private CustomRenderPass m_ScriptablePass;
    

    /// <inheritdoc/>
    public override void Create()
    {
        var passIndex = settings.material != null ? settings.material.passCount - 1 : 1;
        settings.blitMaterialPassIndex = Mathf.Clamp
            (settings.blitMaterialPassIndex, -1, passIndex);
        
        m_ScriptablePass = new CustomRenderPass(settings, name);
        
        if (settings.graphicsFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None) 
        {
            settings.graphicsFormat = SystemInfo.GetGraphicsFormat
                (UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
        }
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) 
        {
            Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute." + 
            " Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }
        
        /*//obsolete
        m_ScriptablePass.Source = renderer.cameraColorTarget;*/

        renderer.EnqueuePass(m_ScriptablePass);
    }
    
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) 
    {
        m_ScriptablePass.Setup(renderer);
    }

    protected override void Dispose(bool disposing) 
    {
        m_ScriptablePass.Dispose();
    }
}


