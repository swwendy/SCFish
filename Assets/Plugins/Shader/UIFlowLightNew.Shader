Shader "UI/Unlit/Flowlight" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FlowTex("Light Texture(A)", 2D) = "black" {} //流光贴图
		_uvaddspeed("",float) = 2//流光uv改变速度
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }//改为透明
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha //指定alphaBlend

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _FlowTex;//属性
			float _uvaddspeed;//属性

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 c = tex2D(_MainTex, i.texcoord);

				float2 uv = i.texcoord;		//计算流光uv
				uv.x /= 2;//取一半
				uv.x += _Time.y * _uvaddspeed;//横向加上
				float flow = tex2D(_FlowTex, uv).a;//取流光亮度							   
				
				float4 color;
				color.rgb = c.rgb + float3(flow, flow, flow);
				color.a = c.a;
				return color;
			}

			ENDCG
		}
	}
	
	//FallBack "Diffuse" 去掉fallback，也就不投影了
}