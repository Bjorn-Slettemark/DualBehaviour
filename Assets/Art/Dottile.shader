Shader "Custom/ScrollingDotsWithMouseInteraction"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _DotColor ("Dot Color", Color) = (1,1,1,1)
        _ScrollSpeed ("Scroll Speed", Vector) = (1,1,0,0)
        _TileAmount ("Tile Amount", Float) = 10
        _MousePosition ("Mouse Position", Vector) = (0,0,0,0)
        _MouseInfluence ("Mouse Influence", Float) = 0.1
        _FalloffDistance ("Falloff Distance", Float) = 0.2
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        fixed4 _BackgroundColor;
        fixed4 _DotColor;
        float2 _ScrollSpeed;
        float _TileAmount;
        float2 _MousePosition;
        float _MouseInfluence;
        float _FalloffDistance;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 uv = IN.uv_MainTex * _TileAmount;
            uv += _Time.y * _ScrollSpeed;
            
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float2 mouseOffset = screenUV - _MousePosition;
            float mouseDistance = length(mouseOffset);
            
            float dotSize = 0.4 + _MouseInfluence * smoothstep(_FalloffDistance, 0, mouseDistance);
            
            float2 nearest = 2 * frac(uv) - 1;
            float dist = length(nearest);
            float circle = smoothstep(dotSize, dotSize - 0.01, dist);
            
            fixed4 c = lerp(_BackgroundColor, _DotColor, circle);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}