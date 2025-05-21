using UnityEngine;

public class IonEmissionEffect : MonoBehaviour
{
    [Header("Ion Particle Settings")]
    public float emissionRate = 5f;
    public float particleLifetime = 1.5f;
    public float particleStartSpeed = 2f;
    public float particleStartSize = 0.2f;
    public float particleEndSize = 0.05f;
    public Vector3 emissionDirection = Vector3.up;
    public float emissionConeAngle = 15f;
    
    [Header("Ion Color Settings")]
    public Color coreColor = new Color(0.7f, 0.95f, 1f, 1f);
    public Color trailColor = new Color(0.3f, 0.8f, 1f, 0.5f);
    
    [Header("Energy Pulse Settings")]
    public float pulseFrequency = 1.2f;
    public float pulseIntensity = 0.3f;
    
    [Header("Texture Settings")]
    public bool useCustomTexture = true;
    public Texture2D particleTexture;
    public string defaultTexturePath = "Assets/UnityTechnologies/ParticlePack/EffectExamples/Magic Effects/Textures/DustPuffSmall.png";
    
    [Header("Animation Settings")]
    public bool useTextureAnimation = true;
    public int gridSizeX = 4;
    public int gridSizeY = 4;
    public float animationSpeed = 10f;
    public bool randomStartFrame = true;
    
    private ParticleSystem particleSystem;
    private ParticleSystemRenderer particleRenderer;
    private Material particleMaterial;
    
    void Awake()
    {
        Debug.Log("IonEmissionEffect: Awake called on " + gameObject.name);
        // Create the particle system
        InitializeParticleSystem();
        
        // Setup the particle texture
        SetupParticleTexture();
    }
    
    void InitializeParticleSystem()
    {
        // Create particle system if it doesn't exist
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.Log("IonEmissionEffect: Creating new ParticleSystem on " + gameObject.name);
            particleSystem = gameObject.AddComponent<ParticleSystem>();
        }
        
        // Get or create the particle renderer
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        if (particleRenderer == null)
        {
            particleRenderer = gameObject.AddComponent<ParticleSystemRenderer>();
        }
        
        Debug.Log("IonEmissionEffect: Configuring particle system on " + gameObject.name);
        
        // Main module configuration
        var main = particleSystem.main;
        main.startLifetime = particleLifetime;
        main.startSpeed = particleStartSpeed;
        main.startSize = particleStartSize;
        main.startColor = coreColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 100;
        
        // Emission module configuration
        var emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = emissionRate;
        Debug.Log("IonEmissionEffect: Set emission rate to " + emissionRate);
        
        // Shape module for emission direction and cone
        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = emissionConeAngle;
        shape.rotation = Quaternion.LookRotation(emissionDirection).eulerAngles;
        
        // Size over lifetime - particles get smaller
        var sizeOverLifetime = particleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, particleEndSize / particleStartSize);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        // Color over lifetime - fade out
        var colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient colorGradient = new Gradient();
        colorGradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(coreColor, 0f), 
                new GradientColorKey(trailColor, 0.5f),
                new GradientColorKey(trailColor, 1f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f), 
                new GradientAlphaKey(0.7f, 0.7f),
                new GradientAlphaKey(0f, 1f) 
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(colorGradient);
        
        // Texture Sheet Animation module for animated texture
        var textureSheetAnimation = particleSystem.textureSheetAnimation;
        textureSheetAnimation.enabled = useTextureAnimation;
        textureSheetAnimation.mode = ParticleSystemAnimationMode.Grid;
        textureSheetAnimation.numTilesX = gridSizeX;
        textureSheetAnimation.numTilesY = gridSizeY;
        
        // Set animation speed
        textureSheetAnimation.frameOverTime = new ParticleSystem.MinMaxCurve(animationSpeed);
        
        // Random starting frame if desired
        if (randomStartFrame)
        {
            textureSheetAnimation.startFrame = new ParticleSystem.MinMaxCurve(0f, 1f);
        }
        else
        {
            textureSheetAnimation.startFrame = new ParticleSystem.MinMaxCurve(0f);
        }
        
        // Set cycling mode
        textureSheetAnimation.cycleCount = 1; // Play once through lifetime
        
        // Setup materials and rendering
        SetupParticleMaterial();
        
        // Ensure particles start playing
        particleSystem.Play(true);
        Debug.Log("IonEmissionEffect: Particle system started playing");
    }
    
    void SetupParticleMaterial()
    {
        // Renderer settings
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        // Create material with proper transparent shader
        Material material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        
        // If the URP shader isn't found, try standard particles shader
        if (material.shader == null || material.shader.name.Contains("Hidden"))
        {
            Debug.Log("IonEmissionEffect: URP shader not found, falling back to standard shader");
            material = new Material(Shader.Find("Particles/Standard Unlit"));
            
            // If still not found, use a built-in transparent shader
            if (material.shader == null || material.shader.name.Contains("Hidden"))
            {
                Debug.Log("IonEmissionEffect: Falling back to built-in transparent shader");
                material = new Material(Shader.Find("Sprites/Default"));
            }
        }
        
        particleMaterial = material;
        particleRenderer.material = particleMaterial;
        
        // Configure transparency
        particleMaterial.SetFloat("_Mode", 2); // Fade mode for standard shader
        particleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        particleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha); // Change to OneMinusSrcAlpha for proper alpha blending
        particleMaterial.SetInt("_ZWrite", 0); // Don't write to depth buffer
        particleMaterial.DisableKeyword("_ALPHATEST_ON");
        particleMaterial.EnableKeyword("_ALPHABLEND_ON");
        particleMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        particleMaterial.renderQueue = 3000; // Transparent render queue
        
        // For URP, set these values
        if (particleMaterial.HasProperty("_Surface"))
        {
            particleMaterial.SetFloat("_Surface", 1); // 1 = Transparent
        }
        
        if (particleMaterial.HasProperty("_Blend"))
        {
            particleMaterial.SetFloat("_Blend", 0); // 0 = SrcAlpha, OneMinusSrcAlpha
        }
        
        // Set alpha test properties if needed
        if (particleMaterial.HasProperty("_AlphaClip"))
        {
            particleMaterial.SetFloat("_AlphaClip", 0); // No alpha clipping
        }
        
        particleMaterial.SetFloat("_Intensity", 1.2f); // Increase brightness
        particleMaterial.EnableKeyword("_EMISSION");
    }
    
    void SetupParticleTexture()
    {
        if (useCustomTexture)
        {
            // Try to find the DustPuffSmall texture specifically
            FindDustPuffTexture();
            
            // Apply the texture to the material
            if (particleTexture != null && particleMaterial != null)
            {
                Debug.Log("IonEmissionEffect: Applied custom texture to material");
                particleMaterial.mainTexture = particleTexture;
                particleMaterial.SetTexture("_BaseMap", particleTexture);
                
                // Make sure the material is using the alpha channel
                particleMaterial.SetFloat("_Mode", 2); // Fade mode
                particleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                particleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                particleMaterial.SetInt("_ZWrite", 0);
                particleMaterial.renderQueue = 3000;
                
                // If using texture animation, update the texture sheet animation settings
                if (useTextureAnimation && particleTexture.name.Contains("DustPuff"))
                {
                    ConfigureTextureSheetAnimation();
                }
            }
            else
            {
                Debug.LogWarning("IonEmissionEffect: Failed to find a suitable texture. Falling back to procedural generation.");
                // Fall back to procedural generation
                GenerateIonParticleTexture();
            }
        }
        else
        {
            // Use procedural texture generation
            GenerateIonParticleTexture();
        }
    }
    
    void FindDustPuffTexture()
    {
        // Try direct loading using AssetDatabase
        #if UNITY_EDITOR
        // First try to look specifically for DustPuffSmall
        string[] dustPuffGuids = UnityEditor.AssetDatabase.FindAssets("t:texture2d DustPuffSmall");
        if (dustPuffGuids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(dustPuffGuids[0]);
            particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (particleTexture != null)
            {
                Debug.Log("IonEmissionEffect: Found DustPuffSmall texture at " + path);
                return;
            }
        }
        
        // If not found, try loading from the default path
        particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(defaultTexturePath);
        if (particleTexture != null)
        {
            Debug.Log("IonEmissionEffect: Loaded texture from " + defaultTexturePath);
            return;
        }
        
        // If still not found, try other paths
        string[] fallbackPaths = new string[]
        {
            "Assets/UnityTechnologies/ParticlePack/EffectExamples/Misc Effects/Textures/DustPuffSmall.png",
            "Assets/UnityTechnologies/ParticlePack/EffectExamples/Weapon Effects/Textures/DustPuffSmallParticleSheet.png"
        };
        
        foreach (string path in fallbackPaths)
        {
            particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (particleTexture != null)
            {
                Debug.Log("IonEmissionEffect: Loaded DustPuff texture from " + path);
                return;
            }
        }
        
        // Last resort - try to find any texture with "DustPuff" in the name
        string[] anyDustPuffGuids = UnityEditor.AssetDatabase.FindAssets("DustPuff");
        foreach (string guid in anyDustPuffGuids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith(".png") || path.EndsWith(".tga") || path.EndsWith(".jpg"))
            {
                particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (particleTexture != null)
                {
                    Debug.Log("IonEmissionEffect: Found dust puff texture at " + path);
                    return;
                }
            }
        }
        
        // After loading the texture, ensure it has the correct texture import settings
        if (particleTexture != null)
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(particleTexture);
            if (!string.IsNullOrEmpty(assetPath))
            {
                UnityEditor.TextureImporter importer = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.TextureImporter>(assetPath);
                if (importer != null)
                {
                    bool needsReimport = false;
                    
                    // Check if alpha is from gray scale
                    if (importer.alphaSource != UnityEditor.TextureImporterAlphaSource.FromGrayScale)
                    {
                        importer.alphaSource = UnityEditor.TextureImporterAlphaSource.FromGrayScale;
                        needsReimport = true;
                    }
                    
                    // Make sure alpha is enabled
                    if (!importer.alphaIsTransparency)
                    {
                        importer.alphaIsTransparency = true;
                        needsReimport = true;
                    }
                    
                    // Make sure it uses proper texture type
                    if (importer.textureType != UnityEditor.TextureImporterType.Sprite &&
                        importer.textureType != UnityEditor.TextureImporterType.Default)
                    {
                        importer.textureType = UnityEditor.TextureImporterType.Default;
                        needsReimport = true;
                    }
                    
                    // Apply changes if needed
                    if (needsReimport)
                    {
                        Debug.Log("IonEmissionEffect: Updating texture import settings for proper alpha channel support");
                        importer.SaveAndReimport();
                    }
                }
            }
        }
        #endif
        
        // If we get here, we couldn't find the texture
        Debug.LogWarning("IonEmissionEffect: Could not find DustPuffSmall texture.");
    }
    
    void ConfigureTextureSheetAnimation()
    {
        if (particleSystem == null) return;
        
        var textureSheetAnimation = particleSystem.textureSheetAnimation;
        
        // Make sure the module is enabled
        textureSheetAnimation.enabled = true;
        
        // Configure based on DustPuffSmall which is typically a 4x4 grid
        textureSheetAnimation.numTilesX = gridSizeX;
        textureSheetAnimation.numTilesY = gridSizeY;
        
        // Set animation to cycle once through the particle's lifetime
        textureSheetAnimation.cycleCount = 1;
        
        // Create animation curve that starts at 0 and ends at 1 (full animation through lifetime)
        AnimationCurve animationCurve = new AnimationCurve();
        animationCurve.AddKey(0f, 0f);
        animationCurve.AddKey(1f, 1f);
        textureSheetAnimation.frameOverTime = new ParticleSystem.MinMaxCurve(1f, animationCurve);
        
        // Random starting frame if desired
        if (randomStartFrame)
        {
            textureSheetAnimation.startFrame = new ParticleSystem.MinMaxCurve(0f, 1f);
        }
        
        Debug.Log("IonEmissionEffect: Configured texture sheet animation for " + particleTexture.name);
    }
    
    Texture2D LoadTextureFromAssetDatabase()
    {
        // This works in the editor only
        #if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:texture2d DustPuffSmall");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        
        // Try other common particle texture names
        guids = UnityEditor.AssetDatabase.FindAssets("t:texture2d SmokePuff");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        
        // Try to find any texture in UnityTechnologies folder
        guids = UnityEditor.AssetDatabase.FindAssets("t:texture2d", new[] { "Assets/UnityTechnologies" });
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            Debug.Log("IonEmissionEffect: Found texture at " + path);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        #endif
        
        return null;
    }
    
    void GenerateIonParticleTexture()
    {
        Debug.Log("IonEmissionEffect: Generating procedural texture");
        int textureSize = 256;
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
                float alpha = 1.0f - Mathf.Clamp01(Mathf.Pow(distance, 2.0f));
                
                // Cut off outer edges completely
                if (distance > 0.95f) alpha = 0f;
                
                // Calculate color gradient from center to edge
                float t = 1.0f - Mathf.Clamp01(distance);
                t = Mathf.Pow(t, 1.5f); // Non-linear falloff for more glow effect
                
                // Blend between core and trail colors
                Color pixelColor = Color.Lerp(trailColor, coreColor, t);
                
                // Add some energy fluctuations
                float angle = Mathf.Atan2(y - center.y, x - center.x);
                float energyPattern = Mathf.PerlinNoise(angle * 3f, distance * 4f) * 0.3f;
                
                // Apply energy pattern more strongly at the center
                pixelColor += new Color(energyPattern * (1-distance) * 0.3f, 
                                       energyPattern * (1-distance) * 0.3f, 
                                       energyPattern * (1-distance) * 0.5f, 0);
                
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
        
        // Apply the texture to the material
        if (particleMaterial != null)
        {
            particleMaterial.mainTexture = texture;
            particleMaterial.SetTexture("_BaseMap", texture);
        }
        
        // Since we're using a procedural texture, disable texture sheet animation
        if (particleSystem != null)
        {
            var textureSheetAnimation = particleSystem.textureSheetAnimation;
            textureSheetAnimation.enabled = false;
        }
    }
    
    void Update()
    {
        // Apply energy pulse to emission rate
        if (particleSystem != null)
        {
            var emission = particleSystem.emission;
            float pulse = 1.0f + Mathf.Sin(Time.time * pulseFrequency) * pulseIntensity;
            emission.rateOverTime = emissionRate * pulse;
            
            // Also pulse the particle color slightly
            var main = particleSystem.main;
            Color pulsedColor = coreColor * (1f + (pulse - 1f) * 0.5f);
            main.startColor = pulsedColor;
        }
    }
} 