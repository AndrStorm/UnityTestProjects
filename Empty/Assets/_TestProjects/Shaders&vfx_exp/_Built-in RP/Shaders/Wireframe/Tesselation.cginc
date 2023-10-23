// Upgrade NOTE: replaced 'defined float3' with 'defined (float3)'

#if !defined(TESSELLATION_INCLUDED)
#define TESSELLATION_INCLUDED

#include "SimpleLightInput.cginc"

float _TesselationUniform;
float _TessellationEdgeLength;
float _FrustumBias;

struct TessellationControlPoint
{
    float4 vertex : INTERNALTESSPOS;
    float3 normals : NORMAL;
    float4 tangent : TANGENT;
    float2 uv0 : TEXCOORD0;
    //float2 uv1 : TEXCOORD1;
    //float2 uv2 : TEXCOORD2;
};

struct TessellationFactors
{
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};


bool TriangleIsBelowClipPlane (float3 p0, float3 p1, float3 p2, int planeIndex, float bias) {
    float4 plane = unity_CameraWorldClipPlanes[planeIndex];
    return
        dot(float4(p0, 1), plane) < bias &&
        dot(float4(p1, 1), plane) < bias &&
        dot(float4(p2, 1), plane) < bias;
}

bool TriangleIsCulled (float3 p0, float3 p1, float3 p2, float bias) {
    return TriangleIsBelowClipPlane(p0, p1, p2, 0, bias) ||
        TriangleIsBelowClipPlane(p0, p1, p2, 1, bias) ||
        TriangleIsBelowClipPlane(p0, p1, p2, 2, bias) ||
        TriangleIsBelowClipPlane(p0, p1, p2, 3, bias);
}

TessellationControlPoint MyTessellationVertexProgram (MeshData v)
{
    TessellationControlPoint p;
    p.vertex = v.vertex;
    p.normals = v.normals;
    p.tangent = v.tangent;
    p.uv0 = v.uv0;
    //p.uv1 = v.uv1;
    //p.uv2 = v.uv2;
    return p;
}

float TessellationEdgeFactor(float3 p1, float3 p2)
{
    #if defined(_FACTOR_MODE_EDGE)
        float edgeLength = distance(p1, p2);

        float3 edgeCenter = (p1 + p2) * 0.5;
        float viewDistance = distance(edgeCenter, _WorldSpaceCameraPos);
        //viewDistance = InverseLerp(0, 50, viewDistance) * 50 + 1;
    
        return edgeLength * _ScreenParams.y / (_TessellationEdgeLength  * viewDistance );
    #else
        return _TesselationUniform;
    #endif
}

TessellationFactors MyPatchConstantFunction(
    InputPatch<TessellationControlPoint, 3> patch)
{
    float3 p0 = mul(unity_ObjectToWorld, float4(patch[0].vertex.xyz,1)).xyz;
    float3 p1 = mul(unity_ObjectToWorld, float4(patch[1].vertex.xyz,1)).xyz;
    float3 p2 = mul(unity_ObjectToWorld, float4(patch[2].vertex.xyz,1)).xyz;
    
    TessellationFactors f;
    float bias = _FrustumBias;
    if (TriangleIsCulled(p0, p1, p2, bias)) {
        f.edge[0] = f.edge[1] = f.edge[2] = f.inside = 0;
    }
    else
    {
        f.edge[0] = TessellationEdgeFactor(p1, p2);
        f.edge[1] = TessellationEdgeFactor(p2, p0);
        f.edge[2] = TessellationEdgeFactor(p0, p1);
    
        //f.inside = (f.edge[0] + f.edge[1] + f.edge[2]) * (1 / 3.0);
        f.inside =
            (TessellationEdgeFactor(p1, p2) +
            TessellationEdgeFactor(p2, p0) +
            TessellationEdgeFactor(p0, p1)) * (1 / 3.0);
    }
    
    return f;
}


[UNITY_domain("tri")]
[UNITY_outputcontrolpoints(3)]
[UNITY_outputtopology("triangle_cw")]
[UNITY_partitioning("integer")]
[UNITY_patchconstantfunc("MyPatchConstantFunction")]
TessellationControlPoint MyHullProgram(
    InputPatch<TessellationControlPoint, 3> patch,
    uint id : SV_OutputControlPointID)
{
    return patch[id];
}


[UNITY_domain("tri")]
InterpolatorsVertex MyDomainProgram(
    TessellationFactors factors,
    OutputPatch<TessellationControlPoint, 3> patch,
    float3 barycentricCoordinates : SV_DomainLocation)
{
    MeshData data;

    #define MY_DOMAIN_PROGRAM_INTERPOLATE(fieldName) data.fieldName = \
    patch[0].fieldName * barycentricCoordinates.x + \
    patch[1].fieldName * barycentricCoordinates.y + \
    patch[2].fieldName * barycentricCoordinates.z;

    MY_DOMAIN_PROGRAM_INTERPOLATE(vertex)
    MY_DOMAIN_PROGRAM_INTERPOLATE(normals)
    MY_DOMAIN_PROGRAM_INTERPOLATE(tangent)
    MY_DOMAIN_PROGRAM_INTERPOLATE(uv0)
    /*MY_DOMAIN_PROGRAM_INTERPOLATE(uv1)
    MY_DOMAIN_PROGRAM_INTERPOLATE(uv2)*/

    return vert(data);
}

#endif