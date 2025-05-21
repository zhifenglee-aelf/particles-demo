using UnityEngine;

public class ParticleTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public float emissionRate = 10f;
    public float particleLifetime = 1f;
    public float minSize = 0.02f;
    public float maxSize = 0.05f;
    public Color startColor = new Color(0.5f, 0.8f, 1f, 1f);
    public Color endColor = new Color(0f, 0.5f, 1f, 0f);
    public Material trailMaterial;
    
    private ParticleSystem trailParticles;
    private ParticleSystem.EmissionModule emissionModule;
    private Transform parentTransform;
    private Vector3 lastPosition;
    
    void Start()
    {
        parentTransform = transform;
        lastPosition = parentTransform.position;
        
        // Create trail particle system
        GameObject trailObj = new GameObject("TrailEffect");
        trailObj.transform.parent = transform;
        trailObj.transform.localPosition = Vector3.zero;
        
        trailParticles = trailObj.AddComponent<ParticleSystem>();
        
        // Configure the particle system
        var main = trailParticles.main;
        main.startLifetime = particleLifetime;
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startColor = startColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // Configure emission
        emissionModule = trailParticles.emission;
        emissionModule.rateOverTime = 0;
        emissionModule.enabled = true;
        
        // Configure renderer
        var renderer = trailParticles.GetComponent<ParticleSystemRenderer>();
        if (trailMaterial != null)
            renderer.material = trailMaterial;
        else
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        
        // Color over lifetime
        var colorOverLifetime = trailParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);
    }
    
    void Update()
    {
        // Calculate the distance moved since last frame
        float distanceMoved = Vector3.Distance(lastPosition, parentTransform.position);
        
        // Emit particles based on movement
        if (distanceMoved > 0)
        {
            int particlesToEmit = Mathf.RoundToInt(distanceMoved * emissionRate);
            trailParticles.Emit(particlesToEmit);
        }
        
        lastPosition = parentTransform.position;
    }
}