float Pow4(half x) {
    return x * x * x * x;
}

inline half3 RotateDirection(half3 R, half degrees)
{
	float3 reflUVW = R;
	half theta = degrees * PI / 180.0f;
	half costha = cos(theta);
	half sintha = sin(theta);
	reflUVW = half3(reflUVW.x * costha - reflUVW.z * sintha, reflUVW.y, reflUVW.x * sintha + reflUVW.z * costha);
	return reflUVW;
}

float3 Diffuse_Lambert(float3 DiffuseColor) {
    return DiffuseColor * (1 / PI);
}

float D_GGX_TR(float3 N, float3 H, float a) {
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float nom = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / denom;
}

float3 F_FresnelSchlick(float cosTheta, float3 F0) {
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

float3 FresnelSchlickRoughness(float NV, float3 F0, float Roughness) {
    float smoothness = 1.0 - Roughness;
    return F0 + (max(smoothness.xxx, F0) - F0) * pow(1.0 - NV, 5.0);
}

float2 EnvBRDFApprox(float Roughness, float NoV) {
    // [ Lazarov 2013, "Getting More Physical in Call of Duty: Black Ops II" ]
    // Adaptation to fit our G term.
    const float4 c0 = {
        - 1, -0.0275, -0.572, 0.022
    };
    const float4 c1 = {
        1, 0.0425, 1.04, -0.04
    };
    float4 r = Roughness * c0 + c1;//mad:multiply add
    float a004 = min(r.x * r.x, exp2(-9.28 * NoV)) * r.x + r.y;//mad
    float2 AB = float2(-1.04, 1.04) * a004 + r.zw;//mad
    return AB;
}

float GeometrySchlickGGX(float NV, float Roughness) {
    float r = Roughness + 1.0;
    float k = r * r / 8.0;
    float nominator = NV;
    float denominator = k + (1.0 - k) * NV;
    return nominator / max(denominator, 0.0000001) ;//防止分母为0

}

float G_GeometrySmith(float3 N, float3 V, float3 L, float Roughness) {
    float NV = max(dot(N, V), 0);
    float NL = max(dot(N, L), 0);

    float ggx1 = GeometrySchlickGGX(NV, Roughness);
    float ggx2 = GeometrySchlickGGX(NL, Roughness);

    return ggx1 * ggx2;
}

float3 StandardBRDF(float3 BaseMapColor, float Metallic, float3 Roughness, float3 N, float3 V) {
    float3 DiffuseColor = lerp(BaseMapColor, float3(0.0, 0.0, 0.0), Metallic);
    float3 SpecularColor = lerp(float3(0.04, 0.04, 0.04), BaseMapColor, Metallic);

    float3 L = _MainLightPosition.xyz;
    float3 Radiance = _MainLightColor.xyz;
    float3 H = normalize(L + V);
    float NH = saturate(dot(N, H));
    float NV = saturate(abs(dot(N, V)) + 1e-5);
    float NL = saturate(dot(N, L));
    float VH = saturate(dot(V, H));

    float D = D_GGX_TR(N, H, Roughness);
    float3 F = F_FresnelSchlick(NV, SpecularColor);
    float G = G_GeometrySmith(N, V, L, Roughness);

    float3 nominator = D * F * G;
    float denominator = max(4 * NV * NL, 0.001);
    float3 Specular = nominator / denominator;

    float3 KS = F;
    float3 KD = (1 - KS) * (1 - Metallic);

    float3 Diffuse = KD * BaseMapColor;
    float3 DirectLight = (Diffuse + Specular) * NL * Radiance;

    return DirectLight;
}

void DirectLighting_float(float3 BaseMapColor, float Metallic, float Roughness,
float3 N, float3 V, out float3 DirectLighting) {
    half3 DirectLighting_MainLight = StandardBRDF(BaseMapColor, Metallic, Roughness, N, V);
    DirectLighting = DirectLighting_MainLight;
}

void IndirectLighting_float(float3 BaseMapColor, float Metallic, float Roughness, float Occlusion,
float3 N, float3 V, out float3 IndirectLighting) {
    IndirectLighting = float3(0, 0, 0);
    float3 DiffuseColor = lerp(BaseMapColor, float3(0.0, 0.0, 0.0), Metallic);
    float3 SpecularColor = lerp(float3(0.04, 0.04, 0.04), BaseMapColor, Metallic);

    float3 R = reflect(-V, N);
    float NV = saturate(abs(dot(N, V)) + 1e-5);
    float3 F_IndirectLight = FresnelSchlickRoughness(NV, SpecularColor, Roughness);
    float mip = Roughness * (1.7 - 0.7 * Roughness) * UNITY_SPECCUBE_LOD_STEPS;

    //float4 rgb_mip = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0,samplerunity_SpecCube0, R, mip);
    half4 rgb_mip = SAMPLE_TEXTURECUBE(unity_SpecCube0, samplerunity_SpecCube0, R);
    float3 EnvSpecularPrefilted = DecodeHDREnvironment(rgb_mip, unity_SpecCube0_HDR);

    float2 env_brdf = EnvBRDFApprox(Roughness, NV);
    float3 Specular_Indirect = EnvSpecularPrefilted * (F_IndirectLight * env_brdf.r + env_brdf.g);
    float3 KD_IndirectLight = float3(1, 1, 1) - F_IndirectLight;
    KD_IndirectLight *= 1 - Metallic;
    float3 irradianceSH = SampleSH(N);
    float3 Diffuse_Indirect = irradianceSH * BaseMapColor * KD_IndirectLight; //没有除以 PI
    IndirectLighting = (Diffuse_Indirect + Specular_Indirect) * Occlusion;
}

void IndirectLighting_floatTest(float3 BaseMapColor, float Metallic, float Roughness, float Occlusion,
float3 N, float3 V,float3 WorldPos, out float3 IndirectLighting) {
    IndirectLighting = float3(0, 0, 0);
    float3 DiffuseColor = lerp(BaseMapColor, float3(0.0, 0.0, 0.0), Metallic);
    float3 SpecularColor = lerp(float3(0.04, 0.04, 0.04), BaseMapColor, Metallic);

    float3 R = reflect(-V, N);
    float NV = saturate(abs(dot(N, V)) + 1e-5);
    float3 F_IndirectLight = FresnelSchlickRoughness(NV, SpecularColor, Roughness);
    float mip = Roughness * (1.7 - 0.7 * Roughness) * UNITY_SPECCUBE_LOD_STEPS;

    //float4 rgb_mip = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0,samplerunity_SpecCube0, R, mip);
    half4 rgb_mip = SAMPLE_TEXTURECUBE(unity_SpecCube0, samplerunity_SpecCube0, R);
    float3 EnvSpecularPrefilted = DecodeHDREnvironment(rgb_mip, unity_SpecCube0_HDR);
    EnvSpecularPrefilted = GlossyEnvironmentReflection(R,WorldPos,Roughness,Occlusion);

    float2 env_brdf = EnvBRDFApprox(Roughness, NV);
    float3 Specular_Indirect = EnvSpecularPrefilted * (F_IndirectLight * env_brdf.r + env_brdf.g);
    float3 KD_IndirectLight = float3(1, 1, 1) - F_IndirectLight;
    KD_IndirectLight *= 1 - Metallic;
    float3 irradianceSH = SampleSH(N);
    float3 Diffuse_Indirect = irradianceSH * BaseMapColor * KD_IndirectLight; //没有除以 PI
    IndirectLighting = (Diffuse_Indirect + Specular_Indirect) * Occlusion;
}
