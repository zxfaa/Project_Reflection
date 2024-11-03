Shader "Custom/LeftToRightFadeOut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "Null" {}
        _Fade ("Fade Amount", Range(-1.0, 1.0)) = 0.7 // 初始左不透明、右透明
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Fade;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                //col = fixed4(0, 0, 0, 1); // 預設黑色

                // 控制：左邊不透明、右邊透明，淡出從左到右
                float alphaFactor = 1.0 - smoothstep(_Fade, 1.0, i.uv.x);
                col.a *= alphaFactor;

                return col;
            }
            ENDCG
        }
    }
}
