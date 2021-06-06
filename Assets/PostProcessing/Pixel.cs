using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;

//Needed to let unity serialize this and extend PostProcessEffectSettings
[Serializable]
//Using [PostProcess()] attrib allows us to tell Unity that the class holds postproccessing data. 
[PostProcess(renderer: typeof(Pixelate),//First parameter links settings with actual renderer
            PostProcessEvent.AfterStack,//Tells Unity when to execute this postpro in the stack
            "Custom/Pixel")] //Creates a menu entry for the effect
                             //Forth parameter that allows to decide if the effect should be shown in scene view
public sealed class PixelateSettings : PostProcessEffectSettings
{
    [Range(0.1f, 1f), Tooltip("Effect Intensity.")]
    public FloatParameter intensity2 = new FloatParameter { value = 0.0f }; //Custom parameter class, full list at: /PostProcessing/Runtime/
                                                                            //The default value is important, since is the one that will be used for blending if only 1 of volume has this effect
    [Range(4f, 50f), Tooltip("Pixel Intensity.")]
    public FloatParameter pixelScalar = new FloatParameter { value = 0.0f };
}

public class Pixelate : PostProcessEffectRenderer<PixelateSettings>//<T> is the setting type
{
    public override void Render(PostProcessRenderContext context)
    {
        //We get the actual shader property sheet
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Pixel"));
        //Set the uniform value for our shader
        sheet.properties.SetFloat("_intensity2", settings.intensity2);
        sheet.properties.SetFloat("_pixelScalar", settings.pixelScalar);

        //We render the scene as a full screen triangle applying the specified shader
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

