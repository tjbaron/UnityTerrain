Shader "Terrain/PlanetShader" {
	Properties {
		_Texture1 ("Main Texture", 2D) = "white" {}
		_Bump1 ("Main Bump", 2D) = "gray" {}
		
		_Texture2 ("Texture 2", 2D) = "white" {}
		_Bump2 ("Bump 2", 2D) = "gray" {}
		_HeightLow2 ("Height Low 2", Float) = 33.0
		_HeightHigh2 ("Height High 2", Float) = 34.0
		_HeightFade2 ("Height Fade 2", Float) = 1.0
		
		_Texture3 ("Texture 3", 2D) = "white" {}
		_Bump3 ("Bump 3", 2D) = "gray" {}
		
		_Texture4 ("Texture 4", 2D) = "white" {}
		_Bump4 ("Bump 4", 2D) = "gray" {}

		_Splatmap ("Texture 4", 2D) = "white" {}

		_CentrePoint ("Centre", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert


		sampler2D _Texture1;
		sampler2D _Bump1;

		sampler2D _Texture2;
		sampler2D _Bump2;
		float _HeightLow2;
		float _HeightHigh2;
		float _HeightFade2;

		sampler2D _Texture3;
		sampler2D _Bump3;

		sampler2D _Texture4;
		sampler2D _Bump4;

		float4 _CentrePoint;

		struct Input {
			float2 uv_Texture1;
			float2 uv_Texture2;
			float2 uv_Texture3;
			float2 uv_Texture4;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 texture1 = tex2D (_Texture1, IN.uv_Texture1);
			half4 bump1 = tex2D (_Bump1, IN.uv_Texture1);
			half4 texture2 = tex2D (_Texture2, IN.uv_Texture2);
			half4 bump2 = tex2D (_Bump2, IN.uv_Texture2);
			half4 texture3 = tex2D (_Texture3, IN.uv_Texture3);
			half4 texture4 = tex2D (_Texture4, IN.uv_Texture4);
			float height = distance(_CentrePoint.xyz, IN.worldPos);

			half4 c = texture1;
			
			if (height > _HeightLow2 && height < _HeightHigh2) {
				float a = (height - _HeightLow2)/_HeightFade2;
				float b = (_HeightHigh2 - height)/_HeightFade2;
				b = a<b?a:b;
				if (b >= 1.0) {
					c = texture2;
				} else {
					float b1;
					float b2;
					if (b > 0.5) {
						b = 2*(b-0.5);
						b1 = bump1.r*(1-b);
						b2 = bump2.r + ((1-bump2.r)*b);
					} else {
						b = 2*(0.5-b);
						b1 = bump1.r + ((1-bump1.r)*b);
						b2 = bump2.r*(1-b);
					}
					float dif =  (((b2 - b1)*3.0)+0.5);
					c = lerp(c, texture2, clamp(dif, 0.0, 1.0));	
				}
			}

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
