Shader "UI/RoundedCorners/RoundedCornersWithOutline" {
    Properties {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        // --- Mask support ---
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _WidthHeight ("WidthHeight", Vector) = (0,0,0,0)
        _CornerRadius ("Corner Radius", Float) = 10
        _OutlineThickness ("Outline Thickness", Float) = 5
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
    }

    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask]
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _WidthHeight;
            float _CornerRadius;
            float _OutlineThickness;
            float4 _OutlineColor;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // Signed Distance Function for a rounded rectangle
            float roundedBoxSDF(float2 p, float2 size, float radius) {
                float2 q = abs(p - size * 0.5) - (size * 0.5 - radius);
                return length(max(q, 0.0)) - radius;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 size = _WidthHeight.xy;
                float radius = _CornerRadius;
                float thickness = _OutlineThickness;

                float2 uv = i.uv * size;
                float dist = roundedBoxSDF(uv, size, radius);

                // "inner" is inside the fill area (after the outline)
                float inner = smoothstep(0.0, 1.0, -dist - thickness);
                // "outline" is between the outer shape and the inner shape
                float outline = smoothstep(0.0, 1.0, -dist) - inner;

                fixed4 texColor = tex2D(_MainTex, i.uv) * i.color;
                fixed4 fillColor = texColor;
                fixed4 outlineColor = _OutlineColor;

                // Blend the two regions
                fixed4 finalColor = fillColor * inner + outlineColor * outline;

                // Make transparent if fully outside
                finalColor.a *= (inner + outline);

                return finalColor;
            }
            ENDCG
        }
    }
}
