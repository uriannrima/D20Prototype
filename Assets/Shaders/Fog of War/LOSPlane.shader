Shader "Fog Of War/Plane" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		#include "FOWIncludes.cginc"

		// Sampler represents a simple Texture object, in this case, a Texture2D.
		sampler2D _MainTex;
		
		// Fixed4 its a "small" data type, with a value not too precise
		// Perfect to store 4 values from 0 to 255 with enough precision
		// As color has R G B A, we can use Fixed4 for it.
		fixed4 _Color;
		
		// Fog of War Texture2D
		sampler2D _FOWTex;
		
		// Floating point.
		float4 _FOWTex_ST;

		// UV means an set of coordinate, from 0 to 1.
		// So uv_MainText its a coordinate from the MainText.
		// Details about Input: http://docs.unity3d.com/Manual/SL-SurfaceShaders.html
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		// To know about SurfaceOutput look at: http://docs.unity3d.com/Manual/SL-SurfaceShaders.html
		void surf (Input IN, inout SurfaceOutput o) {
			// Color from _MainTex at uv_MainTex
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
			// TRANSFORM_TEX(position, texture) returns position as relative position from texture.
			// In this case, we get worldPos.xz from _FowTex
			// So, we have color from _FowTex at IN.worldPos.xz (world position)
			half4 fow = tex2D(_FOWTex, TRANSFORM_TEX(IN.worldPos.xz, _FOWTex));
			
			// Then, we transform Texture color to Fog of War Color.
			half4 t = TransformColourFOWAO(c, fow);
						
			// Setting output color and alpha.
			o.Albedo = t.rgb;
			o.Alpha = t.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
