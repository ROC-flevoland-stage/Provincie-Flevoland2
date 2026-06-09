Shader "Custom/ActivityMarker"  
{  
    Properties  
    {  
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}                   // Base texture
        [BaseColor] _BaseColor("Base Color", Color) = (0.5, 0.5, 0.5, 0.5)    // Base color
        _Speed("Speed", Float) = 1                                            // Color change speed
        [Toggle] _UseRainbow("Use Rainbow", Float) = 1                        // Toggle for color change effect
    }  

    SubShader  
    {  
        // Set tags to support Transparent rendering in URP
        Tags   
        {   
            "RenderType" = "Transparent"  
            "Queue" = "Transparent"  
            "RenderPipeline" = "UniversalPipeline"  
        }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha  
            ZWrite Off  
            Cull Off  

            HLSLPROGRAM  

            #pragma vertex vert  
            #pragma fragment frag  

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"  

            struct Attributes  
            {  
                float4 positionOS : POSITION;  
                float2 uv : TEXCOORD0;  
            };

            struct Varyings  
            {
                float4 positionHCS : SV_POSITION;  
                float2 uv : TEXCOORD0;  
            };

            TEXTURE2D(_BaseMap);  
            SAMPLER(sampler_BaseMap);  

            CBUFFER_START(UnityPerMaterial)  
                float4 _BaseMap_ST;  
                float4 _BaseColor;  
                float _Speed;  
                float _UseRainbow;  
            CBUFFER_END  

            Varyings vert(Attributes IN)  
            {  
                Varyings OUT;  
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);  
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);  
                return OUT;  
            }  

            half4 frag(Varyings IN) : SV_Target  
            {  
                // Sample the base texture
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);  

                // Calculate the rainbow color based on time and speed
                float t = _Time.y * _Speed;  

                // Generate a rainbow color using sine waves
                half3 rainbow = half3(  
                    sin(t) * 0.5 + 0.5,  
                    sin(t + 2.094) * 0.5 + 0.5,  
                    sin(t + 4.188) * 0.5 + 0.5  
                );

                // Interpolate between the base color and the rainbow color based on the toggle
                // I.E. if _UseRainbow is 1, it will use the rainbow color, if it's 0, it will use the base color
                // Since it's a toggle, it will be either 0 or 1, so the lerp will effectively switch between the two colors
                half3 finalColor = lerp(_BaseColor.rgb, rainbow, _UseRainbow);  

                // Multiply the texture color by the final color and apply the alpha from the base color
                return half4(tex.rgb * finalColor, tex.a * _BaseColor.a);  
            }  

            ENDHLSL  
        }  
    }  
}