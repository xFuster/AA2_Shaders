Shader "Unlit/PBR"
{
	Properties
	{
		 _texture("Texture",2D) = "Black"{}
		 _ambientInt("Ambient int", Range(0,1)) = 0.25
		 _ambientColor("Ambient Color", Color) = (0,0,0,1)

		 _diffuseInt("Diffuse int", Range(0,1)) = 1
		_scecularExp("Specular exponent",Float) = 2.0

		_directionalLightDir("Directional light Dir",Vector) = (0,1,0,1)
		_directionalLightColor("Directional light Color",Color) = (0,0,0,1)
		_directionalLightIntensity("Directional light Intensity",Float) = 1
		_metallicness("MetallicParam", Range(0,1)) = 0.5
		_smoothness("SmoothParam", Range(0,1)) = 0.5
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRECTIONAL_LIGHT_ON
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 wPos : TEXCOORD2;
			};

			sampler2D _texture;
			float4 _texture_ST;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _texture);
				o.uv = v.uv;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			float _ambientInt;//How strong it is?
			fixed4 _ambientColor;
			float _diffuseInt;
			float _scecularExp;

			float4 _pointLightPos;
			float4 _pointLightColor;
			float _pointLightIntensity;

			float4 _directionalLightDir;
			float4 _directionalLightColor;
			float _directionalLightIntensity;

			float _metallicness;
			float _smoothness;
			


			fixed4 frag(v2f i) : SV_Target
			{


				//3 phong model light components
				//We assign color to the ambient term		
				fixed4 ambientComp = _ambientColor * _ambientInt;//We calculate the ambient term based on intensity
				fixed4 finalColor = ambientComp;

				float3 viewVec;
				float3 halfVec;
				float3 difuseComp = float4(0, 0, 0, 1);
				float3 specularComp = float4(0, 0, 0, 1);
				float3 lightColor;
				float3 lightDir;
				float fresnel;
				float aa;
				float dotV;
				float distribution;
				float dotNL;
				float dotNV;
				float dotVH;
				float geometry;
				float lightDist;

				// declarar aqui las variables que se utilicen en ambos lados
				//Directional light properties
				lightColor = _directionalLightColor.xyz;
				lightDir = normalize(_directionalLightDir);

				//Diffuse componenet
				difuseComp = lightColor * _diffuseInt * clamp(dot(lightDir, i.worldNormal),0,1);	

				//Specular component	
				viewVec = normalize(_WorldSpaceCameraPos - i.wPos);
				
				//blinnPhong
				halfVec = normalize(viewVec + lightDir);

				//Fresnel formula
				fresnel = pow((_metallicness + (1.0 - _metallicness) * (1.0 - dot(lightDir, i.worldNormal))), 5.0);

				//Distribution formula
				aa = _smoothness * _smoothness;
				dotV = dot(i.worldNormal, halfVec);
				dotV = dotV * dotV;
				distribution = aa / (3.141592f * pow(((dotV * (aa - 1.0f) + 1.0f)), 2.0f));

				//Geometry formula
				dotNL = dot(i.worldNormal, lightDir);
				dotNV = dot(i.worldNormal, viewVec);
				dotVH = dot(viewVec, halfVec);
				dotVH = dotVH * dotVH;
				geometry = dotNL * dotNV / dotVH;
				specularComp = ((fresnel * distribution * geometry) / (4.0 * dot(i.worldNormal, lightDir) * dot(i.worldNormal, viewVec)));
				//specularComp = lightColor * clamp(dot(lightDir, i.worldNormal), 0, 1) * ((fresnel * distribution * geometry) / (4.0 * dot(i.worldNormal, lightDir) * dot(i.worldNormal, viewVec)));
				//
				//Sum
				finalColor += clamp(float4(_directionalLightIntensity * (difuseComp + specularComp),1),0,1);
				fixed4 outTexture = tex2D(_texture, i.uv);
				 //pointLight

				//return ((fresnel * distribution * geometry) / (4.0 * dot(i.worldNormal, lightDir) * dot(i.worldNormal, viewVec)));
				return finalColor * outTexture;
			 }
			 ENDCG
		 }
	}
}
