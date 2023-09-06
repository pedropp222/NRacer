Shader "Custom/FadeOutShader" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _FadeObject ("Fade Object", Range(0, 10)) = 1
        _FadeDistance ("Fade Distance", Range(0, 10)) = 5
    }
 
    SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass {
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects

         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag

         fixed4 _Color;
         float _FadeObject;
         float _FadeDistance;
 
         float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
         {
            return UnityObjectToClipPos(vertexPos);
         }
 
         float4 frag(void) : COLOR 
         {
            float fadeAmount = 1.0 - smoothstep(0, _FadeObject,_FadeDistance);
            fadeAmount = max(fadeAmount, 0.1);
 
            // Apply the fade amount to the alpha channel
            //o.Alpha *= fadeAmount;
 
            // Set the albedo color without applying the fade effect
            //o.Albedo = _Color.rgb;

            return float4(_Color.rgb, fadeAmount); 
               // the fourth component (alpha) is important: 
               // this is semitransparent green
         }
 
         ENDCG  
      }
   }
}
