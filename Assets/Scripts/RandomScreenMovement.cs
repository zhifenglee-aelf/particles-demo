using UnityEngine;

public class RandomScreenMovement : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float changeDirectionInterval = 3.0f;
    public float screenBoundaryPadding = 0.1f; // Keep away from edges
    
    private Vector3 targetPosition;
    private Camera mainCamera;
    private float timer;
    private Vector3 initialPosition;
    
    private void Start()
    {
        mainCamera = Camera.main;
        initialPosition = transform.position;
        SetNewRandomTarget();
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SetNewRandomTarget();
            timer = changeDirectionInterval;
        }
        
        // Smooth movement towards target
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    
    private void SetNewRandomTarget()
    {
        // Calculate a random position in screen space
        Vector3 randomScreenPosition = new Vector3(
            Random.Range(screenBoundaryPadding, 1f - screenBoundaryPadding), 
            Random.Range(screenBoundaryPadding, 1f - screenBoundaryPadding),
            10f // Set this to be in front of the camera
        );
        
        // Convert screen position to world position
        targetPosition = mainCamera.ViewportToWorldPoint(randomScreenPosition);
        
        // Keep the same Z position as initial
        targetPosition.z = initialPosition.z;
    }
}