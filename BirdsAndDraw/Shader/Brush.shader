Shader "Custom/Brush"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Float) = 1.0
        _LineDensity("LineDensity", Float) = 30.0
        _NoiseStrength("NoiseStrength", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float _Alpha;
            float _LineDensity;
            float _NoiseStrength;

            float horizontalNoise(float y)
            {
                float l = sin(y * _LineDensity) * 0.5 + 0.5;
                float noise = frac(sin(y * 2345.678 + 123.456) * 4567.89);
                return lerp(l, noise, _NoiseStrength);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = horizontalNoise(i.uv.y);
                if(noise > 0.75) discard;
                else
                {
                    noise = (float)_Color;
                }
                return fixed4((fixed3)noise, _Alpha);
            }
            ENDCG
        }
    }
}
