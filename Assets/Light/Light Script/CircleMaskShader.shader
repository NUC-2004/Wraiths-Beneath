Shader "Custom/CircleMaskFixed"
{
    Properties
    {
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Range(0, 1)) = 0.2
        _Softness ("Softness", Range(0, 0.5)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
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

            float2 _Center;
            float _Radius;
            float _Softness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算宽高比
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 uv = i.uv;

                // 将UV坐标转换到中心为原点
                uv -= _Center;

                // 使用屏幕宽高比对UV进行缩放调整
                uv.x *= aspect;  // 在宽屏情况下，调整X轴

                // 计算距离，确保在任何屏幕下都能得到圆形
                float dist = length(uv);

                // 使用smoothstep实现柔和边缘
                float reveal = 1.0 - smoothstep(_Radius - _Softness, _Radius + _Softness, dist);

                // 反转：外围不透明（黑），中心透明
                float alpha = 1.0 - reveal;

                return fixed4(0, 0, 0, alpha);
            }
            ENDCG
        }
    }
}
