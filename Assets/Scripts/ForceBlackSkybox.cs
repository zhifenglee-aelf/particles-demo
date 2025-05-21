using UnityEngine;

public class ForceBlackSkybox : MonoBehaviour
{
    void Awake()
    {
        // Create a new material directly with the solid color shader
        Material blackSkybox = new Material(Shader.Find("Skybox/Procedural"));
        
        // Set all relevant parameters to black
        blackSkybox.SetColor("_SkyTint", Color.black);
        blackSkybox.SetColor("_GroundColor", Color.black);
        blackSkybox.SetFloat("_SunSize", 0f);
        blackSkybox.SetFloat("_SunSizeConvergence", 0f);
        blackSkybox.SetFloat("_AtmosphereThickness", 0f);
        blackSkybox.SetFloat("_Exposure", 0f);
        
        // Apply the skybox material to the render settings
        RenderSettings.skybox = blackSkybox;
        
        // Set the default ambient mode to color and set it to black
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        
        // Disable fog
        RenderSettings.fog = false;
        
        // Force a skybox refresh
        DynamicGI.UpdateEnvironment();
    }
}