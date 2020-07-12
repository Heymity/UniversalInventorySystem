Shader "InventorySystem/DurabilityBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Bar Color", Color) = (1,1,1,1)
        _BackGroundColor ("Backgorund Color", Color) = (1,1,1,0)
        _FillAmount ("Fill Amount", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 300

        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _BackGroundColor;
            float _FillAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (i.uv.x >= _FillAmount){
                    col.rgba = _BackGroundColor;
                } else {
                    col.rgba = _Color;
                }

                return col;
            }
            ENDCG
        }
    }
}
