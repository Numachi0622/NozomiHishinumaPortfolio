Shader "Custom/JellyFishSkirt"
{
    Properties {
        _MainTex("MainTex", 2D) = "white"{}
        _Freq("Frequency", float) = 1.0
        _Speed("Speed", float) = 1.0
        _Shift("Shift", float) = 0
        _Power("Power", float) = 1
        _Alpha("Alpha", Range(0,1)) = 0.5 
    }
 
    SubShader {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        Lighting Off
        Cull Off
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade vertex:vert
        #include "UnityCG.cginc"

        struct Input{
            fixed2 uv_MainTex;
            fixed3 worldPos;
        };

        sampler2D _MainTex;
        half _Freq;
        half _Speed;
        half _Shift;
        half _Power;
        half _Alpha;

        float rand(float2 co)
        {
            return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
        }

        float2 random2(float2 st)
        {
            st = float2(dot(st, float2(127.1, 311.7)), dot(st, float2(269.5, 183.3)));
            return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
        }

        void vert (inout appdata_full v) {
            float2 p = (1 - v.texcoord) * 2 - 1;
            float d = length(p);
            half fx = sin(d * _Freq - _Speed * _Time.y + _Shift) * d;
            v.vertex.xyz -= _Power * fx * v.normal;
        }

        void surf(Input IN, inout SurfaceOutput o){
            fixed4 col = fixed4(1,1,1,_Alpha);
            o.Albedo = col.xyz;
            o.Alpha = col.a;
        }
        ENDCG
    }
}
