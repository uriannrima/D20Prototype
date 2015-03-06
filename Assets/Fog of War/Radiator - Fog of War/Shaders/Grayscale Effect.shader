﻿Shader "Hidden/Grayscale Effect" {
	Properties {
 		_MainTex ("Base (RGB)", 2D) = "white" {}
 		_RampTex ("Base (RGB)", 2D) = "grayscaleRamp" {}
	}
	
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM
			#pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest 
            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
            uniform sampler2D _RampTex;
            uniform half _RampOffset;
            
            fixed4 frag (v2f_img i) : COLOR {
            	// Original color at point uv
            	fixed4 original = tex2D(_MainTex, i.uv);
            	
            	// Luminance of the origonal color
            	fixed grayscale = Luminance(original.rgb);
            	
            	// Don't know exactly
            	half2 remap = half2 (grayscale + _RampOffset, .5);
            	
            	// Test
            	fixed4 output = (original * (1 - original.a)) + (tex2D(_RampTex, remap) * original.a);
            	
            	output.a = original.a;
            	
            	return output;
            }
            
			ENDCG
		}
	}
	
	Fallback off
}