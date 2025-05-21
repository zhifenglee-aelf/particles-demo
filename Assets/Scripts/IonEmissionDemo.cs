using UnityEngine;

public class IonEmissionDemo : MonoBehaviour
{
    [Header("Ion Controller References")]
    public IonEmissionController mainController;
    
    [Header("Demo Settings")]
    public KeyCode toggleEmittersKey = KeyCode.Space;
    public KeyCode increaseRateKey = KeyCode.UpArrow;
    public KeyCode decreaseRateKey = KeyCode.DownArrow;
    public KeyCode changeColorKey = KeyCode.C;
    
    [Header("Rate Settings")]
    public float minRate = 1f;
    public float maxRate = 20f;
    public float rateChangeStep = 1f;
    private float currentRate = 5f;
    
    [Header("Color Options")]
    public Color[] availableColors;
    private int currentColorIndex = 0;
    
    private bool emittersActive = true;
    
    void Awake()
    {
        Debug.Log("IonEmissionDemo: Awake called");
        
        // Initialize controller early to make sure it's ready before Start
        InitializeController();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IonEmissionDemo: Start called");
        
        // Make sure controller is initialized
        if (mainController == null)
        {
            InitializeController();
        }
        
        // Ensure emitters are active at start
        if (mainController != null)
        {
            mainController.ToggleEmitters(true);
        }
    }
    
    private void InitializeController()
    {
        // Create a controller if none is assigned
        if (mainController == null)
        {
            Debug.Log("IonEmissionDemo: Creating new controller");
            GameObject controllerObject = new GameObject("IonEmissionController");
            mainController = controllerObject.AddComponent<IonEmissionController>();
            
            // Set default options
            mainController.numberOfEmitters = 3;
            mainController.arrangeInCircle = true;
            mainController.circleRadius = 1.5f;
            
            // Set up color variations
            if (availableColors == null || availableColors.Length == 0)
            {
                Debug.Log("IonEmissionDemo: Setting up default colors");
                availableColors = new Color[]
                {
                    new Color(0.7f, 0.95f, 1f, 1f),  // Blue-ish
                    new Color(1f, 0.7f, 0.4f, 1f),   // Orange-ish
                    new Color(0.5f, 1f, 0.5f, 1f),   // Green-ish
                    new Color(1f, 0.5f, 0.8f, 1f)    // Pink-ish
                };
            }
            
            // Set color variations and make sure particle material can use them
            mainController.randomizeColors = true;
            mainController.colorVariations = availableColors;
        }
        else
        {
            Debug.Log("IonEmissionDemo: Using existing controller: " + mainController.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mainController == null)
        {
            Debug.LogError("IonEmissionDemo: No controller found in Update");
            return;
        }
        
        // Toggle emitters on/off
        if (Input.GetKeyDown(toggleEmittersKey))
        {
            emittersActive = !emittersActive;
            mainController.ToggleEmitters(emittersActive);
            Debug.Log("Ion emitters are now " + (emittersActive ? "active" : "inactive"));
        }
        
        // Adjust emission rate
        if (Input.GetKey(increaseRateKey))
        {
            currentRate = Mathf.Min(currentRate + rateChangeStep * Time.deltaTime * 5f, maxRate);
            mainController.SetEmissionRate(currentRate);
            Debug.Log("Emission rate: " + currentRate.ToString("F1"));
        }
        else if (Input.GetKey(decreaseRateKey))
        {
            currentRate = Mathf.Max(currentRate - rateChangeStep * Time.deltaTime * 5f, minRate);
            mainController.SetEmissionRate(currentRate);
            Debug.Log("Emission rate: " + currentRate.ToString("F1"));
        }
        
        // Change colors
        if (Input.GetKeyDown(changeColorKey) && availableColors.Length > 0)
        {
            currentColorIndex = (currentColorIndex + 1) % availableColors.Length;
            mainController.ionColor = availableColors[currentColorIndex];
            
            // If we have emitters already, we need to recreate them with the new color
            mainController.CreateIonEmitters();
            Debug.Log("Changed ion color to index " + currentColorIndex);
        }
    }
    
    void OnGUI()
    {
        // Display controls information
        GUI.Label(new Rect(10, 10, 300, 20), "Press SPACE to toggle ion emitters");
        GUI.Label(new Rect(10, 30, 300, 20), "Press UP/DOWN arrows to adjust emission rate");
        GUI.Label(new Rect(10, 50, 300, 20), "Press C to cycle through colors");
        GUI.Label(new Rect(10, 80, 300, 20), "Current Rate: " + currentRate.ToString("F1"));
        GUI.Label(new Rect(10, 100, 300, 20), "Emitters: " + (emittersActive ? "Active" : "Inactive"));
    }
} 