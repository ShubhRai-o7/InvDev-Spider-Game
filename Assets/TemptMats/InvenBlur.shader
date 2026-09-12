Shader "Custom/ScreenBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 10)) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurAmount;

            float4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float4 color = float4(0, 0, 0, 0);
                
                // Gaussian blur kernel
                float blurStep = _BlurAmount * _MainTex_TexelSize.xy;

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        float2 samplePos = uv + float2(x, y) * blurStep;
                        color += tex2D(_MainTex, samplePos);
                    }
                }

                color /= 25; // normalize the accumulated color
                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
