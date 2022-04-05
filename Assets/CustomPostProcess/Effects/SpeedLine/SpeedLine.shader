Shader "Hidden/Postprocess/SpeedLine"
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

			float _Amount;
			
			float4 _color;
			float _freq;
			float _thickness;
			float _thicknessRandom;
			float _innerRadius;
			float _innerRadiusRandom;
			
			
			float RadialSpeedlines(float2 UV, float Freq, float2 Thick, float2 Radius, float Seed)
            {
                float2 npos = (UV - 0.5) / sqrt(0.5);
            
                float2 polar;
                polar.x = (atan2(npos.x, npos.y) / PI + 1) / 2;
                polar.y = length(npos);
            
                uint seed = (uint)((polar.x + Seed) * Freq) * 4;
            
                float param = frac(polar.x * Freq) * 2 - 1;
            
                float offs = (Hash(seed) - 0.5) * Thick.y;
                float width = Thick.x * (1 - Hash(seed + 1) * Thick.y) * (1 - abs(offs));
                float radius = saturate(Radius.x + (Hash(seed + 2) - 0.5) * Radius.y);
            
                width *= (polar.y - radius) / (1.001 - radius);
                width *= width <= 0 ? 100 : 1;
            
                return saturate((abs(param + offs) - width) * 1000 / Freq / polar.y);
            }
            
            void RadialSpeedlines_float(float2 UV, float Freq, float2 Thick, float2 Radius, float Seed, out float Out)
            {
                Out = RadialSpeedlines(UV, Freq, Thick, Radius, Seed);
            }
            
            float RadialSpeedlinesX4_float(float2 UV, float Freq, float2 Thick, float2 Radius, float Seed)
            {
                float2 uvdx = ddx(UV) / 4;
                float2 uvdy = ddy(UV) / 4;
            
                float acc = 0;
                acc += RadialSpeedlines(UV              , Freq, Thick, Radius, Seed) * 2;
                acc += RadialSpeedlines(UV - uvdx - uvdy, Freq, Thick, Radius, Seed);
                acc += RadialSpeedlines(UV + uvdx - uvdy, Freq, Thick, Radius, Seed);
                acc += RadialSpeedlines(UV - uvdx + uvdy, Freq, Thick, Radius, Seed);
                acc += RadialSpeedlines(UV + uvdx + uvdy, Freq, Thick, Radius, Seed);
                return acc / 6;
            } 

            float4 frag (Varyings i) : SV_Target
            {
                float2 mainTexUV = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 col = tex2D(_MainTex, mainTexUV);
               
                float speed = _Time.y * 10.0f;
                float2 thick = float2(_thickness, _thicknessRandom);
                float2 innerRadius = float2(_innerRadius, _innerRadiusRandom);
                float speedLine = RadialSpeedlinesX4_float(mainTexUV, _freq, thick, innerRadius, speed);
                speedLine = 1 - saturate(speedLine);
                 
                return lerp(col, _color, speedLine * _Amount);    
            }
            
            ENDHLSL
        }
    }
}

