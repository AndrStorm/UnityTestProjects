Shader "Custom/TesselationShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        
        [KeywordEnum(UNIFORM, EDGE)] _FACTOR_MODE("Factor mode", Float) = 0
        _TesselationUniform ("Tesellation uniform factor", range(0,64)) = 2
        _TessellationEdgeLength ("Tessellation Edge Length", Range(0.1, 300)) = 0.5
        _FrustumBias ("Frustum Bias", Range(-5, 5)) = -0.3
        
        _WireframeColor ("Wireframe Color", Color) = (0, 0, 0)
		_WireframeSmoothing ("Wireframe Smoothing", Range(0, 10)) = 1
		_WireframeThickness ("Wireframe Thickness", Range(0, 10)) = 1
        
        _Gloss ("Gloss", Range(0,1)) = 1
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 1
        
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]_NormalTex("Normal map", 2D) = "bump" {}
        
        [NoScaleOffset] _ParallaxMap ("Parallax", 2D) = "black" {}
		_ParallaxStrength ("Parallax Strength", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            //"Queue" = "Opaque" //"Queue" = "Transparent" //render order // Geometry causing appear AO (pp) stuff
            //"RenderPipeline" = "LightweightPipeline" 
        }
        LOD 100
        

        Pass
        {
            //Tags{"LightMode" = "ForwardBase"}
            //Cull Off //removing mesh render part front/back/off
            //ZWrite Off //Depth buffer
            //ZTest LEqual //drawing through other meshes, default LEqual/Always/GEqual
            Blend One Zero //additive
            //Blend OneMinusDstColor One
            //Blend SrcAlpha OneMinusSrcAlpha // aphaBlending src*a + dist* (1-a) 


            CGPROGRAM
            #pragma target 5.0

            #pragma shader_feature_local _FACTOR_MODE_UNIFORM _FACTOR_MODE_EDGE
            
            #pragma vertex MyTessellationVertexProgram
            #pragma fragment frag
            #pragma hull MyHullProgram
            #pragma domain MyDomainProgram
            #pragma geometry MyGeometryProgram
            #pragma multi_compile_builtin

            //#pragma multi_compile_fog // make fog work
            
            #define BASE_PASS //key difinition
            #include "Wireframe.cginc"
            //#include "SimpleLight.cginc"
            #include "Tesselation.cginc"

            ENDCG
        }
        
        //dir light pass
        Pass
        {
            
            Tags{"LightMode" = "ForwardBase"}
            Blend One One //additive
            ZWrite Off

            CGPROGRAM
            #pragma target 5.0

            #pragma shader_feature_local _FACTOR_MODE_UNIFORM _FACTOR_MODE_EDGE
            
            #pragma vertex MyTessellationVertexProgram
            #pragma fragment frag
            #pragma hull MyHullProgram
            #pragma domain MyDomainProgram
            //#pragma geometry MyGeometryProgram
            #pragma multi_compile_fwdadd //attenation work with
            //#pragma multi_compile_fog // make fog work
            
            #define USE_LIGHTING //key difinition
            //#define BINORMAL_PER_FRAGMENT
            
            //#include "Wireframe.cginc"
            #include "SimpleLight.cginc"
            #include "Tesselation.cginc"

            ENDCG
        }
        
        //add pass
        Pass
        {
            
            Tags{"LightMode" = "ForwardAdd"}
            Blend One One //additive
            ZWrite Off

            CGPROGRAM
            #pragma target 5.0

            #pragma shader_feature_local _FACTOR_MODE_UNIFORM _FACTOR_MODE_EDGE
            
            #pragma vertex MyTessellationVertexProgram
            #pragma fragment frag
            #pragma hull MyHullProgram
            #pragma domain MyDomainProgram
            //#pragma geometry MyGeometryProgram
            #pragma multi_compile_fwdadd //attenation work with
            //#pragma multi_compile_fog // make fog work
            
            #define USE_LIGHTING //key difinition
            //#define BINORMAL_PER_FRAGMENT
            
            //#include "Wireframe.cginc"
            #include "SimpleLight.cginc"
            #include "Tesselation.cginc"

           
            ENDCG
        }
    }
    //FallBack "Diffuse"
}
