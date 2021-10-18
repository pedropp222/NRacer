// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Blend" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BlendTexture ("Texture", 2D) = "white" {}
		_BlendTexture2("Texture2", 2D) = "white" {}
		_BlendTexture3("Texture3", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Tiling("Tiling",Range(0,100)) = 1.0
		_Depth("Depth",Range(0,1)) = 0.2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BlendTexture;
		sampler2D _BlendTexture2;
		sampler2D _BlendTexture3;
		float _Depth;
		float _Tiling;

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
			float3 worldPos;

		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float4 blend(float4 texture1, float a1, float4 texture2, float a2)
		{
			float depth = _Depth;
			float ma = max(texture1.a + a1, texture2.a + a2) - depth;
			float maximo = max(texture1.a, texture2.a);
			float b1 = max(texture1.a + a1 - ma, 0);
			float b2 = max(texture2.a + a2 - ma, 0);

			float3 res = (texture1 * b1 + texture2 * b2) / (b1 + b2);

			return float4(res.rgb, maximo);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c1 = tex2D (_MainTex, IN.worldPos.xz * _Tiling) * _Color;
			fixed4 c2 = tex2D(_BlendTexture, IN.worldPos.xz * _Tiling);
			fixed4 c3 = tex2D(_BlendTexture2, IN.worldPos.xz * _Tiling);
			fixed4 c4 = tex2D(_BlendTexture3, IN.worldPos.xz * _Tiling);

			//lerp(c1, c2, IN.color.r);

			float4 color = blend(c1, 1 - IN.color.r, c2, IN.color.r);
			color = blend(color, 1-IN.color.g, c3, IN.color.g);
			color = blend(color, 1 - IN.color.b, c4, IN.color.b);

			o.Albedo = color.rgb;

			//o.Albedo = lerp(c1, c2, IN.color.r).rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c1.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
