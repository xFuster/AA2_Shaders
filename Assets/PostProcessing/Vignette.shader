Shader "Hidden/Custom/Vignette"
{
	HLSLINCLUDE
	// StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	
	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	
	float _centerX;
	float _centerY;
	float _intensity;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
	
		float2 resolution = float2(_ScreenParams.x, _ScreenParams.y);
		float2 uv = (i.texcoord - 0.5);
	
		float dist = distance(uv, float2(_centerX,_centerY));
		float vignette = 1 - dist;
		vignette = pow(vignette, _intensity);
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		color = clamp((color * vignette),0,1);
		return color;
	}
	ENDHLSL
	
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
			Pass
		{
			HLSLPROGRAM
				#pragma vertex VertDefault
				#pragma fragment Frag
			ENDHLSL
		}
	}
}
