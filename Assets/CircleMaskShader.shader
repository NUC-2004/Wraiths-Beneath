Shader "Custom/CircleMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Range(0, 1)) = 0.2
        _Softness ("Softness", Range(0, 0.5)) = 0.1
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
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
            float2 _Center;
            float _Radius;
            float _Softness;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 修正方案1：考虑屏幕宽高比
                // 获取屏幕宽高比
                float2 aspectRatio;
                aspectRatio.x = 1.0;
                aspectRatio.y = _ScreenParams.x / _ScreenParams.y;
                
                // 调整UV坐标，使其在x轴方向"压缩"
                float2 adjustedUV = i.uv;
                adjustedUV.x = (i.uv.x - 0.5) * aspectRatio.y + 0.5;
                
                // 计算距离（使用调整后的UV）
                float dist = distance(adjustedUV, _Center);
                float alpha = smoothstep(_Radius - _Softness, _Radius + _Softness, dist);
                
                // 黑色遮罩颜色
                fixed4 col = fixed4(0, 0, 0, alpha);
                return col;
            }
            ENDCG
        }
    }
}