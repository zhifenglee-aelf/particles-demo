using UnityEngine;

public class DetailedIonTextureGenerator : MonoBehaviour
{
    public int textureSize = 256;
    public Color coreColor = new Color(0.7f, 0.95f, 1f, 1f);
    public Color midColor = new Color(0.4f, 0.8f, 1f, 0.8f);
    public Color outerColor = new Color(0.2f, 0.6f, 1f, 0f);
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
                
                // Set alpha to create a soft circular edge
                float alpha = 1.0f - Mathf.Clamp01(Mathf.Pow(distance, 1.8f));
                
                // Make outer pixels completely transparent for circular shape
                if (distance > 0.98f) alpha = 0f;
                
                // Calculate angle for energy pattern
                float angle = Mathf.Atan2(y - center.y, x - center.x);
                float normalizedAngle = (angle + Mathf.PI) / (2f * Mathf.PI);
                
                // Create energy pattern
                float energyPattern = 
                    Mathf.PerlinNoise(normalizedAngle * 6f, distance * 5f) * 0.5f + 
                    Mathf.PerlinNoise(normalizedAngle * 12f, distance * 8f) * 0.3f +
                    Mathf.Sin(normalizedAngle * 20f + distance * 5f) * 0.2f;
                
                // Enhance the core
                float coreIntensity = Mathf.Clamp01(1.0f - distance * 3f);
                
                // Blend colors based on distance and energy pattern
                Color pixelColor;
                if (distance < 0.3f)
                {
                    // Core with energy fluctuations
                    pixelColor = Color.Lerp(coreColor, midColor, distance / 0.3f * (0.5f + energyPattern * 0.5f));
                }
                else 
                {
                    // Outer glow with energy patterns
                    float t = (distance - 0.3f) / 0.7f;
                    pixelColor = Color.Lerp(midColor, outerColor, t);
                    
                    // Add energy pattern to outer area
                    pixelColor += new Color(
                        energyPattern * 0.2f * (1f-t), 
                        energyPattern * 0.3f * (1f-t), 
                        energyPattern * 0.4f * (1f-t), 
                        0);
                }
                
                // Apply the calculated alpha
                pixelColor.a = alpha;
                
                // Enhance brightness at core
                if (distance < 0.2f)
                {
                    float coreBrightness = (0.2f - distance) / 0.2f;
                    pixelColor.r += coreBrightness * 0.3f;
                    pixelColor.g += coreBrightness * 0.3f;
                    pixelColor.b += coreBrightness * 0.3f;
                    pixelColor = Color.Lerp(pixelColor, coreColor, coreIntensity);
                }
                
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