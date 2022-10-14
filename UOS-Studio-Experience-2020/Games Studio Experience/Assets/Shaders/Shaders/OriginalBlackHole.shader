Shader "Custom/BlackHoleOld"
{
	Properties
	{
		//HDR colours to enable intensity settings (Useful for controlling bloom)
		[HDR] _Color("Main Color", Color) = (.5,.5,.5,1)
		[HDR]_MainEmission("Main Emission", Color) = (.5,.5,.5,1)

		[HDR]_OutlineColor("Outline Color", Color) = (0,0,0,1)

		_Outline("Outline width", Range(0.0, 20.0)) = 0.5

		_EmissionTex("Emission (RGB)", 2D) = "black" { }
		_MainTex("Base (RGB)", 2D) = "white" { }
		_BumpMap("Bumpmap", 2D) = "bump" {}
	}

		CGINCLUDE
#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float4 color : COLOR;
	};

	uniform float _Outline;
	uniform float4 _OutlineColor;

	//This is what moves the outline out, from the normal
	v2f vert(appdata v)
	{
		// copy of incoming vertex data but scaled according to normal direction to create outline
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);

		o.pos.xy += offset * o.pos.z * _Outline;

		o.color = _OutlineColor;
		return o;
	}
	ENDCG

		//MAIN OBJECT ITSELF
		SubShader
	{
		Tags { "Queue" = "Transparent" }

		// note that a vertex shader is specified here but its using the one above
		Pass
		{
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Off
			ZWrite Off
		//ZTest Always

		// you can choose what kind of blending mode you want for the outline
		Blend SrcAlpha OneMinusSrcAlpha // Normal
		//Blend One One // Additive
		//Blend One OneMinusDstColor // Soft Additive
		//Blend DstColor Zero // Multiplicative
		//Blend DstColor SrcColor // 2x Multiplicative

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		half4 frag(v2f i) : COLOR
		{
			return i.color;
		}
		ENDCG
	}

	CGPROGRAM
			//added alpha for transparency but transparency doesnt work with the outline, values like 254 look good tho
			#pragma surface surf Lambert alpha

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_EmissionTex;
				float2 uv_BumpMap;
			};

			sampler2D _MainTex;
			sampler2D _EmissionTex;
			sampler2D _BumpMap;
			uniform float4 _Color;
			uniform float3 _MainEmission;

			//Surface output
			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
				o.Alpha = _Color.a;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				o.Emission = (tex2D(_EmissionTex, IN.uv_EmissionTex).rgb * _MainEmission);
			}
			ENDCG
	}



		FallBack "Diffuse"
}
