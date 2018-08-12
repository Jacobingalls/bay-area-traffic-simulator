Shader "Custom/terrainShader" {
	Properties 
	{
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};

		const static int maxColorCount = 5;
		const float offset = 0.0001;

		float minHeight;
		float maxHeight;

		float3 baseColors[maxColorCount];
		float baseBlendValues[maxColorCount];
		float baseStartHeights[maxColorCount];
		float worldHeight;
		int baseColorCount;

		float inverseLerp(float a, float b, float val)
		{
			return saturate((val - a) / (b - a)); 
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//float y = sign(IN.worldPos.y) * IN.worldPos.y;
			float calculatedHeight = IN.worldPos.y - worldHeight;
			float percentage = inverseLerp(minHeight, maxHeight, calculatedHeight);

			for(int i = 0; i < baseColorCount; i++)
			{
				float drawStr = inverseLerp(-baseBlendValues[i]/2 - offset, baseBlendValues[i]/2, percentage - baseStartHeights[i]);
				o.Albedo = o.Albedo * (1-drawStr) + baseColors[i] * drawStr;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
	// Properties {
	// 	_Color ("Color", Color) = (1,1,1,1)
	// 	_MainTex ("Albedo (RGB)", 2D) = "white" {}
	// 	_Glossiness ("Smoothness", Range(0,1)) = 0.5
	// 	_Metallic ("Metallic", Range(0,1)) = 0.0
	// 	_MinY ("MinY", Float) = 0.0
	// 	_MaxY ("MaxY", Float) = 1.0
	// }
	// SubShader {
	// 	Tags { "RenderType"="Opaque" }
	// 	LOD 200

	// 	CGPROGRAM
	// 	// Physically based Standard lighting model, and enable shadows on all light types
	// 	#pragma surface surf Standard fullforwardshadows

	// 	// Use shader model 3.0 target, to get nicer looking lighting
	// 	#pragma target 3.0

	// 	sampler2D _MainTex;

	// 	struct Input {
	// 		float2 uv_MainTex;
	// 	};

	// 	half _Glossiness;
	// 	half _Metallic;
	// 	fixed4 _Color;

	// 	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// 	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// 	// #pragma instancing_options assumeuniformscaling
	// 	UNITY_INSTANCING_BUFFER_START(Props)
	// 		// put more per-instance properties here
	// 	UNITY_INSTANCING_BUFFER_END(Props)

	// 	void surf (Input IN, inout SurfaceOutputStandard o) {
	// 		// Albedo comes from a texture tinted by color
	// 		fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

	// 		o.Albedo = c.rgb;
	// 		// Metallic and smoothness come from slider variables
	// 		o.Metallic = _Metallic;
	// 		o.Smoothness = _Glossiness;
	// 		o.Alpha = c.a;
	// 	}
	// 	ENDCG
	// }
	// FallBack "Diffuse"
}
