Shader "Research/Fog of War/Custom Shader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Pass {
			Tags { "RenderType"="Opaque" }
			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			sampler2D _FOWTex;
			
			// Input of the Vertex function
			struct vertexInput {
				float4 vertex : POSITION; // Position of the vertex in object space.
				float3 normal : NORMAL; // Normal of the vertex in world space.
				// float4 col : COLOR; // Color of the vertex in the object.
			};
				
			// Output of the Vertex function
			struct vertexOutput {
				float4 pos : SV_POSITION; // Position of the vertex in World Space.
				float4 col : COLOR; // Color of the vertex after the vertex function.
			};
			
			// Vertex function
			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				
				// First thing, always must return the Vertex position in the World position:
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex); // Now o.pos has Vertex World Position.
				
				return o;
			}
			
			// Fragment function
			fixed4 frag(vertexOutput i) : COLOR {
				// Just return the color that vertex function calculated.
				return fixed4(0,255,255,255);
			}
			
			ENDCG
		}
	}
}