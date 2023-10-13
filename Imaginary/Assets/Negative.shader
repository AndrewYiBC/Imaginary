Shader "Imaginary/Negative"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }
        ZWrite Off
        Blend OneMinusDstColor Zero

        Pass
        {

        }
    }
}
