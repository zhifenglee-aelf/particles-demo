using UnityEngine;
using UnityEngine.Rendering;

public class LightingSettings : MonoBehaviour
{
    void Awake()
    {
        // Set all lighting related ambient settings to black
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientSkyColor = Color.black;
        RenderSettings.ambientEquatorColor = Color.black;
        RenderSettings.ambientGroundColor = Color.black;
        RenderSettings.ambientIntensity = 0;
        RenderSettings.ambientLight = Color.black;
        
        // Disable fog
        RenderSettings.fog = false;
        
        // Disable reflections
        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        RenderSettings.customReflection = null;
        RenderSettings.reflectionIntensity = 0;
        
        // Force update
        DynamicGI.UpdateEnvironment();
    }
}