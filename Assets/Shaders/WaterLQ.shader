Shader "Custom/WaterLQ"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Tint("Tint", Color) = (1,1,1,1)
		_Tint2("Tint2", Color) = (1,1,1,1)
		_Tint2Range("Tint2 Range (RGB)", range(0.01,2)) = 2
		_Tint2Width("Tint2 Width (RGB)", range(0.01,2)) = 2
		_Tint2Power("Tint2 Power (RGB)", range(0.01,2)) = 2
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_WaterNormal1("WaterNormal1 (RGB)", 2D) = "white" {}
	_WaterNormal2("WaterNormal2 (RGB)", 2D) = "white" {}
	_WaterDepth("Water Depth (RGB)", float) = 2
		_WaterPower("Water Power (RGB)", range(0,2)) = 2
		_TextureScale("TexutreScale", float) = 2
		_TextureScale2("TexutreScale2", float) = 2
		_Refraction("RefractionWeight", float) = 2
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		Zwrite Off
		LOD 200
		GrabPass
	{
		"_BackgroundTexture"
	}

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows alpha:fade
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _BackgroundTexture;
	sampler2D _WaterNormal1, _WaterNormal2;

	uniform sampler2D _CameraDepthTexture;

	struct Input
	{
		float2 uv_MainTex;
		float3 worldPos;
		float4 screenPos;
	};

	half _Glossiness;
	half _TextureScale, _TextureScale2, _Refraction, _WaterDepth, _WaterPower, _Tint2Range, _Tint2Power, _Tint2Width;
	half _Metallic;
	fixed4 _Color, _Tint, _Tint2;


	UNITY_INSTANCING_BUFFER_START(Props)

		UNITY_INSTANCING_BUFFER_END(Props)

		float depth(float4 worldPos, float2 refraction) {
		float sceneZ = tex2D(_CameraDepthTexture, worldPos + refraction).r;
		float l_Obj = Linear01Depth(worldPos.z);
		float l_surf = Linear01Depth(sceneZ);
		float constant = _WaterDepth;
		float w_surf_depth = l_surf * _ProjectionParams.z;
		float w_obj_depth = l_Obj * _ProjectionParams.z;
		float depth = w_surf_depth - w_obj_depth;
		float water_depth = clamp(abs(depth),0,constant) / constant;
		float water_depth_before = water_depth;
		water_depth = pow(water_depth, _WaterPower);
		return water_depth;
	}

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		// Albedo comes from a texture tinted by color
		float2 uv_coords = IN.worldPos.xz;
		float2 displace1 = float2(_Time.x, _Time.y)*0.2;
		float2 displace2 = float2(_Time.y, _Time.y)*-0.2;
		fixed4 c = tex2D(_MainTex, uv_coords) * _Color;
		float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

		float4 worldPos = IN.screenPos / IN.screenPos.w;
		//worldPos.z /=IN.screenPos.w;


		float3 normal1 = UnpackNormal(tex2D(_WaterNormal1, (uv_coords + displace1) * _TextureScale));
		float3 normal2 = UnpackNormal(tex2D(_WaterNormal2, (uv_coords + displace2) * _TextureScale2));

		float2 refraction = normalize(normal1 + normal2)*_Refraction;

		//float sceneZ = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(worldPos)).r;

		float water_depth = depth(worldPos, float2(0,0));
		// Metallic and smoothness come from slider variables


		refraction *= pow(smoothstep(0.0,1.0,water_depth), 1.0);
		refraction /= IN.screenPos.w;
		float water_depth2 = depth(worldPos, refraction);
		float transparency_depth = depth(worldPos, refraction);
		refraction *= water_depth2;
		float3 tint = _Tint * water_depth;
		float3 refractionTexture = tex2D(_BackgroundTexture, screenUV + refraction);

		float tint2_f = pow(smoothstep(_Tint2Range, _Tint2Width, water_depth), _Tint2Power);
		float3 tint2 = lerp(refractionTexture, _Tint2, tint2_f);

		o.Albedo = lerp(tint2, tint, water_depth);
		float transparency = smoothstep(0, 0.2, transparency_depth);

		o.Normal = normalize(normal1 + normal2) * (water_depth);

		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = transparency;
	}
	ENDCG
	}
		FallBack "Diffuse"
}