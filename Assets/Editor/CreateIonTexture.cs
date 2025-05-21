using UnityEngine;
using UnityEditor;

public class CreateIonTexture : EditorWindow
{
    [MenuItem("Assets/Create/Ion Particle Texture")]
    static void CreateTexture()
    {
        // Create a new texture
        int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        // Calculate the center and radius
        Vector2 center = new Vector2(size / 2, size / 2);
        float maxRadius = size / 2;
        
        // Generate the ion texture pixel by pixel
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Calculate distance from center (normalized 0-1)
                float distance = Vector2.Distance(new Vector2(x, y), center) / maxRadius;
                
                // Create a circular alpha mask
                float alpha = 1.0f - Mathf.Pow(Mathf.Clamp01(distance), 1.5f);
                
                // Ensure it's clearly circular
                if (distance > 1.0f) alpha = 0f;
                
                // Create a bright core that fades out
                float intensity = Mathf.Pow(1.0f - Mathf.Clamp01(distance), 2.0f);
                
                // Base color - blue for ions
                Color color = new Color(0.5f + intensity * 0.5f, 0.8f + intensity * 0.2f, 1.0f, alpha);
                
                // Set the pixel color
                texture.SetPixel(x, y, color);
            }
        }
        
        // Apply all pixel changes
        texture.Apply();
        
        // Save the texture to disk
        string path = "Assets/Textures/IonParticle.png";
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        
        // Update the texture import settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            
            // Apply the import settings
            importer.SaveAndReimport();
        }
        
        Debug.Log("Ion particle texture created at: " + path);
    }
}