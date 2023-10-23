#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


// This macro declares _BaseMap as a Texture2D object.  
TEXTURE2D(_MainTex);
// This macro declares the sampler for the _BaseMap texture.
SAMPLER(sampler_MainTex);

CBUFFER_START(UnityPerMaterial)

float4 _Color;
float4 _MainTex_ST;

CBUFFER_END


struct MeshData
{
    float4 vertex : POSITION;
    float3 normals : NORMAL; //local space normal direction
    float2 uv0 : TEXCOORD0; //uv0 diffuse/normal map texture
    float4 tangent : TANGENT; // tangent dirrection = (xyz) tangent sign = (w)
};

struct Interpolators
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal :TEXCOORD1;
    float3 worldPos :TEXCOORD2;
    float3 tangent: TEXCOORD3;
    float3 bitangent: TEXCOORD4;
};

Interpolators vert(MeshData v)
{
    Interpolators o;
    //o.uv = v.uv0;
    o.uv = TRANSFORM_TEX(v.uv0, _MainTex);

    o.vertex = TransformObjectToHClip(v.vertex.xyz);
    //o.vertex = v.vertex;
    o.normal = TransformObjectToWorldNormal(v.normals);
    o.tangent = TransformObjectToWorldDir(v.tangent.xyz);
    o.bitangent = cross(o.normal, o.tangent);
    o.bitangent *= v.tangent.w * unity_WorldTransformParams.w; //correctly handle flipping and
    // mirroring when minus scaling in unity

    float4 pos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1)); //unity_ObjectToWorld = UNITY_MATRIX_M
    o.worldPos = pos.xyz;

    return o;
}

float4 frag(Interpolators i) : SV_Target
{
    float4 col = _Color;

    return col;
}
