#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "ShadersLib.cginc"     // custom library with utility functions and values

sampler2D _AlbedoTexture;
sampler2D _NormalMapTexture;
sampler2D _DisplacementMapTexture;
float4    _AlbedoTexture_ST;
float     _NormalMapInfluence;
float     _DisplacementMapInfluence;
sampler2D _DiffuseIBLTexture;
sampler2D _SpecularIBLTexture;
float     _DiffuseIBLIntensity;
float     _SpecularIBLIntensity;
float     _Gloss;

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

    /*
    // --- Lighting ---
    float3 worldPosition = i.worldPos;
    float3 LightVector = normalize(UnityWorldSpaceLightDir(worldPosition));
    float attenuation = LIGHT_ATTENUATION(i);


    // --- Diffuse Lighting ---
    float3 lambertianLight = max(0, dot(normalVector, LightVector));  // Lambertian Light
    // Per un risultato più realistico dobbiamo tenere in considerazione i colori: sia quello dell'oggetto, sia quello della luce
    // La variabile _LightColor0 legge la proprietà di colore della directional light (nell'inspector)
    float3 diffuseLight = lambertianLight * attenuation * _LightColor0.xyz;   // Colore della luce + attenuazione


    // --- Specular Lighting ---
    // Per prima cosa ci serve il vettore tra il fragment corrente e la posizione della camera
    // NB. Il vettore forward della camera in questo caso andrebbe bene solo con una camera ortografica, ma non con quella prospettica
    // Quindi lo ricaveremo come differenza tra posizione del pixel e posizione della camera nella variabile _WorldSpaceCameraPos
    float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition);

    // Phong Lighting
    // Per calcolare il vettore di luce riflesso o possiamo sottrarre al vettore -L la sua componente normale 2 volte (visto in altro corso)
    // oppure gli shader hanno una funzione built-in che possiamo sfruttare:
    //float3 reflectVector = reflect(-LightVector, normalVector);        // vettore Luce Riflessa
    //float3 specularLight = max(0, dot(reflectVector, viewDirection));  // Phong Lighting

    // Blinn-Phong Lighting
    // Ci serve il vettore Half
    float3 halfVector = normalize(LightVector + viewDirection);     // Half Vector
    float3 specularLight = max(0, dot(halfVector, normalVector));   // Blinn-Phong Lighting


    // Correzione bug della luce speculare che penetra puntiforme alle spalle dell'oggetto
    specularLight *= (lambertianLight > 0);        // risolto moltiplicando la specular light per la condizione sul Lambertian
    // Rimappiamo la Glossiness
    float specularExponent = exp2(_Gloss * 6) + 2;
    // Applichiamola alla nostra specular light
    specularLight = pow(specularLight, specularExponent) * attenuation;    // Specular Exponent and attenuation
    // E infine il colore settato nell'inspector
    specularLight *= _LightColor0.xyz;   // Colore della luce
    */
    float3 diffuseLight, specularLight;
    // --- Ambient Lighting ---
    #ifdef IS_IN_BASE_PASS  // base pass
        // Ambient Light Metodo - Diffuse IBL
        // campioniamo la texture dello Skybox secondo le normali della Normal Map (non quelle default dei vertici!!)
        // (opportunamnte convertite con la funzione già vista nello shader dello skybox)
        float3 diffuseIBL = tex2Dlod(_DiffuseIBLTexture, float4(DirToRectilinear(normalVector), 0, 0)).xyz;
        diffuseLight += diffuseIBL * _DiffuseIBLIntensity;

        // Ambient Light Metodo 2 - Specular IBL
        // calcoliamo la direzione ViewReflected (in base alla normale della Normal Map)
        float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition);
        float3 viewReflectionDir = reflect(-viewDirection, normalVector);   // viewDirection va negato perchè reflect vuole il vettore entrante
        // settiamo il livello di MIP con la glossiness
        // ricorda che 0 è il livello più alto di MIP, quello a miglior risoluzione
        // e poi al crescere degli interi (1,2,3...), la risoluzione diminuisce pian piano
        float mip_level = (1 - _Gloss) * 6; // stiamo supponendo di avere 6 mip levels
        // e calcoliamo la componente speculare con il la direzione riflessa, e il livello MIP impostato
        float3 specularIBL = tex2Dlod(_SpecularIBLTexture, float4(DirToRectilinear(viewReflectionDir), mip_level, mip_level)).xyz;
        specularLight += specularIBL * _SpecularIBLIntensity;
    #endif

    // --- Albedo Texture ---
    diffuseLight *= texturedSurface;
    //specularLight *= texturedSurface;   // only if we have a metallic object

    return float4(diffuseLight + specularLight, 1);

}