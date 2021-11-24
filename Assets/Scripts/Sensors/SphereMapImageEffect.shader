Shader "Custom/SphereMapImageEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Fov ("FOV", Float) = 0.333
		_SupersampleScale ("actual sample size", Int) = 2
		//_WidthScale ("Width / Height", Float) = 1
	}
	SubShader {
		Pass {
	  		ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_sphere_mapping
			#include "UnityCG.cginc"
		
			uniform sampler2D _MainTex;

			sampler2D _CameraDepthNormalsTexture;
			sampler2D_float _CameraDepthTexture;

			uniform float _Fov;
			uniform int _SupersampleScale;
			//uniform float _WidthScale;
 
			fixed4 frag_sphere_mapping (v2f_img i) : COLOR {
			
				float linearDepth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));
				return linearDepth;
			}
 
			// fixed4 frag (v2f_img i) : COLOR {	
			// 	float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));
			// 	return 2 / (1+ exp(-15 * depth)) - 1;
			// }
			ENDCG
		}
	}
}
