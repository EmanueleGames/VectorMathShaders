Shader "Unlit/VertexWaveShader"
{
    Properties  // Input Data
    {
        _Amplitude("Wave Amplitude", Range(0,0.04)) = 0.01
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

                // waves along the axes
                float waveY = _Amplitude * cos((v.uv0.y - _Time.y * 0.1) * 8 * PI);
                float waveX = _Amplitude * cos((v.uv0.x - _Time.y * 0.05) * 4 * PI);
                v.vertex.y = waveY + waveX;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv0;
                return o;
            }

            // Fragment Shader function
            fixed4 frag(Interpolators i) : SV_Target
            {
                // Waves color setup
                float4 outputColor = lerp(_ColorA, _ColorB, i.uv.x);
                return outputColor;
            }
        ENDCG
        }
    }
}