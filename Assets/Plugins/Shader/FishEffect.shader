Shader "Shader Forge/FishEffect"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_LightTex("LightTex (RGB) Trans (A)", 2D) = "black" {}
		_AreaTex("AreaTex (RGB) Trans(A)", 2D) = "white" {}
		_GlossTex("GlossTex (RGB) Trans(A)", 2D) = "black" {}

		_speed("Light Speed", Vector) = (1,1,0,0)
		_ModelLight("ModelLight", Range(0,1)) = 0
		_NoizeLight("NoizeLight", Range(0,1)) = 0

		_RimColor("_RimColor", Color) = (0.17,0.36,0.81,0.0)
		_RimWidth("_RimWidth", Range(0.2,9.0)) = 0.9
		_Emission("Emission", Range(0.0, 1.0)) = 1.0

		_Metallic("Metallic", Range(0,1)) = 0.5
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Glossrate("Glossrate", Range(0,1)) = 0.5
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows 
#pragma target 3.0 

	sampler2D _MainTex;
	sampler2D _LightTex;
	sampler2D _AreaTex;
	sampler2D _GlossTex;

	half _Metallic;
	half _Glossiness;
	float2 _speed;
	float _ModelLight;
	float _NoizeLight;
	float _Glossrate;

	fixed4 _RimColor;
	fixed _RimWidth;
	float _Emission;

	struct Input
	{
		float2 uv_MainTex;
		float3 worldNormal;
		float3 viewDir;
	};

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		float2 ruv = IN.worldNormal.xy;
		ruv = ruv * 0.5;
		ruv += _Time.yx * _speed;

		half4 area = tex2D(_AreaTex, IN.uv_MainTex);
		half4 glosstex = tex2D(_GlossTex, IN.uv_MainTex);

		half4 light = tex2D(_LightTex, ruv.xy);
		light.rgb *= _NoizeLight;
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * area + light + glosstex * _Glossrate;

		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Albedo = c.rgb * _ModelLight;
		o.Alpha = c.a;

		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow(rim, _RimWidth) * _Emission;
	}
	ENDCG
	}
	FallBack "Diffuse"
}