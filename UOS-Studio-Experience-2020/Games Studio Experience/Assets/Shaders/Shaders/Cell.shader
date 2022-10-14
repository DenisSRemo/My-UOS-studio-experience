Shader "Custom/Cell"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}

		// Ambient light is applied uniformly to all surfaces on the object.
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.6509434,0.6509434,0.6509434,1)

		[HDR]
		_SpecularColor("Specular Color", Color) = (0.1037736,0.1037736,0.1037736,1)
		// Controls the size of the specular reflection.
		_Glossiness("Glossiness", Float) = 4.3

		//[MaterialToggle] _OutlineLightScale("Outline light influence", Float) =1
		_OutlineColor("Outline Color", Color) = (0.8962264,0.8962264,0.8962264,1.0)
		_Outline("Outline width", Float) = 2
	}
	SubShader
	{ 				
		//First pass is for the shading of the faces
		//Sourced from: https://roystan.net/articles/toon-shader.html, excluding their method for outline
		Pass
		{
			// Setup our pass to use Forward rendering, and only receive
			// data on the main directional light and ambient light.
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			CGPROGRAM

			#include "UnityCG.cginc"

			// Files below include macros and functions to assist
			// with lighting and shadows.
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			//Have a vertex and frgment shader
			#pragma vertex vert
			#pragma fragment frag

			// Compile multiple versions of this shader depending on lighting settings.
			#pragma multi_compile_fwdbase

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;				
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// Defined in Autolight.cginc. Assigns the above shadow coordinate
				// by transforming the vertex from world space to shadow-map space.
				TRANSFER_SHADOW(o)

				return o;
			}

			//Used for the fragment shader
			float4 _Color;
			float4 _AmbientColor;
			float4 _SpecularColor;
			float _Glossiness;

			float4 frag(v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);

				// Lighting below is calculated using Blinn-Phong,
				// with values thresholded to creat the "toon" look.
				// https://en.wikipedia.org/wiki/Blinn-Phong_shading_model

				// Calculate illumination from directional light.
				// _WorldSpaceLightPos0 is a vector pointing the OPPOSITE
				// direction of the main directional light.
				//(dot product between the normal and the light direction
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				// Samples the shadow map, returning a value in the 0...1 range,
				// where 0 is in the shadow, and 1 is not.
				float shadow = SHADOW_ATTENUATION(i);
				// Partition the intensity into light and dark, smoothly interpolated
				// between the two to avoid a jagged break.
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
				// Multiply by the main directional light's intensity and color.
				float4 light = lightIntensity * _LightColor0;

				// Calculate specular reflection.
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				// Multiply _Glossiness by itself to allow artist to use smaller
				// glossiness values in the inspector.
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;

				float4 sample = tex2D(_MainTex, i.uv);

				return (light + _AmbientColor + specular) * _Color * sample;
			}
			ENDCG
		}
		//This pass is used for the outline
		//Sourced from: https://assetstore.unity.com/packages/vfx/shaders/toon-shader-free-21288#content (outline diffuse1), modified with light scaling
		Pass
		{
			//prevents the outline for being visible from the front, only on outside
			Cull Front

			//on keeps it hidden behind other geo
			ZWrite On

			CGPROGRAM
			#include "UnityCG.cginc"		

			// Files below include macros and functions to assist
			// with lighting and shadows.
			#include "Lighting.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0

			//Have a vertex and frgment shader
			#pragma vertex vert
			#pragma fragment frag

			struct appdata_t
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0; // diffuse lighting color
				float4 pos : SV_POSITION;
			};

			fixed _Outline;
			bool _OutlineLightScale;
			//moves out in the direction of the normal by the outline width
			v2f vert(appdata_t v)
			{
				v2f o;

				o.uv = v.texcoord;
				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// dot product between normal and light direction for
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				o.diff = nl * _LightColor0;

				o.pos = v.vertex;

				o.pos.xyz += normalize(v.normal.xyz) * (_Outline * nl) * 0.01;
				/*#if _OutlineLightScale == 1
					o.pos.xyz += normalize(v.normal.xyz) * (_Outline * nl) * 0.01;
				#else
					o.pos.xyz += normalize(v.normal.xyz) * _Outline  * 0.01;
				#endif*/
				
				o.pos = UnityObjectToClipPos(o.pos);				

				return o;
			}

			fixed4 _OutlineColor;

			fixed4 frag(v2f i) :COLOR
			{
				//_OutlineColor.a *=i.diff;
				return _OutlineColor;
			}
			ENDCG
		}
			
		// Shadow casting support.
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}