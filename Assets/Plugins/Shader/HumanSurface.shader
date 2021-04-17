Shader "Shader Forge/HumanSurface" 
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BumpMap("Bump Map", 2D) = "bump"{}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM
#pragma surface surf LightModel  

		//命名规则：Lighting接#pragma suface之后起的名字 
		//lightDir :点到光源的单位向量   viewDir:点到摄像机的单位向量   atten:衰减系数 
		float4 LightingLightModel(SurfaceOutput s, float3 lightDir,half3 viewDir, half atten)
		{
			float4 c;

			c.rgb = s.Albedo;
			c.a = s.Alpha;

			return c;
		}

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
			float4 _Color;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed3 normals = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			o.Normal = normals;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
