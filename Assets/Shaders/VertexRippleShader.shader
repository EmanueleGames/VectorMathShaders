Shader "Unlit/VertexRippleShader"
{
    Properties
    {
        _Amplitude("Wave Height", Range(0,0.04)) = 0.01
        _ColorA("Color A", Color) = (0,0,0,1)
        _ColorB("Color B", Color) = (1,1,1,1)
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ShadersLib.cginc"   // custom library with utility functions and values

            float _Amplitude;
            float4 _ColorA;
            float4 _ColorB;

            struct MeshData
            {
                float4 vertex   : POSITION;
                float4 uv0      : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            // Vertex Shader function
            Interpolators vert(MeshData v)
            {
                Interpolators o;

                // ripple effect
                float2 uvCenteredZero = v.uv0 * 2 - 1;          // lets define a radial UV system reference centered of the quad
                float radialDistance = length(uvCenteredZero);  // get the radial distance
                float4 radialWaves = _Amplitude * cos((radialDistance - _Time.y * 0.1) * 6 * PI);
                v.vertex.y += radialWaves;                      // add the offset to the static mesh

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv0;
                return o;
            }

            // Fragment Shader function
            fixed4 frag(Interpolators i) : SV_Target
            {
                // we get the radial distance again to sync the colors with the ripple
                float2 uvCenteredZero = i.uv * 2 - 1;
                float radialDistance = length(uvCenteredZero);
                float4 radialWaves = cos((radialDistance - _Time.y * 0.1) * 6 * PI);

                // waves color setup
                float4 outputColor = lerp(_ColorA, _ColorB, radialWaves);
                return outputColor;
            }
        ENDCG
        }
    }
}