#ifndef SG_DEFAULT_BXDF_INCLUDED
#define SG_DEFAULT_BXDF_INCLUDED
#include "Common.hlsl"

float acosFast(float inX) 
{
    float x = abs(inX);
    float res = -0.156583f * x + (0.5 * PI);
    res *= sqrt(1.0f - x);
    return (inX >= 0) ? res : PI - res;
}

// Same cost as acosFast + 1 FR
// Same error
// input [-1, 1] and output [-PI/2, PI/2]
float asinFast( float x )
{
    return (0.5 * PI) - acosFast(x);
}

float Hair_g(float B, float Theta)
{
	return exp(-0.5 * Pow2(Theta) / (B * B)) / (sqrt(2 * PI) * B);
}

float Hair_F(float CosTheta)
{
	const float n = 1.55;
	const float F0 = Pow2((1 - n) / (1 + n));
	return F0 + (1 - F0) * Pow5(1 - CosTheta);
}

// Reference: A Practical and Controllable Hair and Fur Model for Production Path Tracing.
float3 HairColorToAbsorption(float3 C, float B = 0.3f)
{
	const float b2 = B * B;
	const float b3 = B * b2;
	const float b4 = b2 * b2;
	const float b5 = B * b4;
	const float D = (5.969f - 0.215f * B + 2.532f * b2 - 10.73f * b3 + 5.574f * b4 + 0.245f * b5);
	return Pow2(log(C) / D);
}

float3 KajiyaKayDiffuseAttenuation(float3 BaseColor,float Scatter, float3 L, float3 V, half3 N, float Shadow)
{
	// Use soft Kajiya Kay diffuse attenuation
	float KajiyaDiffuse = 1 - abs(dot(N, L));

	float3 FakeNormal = normalize(V - N * dot(V, N));
	//N = normalize( DiffuseN + FakeNormal * 2 );
	N = FakeNormal;

	// Hack approximation for multiple scattering.
	float Wrap = 1;
	float NoL = saturate((dot(N, L) + Wrap) / Pow2(1 + Wrap));
	float DiffuseScatter = (1 / PI) * lerp(NoL, KajiyaDiffuse, 0.33) * Scatter;
	float Luma = Luminance(BaseColor);
	float3 ScatterTint = pow(BaseColor / Luma, 1 - Shadow);
	return sqrt(BaseColor) * DiffuseScatter * ScatterTint;
}

#define HAIR_COMPONENT_R 1
#define HAIR_COMPONENT_TT 1
#define HAIR_COMPONENT_TRT 1
#define HAIR_COMPONENT_MULTISCATTER 1

float3 HairShading(float3 BaseColor,float Specular,float Roughness,float Scatter, float3 N, float3 V, half3 L, float Shadow,float InBacklit,float Area)
{
	float ClampedRoughness = clamp(Roughness, 1/255.0f, 1.0f);
	const float Backlit	= min(InBacklit,1.0f);

	const float VoL       = dot(V,L);                                                      
	const float SinThetaL = clamp(dot(N,L), -1.f, 1.f);
	const float SinThetaV = clamp(dot(N,V), -1.f, 1.f);
	float CosThetaD = cos( 0.5 * abs( asinFast( SinThetaV ) - asinFast( SinThetaL ) ) );

	const float3 Lp = L - SinThetaL * N;
	const float3 Vp = V - SinThetaV * N;
	const float CosPhi = dot(Lp,Vp) * rsqrt( dot(Lp,Lp) * dot(Vp,Vp) + 1e-4 );
	const float CosHalfPhi = sqrt( saturate( 0.5 + 0.5 * CosPhi ) );

	float n = 1.55;
	//float n_prime = sqrt( n*n - 1 + Pow2( CosThetaD ) ) / CosThetaD;
	float n_prime = 1.19 / CosThetaD + 0.36 * CosThetaD;

	float Shift = 0.035;
	float Alpha[] =
	{
		-Shift * 2,
		Shift,
		Shift * 4,
	};	
	float B[] =
	{
		Area + Pow2(ClampedRoughness),
		Area + Pow2(ClampedRoughness) / 2,
		Area + Pow2(ClampedRoughness) * 2,
	};

	float3 S = 0;
	if (HAIR_COMPONENT_R)
	{
		const float sa = sin(Alpha[0]);
		const float ca = cos(Alpha[0]);
		float Shift = 2 * sa * (ca * CosHalfPhi * sqrt(1 - SinThetaV * SinThetaV) + sa * SinThetaV);
		float BScale = sqrt(2.0) * CosHalfPhi;
		float Mp = Hair_g(B[0] * BScale, SinThetaL + SinThetaV - Shift);
		float Np = 0.25 * CosHalfPhi;
		float Fp = Hair_F(sqrt(saturate(0.5 + 0.5 * VoL)));
		S += Mp * Np * Fp * Specular * 2 * lerp(1, Backlit, saturate(-VoL));
	}

	////// TT
	if (HAIR_COMPONENT_TT)
	{
		float Mp = Hair_g( B[1], SinThetaL + SinThetaV - Alpha[1] );
		float a = 1 / n_prime;
		float h = CosHalfPhi * ( 1 + a * ( 0.6 - 0.8 * CosPhi ) );
		float f = Hair_F( CosThetaD * sqrt( saturate( 1 - h*h ) ) );
		float Fp = Pow2(1 - f);
		float3 Tp = 0;

		const float3 AbsorptionColor = HairColorToAbsorption(BaseColor);
		Tp = exp(-AbsorptionColor * 2 * abs(1 - Pow2(h * a) / CosThetaD));

		float Np = exp( -3.65 * CosPhi - 3.98 );

		S += Mp * Np * Fp * Tp * Backlit;
	}

	////// TRT
	if (HAIR_COMPONENT_TRT)
	{
		float Mp = Hair_g( B[2], SinThetaL + SinThetaV - Alpha[2] );
		float f = Hair_F( CosThetaD * 0.5 );
		float Fp = Pow2(1 - f) * f;
		float3 Tp = pow( BaseColor, 0.8 / CosThetaD );
		float Np = exp( 17 * CosPhi - 16.78 );
		S += Mp * Np * Fp * Tp;
	}
	
	if(HAIR_COMPONENT_MULTISCATTER)
	{
		S += max(KajiyaKayDiffuseAttenuation(BaseColor,Scatter, L, V, N, Shadow),0.0);//一定要加Max，坑
	}

	S = -min(-S, 0.0);
	return S;
}

void DirectLighting_float (float3 DiffuseColor,float Specular,float Roughness,float3 WorldPos, half3 N, half3 V,
								float Scatter,out float3 DirectLighting)
{
    DirectLighting = float3(0.5, 0.5, 0);
    #ifndef SHADERGRAPH_PREVIEW
        #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
        float4 clipPos = TransformWorldToHClip(WorldPos);
        float4 ShadowCoord = ComputeScreenPos(clipPos);
        #else
        float4 ShadowCoord = TransformWorldToShadowCoord(WorldPos);
        #endif
        float ShadowMask = float4(1.0,1.0,1.0,1.0);
        //--------直接光照--------
        //主光
        half3 DirectLighting_MainLight = half3(0,0,0);
        {
			Light light = GetMainLight(ShadowCoord,WorldPos,ShadowMask);
			half3 L = light.direction;
			half Shadow = light.shadowAttenuation;
			half3 bsdfValue = HairShading(DiffuseColor,Specular,Roughness,Scatter,N,V,L,Shadow,1.0,0.0);
			half VoL = clamp(dot(V,L),0.0,1.0);
			half ShadowScatter = lerp(0.5,0.3,Scatter);
			Shadow = lerp(light.shadowAttenuation,saturate(light.shadowAttenuation + ShadowScatter), VoL);
			half3 LightColor = light.color * PI * Shadow;
			DirectLighting_MainLight = bsdfValue * LightColor;
        }
        //附加光
        half3 DirectLighting_AddLight = half3(0,0,0);
		#ifdef _ADDITIONAL_LIGHTS
		uint pixelLightCount = GetAdditionalLightsCount();
        for (int lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
        {
            Light light = GetAdditionalLight(lightIndex, WorldPos,ShadowMask);
            half3 L = light.direction;
			half Shadow = light.shadowAttenuation * light.distanceAttenuation;
			half3 bsdfValue = HairShading(DiffuseColor,Specular,Roughness,Scatter,N,V,L,Shadow,1.0,0.0);
			half VoL = clamp(dot(V,L),0.0,1.0);
			half ShadowScatter = lerp(0.5,0.3,Scatter);
			Shadow = lerp(light.shadowAttenuation,saturate(light.shadowAttenuation + ShadowScatter), VoL);
			half3 LightColor = light.color * PI * Shadow * light.distanceAttenuation;
            DirectLighting_AddLight += bsdfValue * LightColor;
        }
        #endif
        DirectLighting = DirectLighting_MainLight + DirectLighting_AddLight;
    #endif
}

//间接（环境）光照
void IndirectLighting_float(float3 DiffuseColor,float Specular,float Roughness,half3 WorldPos,half3 N, half3 V,
            float Scatter,half Occlusion,half EnvRotate,out float3 IndirectLighting)
{
    IndirectLighting = float3(0.0,0.0,0.0);

    #ifndef SHADERGRAPH_PREVIEW
	//float NoV = saturate(abs(dot(N,V)) + 1e-5);
	//half3 SpecularColor = (Specular * 0.04f).xxx;
	//float3 DiffuseAO = AOMultiBounce(DiffuseColor,Occlusion);
	//float MainLightShadow = clamp(GetMainLightShadow(WorldPos),0.35,1.0);
	//float SpecularOcclusion = GetSpecularOcclusion(NoV,Pow2(Roughness),Occlusion) * MainLightShadow;
	//float3 SpecularAO = AOMultiBounce(SpecularColor,SpecularOcclusion);

    //-------------SH---------
	float3 L = normalize( V - N * dot(V,N) );
	half3 IndirectDiffuseBRDF = 2 * PI * HairShading(DiffuseColor,Specular,Roughness,Scatter,N,V,L, 1, 0, 0.2);
    half3 RadianceSH = SampleSH(N);
    half3 IndirectDiffuse = IndirectDiffuseBRDF * RadianceSH;
    //-------------IBL-------------
	//N = normalize(cross(cross(N,V),N));
	//half3 R = reflect(-V, N);
	//R = RotateDirection(R,EnvRotate);
	//half3 SpecularLobe = SpecularIBL(R,WorldPos,Roughness,SpecularColor,NoV);
	//float3 IndirectSpecular = SpecularLobe * SpecularAO;
	//IndirectLighting = IndirectDiffuse + IndirectSpecular;

	IndirectLighting = IndirectDiffuse;
    #endif
}


#endif
