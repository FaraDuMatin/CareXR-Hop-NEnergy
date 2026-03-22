using UnityEngine;

/// <summary>
/// WarpPad: Creates the magical floating game arena platform.
/// Large flattened cylinder positioned under player feet at (0,0,0).
/// Provides visual boundary and VR safety containment.
/// Attach to empty GameObject named "WarpPad" at scene origin.
/// </summary>
public class WarpPad : MonoBehaviour
{
    [SerializeField] private Vector3 padScale = new Vector3(3f, 0.05f, 3f);
    [SerializeField] private Color padColor = new Color(0.4f, 0.8f, 1f, 1f);  // Crystal blue (like gems)
    [SerializeField] private Material platformMaterial;  // Optional: assign Gems.mat here
    [SerializeField] private float metallic = 0.8f;  // Shiny effect
    [SerializeField] private float smoothness = 0.9f;

    private GameObject padObject;
    private Renderer padRenderer;

    void Start()
    {
        CreateWarpPad();
    }

    /// <summary>
    /// Creates the warp pad cylinder with correct scale, position, and material.
    /// </summary>
    void CreateWarpPad()
    {
        if (padObject != null)
            return;  // Already created

        // Create cylinder primitive
        padObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        padObject.name = "WarpPad_Platform";
        padObject.transform.SetParent(transform);
        padObject.transform.localPosition = Vector3.zero;  // Position at (0, 0, 0)
        padObject.transform.localRotation = Quaternion.identity;
        padObject.transform.localScale = padScale;

        // Remove collider from trigger (keep it solid for physics)
        Collider collider = padObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }

        // Remove default mesh collider script if present
        Rigidbody rb = padObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;  // Static platform, no physics
        }

        // Assign or create material
        padRenderer = padObject.GetComponent<Renderer>();
        
        if (platformMaterial != null)
        {
            // Use assigned material (e.g., Gems.mat)
            padRenderer.material = platformMaterial;
            Debug.Log($"[WarpPad] Using assigned material: {platformMaterial.name}");
        }
        else
        {
            // Create crystal blue material (URP compatible)
            Material platMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            
            if (platMaterial.shader == null)
            {
                // Fallback for standard shader
                platMaterial = new Material(Shader.Find("Standard"));
            }
            
            // Set material properties for shiny crystal blue
            platMaterial.SetColor("_BaseColor", padColor);
            platMaterial.SetColor("_Color", padColor);  // Fallback for Standard shader
            platMaterial.SetFloat("_Metallic", metallic);
            platMaterial.SetFloat("_Smoothness", smoothness);
            platMaterial.SetFloat("_Glossiness", smoothness);  // For Standard shader
            
            // Add emission for crystal glow effect
            platMaterial.SetColor("_EmissionColor", padColor * 0.4f);
            platMaterial.EnableKeyword("_EMISSION");

            padRenderer.material = platMaterial;
            
            Debug.Log($"[WarpPad] Created crystal blue material");
            Debug.Log($"[WarpPad] Shader: {platMaterial.shader.name}");
        }

        Debug.Log($"[WarpPad] Created magical platform at {padObject.transform.position}");
        Debug.Log($"[WarpPad] Scale: {padScale}, Color: {padColor}");
    }

    /// <summary>
    /// Changes warp pad color (for visual feedback during challenges).
    /// </summary>
    public void SetPadColor(Color newColor)
    {
        if (padRenderer != null)
        {
            padRenderer.material.SetColor("_Color", newColor);
            padRenderer.material.SetColor("_EmissionColor", newColor * 0.3f);
        }
    }

    /// <summary>
    /// Resets pad to original purple color.
    /// </summary>
    public void ResetPadColor()
    {
        SetPadColor(padColor);
    }

    /// <summary>
    /// Returns the current warp pad GameObject.
    /// </summary>
    public GameObject GetPadObject()
    {
        return padObject;
    }

    /// <summary>
    /// Check if a position is on the warp pad (for safety checks).
    /// </summary>
    public bool IsOnPad(Vector3 position)
    {
        // Check if X and Z are within pad bounds, and Y is roughly on the surface
        float padRadiusX = padScale.x / 2f;
        float padRadiusZ = padScale.z / 2f;
        float padHeight = padScale.y / 2f;

        bool withinXZ = Mathf.Abs(position.x) <= padRadiusX && 
                        Mathf.Abs(position.z) <= padRadiusZ;
        bool nearTop = position.y >= -padHeight && position.y <= padHeight + 0.5f;

        return withinXZ && nearTop;
    }
}
