Shader "Custom/Wall"
{
    Properties{
        _Alpha("Alpha", Range(0,1)) = 0.5
        _Color("Color",Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #include "UnityCG.cginc"

            fixed _Alpha;
            fixed4 _Color;

            struct appdata{
                float4 vertex : POSITION;
            };

            struct v2f{
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(_Color.rbg,_Alpha);
            }
            ENDCG
        }
    }
}
