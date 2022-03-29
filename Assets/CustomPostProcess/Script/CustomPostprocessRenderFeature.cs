using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class CustomPostprocessRenderFeature : ScriptableRendererFeature
{
    private CustomPostprocessRenderPass<InverseGrayscale> _renderPass = null;
    private CustomPostprocessRenderPass<RadialBlur> _renderPass2 = null;
    
    public override void Create()
    {
        _renderPass = new CustomPostprocessRenderPass<InverseGrayscale>("InverseGrayscalePass", RenderPassEvent.AfterRenderingPostProcessing);
        _renderPass2 = new CustomPostprocessRenderPass<RadialBlur>("RadialBlur", RenderPassEvent.AfterRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        _renderPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(_renderPass);
        
        _renderPass2.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(_renderPass2);
    }
}


