Shader "Custom/GlowAndOutline" {
	Properties {
		_ColorTint ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap( "Normal Map", 2D) = "bump" {}
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range(0.1, 3.0)) = 3.0
		
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
	}

CGINCLUDE
#include "UnityCG.cginc"

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct v2f {
	float4 pos : POSITION;
	float4 color : COLOR;
};

uniform float _Outline;
uniform float4 _OutlineColor;

v2f vert(appdata v) {
	// just make a copy of incoming vertex data but scaled according to normal direction
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);

	float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
	float2 offset = TransformViewToProjection(norm.xy);

	o.pos.xy += offset * o.pos.z * _Outline;
	o.color = _OutlineColor;
	return o;
}
ENDCG

	// Outline
	SubShader {
		// changes alpha transparency 
		Tags { "Queue" = "Transparent" "RenderType"= "Transparent" }

		// note that a vertex shader is specified here but its using the one above
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Off
			ZWrite Off
			ZTest Always
			ColorMask RGB // alpha not used

			// you can choose what kind of blending mode you want for the outline
			Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			half4 frag(v2f i) :COLOR {
				return i.color;
			}
			ENDCG
		}

		// Pass {
		// 	Name "BASE"
		// 	ZWrite On
		// 	ZTest LEqual
		// 	Blend SrcAlpha OneMinusSrcAlpha
		// 	// Material {
		// 	// 	Diffuse [_ColorTint]
		// 	// 	Ambient [_ColorTint]
		// 	// }
		// 	// // Lighting On
		// 	// SetTexture [_MainTex] {
		// 	// 	ConstantColor [_ColorTint]
		// 	// 	Combine texture * constant
		// 	// }
		// 	// SetTexture [_MainTex] {
		// 	// 	Combine previous * primary DOUBLE
		// 	// }

		// }
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		// #pragma surface surf Standard fullforwardshadows
		#pragma surface surf Lambert alpha:fade // alpha:fade is an optional parameter to Enable traditional fade-transparency

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float4 color : Color;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		float4 _ColorTint;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _RimColor;
		float _RimPower;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			
			IN.color = _ColorTint;
			
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			//o.Emission = _RimColor.rgb * pow(_RimPower, rim);
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			o.Alpha = c.a;
		}
		ENDCG			
	}

	FallBack "Diffuse"
}
