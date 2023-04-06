Shader "Unlit/MultiPassShader"
{
    Properties
    {
        _Gloss("Glossiness", Range(0,1)) = 0.5
        _SurfaceColor("Mat Surface Color", Color) = (1,1,1,1)
        _FresnelColor("Hidden Object Color", Color) = (1,1,1,1)
        _PulsingSpeed("Pulsing Speed", Range(0,6)) = 1
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque"}

        Pass // Base Fresnel Pass
        {
            Tags { "LightMode" = "ForwardBase" }  // Base Pass tag

            ZWrite Off
            ZTest GEqual
            Blend One One

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define IS_IN_FRESNEL_PASS
            #include "MultiPassLib.cginc"   // custom include for Fresnel Effect
            ENDCG
        }

        Pass // Base Pass
        {
            Tags { "LightMode" = "ForwardBase" }  // Base Pass tag

            ZTest LEqual

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define IS_IN_BASE_PASS
            #include "MultiPassLib.cginc"   // custom include for MultiPass
            ENDCG
        }

        Pass // Add Pass
        {
            Tags { "LightMode" = "ForwardAdd" }  // Add Pass tag

            Blend One One  // additive blending

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #define IS_IN_ADD_PASS
            #include "MultiPassLib.cginc"   // custom include for MultiPass
            ENDCG
        }
    }
}