Shader "Custom/JellyFishFoot"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Freq("Frequency", float) = 1.0
        _Speed("Speed", float) = 1.0
        _Shift("Shift", float) = 0
        _Power("Power", float) = 1
        _Alpha("Alpha", Range(0,1)) = 0.5
    }
 
    SubShader {
        Tags { 
            "RenderType"="Transpaernt"
            "Queue"="Transparent" 
        }
        Lighting Off
        Cull Off
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade vertex:vert
        #include "UnityCG.cginc"

        struct Input{
            fixed2 uv_MainTex;
        };

        sampler2D _MainTex;
        half _Freq;
        half _Speed;
        half _Shift;
        half _Power;
        half _Alpha;

        void vert (inout appdata_full v) {
            half fx = sin(v.texcoord.y * _Freq - _Speed * _Time.y + _Shift) * v.texcoord.y;
            v.vertex.xyz += _Power * fx * v.normal;
        }

        void surf(Input IN, inout SurfaceOutput o){
            fixed4 col = fixed4(1,1,1,_Alpha);
            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
}
