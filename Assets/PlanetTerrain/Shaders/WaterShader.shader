Shader "Terrain/Water Shader" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
     }
     SubShader {
         Tags { "RenderType"="Opaque" }
         ZWrite Off
         CGPROGRAM
         #pragma surface surf Lambert alpha
         
         struct Input {
                float4 screenPos;
         };
         
         float4 _Color;
         sampler2D _CameraDepthTexture;
 
         void surf (Input IN, inout SurfaceOutput o) {
         	float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos))));
			o.Albedo = _Color.rgb;
			o.Albedo.r = IN.screenPos.z;//saturate((sceneZ-IN.screenPos.z)*0.01);
			//o.Alpha = saturate((sceneZ-IN.screenPos)*0.01);
			o.Gloss = 0.0;
         }
         ENDCG
     }
     Fallback "VertexLit"
 } 