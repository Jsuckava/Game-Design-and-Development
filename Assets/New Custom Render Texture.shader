// Shader "Custom/DualTransparentKey"
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
        
//         --- COLOR 1 SETTINGS (e.g., Black) ---
//         _KeyColor1 ("Key Color 1", Color) = (0,0,0,1) 
//         _Threshold1 ("Threshold 1", Range(0, 1.0)) = 0.1
//         _Softness1 ("Softness 1", Range(0, 1.0)) = 0.1
        
//         --- COLOR 2 SETTINGS (e.g., White) ---
//         _KeyColor2 ("Key Color 2", Color) = (1,1,1,1) 
//         _Threshold2 ("Threshold 2", Range(0, 1.0)) = 0.1
//         _Softness2 ("Softness 2", Range(0, 1.0)) = 0.1
//     }
//     SubShader
//     {
//         Tags { "Queue"="Transparent" "RenderType"="Transparent" }
//         Blend SrcAlpha OneMinusSrcAlpha
//         Cull Off 
//         ZWrite Off

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f {
//                 float2 uv : TEXCOORD0;
//                 float4 vertex : SV_POSITION;
//             };

//             sampler2D _MainTex;
//             float4 _MainTex_ST;
            
//             fixed4 _KeyColor1;
//             float _Threshold1;
//             float _Softness1;
            
//             fixed4 _KeyColor2;
//             float _Threshold2;
//             float _Softness2;

//             v2f vert (appdata v) {
//                 v2f o;
//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target {
//                 fixed4 col = tex2D(_MainTex, i.uv);
                
//                 --- CHECK COLOR 1 (Black) ---
//                 float dist1 = distance(col.rgb, _KeyColor1.rgb);
//                 float alpha1 = smoothstep(_Threshold1, _Threshold1 + _Softness1, dist1);

//                 --- CHECK COLOR 2 (White) ---
//                 float dist2 = distance(col.rgb, _KeyColor2.rgb);
//                 float alpha2 = smoothstep(_Threshold2, _Threshold2 + _Softness2, dist2);

//                 Combine: If it matches EITHER color, make it transparent
//                 We take the lowest alpha value (min) to ensure transparency wins
//                 float finalAlpha = min(alpha1, alpha2);
                
//                 return fixed4(col.rgb, finalAlpha);
//             }
//             ENDCG
//         }
//     }
// }

Shader "Custom/UI_ColorKey"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorToMask ("Color to Mask", Color) = (1,1,1,1)
        _Threshold ("Threshold", Range(0, 1)) = 0.1
        _Softness ("Softness", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorToMask;
            float _Threshold;
            float _Softness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                
                // Calculate distance between current pixel color and the mask color
                float dist = distance(col.rgb, _ColorToMask.rgb);
                
                // Create alpha based on distance
                float alpha = smoothstep(_Threshold, _Threshold + _Softness, dist);
                
                return fixed4(col.rgb, alpha);
            }
            ENDCG
        }
    }
}