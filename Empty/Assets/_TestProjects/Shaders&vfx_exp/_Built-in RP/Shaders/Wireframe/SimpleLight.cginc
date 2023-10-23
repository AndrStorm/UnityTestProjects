#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

#include "SimpleLightInput.cginc"

#if !defined(ALBEDO_FUNCTION)
    #define ALBEDO_FUNCTION GetAlbedo
#endif


float3 CreateBinormal (float3 normal, float3 tangent, float binormalSign) {
    return cross(normal, tangent.xyz) *
        (binormalSign * unity_WorldTransformParams.w);
}

InterpolatorsVertex vert(MeshData v)
{
    InterpolatorsVertex o;
    //o.uv = v.uv0;
    o.uv = TRANSFORM_TEX(v.uv0, _MainTex);

    float height = tex2Dlod(_DisplacementMap, float4(o.uv, 0, 0)).x * 2 - 1;
    v.vertex.xyz += v.normals * height * _DisplacementStrength;
    
    o.vertex = UnityObjectToClipPos(v.vertex.xyz);
    //o.vertex = v.vertex;
    o.normal = UnityObjectToWorldNormal(v.normals);

    #if defined(BINORMAL_PER_FRAGMENT)
    o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
    #else
    o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
    o.bitangent = CreateBinormal(o.normal, o.tangent, v.tangent.w);
    #endif
    
    //o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
    /*o.bitangent = cross(o.normal, o.tangent);
    o.bitangent *= v.tangent.w * unity_WorldTransformParams.w;*/

    float4 pos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1)); //unity_ObjectToWorld = UNITY_MATRIX_M
    o.worldPos = pos.xyz;

    TRANSFER_VERTEX_TO_FRAGMENT(o); //lighting
    
    return o;
}

void InitializeFragmentNormal(inout Interpolators i)
{
    float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalTex, i.uv));
    tangentSpaceNormal = lerp(float3(0, 0, 1), tangentSpaceNormal, _NormalIntensity);

    #if defined(BINORMAL_PER_FRAGMENT)
    float3 bitangent =
        CreateBinormal(i.normal, i.tangent.xyz, i.tangent.w);
    #else
    float3 bitangent = i.bitangent;
    #endif
	
    /*i.normal = normalize(
        tangentSpaceNormal.x * i.tangent +
        tangentSpaceNormal.y * bitangent +
        tangentSpaceNormal.z * i.normal
    );*/

    float3x3 mtxTangToWorld = {
        i.tangent.x, bitangent.x, i.normal.x,
        i.tangent.y, bitangent.y, i.normal.y,
        i.tangent.z, bitangent.z, i.normal.z,
    };

    i.normal = mul(mtxTangToWorld, tangentSpaceNormal);
}

float4 frag(Interpolators i) : SV_Target
{
    float4 col = float4(ALBEDO_FUNCTION(i), 1);

    /*//Flat shading based on screen-space derivatives
    float3 dpdx = ddx(i.worldPos);
    float3 dpdy = ddy(i.worldPos);
    i.normal = normalize(cross(dpdy, dpdx));*/

    
    #ifdef BASE_PASS
    return col/* + _AmbientLight*/;

    
    #else
    #ifdef USE_LIGHTING

    
    //textures normal
    //float3 normalMap = UnpackNormal(tex2D(_NormalTex,i.uv)) * 0.5 + 0.5; //looks like normal map

    InitializeFragmentNormal(i);
    float3 N = i.normal;
    

    //Diffuse Lighting
    //float3 N = normalize( i.normal); //normolize normals to fix explicit normal interpolation 
    float3 L = normalize(UnityWorldSpaceLightDir(i.worldPos)); //***//direction light //_WorldSpaceLightPos0.w = 0;
    float attenuation = LIGHT_ATTENUATION(i);//***//attenuation of spot light, use LIGHTING_COORDS(,) value
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
    
    return float4(diffuseLight * 0.2 + specularLight, 1);

    
    #else
    #endif
    #endif
    
    return float4(0, 0, 0, 0);
}

#endif
