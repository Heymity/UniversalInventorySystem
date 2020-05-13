Shader "InventorySystem/DragItem"
{
    Properties
    {
        _Size ("Outline size", float) = 0.5
        _Color ("Outline color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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
            float _Size;
            float4 _Color;

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

                fixed4 colOutLineLeft = tex2D(_MainTex, i.uv + float2(_Size / 100, 0));
                fixed4 colOutLineRight = tex2D(_MainTex, i.uv + float2(-_Size / 100, 0));
                fixed4 colOutLineDown = tex2D(_MainTex, i.uv + float2(0, _Size / 100));
                fixed4 colOutLineUp = tex2D(_MainTex, i.uv + float2(0, -_Size / 100));

                colOutLineLeft.a = colOutLineLeft.a - col.a * colOutLineLeft.a;
                colOutLineRight.a = colOutLineRight.a - col.a * colOutLineRight.a;
                colOutLineDown.a = colOutLineDown.a - col.a * colOutLineDown.a;
                colOutLineUp.a = colOutLineUp.a - col.a * colOutLineUp.a;

                fixed4 cola = colOutLineLeft + colOutLineRight + colOutLineUp + colOutLineDown;

                cola.rgb = _Color;

                cola.rgb *= cola.a;

                col = col + cola;
            
                return col;
            }
            ENDCG
        }
    }
}
