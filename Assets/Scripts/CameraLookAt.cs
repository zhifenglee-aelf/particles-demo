using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Transform target;
    
    void Start()
    {
        // Find the particle system if target is not set
        if (target == null)
        {
            GameObject particleSystem = GameObject.Find("Particle System");
            if (particleSystem != null)
            {
                target = particleSystem.transform;
            }
        }
    }
    
    void Update()
    {
        if (target != null)
        {
            // Look at the target
            transform.LookAt(target);
        }
    }
}