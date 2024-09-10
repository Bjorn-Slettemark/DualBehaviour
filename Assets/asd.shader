Shader "Custom/TilingDots"
{
    Properties
    {
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _DotColor ("Dot Color", Color) = (1,1,1,1)
        _DotSize ("Dot Size", Range(0, 0.5)) = 0.4
        _GridSize ("Grid Size", Float) = 10
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        fixed4 _BackgroundColor;
        fixed4 _DotColor;
        float _DotSize;
        float _GridSize;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 uv = IN.uv_MainTex * _GridSize;
            float2 center = frac(uv) - 0.5;
            float dist = length(center);
            float circle = step(dist, _DotSize);
            
            fixed4 c = lerp(_BackgroundColor, _DotColor, circle);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}