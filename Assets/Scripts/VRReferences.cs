using UnityEngine;

/// <summary>
/// VRReferences: Singleton that caches the VR head transform (CenterEyeAnchor).
/// Replaces all Camera.main.transform calls for Meta SDK compatibility.
/// Attach to an empty GameObject and drag CenterEyeAnchor into headTransform.
/// </summary>
public class VRReferences : MonoBehaviour
{
    public static VRReferences Instance { get; private set; }

    [Tooltip("Drag CenterEyeAnchor here (Camera Rig > TrackingSpace > CenterEyeAnchor)")]
    public Transform headTransform;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-find CenterEyeAnchor if not assigned
        if (headTransform == null)
        {
            GameObject centerEye = GameObject.Find("CenterEyeAnchor");
            if (centerEye != null)
            {
                headTransform = centerEye.transform;
                Debug.Log("[VRReferences] Auto-found CenterEyeAnchor");
            }
            else
            {
                Debug.LogError("[VRReferences] CenterEyeAnchor not found! Drag it into the Inspector.");
            }
        }
    }
}
