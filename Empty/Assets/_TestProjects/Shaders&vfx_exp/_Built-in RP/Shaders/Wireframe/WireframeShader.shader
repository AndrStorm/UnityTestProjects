Shader "Custom/WireframeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        
        _WireframeColor ("Wireframe Color", Color) = (0, 0, 0)
		_WireframeSmoothing ("Wireframe Smoothing", Range(0, 10)) = 1
		_WireframeThickness ("Wireframe Thickness", Range(0, 10)) = 1
        
        _Gloss ("Gloss", Range(0,1)) = 1
        _NormalIntensity ("Normal map Intensity", Range(0, 1)) = 1
        
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]_NormalTex("Normal map", 2D) = "bump" {}
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
            #pragma target 4.0
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry MyGeometryProgram
            #pragma multi_compile_builtin

            //#pragma multi_compile_fog // make fog work
            
            #define BASE_PASS
            #include "Wireframe.cginc"

            
            ENDCG
        }
        
        //dir light pass
        Pass
        {
            
            Tags{"LightMode" = "ForwardBase"}
            Blend One One //additive
            ZWrite Off

            CGPROGRAM
            #pragma target 4.0
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry MyGeometryProgram
            #pragma multi_compile_fwdadd //attenation work with
            //#pragma multi_compile_fog // make fog work
            
            #define USE_LIGHTING 
            #include "Wireframe.cginc"
            
            ENDCG
        }
        
        //add pass
        Pass
        {
            
            Tags{"LightMode" = "ForwardAdd"}
            Blend One One //additive
            ZWrite Off

            CGPROGRAM
            #pragma target 4.0
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry MyGeometryProgram
            #pragma multi_compile_fwdadd //attenation work with
            //#pragma multi_compile_fog // make fog work
            
            #define USE_LIGHTING
            #include "Wireframe.cginc"
           
            ENDCG
        }
        
        /* HLSL
        Pass
        {
            //Tags{"LightMode" = "ForwardBase"}
            //Cull Off //removing mesh render part front/back/off
            ZWrite Off //Depth buffer
            //ZTest LEqual //drawing through other meshes, default LEqual/Always/GEqual
            Blend One One //additive
            //Blend OneMinusDstColor One
            //Blend SrcAlpha OneMinusSrcAlpha // aphaBlending src*a + dist* (1-a) 


            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_builtin
            //#pragma multi_compile_fog // make fog work
            
            //#define BASE_PASS //key difinition
            #include "WireframeMain_HLSR.cginc"
            
            ENDHLSL
        }
        
        //dir light pass
        Pass
        {
            
            Tags{"LightMode" = "ForwardBase"}
            Blend One One //additive

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd //attenation work with
            //#pragma multi_compile_fog // make fog work
            
            #define USE_LIGHTING //key difinition
            #include "WireframeMain_HLSR.cginc"
            
            ENDHLSL
        }
        
        //add pass
        Pass
        {
            
            Tags{"LightMode" = "ForwardAdd"}
            Blend One One //additive

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd //attenation work with
            //#pragma multi_compile_fog // make fog work
            
            #define USE_LIGHTING //key difinition
            #include "WireframeMain_HLSR.cginc"
           
            ENDHLSL
        }
        */
    }
    
    //FallBack "Diffuse"
}
