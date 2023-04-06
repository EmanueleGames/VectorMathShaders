Shader "Unlit/3DTextureShader"
{
    Properties
    {
        _AlbedoTexture("Albedo Texture", 2D) = "white" {}
        [NoScaleOffset] _NormalMapTexture("Normal Map", 2D) = "bump" {}
        [NoScaleOffset] _DisplacementMapTexture("Displacement Map", 2D) = "grey" {}
        _Gloss("Glossiness", Range(0,1)) = 0.5
        _NormalMapInfluence("Normal Map Influence", Range(0,1)) = 1
        _DisplacementMapInfluence("Displacement Map Influence", Range(0,0.1)) = 0.02
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
    
            #define IS_IN_BASE_PASS
    
            #include "3DTextureLib.cginc"    // custom include
            ENDCG
        }
        
        Pass  // Add Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
        
            Blend One One
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
        
            #include "3DTextureLib.cginc"    // custom include
            ENDCG
        }
    }
}