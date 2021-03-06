using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom Post-processing/SpeedLine", typeof(UniversalRenderPipeline))]
public class SpeedLine : CustomVolumeComponent
{
    private const string SHADER_NAME = "Hidden/Postprocess/SpeedLine";
    private const string PROPERTY_AMOUNT = "_Amount";
    private const string PROPERTY_COLOR = "_color";
    private const string PROPERTY_FREQUENCY = "_freq";
    private const string PROPERTY_THICKNESS = "_thickness";
    private const string PROPERTY_THICKNESS_RANDOM = "_thicknessRandom";
    private const string PROPERTY_INNERRADIUS = "_innerRadius";
    private const string PROPERTY_INNERRADIUS_RANDOM = "_innerRadiusRandom";
    
    private Material _material;

    public ClampedFloatParameter amount = new ClampedFloatParameter(0f, 0f, 1f);
    public ColorParameter color = new ColorParameter(Color.white);
    public FloatParameter freq = new FloatParameter(100);
    public ClampedFloatParameter thickness = new ClampedFloatParameter(1f, 0f, 1f);
    public ClampedFloatParameter thicknessRandom = new ClampedFloatParameter(1f, 0f, 1f);
    public ClampedFloatParameter innerRadius = new ClampedFloatParameter(0.4f, 0f, 1f);
    public ClampedFloatParameter innerRadiusRandom = new ClampedFloatParameter(0.2f, 0f, 1f);
 

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
        
        _material.SetColor(PROPERTY_COLOR, color.value);
        _material.SetFloat(PROPERTY_FREQUENCY, freq.value);
        _material.SetFloat(PROPERTY_THICKNESS, thickness.value);
        _material.SetFloat(PROPERTY_THICKNESS_RANDOM, thicknessRandom.value);
        _material.SetFloat(PROPERTY_INNERRADIUS, innerRadius.value);
        _material.SetFloat(PROPERTY_INNERRADIUS_RANDOM, innerRadiusRandom.value);

        _material.SetFloat(PROPERTY_AMOUNT, amount.value);
        
        commandBuffer.Blit(source, destination, _material);
    }

}


