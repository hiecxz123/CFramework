//2023年02月27日 00:38:10 星期一
Shader "Custom_URP/StandardPBR"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        _MetallicMap("Metallic Map",2D) = "white"{}
        _Metallic("Metallic",Range(0.0,1.0)) = 1.0

        _RoughnessMap("Roughness Map",2D) = "white"{}
        _Roughness("Roughness",Range(0.0,1.0)) = 1.0

        _NormalMap("Normal Map",2D) = "bump"{}
        _Normal("Normal",float) = 1.0

        _OcclusionMap("OcclusionMap",2D) = "white"{}
        _OcclusionStrength("Occlusion Strength",Range(0.0,1.0)) = 1.0
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "LightMode" = "UniversalForward" 
            }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            #include "PBRLighting.hlsl"

            struct appdata
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                half3 normalWS : TEXCOORD2;
                half4 tangentWS : TEXCOORD3;    // xyz: tangent, w: sign
                float4 shadowCoord : TEXCOORD4;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_MetallicMap);    SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_RoughnessMap);    SAMPLER(sampler_RoughnessMap);
            TEXTURE2D(_NormalMap);    SAMPLER(sampler_NormalMap);
            TEXTURE2D(_OcclusionMap);    SAMPLER(sampler_OcclusionMap);

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half _Metallic;
            half _Roughness;
            half _Normal;
            half _OcclusionStrength;

            CBUFFER_END

            


            v2f vert (appdata input)
            {
                v2f output = (v2f)0;

                //开启GPU Instance
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;
                //获取不同API下切线的方向
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                output.positionWS = vertexInput.positionWS;
                output.shadowCoord = GetShadowCoord(vertexInput);
                output.positionCS = vertexInput.positionCS;

                return output;
            }

            float4 frag (v2f input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                float2 UV = input.uv;
                float3 WorldPos = input.positionWS;
                half3 ViewDir = GetWorldSpaceNormalizeViewDir(WorldPos);

                half3 WorldNormal = normalize(input.normalWS);
                half3 WorldTangent = normalize(input.tangentWS.xyz);
                half3 WorldBinormal = normalize(cross(WorldNormal,WorldTangent) * input.tangentWS.w);
                half3x3 TBN = half3x3(WorldTangent,WorldBinormal,WorldNormal);

                float4 ShadowCoord = input.shadowCoord;
                float2 ScreenUV = GetNormalizedScreenSpaceUV(input.positionCS);
                half4 ShadowMask = float4(1.0,1.0,1.0,1.0);

                half4 BaseMapColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) * _BaseColor;
                half3 BaseMapColor = BaseMapColorAlpha.rgb;
                half BaseMapAlpha = BaseMapColorAlpha.a;

                float Metallic = saturate(SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV).r * _Metallic);
                float Roughness = saturate(SAMPLE_TEXTURE2D(_RoughnessMap,sampler_RoughnessMap,UV).r * _Roughness);
                //get TangentSpace Normal 
                half3 NormalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV),_Normal);
                WorldNormal = normalize(mul(NormalTS,TBN));

                float Occlusion = SAMPLE_TEXTURE2D(_OcclusionMap,sampler_OcclusionMap,UV).r;
                Occlusion = lerp(1.0,Occlusion,_OcclusionStrength);

                float3 DiffuseColor = lerp(BaseMapColor,float3(0.0,0.0,0.0),Metallic);
                float3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseMapColor,Metallic);

                float3 V = ViewDir;
                float3 N = WorldNormal;

                //直接光
                float3 DirectMainLight = float3(0, 0, 0);
                DirectLighting_float(DiffuseColor,Metallic,Roughness,N,V,DirectMainLight);
                //间接光
                float3 IndirectLight = float3(0, 0, 0);
                IndirectLighting_float(DiffuseColor,Metallic,Roughness,Occlusion,N,V,IndirectLight);
                
                return float4(DirectMainLight+IndirectLight,1);
                
            
            }
            ENDHLSL
        }
    }
}
//UNITY_MATRIX_M     replace: unity_ObjectToWorld
//UNITY_MATRIX_I_M   replace: unity_WorldToObject
//UNITY_MATRIX_V     replace: unity_MatrixV
//UNITY_MATRIX_I_V   replace: unity_MatrixInvV
//UNITY_MATRIX_VP    replace: unity_MatrixVP
//UNITY_MSTRIX_I_VP  replace: unity_MatrixInvVP