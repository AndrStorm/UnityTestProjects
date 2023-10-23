#if !defined(MY_LIGHTING_INPUT_INCLUDED)
#define MY_LIGHTING_INPUT_INCLUDED

#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"

#define _DisplacementMap _ParallaxMap
#define _DisplacementStrength _ParallaxStrength


CBUFFER_START(UnityPerMaterial)

float4 _Color;

float _Gloss;
float _NormalIntensity;

sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _NormalTex;

sampler2D _ParallaxMap;
float _ParallaxStrength;

CBUFFER_END

float InverseLerp(float a, float b, float v)
{
    return (v - a) / (b - a);
}

struct MeshData
{
    float4 vertex : POSITION;
    float3 normals : NORMAL; //local space normal direction
    float2 uv0 : TEXCOORD0; //uv0 diffuse/normal map texture
    float4 tangent : TANGENT; // tangent dirrection = (xyz) tangent sign = (w)
};

struct InterpolatorsVertex
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal :TEXCOORD1;
    float3 worldPos :TEXCOORD2;
    
    #if defined(BINORMAL_PER_FRAGMENT)
    float4 tangent: TEXCOORD3;
    #else
    float3 tangent: TEXCOORD3;
    float3 bitangent: TEXCOORD4;
    #endif
    
    LIGHTING_COORDS(5, 6)
};

struct Interpolators
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal :TEXCOORD1;
    float3 worldPos :TEXCOORD2;
    
    #if defined(BINORMAL_PER_FRAGMENT)
    float4 tangent: TEXCOORD3;
    #else
    float3 tangent: TEXCOORD3;
    float3 bitangent: TEXCOORD4;
    #endif
    
    LIGHTING_COORDS(5, 6)
    
    #if defined (CUSTOM_GEOMETRY_INTERPOLATORS)
        CUSTOM_GEOMETRY_INTERPOLATORS
    #endif
};

float3 GetAlbedo(Interpolators i)
{
    return _Color;
}

#endif
