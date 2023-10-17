

#if !defined(MY_LIGHTING_INPUT_INCLUDED)
#define MY_LIGHTING_INPUT_INCLUDED

#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"


#if !defined(ALBEDO_FUNCTION)
    #define ALBEDO_FUNCTION GetAlbedo
#endif


CBUFFER_START(UnityPerMaterial)

float4 _Color;

float _Gloss;
float _NormalIntensity;

sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _NormalTex;

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
    LIGHTING_COORDS(5, 6)
    
    #if defined (CUSTOM_GEOMETRY_INTERPOLATORS)
        CUSTOM_GEOMETRY_INTERPOLATORS
    #endif
};

/*Interpolators vert(MeshData v)
{
    Interpolators o;
    //o.uv = v.uv0;
    o.uv = TRANSFORM_TEX(v.uv0, _MainTex);

    o.vertex = UnityObjectToClipPos(v.vertex.xyz);
    //o.vertex = v.vertex;
    o.normal = UnityObjectToWorldNormal(v.normals);
    o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
    o.bitangent = cross(o.normal, o.tangent);
    o.bitangent *= v.tangent.w * unity_WorldTransformParams.w; 

    float4 pos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1)); //unity_ObjectToWorld = UNITY_MATRIX_M
    o.worldPos = pos.xyz;

    TRANSFER_VERTEX_TO_FRAGMENT(o); //lighting
    
    return o;
}*/


float3 GetAlbedo(Interpolators i)
{
    return _Color;
}


#endif


/*float4 frag(Interpolators i) : SV_Target
{
    float4 col = ALBEDO_FUNCTION(i);

    /#1#/Flat shading based on screen-space derivatives
    float3 dpdx = ddx(i.worldPos);
    float3 dpdy = ddy(i.worldPos);
    i.normal = normalize(cross(dpdy, dpdx));#1#

    
    #ifdef BASE_PASS

    
    return col/* + _AmbientLight#1#;

    
    #else
    #ifdef USE_LIGHTING

    
    //textures normal
    //float3 normalMap = UnpackNormal(tex2D(_NormalTex,i.uv)) * 0.5 + 0.5; //looks like normal map
    
    float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalTex, i.uv));
    tangentSpaceNormal = lerp(float3(0, 0, 1), tangentSpaceNormal, _NormalIntensity);
    //tangentSpaceNormal = lerp(i.normal, tangentSpaceNormal, _NormalIntensity);

    float3x3 mtxTangToWorld = {
        i.tangent.x, i.bitangent.x, i.normal.x,
        i.tangent.y, i.bitangent.y, i.normal.y,
        i.tangent.z, i.bitangent.z, i.normal.z,
    };
    
    float3 N = mul(mtxTangToWorld, tangentSpaceNormal);
    //float3 N = i.normal;

    //float3 N = tangentSpaceNormal;
    //return float4 (N, 0);


    //Diffuse Lighting
    //float3 N = normalize( i.normal); //normolize normals to fix explicit normal interpolation 
    float3 L = normalize(UnityWorldSpaceLightDir(i.worldPos)); //**#1#/direction light //_WorldSpaceLightPos0.w = 0;
    float attenuation = LIGHT_ATTENUATION(i);//**#1#/attenuation of spot light, use LIGHTING_COORDS(,) value
    //UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz); 
    //attenuation = FadeShadows(i, attenuation);
    
    float3 lambert = saturate(dot(N, L));
    float3 diffuseLight = (lambert * attenuation) * _LightColor0.xyz;

    
    //specular lighting
    float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
    
    //1 specular lighting phong
    //float3 R = reflect(-L, N);
    //2 specular blin-phong
    float3 H = normalize(L + V);
    
    //float3 specularLight = saturate(dot(V, R)); //1
    float3 specularLight = saturate(dot(H, N)) * (lambert > 0); //2
    //specularLight *= lambert > 0;  //2 remove back Lighting

    float specularExponent = exp2(_Gloss * 11) + 2;
    specularLight = pow(specularLight, specularExponent); //specular exponent
    specularLight *= _Gloss; //- uncorrect but energy conservation
    specularLight *= _LightColor0.xyz;
    specularLight *= attenuation;
    
    return float4(diffuseLight * 1 + specularLight, 1);

    
    #else

    #endif
    #endif
    
    
    return float4(0, 0, 0, 0);
}*/
