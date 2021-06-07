using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class MainController : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    PixelateSettings pixelateSettings;
    VignetteSettings vignetteSettings;

    float pixAmount = 0.001f;

    private void Start()
    {
        postProcessVolume.profile.TryGetSettings(out pixelateSettings);
        postProcessVolume.profile.TryGetSettings(out vignetteSettings);
        pixelateSettings.intensity2.value = 0.1f;
        pixelateSettings.pixelScalar.value = 4f;
        vignetteSettings.centerX.value = 0;
        vignetteSettings.centerY.value = 0;
        vignetteSettings.intensity.value = 1;
    }

    private void Update()
    {
        if (Camera.main.transform.position.y > 25f)
        {

            pixelateSettings.intensity2.value = Mathf.Lerp(pixelateSettings.intensity2 , 0.7f, Time.deltaTime);
            pixelateSettings.pixelScalar.value = Mathf.Lerp(pixelateSettings.pixelScalar, 6f, Time.deltaTime);
            vignetteSettings.intensity.value = Mathf.Lerp(vignetteSettings.intensity, 4f, Time.deltaTime);

        }
        else
        {
            pixelateSettings.intensity2.value = Mathf.Lerp(pixelateSettings.intensity2 , 0.1f, Time.deltaTime);
            pixelateSettings.pixelScalar.value = Mathf.Lerp(pixelateSettings.pixelScalar, 4f, Time.deltaTime);
            vignetteSettings.intensity.value = Mathf.Lerp(vignetteSettings.intensity, 1f, Time.deltaTime);
        }
    }
}
