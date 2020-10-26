// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "Outlined/Unlit" {
 Properties {
  _Color ("Main Color", Color) = (.5,.5,.5,1)
  _OutlineColor ("Outline Color", Color) = (0,0,0,1)
  _Outline ("Outline width", Range (0, 1)) = .1
  _MainTex ("Base (RGB)", 2D) = "white" { }
 }
 
CGINCLUDE
#include "UnityCG.cginc"
 
struct appdata {
 float4 vertex : POSITION;
 float3 normal : NORMAL;
};
 
struct v2f {
 float4 pos : POSITION;
 float4 color : COLOR;
};
 
uniform float _Outline;
uniform float4 _OutlineColor;
 
v2f vert(appdata v) {
 // just make a copy of incoming vertex data but scaled according to normal direction
 v2f o;
     o.pos = v.vertex;
     o.pos.xyz += v.normal.xyz *_Outline*0.01;
     o.pos = UnityObjectToClipPos(o.pos);
 
     o.color = _OutlineColor;
     return o;
}
ENDCG
 
 SubShader {
  Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"}
  Cull Back
  CGPROGRAM
  #pragma surface surf NoLighting
    
  sampler2D _MainTex;
  fixed4 _Color;
  
  struct Input 
  {
   float2 uv_MainTex;
  };
  
 void surf (Input IN, inout SurfaceOutput s) 
    {
         //o.Albedo = _Color.rgb * _Color.a;
         s.Emission = tex2D (_MainTex, IN.uv_MainTex);
           s.Alpha = _Color.a;
    }
     
 fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
    {
         return fixed4(0,0,0,0);//half4(s.Albedo, s.Alpha);
    }
  

  ENDCG
  Pass {
   Name "OUTLINE"
   Tags { "Queue" = "Transparent" "IgnoreProjector" = "True"}
   Cull Front
   ZWrite Off
   //ZTest Less
   //Offset 1, 1
 
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   half4 frag(v2f i) :COLOR { return i.color; }
   ENDCG
  }
 }
 
 Fallback "Diffuse"
}