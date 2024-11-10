Shader "Custom/Fish"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _Frequency("Frequency", Float) = 1.0
        _Speed("Speed", Float) = 1.0
        _Shift("Shift", Float) = 0.0
        _Steepness("Steepness", Float) = 10.0
        _Center("Center", Float) = 0.75
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _ALPHATEST_ON
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Cutoff;
            float _Frequency;
            float _Speed;
            float _Shift;
            float _Steepness;
            float _Center;

            v2f vert (appdata v)
            {
                v2f o;
                float3 pos = v.vertex;
                pos.z += sin(_Frequency * v.uv.x + (_Shift + _Time.y) * _Speed);
                float logisticValue = 1.0 / (1.0 + exp(-_Steepness * (v.uv.x - _Center)));
                pos.z *= logisticValue * 0.7;
                v.vertex.xyz = pos;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
