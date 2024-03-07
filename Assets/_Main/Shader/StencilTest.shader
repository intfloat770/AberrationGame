Shader "Custom/StencilTest"
{
    Properties
    {

    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue" = "Transparent"
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        HLSLINCLUDE

        float _LightingX;
        float _LightingY;
        float _LightingZ;
        float _TempRotation;

        struct VertexInput
        {
            float4 position : SV_POSITION;
            float3 surfaceNormal : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            #pragma VertexInput vert
            #pragma fragment frag

             
            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.uv = i.uv;
                o.normal = i.surfaceNormal;
                return o;
            }

            float4 frag(VertexOutput o) : SV_Target
            {
                return float4(1, 1, 1, 1);
            }

            ENDHLSL
        }
    }
}
