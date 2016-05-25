Shader "Unlit/GrabGlow"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	threshold("threshold", Range(0,1)) = 0

	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
		Name "MAIN"
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag       
#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

	uniform  float threshold = 0.5;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float3 th = float3(threshold,threshold,threshold);
		float2 tc = i.uv;

		float4 texColor = tex2D(_MainTex, float2(tc.x, tc.y));

		if (texColor.r >= th.r && texColor.g >= th.g && texColor.b >= th.b) {
			return texColor;
		}
		return float4(0.0, 0.0, 0.0, 0.0);
	}
		ENDCG
	}
	}
}