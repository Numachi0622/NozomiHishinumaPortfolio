Shader "Custom/BoidsRender"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _AnimTex("AnimTex", 2D) = "white"{}
        _AnimLength("AnimLength", Float) = 1.0
        _DeltaTime("DeltaTime", Float) = 0.0
        _Size("Size", Vector) = (1,1,1)
        [Toggle(ANIM_LOOP)] _Loop("Loop", Int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile ___ ANIM_LOOP
            #include "UnityCG.cginc"

            #define ts _AnimTex_TexelSize

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vs_out
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            // Boidの構造体
            struct BoidData
            {
                float3 position;
                float3 velocity;
            };

            StructuredBuffer<BoidData> _BoidDataBuffer;

            sampler2D _AnimTex;
            float4 _AnimTex_TexelSize;
            float3 _Size;
            fixed4 _Color;
            float _AnimLength, _DeltaTime;

            // オイラー角（ラジアン）を回転行列に変換
            float4x4 eulerAnglesToRotationMatrix(float3 angles)
            {
                float ch = cos(angles.y);
                float sh = sin(angles.y);
                float ca = cos(angles.z);
                float sa = sin(angles.z);
                float cb = cos(angles.x);
                float sb = sin(angles.x);

                return float4x4(
                    ch * ca + sh * sb * sa, -ch * sa + sh * sb * ca, sh * cb, 0,
                    cb * sa, cb * ca, -sb, 0,
                    -sh * ca + ch * sb * sa, sh * sa + ch * sb * ca, ch * cb, 0,
                    0, 0, 0, 1
                );
            }

            // vertex shader
            vs_out vert(appdata v, uint iID : SV_InstanceID, uint vId : SV_VertexID)
            {
                vs_out o;

                // Boidデータ取得
                BoidData boid = _BoidDataBuffer[iID];
                float3 pos = boid.position.xyz;
                float3 size = _Size;

                // アニメーション時間計算
                float time = ((_Time.y + sin(iID) * 0.25 + 0.25) - _DeltaTime) / _AnimLength;
                #if ANIM_LOOP
                time = fmod(time, 1.0);
                #else
                time = saturate(time);
                #endif

                // アニメーションの位置を取得
                float x = (vId + 0.5) * ts.x;
                float y = time;
                float4 animatedPos = tex2Dlod(_AnimTex, float4(x, y, 0, 0));

                // オブジェクト座標からワールド座標に変換する行列
                float4x4 objectToWorld = (float4x4)0;
                objectToWorld._11_22_33_44 = float4(size.xyz, 1.0);

                // 回転計算
                float rotY = atan2(boid.velocity.x, boid.velocity.z);
                float rotX = -asin(boid.velocity.y / (length(boid.velocity.xyz) + 1e-8));
                rotX -= UNITY_PI / 3.0;
                float4x4 rotMatrix = eulerAnglesToRotationMatrix(float3(rotX, rotY, 0));

                // 回転と位置を適用
                objectToWorld = mul(rotMatrix, objectToWorld);
                objectToWorld._14_24_34 += pos.xyz;
                
                // 頂点座標を変換
                v.vertex = mul(objectToWorld, animatedPos);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = _Color;
                return o;
            }

            // fragment shader
            fixed4 frag(vs_out input) : SV_Target
            {
                fixed4 col = _Color;
                return col;
            }

            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}