Shader "Custom/PointGPUinstanceShader"
{
  
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
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
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            StructuredBuffer<float4x4> _PositionBuffer;
            StructuredBuffer<float4> _ColorBuffer;
            
            v2f vert (appdata v, uint instanceID : SV_InstanceID)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                // 获取实例化数据
                float4x4 instanceMatrix = _PositionBuffer[instanceID];
                float4 instanceColor = _ColorBuffer[instanceID];
                
                // 应用变换
                float4 worldPosition = mul(instanceMatrix, v.vertex);
                o.vertex = UnityObjectToClipPos(worldPosition);
                o.color = instanceColor;
                o.uv = v.uv;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                return i.color;
            }
            ENDCG
        }
    }
}
