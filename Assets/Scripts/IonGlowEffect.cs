using UnityEngine;

public class IonGlowEffect : MonoBehaviour
{
    public ParticleSystem targetParticleSystem;
    public Color baseColor = new Color(0.3f, 0.8f, 1f, 1f);
    public Color glowColor = new Color(0.5f, 0.9f, 1f, 1f);
    public float pulseSpeed = 1.5f;
    public float pulseMinIntensity = 0.7f;
    public float pulseMaxIntensity = 1.3f;
    
    private ParticleSystem.MainModule mainModule;
    
    void Start()
    {
        if (targetParticleSystem == null)
        {
            targetParticleSystem = GetComponent<ParticleSystem>();
        }
        
        if (targetParticleSystem != null)
        {
            mainModule = targetParticleSystem.main;
        }
    }
    
    void Update()
    {
        if (targetParticleSystem == null) return;
        
        // Create a pulsating effect by lerping between colors
        float pulse = Mathf.Lerp(pulseMinIntensity, pulseMaxIntensity, 
                               (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
        
        // Apply the pulsating color
        Color currentColor = Color.Lerp(baseColor, glowColor, pulse - pulseMinIntensity);
        
        // Set the start color with the pulsing effect
        ParticleSystem.MinMaxGradient colorGradient = new ParticleSystem.MinMaxGradient(currentColor);
        mainModule.startColor = colorGradient;
    }
}