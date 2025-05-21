using UnityEngine;

public class IonTextureGenerator : MonoBehaviour
{
    public int textureSize = 256;
    public Color coreColor = new Color(0.5f, 0.9f, 1f, 1f);
    public Color glowColor = new Color(0.2f, 0.6f, 1f, 0.0f);
    public Material targetMaterial;
    
    void Start()
    {
        GenerateIonTexture();
    }
    
    void GenerateIonTexture()
    {
        // Create a new texture
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        
        // Calculate the center and radius
        Vector2 center = new Vector2(textureSize / 2, textureSize / 2);
        float maxRadius = textureSize / 2;
        
        // Generate the ion texture pixel by pixel
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                // Calculate distance from center (normalized 0-1)
                float distance = Vector2.Distance(new Vector2(x, y), center) / maxRadius;
                
                // Set alpha to create a soft circular edge - critical for proper particle shape
                float alpha = 1.0f - Mathf.Clamp01(Mathf.Pow(distance, 2.0f));
                
                // Make outer pixels completely transparent for circular shape
                if (distance > 0.98f) alpha = 0f;
                
                // Apply a radial gradient for a glowing effect
                // Inverse distance for inner to outer gradient
                float t = 1.0f - Mathf.Clamp01(distance);
                
                // Make the gradient non-linear for a more pronounced glow
                t = Mathf.Pow(t, 2.0f);
                
                // Blend between core color and glow color based on distance
                Color pixelColor = Color.Lerp(glowColor, coreColor, t);
                
                // Apply the calculated alpha
                pixelColor.a = alpha;
                
                // Set the pixel color
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        // Apply the changes to the texture
        texture.Apply();
        
        // Set texture properties
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        
        // Apply the texture to the target material
        if (targetMaterial != null)
        {
            targetMaterial.mainTexture = texture;
            targetMaterial.SetTexture("_BaseMap", texture);
        }
    }
}