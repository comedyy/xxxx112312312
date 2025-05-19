Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}
            HLSLPROGRAM

            #pragma target 3.0
            #pragma exclude_renderers nomrt

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #pragma multi_compile _ DISSOLVE 
            #pragma multi_compile _ DISSOLVE_APPEAR

            #pragma instancing_options procedural:setup

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            StructuredBuffer<float3> objectPositionBuffer;
            void setup()
			{
			}

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                UNITY_SETUP_INSTANCE_ID(v);
                float3 objectPosition = objectPositionBuffer[unity_InstanceID];

                objectPosition.xyz = objectPosition.xyz + v.vertex;
				o.vertex = TransformWorldToHClip(objectPosition.xyz);
#else
				o.vertex = v.vertex;
#endif

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDHLSL
        }
    }
}
