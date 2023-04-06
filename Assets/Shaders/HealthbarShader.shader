Shader "Unlit/HealthbarMat"
{
    Properties
    {
        _Health("HP %", Range(0,1)) = 0.5
        _Border("Border Thickness", Range(0,0.4)) = 0.1
        _BorderColor("Border Color", Color) = (0.1,0.1,0.1,1)
        _BackgroundColor("Background Color", Color) = (0.2,0.2,0.2,1)
        _Antialiasing("Anti Aliasing", Range(0,1)) = 0
        [NoScaleOffset] _MainTex("HealthBar Texture", 2D) = "black" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent"  "Queue" = "Transparent" }

        Pass
        {
            ZWrite Off                          // the shader won't write to the Depth Buffer
            Blend SrcAlpha OneMinusSrcAlpha     // alpha blending

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ShadersLib.cginc"   // custom library with utility functions and values

            float  _Health;
            float  _Border;
            float4 _BorderColor;
            float4 _BackgroundColor;
            float  _Antialiasing;
            sampler2D _MainTex;

            struct MeshData
            {
                float4 vertex : POSITION;
                float4 uv     : TEXCOORD0;
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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment Shader function
            fixed4 frag(Interpolators i) : SV_Target
            {
                // --- Round shape ---

                // Transormation from UV coordinates to a Signed Distance Field
                float2 newCoords = i.uv;             
                newCoords.x *= 8;  // because the Y is scaled to 0.125

                // we need, for each fragment, to calculate the closest point on line around witch we will set up our SDF
                float2 closestPointOnLine = float2(clamp(newCoords.x, 0.5, 7.5), 0.5);

                // SDF definition: distance from the current fragment and the closest point on the line
                float sdf = distance(newCoords, closestPointOnLine);
                sdf = sdf * 2;          // normalization ( distance belongs to [-1,1] instead of [-0.5,0.5] )
                sdf = sdf - 1;          // offset (to set the threshold value to 0 instead of 1)

                // rounded mask
                float roundedPD = fwidth(sdf);          // Antialiasing: screen-space partial derivatives
                float roundedMask = (1 - _Antialiasing) * (step(0, -sdf)) + _Antialiasing * (1 - saturate(sdf / roundedPD));

                // --- Border mask ---
                float colorSdf = sdf + _Border;
                float colorPD = fwidth(colorSdf);       // Antialiasing: screen-space partial derivatives
                float colorMask = (1 - _Antialiasing) * (step(0, -colorSdf)) + _Antialiasing * (1 - saturate(colorSdf / colorPD));

                // --- Color mask ---
                float borderMask = roundedMask - colorMask;

                // --- Texture Tampling ---
                float2 colorPickXY = float2(_Health.x, i.uv.y);
                float4 textureCol = tex2D(_MainTex, colorPickXY);

                // --- Pulsing ---
                float pulsing = 0.1 * cos(_Time.y * 4 * PI) + 0.8;

                // --- Putting all together  ---
                if (i.uv.x > _Health)
                    textureCol = _BackgroundColor;
                else {
                    if (_Health < 0.2)
                        textureCol *= pulsing;
                }
                return float4 (colorMask * textureCol + borderMask * _BorderColor);
            }
        ENDCG
        }
    }
}

