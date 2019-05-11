﻿Shader "Custom/ItemGlow" {
	Properties {
		_ColorTint ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap( "Normal Map", 2D) = "bump" {}
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range(0.1, 6.0)) = 6.0
		_Alpha ("Alpha", Range(0.0, 1.0)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		// LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		// #pragma surface surf Standard fullforwardshadows
		#pragma surface surf Standard

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
		float _Alpha;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			IN.color = _ColorTint;
			
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			//o.Emission = _RimColor.rgb * pow(_RimPower, rim);
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			o.Alpha = c.a * _Alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
