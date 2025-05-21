using UnityEngine;

public class SkyboxSetter : MonoBehaviour
{
    public Material skyboxMaterial;

    void Awake()
    {
        // Apply the skybox if material is assigned
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
        else
        {
            Debug.LogError("Skybox material not assigned. Please assign it in the inspector.");
        }
    }
}