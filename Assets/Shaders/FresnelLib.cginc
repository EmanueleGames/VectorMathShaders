#include "UnityCG.cginc"
#include "ShadersLib.cginc"     // custom library with utility functions and values

float4 _FresnelColor;

struct MeshData
{
    float4 vertex : POSITION;
    float4 uv     : TEXCOORD0;
    float3 normal : NORMAL;
};

struct Interpolators
{
    float4 vertex   : SV_POSITION;
    float2 uv       : TEXCOORD0;
    float3 normal   : TEXCOORD1;
    float3 worldPos : TEXCOORD2;
};

// Vertex Shader function
Interpolators vert(MeshData v)
{
    Interpolators o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
    return o;
}

// Fragment Shader function
float4 frag(Interpolators i) : SV_Target
{
    float3 normalVector = normalize(i.normal);
    float3 worldPosition = i.worldPos;
    float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition);

    // Fresnel
    float3 fresnel = dot(viewDirection, normalVector);
    // Pulsing based on Fresnel
    float3 pulsingGlow = (1 - fresnel) * (0.1 * cos(_Time.y * 1.5 * PI) + 0.6);

    return float4(pulsingGlow * _FresnelColor, .8);
}