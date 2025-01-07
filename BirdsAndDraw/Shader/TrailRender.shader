Shader "Custom/TrailRender"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {
            "RenderType"="Transparent"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vs_out
            {
                float4 pos : POSITION0;
                float3 dir : TANGENT0;
                float4 posNext : POSITION1;
                float3 dirNext : TANGENT1;
                float2 uv : TEXCOORD0;
                uint instanceId : TEXCOORD1;
                uint vertexId : TEXCOORD2;
            };

            struct gs_out
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                uint instanceId : TEXCOORD1;
                uint vertexId : TEXCOORD2;
            };

            struct Trail
            {
                int currentNodeIdx;
                float elapsedTime;
                float lineDensity;
                float noiseStrength;
            };

            struct Node
            {
                float updatedTime;
                float3 position;
                float life;
            };

            int _NodeNumPerTrail;
            float _Width;
            float4 _Color;
            
            StructuredBuffer<Trail> _TrailBuffer;
            StructuredBuffer<Node> _NodeBuffer;

            Node getNode(int trailIdx, int nodeIdx)
            {
                nodeIdx %= _NodeNumPerTrail;
                // 全体のNodeBufferの中のインデックスを取得
                int nodeBufIdx = trailIdx * _NodeNumPerTrail + nodeIdx;
                return _NodeBuffer[nodeBufIdx];
            }

            bool isValid(Node node)
            {
                return node.updatedTime >= 0;
            }

            float horizontalNoise(float y, float lineDensity, float noiseStrength)
            {
                float l = sin(y * lineDensity) * 0.5 + 0.5;
                float noise = frac(sin(y * 2345.678 + 123.456) * 4567.89);
                return lerp(l, noise, noiseStrength);
            }

            vs_out vert(uint vId : SV_VertexID, uint iId : SV_InstanceID, appdata_full v)
            {
                vs_out o;
                Trail trail = _TrailBuffer[iId];
                uint currentNodeIdx = trail.currentNodeIdx;

                Node node0 = getNode(iId, vId - 1);
                Node node1 = getNode(iId, vId); // current
                Node node2 = getNode(iId, vId + 1);
                Node node3 = getNode(iId, vId + 2);

                bool isLastNode = (int)vId == currentNodeIdx;
                if(isLastNode || !isValid(node1))
                {
                    node1 = node2 = node3 = getNode(iId, vId);
                }

                float3 pos1 = node1.position;
                float3 pos0 = isValid(node0) ? node0.position : pos1;
                float3 pos2 = isValid(node2) ? node2.position : pos1;
                float3 pos3 = isValid(node3) ? node3.position : pos2;

                o.pos = float4(pos1, 1);
                o.posNext = float4(pos2, 1);
                o.dir = normalize(pos2 - pos0);
                o.dirNext = normalize(pos3 - pos1);
                o.instanceId = iId;
                o.vertexId = vId;
                o.uv = v.texcoord;
                return o;
            }

            [maxvertexcount(4)]
            void geom(point vs_out input[1], inout TriangleStream<gs_out> outStream)
            {
                gs_out out0, out1, out2, out3;
                float3 pos = input[0].pos.xyz;
                float3 dir = input[0].dir;
                float3 posNext = input[0].posNext.xyz;
                float3 dirNext = input[0].dirNext;

                float3 camPos = _WorldSpaceCameraPos;
                float3 toCamDir = normalize(camPos - pos);
                float3 sideDir = normalize(cross(toCamDir, dir));
                float3 toCamDirNext = normalize(camPos - posNext);
                float3 sideDirNext = normalize(cross(toCamDirNext, dirNext));
                float width = _Width * 0.5;

                out0.pos = UnityWorldToClipPos(pos + sideDir * width);
                out1.pos = UnityWorldToClipPos(pos - sideDir * width);
                out2.pos = UnityWorldToClipPos(posNext + sideDirNext * width);
                out3.pos = UnityWorldToClipPos(posNext - sideDirNext * width);
                out0.instanceId = out1.instanceId
                = out2.instanceId = out3.instanceId = input[0].instanceId;
                out0.vertexId = out1.vertexId
                = out2.vertexId = out3.vertexId = input[0].vertexId;

                out0.uv = float2(1, 1);
                out1.uv = float2(1, 0);
                out2.uv = float2(0, 1);
                out3.uv = float2(0, 0);
                
                outStream.Append(out0);
                outStream.Append(out1);
                outStream.Append(out2);
                outStream.Append(out3);
                outStream.RestartStrip();
            }

            float4 frag(gs_out input) : COLOR
            {
                uint iId = input.instanceId;
                uint vId = input.vertexId;
                
                Trail trail = _TrailBuffer[iId];
                float trailElapsedTime = trail.elapsedTime;
                float lineDensity = trail.lineDensity;
                float noiseStrength = trail.noiseStrength;

                Node node = getNode(iId, vId);
                float nodeUpdatedTime = node.updatedTime;
                float nodeLifeTime = node.life != 0.0 ? node.life : 0.01;

                float alpha = 1.0 - saturate((trailElapsedTime - nodeUpdatedTime) / nodeLifeTime);
                if(alpha == 0.0) discard;
                
                float3 baseCol = _Color.rgb;
                float noise = horizontalNoise(input.uv.y, lineDensity, noiseStrength);
                if(noise > 0.75) discard;
                else
                {
                    noise = (float)baseCol;
                }
                float4 col = float4((float3)noise, alpha);
                return col;
            }
            
            ENDCG
        }
    }
}
