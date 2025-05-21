using UnityEditor;
using UnityEngine;

public class SetBlackSkybox : Editor
{
    [MenuItem("Window/Rendering/Set Black Skybox")]
    static void SetSkybox()
    {
        // Find the black skybox material
        Material skyboxMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/BlackSkybox.mat");
        
        if (skyboxMaterial != null)
        {
            // Apply the skybox material to the render settings
            RenderSettings.skybox = skyboxMaterial;
            
            Debug.Log("Black skybox applied successfully!");
        }
        else
        {
            Debug.LogError("Black skybox material not found at path: Assets/Materials/BlackSkybox.mat");
        }
    }
}