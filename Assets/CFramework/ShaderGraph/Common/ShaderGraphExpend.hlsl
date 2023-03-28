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
#endif
