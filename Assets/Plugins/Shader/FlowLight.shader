Shader "Shader Forge/FlowLight" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_LightTex("LightTex (RGB) Trans (A)", 2D) = "black" {}
		_speed("Light Speed", Vector) = (1,1,0,0)
		_ModelLight("ModelLight", Range(0,1)) = 0
		_NoizeLight("NoizeLight", Range(0,1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		Cull Off
		//ZWrite Off

		CGPROGRAM
		#pragma surface surf Lambert
		sampler2D _MainTex;
		sampler2D _LightTex;
		float2 _speed;
		float _ModelLight;
		float _NoizeLight;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldNormal;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			float2 ruv = IN.worldNormal.xy;
			ruv = ruv * 0.5;
			ruv += _Time.yx * _speed;

			half4 light = tex2D(_LightTex, ruv.xy);
			light.rgb *= _NoizeLight;
			half4 c = tex2D(_MainTex, IN.uv_MainTex) + light;
			o.Albedo = c.rgb * _ModelLight;
			o.Alpha = c.a;
			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
