#ifndef CUSTOMFUNC_INCLUDE
#define CUSTOMFUNC_INCLUDE
void GetMainLight_float(out float3 MainLightColor, out float3 MainLightPos) {
    MainLightColor = float3(0, 0, 0);
    MainLightPos = float3(0, 0, 0);
    #ifndef SHADERGRAPH_PREVIEW

        MainLightColor = _MainLightColor.xyz;
        MainLightPos = _MainLightPosition.xyz;

    #endif
}

void ACESToonMapping_float(float3 x, out float3 value) {
    value = 0;
    #ifndef SHADERGRAPH_PREVIEW
    float a = 2.51f;
    float b = 0.03f;
    float c = 2.43f;
    float d = 0.59f;
    float e = 0.14f;
    value = saturate((x * (a * x + b)) / (x * (c * x + d) + e));
    #endif
}

#endif
