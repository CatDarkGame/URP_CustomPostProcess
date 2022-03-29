Shader "Hidden/Postprocess/RadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZTest Always ZWrite Off Cull Off

       Pass
       {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
           
            #pragma prefer_hlslcc gles   
            #pragma exclude_renderers d3d11_9x  
           
            #pragma vertex FullscreenVert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float2 _BlurCenterPos;
			float _BlurSize;
			float _Samples;
			float _Amount;

            float4 frag (Varyings i) : SV_Target
            {
                float2 mainTexUV = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 col = tex2D(_MainTex, mainTexUV);
                float4 col_Blur = 0;
                float2 uvOffset = mainTexUV - _BlurCenterPos;  
                  
                for (int i = 0; i < _Samples; i++)
                {
                    float Scale = 1.0f - _BlurSize * _MainTex_ST.x * i;
                    col_Blur.rgb += tex2D(_MainTex, uvOffset * Scale + _BlurCenterPos).rgb;
                }   
                col_Blur.rgb *= 1.0f / _Samples;
                
                return lerp(col, col_Blur, _Amount);    
            }
            
            ENDHLSL
        }
    }
}
