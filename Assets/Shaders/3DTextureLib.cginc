#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "ShadersLib.cginc"     // custom library with utility functions and values

sampler2D _AlbedoTexture;
sampler2D _NormalMapTexture;
sampler2D _DisplacementMapTexture;
float4    _AlbedoTexture_ST;
float     _Gloss;
float     _NormalMapInfluence;
float     _DisplacementMapInfluence;

struct MeshData
{
    float4 vertex  : POSITION;
    float3 normal  : NORMAL;
    float4 tangent : TANGENT;
    float4 uv      : TEXCOORD0;
};

struct Interpolators
{
    float4 vertex    : SV_POSITION;
    float2 uv        : TEXCOORD0;
    float3 normal    : TEXCOORD1;
    float3 tangent   : TEXCOORD2;
    float3 bitangent : TEXCOORD3;
    float3 worldPos  : TEXCOORD4;
    LIGHTING_COORDS(5, 6)
};

// Vertex Shader function
Interpolators vert(MeshData v)
{
    Interpolators o;

    o.uv = TRANSFORM_TEX(v.uv, _AlbedoTexture);

    // --- Displacement Map ---
    float displacementHeight = tex2Dlod(_DisplacementMapTexture, float4(o.uv, 0, 0)).x;
    displacementHeight = displacementHeight * 2 - 1; // remap displacement from [0,1] to [-1,1]
    v.vertex.xyz += v.normal * displacementHeight * _DisplacementMapInfluence;

    o.vertex = UnityObjectToClipPos(v.vertex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);

    // --- Tangent Space Reference ---
    o.bitangent = cross(o.normal, o.tangent);                    // Bitangent
    o.bitangent *= (v.tangent.w * unity_WorldTransformParams.w); // takes flip/mirror into account

    TRANSFER_VERTEX_TO_FRAGMENT(o)
    return o;
}

// Fragment Shader function
float4 frag(Interpolators i) : SV_Target
{
    // --- Albedo Texture ---
    float3 texturedSurface = tex2D(_AlbedoTexture, i.uv).rgb;

    // --- Normal Map ---
    float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalMapTexture, i.uv));
    tangentSpaceNormal = normalize(lerp(float3(0, 0, 1), tangentSpaceNormal, _NormalMapInfluence));

    // --- Tangent Space Transformation Matrix ---
    float3x3 mtxTangentToWorld = {
        i.tangent.x, i.bitangent.x, i.normal.x,
        i.tangent.y, i.bitangent.y, i.normal.y,
        i.tangent.z, i.bitangent.z, i.normal.z
    };
    // normals from Tangent-Space to World-Space
    float3 normalVector = mul(mtxTangentToWorld, tangentSpaceNormal);


    // --- Lighting ---
    float3 worldPosition = i.worldPos;
    float3 LightVector = normalize(UnityWorldSpaceLightDir(worldPosition));
    float attenuation = LIGHT_ATTENUATION(i);

    // --- Diffuse Lighting ---
    float3 lambertianLight = max(0, dot(normalVector, LightVector));  // Lambertian Light
    float3 diffuseLight = lambertianLight * attenuation * _LightColor0.xyz;

    // --- Specular Lighting ---
    float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition);

    // Phong Lighting
    //float3 reflectVector = reflect(-LightVector, normalVector);
    //float3 specularLight = max(0, dot(reflectVector, viewDirection));

    // Blinn-Phong Lighting
    float3 halfVector = normalize(LightVector + viewDirection);     // Half Vector
    float3 specularLight = max(0, dot(halfVector, normalVector));   // Blinn-Phong Lighting

    specularLight *= (lambertianLight > 0);         // fixes a bug where specular light is visible on the edge of the object's dark side
    float specularExponent = exp2(_Gloss * 6) + 2;  // glossiness remap function into specular exponent
    specularLight = pow(specularLight, specularExponent) * attenuation;     // Specular Exponent and attenuation
    specularLight *= _LightColor0.xyz;

    // --- Albedo Texture ---
    diffuseLight *= texturedSurface;
    //specularLight *= texturedSurface;   // only if we have a metallic object

    // --- Compositing ---
    return float4(diffuseLight + specularLight, 1);

}