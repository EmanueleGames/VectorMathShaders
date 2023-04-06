#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "ShadersLib.cginc"     // custom library with utility functions and values

float  _Gloss;
float4 _SurfaceColor;
float4 _FresnelColor;
float  _PulsingSpeed;

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
    LIGHTING_COORDS(3, 4)
};

// Vertex Shader function
Interpolators vert(MeshData v)
{
    Interpolators o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
    TRANSFER_VERTEX_TO_FRAGMENT(o)
    return o;
}

// Fragment Shader function
float4 frag(Interpolators i) : SV_Target
{

    float3 worldPosition = i.worldPos;
    float3 lightVector = normalize(UnityWorldSpaceLightDir(worldPosition));    // light direction
    float  attenuation = LIGHT_ATTENUATION(i);      // takes into account light attenuation
    float3 normalVector = normalize(i.normal);      // to avoid errors due to normals interpolation

    // --- Diffuse Lighting ---
    float3 lambertianLight = max(0, dot(normalVector, lightVector));        // Lambertian Light
    float3 diffuseLight = lambertianLight * attenuation * _LightColor0.xyz;
    
    // --- Specular Lighting ---
    float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition); // world-space direction from camera to fragment
    
    // Phong Lighting
    //float3 reflectVector = reflect(-LightVector, normalVector);
    //float3 specularLight = max(0, dot(reflectVector, viewDirection));
    
    // Blinn-Phong Lighting
    float3 halfVector = normalize(lightVector + viewDirection);     // Half Vector
    float3 specularLight = max(0, dot(halfVector, normalVector));   // Blinn-Phong Light
    
    specularLight *= (lambertianLight > 0);         // fixes a bug where specular light is visible on the edge of the object's dark side
    float specularExponent = exp2(_Gloss * 6) + 2;  // glossiness remap function into specular exponent
    specularLight = pow(specularLight, specularExponent) * attenuation;     // Specular Exponent and attenuation
    specularLight *= _LightColor0.xyz;
    
    // --- Material Surface Color ---
    diffuseLight *= _SurfaceColor;
    //specularLight *= _SurfaceColor;   // only if we have a metallic object
    
    // --- Fresnel ---
    float3 fresnel = dot(viewDirection, normalVector);
    float3 pulsingGlow = (1 - fresnel) * (0.1 * cos(_Time.y * _PulsingSpeed * PI) + 0.6);

    // --- Compositing ---
    #ifdef IS_IN_FRESNEL_PASS
        return float4(pulsingGlow * _FresnelColor, 1);
    #else
        return float4(diffuseLight + specularLight, 1);
    #endif
}