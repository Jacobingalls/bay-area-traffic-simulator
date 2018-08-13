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

		const static int maxColorCount = 8;
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
}
