using UnityEngine;
using System.Collections.Generic;

public class IonEmissionTextureAssigner : MonoBehaviour
{
    [Header("Texture Settings")]
    public Texture2D ionTexture;
    public string texturePath = "Assets/UnityTechnologies/ParticlePack/EffectExamples/Magic Effects/Textures/SmokePuff.png";
    public bool findTextureAtRuntime = true;
    public bool applyToAllEmitters = true;
    
    void Start()
    {
        // If we don't have a texture assigned, try to find one
        if (ionTexture == null && findTextureAtRuntime)
        {
            FindAndAssignTexture();
        }
        
        // Apply to all emitters in the scene
        if (applyToAllEmitters)
        {
            ApplyTextureToAllEmitters();
        }
    }
    
    void FindAndAssignTexture()
    {
        Debug.Log("IonEmissionTextureAssigner: Searching for texture...");
        
        // Try direct loading using AssetDatabase
        #if UNITY_EDITOR
        ionTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (ionTexture != null)
        {
            Debug.Log("IonEmissionTextureAssigner: Found texture at " + texturePath);
            return;
        }
        
        // Try to find any texture in the project that might work for particles
        string[] searchTerms = new string[] { "SmokePuff", "Glow", "Particle", "DustMote", "Twinkle" };
        
        foreach (string term in searchTerms)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:texture2d " + term);
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                ionTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (ionTexture != null)
                {
                    Debug.Log("IonEmissionTextureAssigner: Found texture using search term '" + term + "' at " + path);
                    texturePath = path;
                    return;
                }
            }
        }
        
        // Last resort: find any texture in UnityTechnologies folder
        string[] allTextureGuids = UnityEditor.AssetDatabase.FindAssets("t:texture2d", new[] { "Assets/UnityTechnologies" });
        if (allTextureGuids.Length > 0)
        {
            // Prefer ones with "particle" in the name
            foreach (string guid in allTextureGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (path.ToLower().Contains("particle") || path.ToLower().Contains("effect"))
                {
                    ionTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (ionTexture != null)
                    {
                        Debug.Log("IonEmissionTextureAssigner: Found particle texture at " + path);
                        texturePath = path;
                        return;
                    }
                }
            }
            
            // If still not found, use the first texture found
            string firstPath = UnityEditor.AssetDatabase.GUIDToAssetPath(allTextureGuids[0]);
            ionTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(firstPath);
            if (ionTexture != null)
            {
                Debug.Log("IonEmissionTextureAssigner: Found texture at " + firstPath);
                texturePath = firstPath;
                return;
            }
        }
        #endif
        
        // If no texture was found, create a fallback
        if (ionTexture == null)
        {
            Debug.LogWarning("IonEmissionTextureAssigner: No suitable texture found. Creating a fallback texture.");
            CreateFallbackTexture();
        }
    }
    
    void CreateFallbackTexture()
    {
        // Create a simple circular gradient texture
        int textureSize = 256;
        ionTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        
        Vector2 center = new Vector2(textureSize / 2, textureSize / 2);
        float maxRadius = textureSize / 2;
        
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / maxRadius;
                float alpha = Mathf.Clamp01(1f - Mathf.Pow(distance, 2f));
                if (distance > 0.95f) alpha = 0f;
                
                Color color = new Color(1f, 1f, 1f, alpha);
                ionTexture.SetPixel(x, y, color);
            }
        }
        
        ionTexture.Apply();
        ionTexture.wrapMode = TextureWrapMode.Clamp;
        ionTexture.filterMode = FilterMode.Bilinear;
    }
    
    void ApplyTextureToAllEmitters()
    {
        if (ionTexture == null)
        {
            Debug.LogError("IonEmissionTextureAssigner: No texture to apply!");
            return;
        }
        
        // Find all IonEmissionEffect components in the scene
        IonEmissionEffect[] emitters = FindObjectsOfType<IonEmissionEffect>();
        
        Debug.Log("IonEmissionTextureAssigner: Found " + emitters.Length + " ion emitters to apply texture to");
        
        foreach (IonEmissionEffect emitter in emitters)
        {
            // Set to use custom texture
            emitter.useCustomTexture = true;
            emitter.particleTexture = ionTexture;
            
            // Update the material if it already exists
            ParticleSystemRenderer renderer = emitter.GetComponent<ParticleSystemRenderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.mainTexture = ionTexture;
                renderer.material.SetTexture("_BaseMap", ionTexture);
                Debug.Log("IonEmissionTextureAssigner: Applied texture to " + emitter.gameObject.name);
            }
        }
    }
} 