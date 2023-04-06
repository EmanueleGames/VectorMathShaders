Shader "Unlit/AdditiveBlendingShader"
{
    Properties
    {
        _ColorA("Colore A", Color) = (0,0,0,1)
        _ColorB("Colore B", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent"
               "Queue" = "Transparent"
        }

        Pass
        {
            Blend One One  // additive blending
            ZWrite Off     // the shader won't write to the Depth Buffer
            Cull Off       // no culling

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ShadersLib.cginc"   // custom library with utility functions and values

            float4 _ColorA;
            float4 _ColorB;

            struct MeshData
            {
                float4 vertex   : POSITION;
                float4 uv0      : TEXCOORD0;
                float3 normal   : NORMAL;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            // Vertex Shader function
            Interpolators vert(MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);  // passiamo le normali in world-space al Fragment Shader
                o.uv = v.uv0;                                   // semplice passaggio dati 
                return o;
            }

            // Fragment Shader function
            fixed4 frag(Interpolators i) : SV_Target
            {
                // setting animation waves parameters
                float offsetX = 0.02 * cos(i.uv.x * 20 * PI);
                float t = 0.5 * cos((i.uv.y + offsetX - _Time.y * 0.1) * 10 * PI) + 0.5;

                // setting transparency
                float trasparencySetting = (1 - i.uv.y);
                t = t * trasparencySetting;

                // remove cylinder top/bottom
                float topBottomRemover = (abs(i.normal.y) < 0.999);
                float waves = t * topBottomRemover;

                // set the color
                float4 outputColor = lerp(_ColorA, _ColorB, i.uv.y);

                return waves * outputColor;
            }
        ENDCG
        }
    }
}