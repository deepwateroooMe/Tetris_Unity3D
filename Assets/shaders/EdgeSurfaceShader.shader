Shader "Standard/EdgeSurfaceShader" {

    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        // exist in Unity's Edge Detection shader
        _SensitivityDepth("SensitivityDepth", Range(0, 5)) = 3.75     // How sensitive the shader is towards depth changes in the testing locations for detecting an edge
        _SensitivityNormals("SensitivityNormals", Range(0, 5)) = 0.82 // How sensitive the shader is towards normals changes in the testing locations for detecting an edge
        _sampleDistance("SampleDistance", Range(0, 2)) = 1            // How far these samples are for detecting whether it is (they are) an edge of not
        
        _Falloff("Falloff", Range(0, 100)) = 10.0                     // determines how fast the black edges will vanish with distance
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows finalcolor:edgecolor
        // edgecolor function: calculate the edges for us, and make the fragment turns black if it is not an edge

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        uniform float4 _MainTex_TexelSize;
        uniform float4 _CameraDepthNormalsTexture_TexelSize;
        sampler2D _CameraDepthNormalsTexture; // created in the first camera
        
        struct Input {
            float2 uv_MainTex;
            float4 screenPos; // check documents
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        uniform half _SensitivityDepth;
        uniform half _SensitivityNormals;
        uniform half _SampleDistance;
        uniform half _Falloff;

        // copy paste of Unity's Edge Detection shader        
        inline half CheckSame(half2 centerNormal, float centerDepth, half4 theSample) {
            // Difference in Normals
            // do not bother decoding normals - there's NO need here
            half2 diff = abs(centerNormal - theSample.xy) * _SensitivityNormals;
            int isSameNormal = (diff.x + diff.y) * _SensitivityNormals < 0.1;
            // Difference in Depth
            float sampleDepth = DecodeFloatRG(theSample.zw);
            float zdiff = abs(centerDepth - sampleDepth);
            // scale the required threshold by the distance
            int isSameDepth = zdiff * _SensitivityDepth < 0.09 * centerDepth;
            // return
            // 1 - if normals and depth are similar enough
            // 0 - otherwise
            return isSameNormal * isSameDepth ? 1.0 : 0.0;
        }

        // commented out instance support
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)

        void edgecolor(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
            // create our three screen UVs where we sample
            // 3 screen UV positions Unity samples in order to detect whether it is an edge or not, get from the camera
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float sampleSizeX = _CameraDepthNormalsTexture_TexelSize.x;
            float sampleSizeY = _CameraDepthNormalsTexture_TexelSize.y;
            float2 _uv2 = screenUV + float2(-sampleSizeX, -sampleSizeY) * _SampleDistance;
            float2 _uv3 = screenUV + float2(+sampleSizeX, -sampleSizeY) * _SampleDistance;

            // get depth and normals from there, get from _CameraDepthNormalsTexture
            half4 center = tex2D(_CameraDepthNormalsTexture, screenUV);
            half4 sample1 = tex2D(_CameraDepthNormalsTexture, _uv2);
            half4 sample2 = tex2D(_CameraDepthNormalsTexture, _uv3);

            // encoded normal
            half2 centerNormal = center.xy;
            // decoded depth
            float centerDepth = DecodeFloatRG(center.zw);

            // calculate how faded the edge is
            // nice effect, but maybe you don't want it in your scene, it's up to you!!!
            float d = clamp(centerDepth * _Falloff - 0.05, 0.0, 1.0);
            half4 depthFade = half4(d, d, d, 1.0);

            // is it an edge? 0 if yes, 1 if no
            // check if our sample forms an edge or not
            half edge = 1.0;
            edge *= CheckSame(centerNormal, centerDepth, sample1);
            edge *= CheckSame(centerNormal, centerDepth, sample2);

            // calculate this fragment/pixel's color!
            color = edge * color + (1.0 - edge) * depthFade * color;
        }
         
        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
  