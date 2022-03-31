using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom Post-processing/RadialBlur", typeof(UniversalRenderPipeline))]
public class RadialBlur : CustomVolumeComponent
{
    private const string SHADER_NAME = "Hidden/Postprocess/RadialBlur";
    private const string PROPERTY_AMOUNT = "_Amount";
    private const string PROPERTY_BLURSIZE = "_BlurSize";
    private const string PROPERTY_BLURCENTERPOS = "_BlurCenterPos";
    private const string PROPERTY_SAMPLES = "_Samples";
    
    private Material _material;

    public ClampedFloatParameter amount = new ClampedFloatParameter(0f, 0f, 1f);
    public FloatParameter blurSize = new FloatParameter(0.1f);
    public Vector2Parameter blurCenterPos = new Vector2Parameter(new Vector2(0.5f, 0.5f));
    public ClampedIntParameter sampleCount = new ClampedIntParameter(8, 1, 48);

    public override bool IsActive()
    {
        if (IsEnable.value == false) return false;
        if (!active || 
            !_material || 
            amount.value <= 0.0f) return false;
        return true;
    }

    public override void Setup()
    {
        if (!_material)
        {
            Shader shader = Shader.Find(SHADER_NAME);
            _material = CoreUtils.CreateEngineMaterial(shader);
        }
    }

    public override void Destroy()
    {
        if (_material)
        {
            CoreUtils.Destroy(_material);
            _material = null;
        }
    }

    public override void Render(CommandBuffer commandBuffer, ref RenderingData renderingData, RenderTargetIdentifier source, RenderTargetIdentifier destination)
    {
        if (!_material) return;
        
        _material.SetInt(PROPERTY_SAMPLES, sampleCount.value);
        _material.SetFloat(PROPERTY_BLURSIZE, blurSize.value*0.1f);
        _material.SetVector(PROPERTY_BLURCENTERPOS, blurCenterPos.value);
        _material.SetFloat(PROPERTY_AMOUNT, amount.value);
        
        _material.SetFloat(PROPERTY_AMOUNT, amount.value);
        
        commandBuffer.Blit(source, destination, _material);
    }

}


