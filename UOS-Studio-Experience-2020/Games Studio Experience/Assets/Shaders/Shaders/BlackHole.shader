Shader "Custom/BlackHole"
{
	Properties
	{
		//HDR colours to enable intensity settings (Useful for controlling bloom)
		[HDR] _Color("Main Color", Color) = (.5,.5,.5,1)
		[HDR] _MainEmission("Main Emission", Color) = (.5,.5,.5,1)
		_EmissionTex("Emission (RGB)", 2D) = "black" { }
		_MainTex("Base (RGB)", 2D) = "white" { }
		_BumpMap("Bumpmap", 2D) = "bump" {}

		//Distortion
		_Scale("Effect Scale", float) = 1.0
		_Speed("Effect Speed", float) = 1.0

		_WorldScale("World Scale", float) = 1.0
	
		//Outline
		[HDR]_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	}

	SubShader{
			Tags { "RenderType" = "Opaque" }
			Cull Off
			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert           
			#pragma target 3.5
			#include "UnityCG.cginc"

			//Distortion
			float _Scale;
			float _WorldScale;
			float _Speed;
			float _RandTimeOffset;

			uniform float4 _Color;
			sampler2D _MainTex;				
			sampler2D _BumpMap;

			//Emission
			sampler2D _EmissionTex;				
			uniform float3 _MainEmission;

			//Outline
			float4 _RimColor;
			float _RimPower;

			struct Input 
			{
				float2 uv_MainTex;
				float3 viewDir;
				float2 uv_EmissionTex;
				float2 uv_BumpMap;
			};

			//moves verts on a sine wave for a heat distortion-esque effect
			void vert(inout appdata_full v)
			{

					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

					float x = sin(worldPos.x / _WorldScale + ((_Time.y + _RandTimeOffset) * _Speed)) * (v.vertex.x) * _Scale * 0.01;
					float y = sin(worldPos.y / _WorldScale + ((_Time.y + _RandTimeOffset) * _Speed)) * (v.vertex.y) * _Scale * 0.01;
					float z = sin(worldPos.z / _WorldScale + ((_Time.y + _RandTimeOffset) * _Speed)) * (v.vertex.z) * _Scale * 0.01;


					v.vertex.z += z;
					v.vertex.x += x;
					//v.vertex.y += y;

			}

			void surf(Input IN, inout SurfaceOutputStandard o) 
			{
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
				o.Alpha = _Color.a;
				o.Metallic = 0.6f;
				o.Smoothness = 0.0f;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

				half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
				o.Emission = (_RimColor.rgb * pow(rim, _RimPower)) + (tex2D(_EmissionTex, IN.uv_EmissionTex).rgb * _MainEmission);
			}

			ENDCG
	}
		Fallback "Diffuse"
}