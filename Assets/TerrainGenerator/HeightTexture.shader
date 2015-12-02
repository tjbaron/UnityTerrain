﻿Shader "Terrain/HeightTexture" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 0)
		_EquatorColor ("Equator Color", Color) = (1, 1, 1, 0)
		_EquatorWidth ("Equator Width", Float) = 10.0
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ChangePoint ("Change at this distance", Float) = 3
		_OuterTex ("Base (RGB)", 2D) = "black" {}
		_CentrePoint ("Centre", Vector) = (0, 0, 0, 0)
		_BlendThreshold ("Blend Distance", Float) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		float4 _Color;
		float4 _EquatorColor;
		float _EquatorWidth;
		sampler2D _MainTex;
		float _ChangePoint;
		float4 _CentrePoint;
		sampler2D _OuterTex;
		float _BlendThreshold;

		struct Input {
			float2 uv_MainTex;
			float2 uv_OuterTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 main = tex2D (_MainTex, IN.uv_MainTex);
			half4 outer = tex2D (_OuterTex, IN.uv_OuterTex);

			float startBlending = _ChangePoint - _BlendThreshold;
			float endBlending = _ChangePoint + _BlendThreshold;

			float curDistance = distance(_CentrePoint.xyz, IN.worldPos);
			float changeFactor = saturate((curDistance - startBlending) / (_BlendThreshold * 2));

			half4 c = lerp(main, outer, changeFactor);
			c.rgba *= lerp(_Color, _EquatorColor, clamp(_EquatorWidth-abs(IN.worldPos.y), 0.0, _EquatorWidth)/_EquatorWidth); 

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}