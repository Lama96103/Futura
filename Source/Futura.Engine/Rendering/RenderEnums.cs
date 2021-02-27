using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Rendering
{
    public enum RenderBuffer
    {
        FrameBuffer,
        ModelBuffer,
        LightBuffer
    }

    public enum RenderTarget
    {
        Undefined,
        GbufferAlbedo,
        GbufferNormal,
        GbufferMaterial,
        GbufferVelocity,
        GbufferDepth,
        BrdfPrefiltered_Environment,
        BrdfSpecular_Lut,
        LightDiffuse,
        LightDiffuseTransparent,
        LightSpecular,
        LightSpecularTransparent,
        LightVolumetric,
        FrameHdr,
        FrameLdr,
        FrameHdr_2,
        FrameLdr_2,
        DofHalf,
        DofHalf_2,
        Bloom,
        Ssao,
        SsaoBlurred,
        Ssgi,
        Ssr,
        AccumulationTaa,
        AccumulationSsgi
    }

    public enum RenderShader
    {
        Gbuffer_V,
        Gbuffer_P,
        Depth_V,
        Depth_P,
        Quad_V,
        Texture_P,
        Copy_C,
        Fxaa_C,
        Fxaa_Luminance_C,
        FilmGrain_C,
        Taa_C,
        MotionBlur_C,
        Dof_DownsampleCoc_C,
        Dof_Bokeh_C,
        Dof_Tent_C,
        Dof_UpscaleBlend_C,
        Sharpening_C,
        ChromaticAberration_C,
        BloomDownsampleLuminance_C,
        BloomDownsample_C,
        BloomUpsampleBlendFrame_C,
        BloomUpsampleBlendMip_C,
        ToneMapping_C,
        GammaCorrection_C,
        Dithering_C,
        DebugNormal_C,
        DebugVelocity_C,
        DebugChannelR_C,
        DebugChannelA_C,
        DebugChannelRgbGammaCorrect_C,
        BrdfSpecularLut_C,
        Light_C,
        Light_Composition_C,
        Light_ImageBased_P,
        Color_V,
        Color_P,
        Font_V,
        Font_P,
        Hbao_C,
        Ssgi_C,
        SsgiInject_C,
        SsrTrace_C,
        Reflections_P,
        Entity_V,
        Entity_Transform_P,
        BlurBox_P,
        BlurGaussian_P,
        BlurGaussianBilateral_P,
        Entity_Outline_P,
        GenerateMips_C
    }

}
