Shader "Unlit/SkyboxShader"
{
    Properties
    {
        _SkyboxTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "ShadersLib.cginc"     // custom library with utility functions and values

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            sampler2D _SkyboxTex;
            float4 _SkyboxTex_ST;

            Interpolators vert(MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 frag(Interpolators i) : SV_Target
            {
                float3 col = tex2Dlod(_SkyboxTex, float4(DirToRectilinear(i.uv),0,0));
                return col;
            }
            ENDCG
        }
    }
}
