Shader "Unlit/AmbientLightShader"
{
    Properties  // Input Data
    {
        _AlbedoTexture("Albedo Texture", 2D) = "white" {}
        [NoScaleOffset] _NormalMapTexture("Normal Map", 2D) = "bump" {}
        [NoScaleOffset] _DisplacementMapTexture("Displacement Map", 2D) = "grey" {}
        _NormalMapInfluence("Normal Map Influence", Range(0,1)) = 1
        _DisplacementMapInfluence("Displacement Map Influence", Range(0,0.1)) = 0.02

        _DiffuseIBLTexture("Diffuse IBL", 2D) = "black" {}
        _SpecularIBLTexture("Specular IBL", 2D) = "black" {}
        _DiffuseIBLIntensity("Diffuse IBL Intensity", Range(0.2,0.8)) = 0.5
        _SpecularIBLIntensity("Specular IBL Intensity", Range(0.2,0.8)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
    
        Pass // Base Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
    
            #pragma vertex vert
            #pragma fragment frag
    
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
            
                // --- Albedo Texture ---
                float3 diffuseLight = texturedSurface * 2;

                // --- IBL (Image Based Lighting) ---

                // Diffuse IBL
                // Skybox texture sampling (using object's Normal Map)
                float3 diffuseIBL = tex2Dlod(_DiffuseIBLTexture, float4(DirToRectilinear(normalVector), 0, 0)).xyz;
                diffuseLight *= diffuseIBL *_DiffuseIBLIntensity;
            
                // Specular IBL
                float3 worldPosition = i.worldPos;
                float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition);
                float3 viewReflectionDir = reflect(-viewDirection, normalVector);   // ViewReflected Vector
                // Skybox texture sampling (using ViewReflected Direction)
                float3 specularIBL = tex2Dlod(_SpecularIBLTexture, float4(DirToRectilinear(viewReflectionDir), 0, 0)).xyz;
                float3 specularLight = specularIBL * _SpecularIBLIntensity;

                // --- Compositing ---
                return float4(diffuseLight + specularLight, 1);
            }
            ENDCG
        }
    }
}