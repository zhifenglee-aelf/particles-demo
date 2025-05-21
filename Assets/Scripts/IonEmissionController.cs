using UnityEngine;

public class IonEmissionController : MonoBehaviour
{
    [Header("Emission Settings")]
    public int numberOfEmitters = 1;
    public float emitterSpacing = 1.0f;
    public bool arrangeInCircle = false;
    public float circleRadius = 2.0f;
    
    [Header("Particle Settings")]
    public bool randomizeDirection = true;
    public Vector3 baseDirection = Vector3.up;
    public float emissionSpeed = 2.0f;
    public float speedVariation = 0.5f;
    
    [Header("Visual Settings")]
    public Color ionColor = new Color(0.7f, 0.95f, 1f, 1f);
    public bool randomizeColors = false;
    public Color[] colorVariations;
    
    [Header("Texture Settings")]
    public Texture2D particleTexture;
    public string texturePath = "Assets/UnityTechnologies/ParticlePack/EffectExamples/Magic Effects/Textures/DustPuffSmall.png";
    
    [Header("Animation Settings")]
    public bool enableTextureAnimation = true;
    public int gridSizeX = 4;
    public int gridSizeY = 4;
    public float animationSpeed = 10f;
    
    [Header("Rendering Settings")]
    public ParticleSystemRenderMode renderMode = ParticleSystemRenderMode.Billboard;
    public bool useAlphaBlending = true;
    public float particleIntensity = 1.2f;
    
    private GameObject[] emitters;
    
    void Awake()
    {
        // Try to find and load a texture if none is assigned
        FindParticleTexture();
    }
    
    void Start()
    {
        Debug.Log("IonEmissionController: Start called on " + gameObject.name);
        CreateIonEmitters();
    }
    
    void FindParticleTexture()
    {
        if (particleTexture != null)
            return;
            
        Debug.Log("IonEmissionController: Looking for particle texture...");
        
        // Try to load the texture from the path
        #if UNITY_EDITOR
        particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        
        if (particleTexture != null)
        {
            Debug.Log("IonEmissionController: Found texture at " + texturePath);
            EnsureProperTextureImportSettings(particleTexture);
            return;
        }
        
        // Try to find DustPuffSmall specifically
        string[] dustPuffGuids = UnityEditor.AssetDatabase.FindAssets("t:texture2d DustPuffSmall");
        if (dustPuffGuids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(dustPuffGuids[0]);
            particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (particleTexture != null)
            {
                Debug.Log("IonEmissionController: Found DustPuffSmall texture at " + path);
                texturePath = path;
                EnsureProperTextureImportSettings(particleTexture);
                return;
            }
        }
        
        // Try alternate paths for DustPuffSmall
        string[] dustPuffPaths = new string[]
        {
            "Assets/UnityTechnologies/ParticlePack/EffectExamples/Misc Effects/Textures/DustPuffSmall.png",
            "Assets/UnityTechnologies/ParticlePack/EffectExamples/Weapon Effects/Textures/DustPuffSmallParticleSheet.png",
            "Assets/UnityTechnologies/ParticlePack/EffectExamples/Magic Effects/Textures/DustPuffSmall.png"
        };
        
        foreach (string path in dustPuffPaths)
        {
            particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (particleTexture != null)
            {
                Debug.Log("IonEmissionController: Found DustPuffSmall texture at " + path);
                texturePath = path;
                EnsureProperTextureImportSettings(particleTexture);
                return;
            }
        }
        
        // Fall back to other textures if DustPuffSmall is not found
        string[] searchTerms = new string[] { "DustPuff", "Particle", "SmokePuff", "Glow", "Twinkle" };
        
        foreach (string term in searchTerms)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:texture2d " + term);
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (particleTexture != null)
                {
                    Debug.Log("IonEmissionController: Found texture using search term '" + term + "' at " + path);
                    texturePath = path;
                    EnsureProperTextureImportSettings(particleTexture);
                    return;
                }
            }
        }
        
        // Last resort: try to find any texture in UnityTechnologies folder
        string[] allTextureGuids = UnityEditor.AssetDatabase.FindAssets("t:texture2d", new[] { "Assets/UnityTechnologies" });
        if (allTextureGuids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(allTextureGuids[0]);
            particleTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (particleTexture != null)
            {
                Debug.Log("IonEmissionController: Found texture at " + path);
                texturePath = path;
                EnsureProperTextureImportSettings(particleTexture);
            }
        }
        #endif
    }
    
    #if UNITY_EDITOR
    private void EnsureProperTextureImportSettings(Texture2D texture)
    {
        if (texture == null) return;
        
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(assetPath)) return;
        
        UnityEditor.TextureImporter importer = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.TextureImporter>(assetPath);
        if (importer == null) return;
        
        bool needsReimport = false;
        
        // Make sure alpha is enabled
        if (!importer.alphaIsTransparency)
        {
            importer.alphaIsTransparency = true;
            needsReimport = true;
        }
        
        // Make sure texture has proper settings for particles
        if (importer.textureType != UnityEditor.TextureImporterType.Default)
        {
            importer.textureType = UnityEditor.TextureImporterType.Default;
            needsReimport = true;
        }
        
        // For DustPuffSmall, ensure it's set up as a sprite sheet
        if (texture.name.Contains("DustPuff") && importer.spriteImportMode != UnityEditor.SpriteImportMode.Multiple)
        {
            importer.spriteImportMode = UnityEditor.SpriteImportMode.Multiple;
            needsReimport = true;
        }
        
        // Apply changes if needed
        if (needsReimport)
        {
            Debug.Log("IonEmissionController: Updating texture import settings for proper alpha channel support");
            importer.SaveAndReimport();
        }
    }
    #endif
    
    public void CreateIonEmitters()
    {
        Debug.Log("IonEmissionController: Creating " + numberOfEmitters + " emitters");
        
        // Clear existing emitters if any
        if (emitters != null)
        {
            foreach (GameObject emitter in emitters)
            {
                if (emitter != null)
                {
                    Destroy(emitter);
                }
            }
        }
        
        emitters = new GameObject[numberOfEmitters];
        
        for (int i = 0; i < numberOfEmitters; i++)
        {
            // Create emitter GameObject
            GameObject emitter = new GameObject("IonEmitter_" + i);
            emitter.transform.parent = transform;
            
            // Position the emitter
            if (arrangeInCircle)
            {
                float angle = (360f / numberOfEmitters) * i * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * circleRadius;
                float z = Mathf.Sin(angle) * circleRadius;
                emitter.transform.localPosition = new Vector3(x, 0, z);
            }
            else
            {
                emitter.transform.localPosition = new Vector3(i * emitterSpacing, 0, 0);
            }
            
            // Add ion emission component
            IonEmissionEffect ionEffect = emitter.AddComponent<IonEmissionEffect>();
            Debug.Log("IonEmissionController: Added IonEmissionEffect to " + emitter.name);
            
            // Set render mode
            ionEffect.GetComponent<ParticleSystemRenderer>().renderMode = renderMode;
            
            // Set alpha blending option
            ionEffect.useCustomTexture = true;
            
            // Set custom texture if available
            if (particleTexture != null)
            {
                ionEffect.particleTexture = particleTexture;
                
                // Configure texture animation if this is an animated texture
                ionEffect.useTextureAnimation = enableTextureAnimation;
                ionEffect.gridSizeX = gridSizeX;
                ionEffect.gridSizeY = gridSizeY;
                ionEffect.animationSpeed = animationSpeed;
                
                Debug.Log("IonEmissionController: Assigned texture and animation settings to " + emitter.name);
                
                // Check if we need to manually adjust the particle renderer
                ParticleSystemRenderer renderer = ionEffect.GetComponent<ParticleSystemRenderer>();
                if (renderer != null && renderer.material != null)
                {
                    if (useAlphaBlending)
                    {
                        // Use transparency blend mode
                        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        renderer.material.SetFloat("_Mode", 2); // Fade mode
                        renderer.material.SetInt("_ZWrite", 0);
                        renderer.material.renderQueue = 3000;
                        renderer.material.EnableKeyword("_ALPHABLEND_ON");
                    }
                    else
                    {
                        // Use additive blend mode for glow effect
                        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    }
                    
                    // Set intensity
                    renderer.material.SetFloat("_Intensity", particleIntensity);
                }
            }
            
            // Configure emission direction
            if (randomizeDirection)
            {
                Vector3 randomDir = Random.insideUnitSphere.normalized;
                randomDir.y = Mathf.Abs(randomDir.y); // Make sure it's pointing at least partially upward
                ionEffect.emissionDirection = randomDir;
            }
            else
            {
                ionEffect.emissionDirection = baseDirection;
            }
            
            // Apply random speed variation
            float speedMultiplier = 1.0f + Random.Range(-speedVariation, speedVariation);
            ionEffect.particleStartSpeed = emissionSpeed * speedMultiplier;
            
            // Apply color settings
            if (randomizeColors && colorVariations != null && colorVariations.Length > 0)
            {
                ionEffect.coreColor = colorVariations[Random.Range(0, colorVariations.Length)];
            }
            else
            {
                ionEffect.coreColor = ionColor;
            }
            
            // Store reference to the emitter
            emitters[i] = emitter;
        }
    }
    
    public void ToggleEmitters(bool active)
    {
        Debug.Log("IonEmissionController: Toggling emitters to " + (active ? "active" : "inactive"));
        
        if (emitters == null)
        {
            Debug.LogWarning("IonEmissionController: Cannot toggle emitters - emitters array is null");
            return;
        }
        
        foreach (GameObject emitter in emitters)
        {
            if (emitter != null)
            {
                ParticleSystem particleSystem = emitter.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    if (active)
                    {
                        Debug.Log("IonEmissionController: Playing particles on " + emitter.name);
                        particleSystem.Play(true);
                    }
                    else
                    {
                        Debug.Log("IonEmissionController: Stopping particles on " + emitter.name);
                        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
                else
                {
                    Debug.LogWarning("IonEmissionController: No ParticleSystem found on " + emitter.name);
                }
            }
        }
    }
    
    public void SetEmissionRate(float rate)
    {
        if (emitters == null) return;
        
        foreach (GameObject emitter in emitters)
        {
            if (emitter != null)
            {
                IonEmissionEffect ionEffect = emitter.GetComponent<IonEmissionEffect>();
                if (ionEffect != null)
                {
                    ionEffect.emissionRate = rate;
                    
                    // Also update the emission rate directly on the particle system
                    ParticleSystem particleSystem = emitter.GetComponent<ParticleSystem>();
                    if (particleSystem != null)
                    {
                        var emission = particleSystem.emission;
                        emission.rateOverTime = rate;
                    }
                }
            }
        }
    }
} 