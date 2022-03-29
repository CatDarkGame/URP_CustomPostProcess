using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class GrayScale_RenderPass : ScriptableRenderPass
{
    protected const string TEMP_BUFFER_NAME = "_TempColorBuffer";
   
    protected GrayScale Component;
    protected string RenderTag { get; }
    
    private RenderTargetIdentifier _source;
    private RenderTargetHandle _tempTexture;
    
    public GrayScale_RenderPass(string renderTag, RenderPassEvent passEvent) 
    {
        renderPassEvent = passEvent;
        RenderTag = renderTag;
    }

    public virtual void Setup(in RenderTargetIdentifier source)
    {
        _source = source;
        _tempTexture.Init(TEMP_BUFFER_NAME);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;

        VolumeStack volumeStack = VolumeManager.instance.stack;
        Component = volumeStack.GetComponent<GrayScale>();
        if(Component) Component.Setup();
        if (!Component || !Component.IsActive())  return;
        
        CommandBuffer commandBuffer = CommandBufferPool.Get(RenderTag);
        RenderTargetIdentifier destination = _tempTexture.Identifier();
        
        // 렌더 텍스처 생성
        CameraData cameraData = renderingData.cameraData;
        RenderTextureDescriptor descriptor = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight);
        descriptor.colorFormat = cameraData.isHdrEnabled ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        commandBuffer.GetTemporaryRT(_tempTexture.id, descriptor);
        
        // 임시 버퍼 생성
        commandBuffer.Blit(_source, destination);
        // Pass 렌더링
        Component.Render(commandBuffer, ref renderingData, destination, _source);
        commandBuffer.ReleaseTemporaryRT(_tempTexture.id);
        
        context.ExecuteCommandBuffer(commandBuffer);
        CommandBufferPool.Release(commandBuffer);
    }
}