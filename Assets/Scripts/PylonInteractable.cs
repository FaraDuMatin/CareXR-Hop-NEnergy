using UnityEngine;
using Oculus.Interaction;

/// <summary>
/// PylonInteractable: Handles ray-select callback on a pylon.
/// Automatically subscribes to the RayInteractable's state changes via code
/// so NO manual event wiring is needed in the Unity Inspector.
/// You can remove the PointableUnityEventWrapper from the prefab — it's not used.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PylonInteractable : MonoBehaviour
{
    /// <summary>
    /// Index of this pylon in the spawner's array (set at spawn time).
    /// </summary>
    [HideInInspector]
    public int pylonIndex;

    /// <summary>
    /// Reference to the spawner (set at spawn time).
    /// </summary>
    [HideInInspector]
    public PylonSpawner spawner;

    private RayInteractable rayInteractable;
    private bool hasBeenSelected = false;

    void Start()
    {
        // Auto-wire: find RayInteractable on this GameObject and subscribe to state changes
        rayInteractable = GetComponent<RayInteractable>();

        if (rayInteractable != null)
        {
            rayInteractable.WhenStateChanged += OnInteractableStateChanged;
            Debug.Log($"[PylonInteractable] Subscribed to RayInteractable on {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[PylonInteractable] No RayInteractable found on {gameObject.name}!");
        }
    }

    void OnDestroy()
    {
        if (rayInteractable != null)
        {
            rayInteractable.WhenStateChanged -= OnInteractableStateChanged;
        }
    }

    /// <summary>
    /// Called when RayInteractable state changes (Normal → Hover → Select).
    /// We trigger selection when the state becomes Select (user pulls trigger while pointing).
    /// </summary>
    private void OnInteractableStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select && !hasBeenSelected)
        {
            hasBeenSelected = true;
            OnPylonSelected();
        }
    }

    private void OnPylonSelected()
    {
        Debug.Log($"[PylonInteractable] Pylon {pylonIndex} ({gameObject.name}) selected by ray!");

        if (spawner != null)
        {
            spawner.OnPylonValidated(pylonIndex);
        }
        else
        {
            Debug.LogWarning("[PylonInteractable] No spawner reference — cannot validate!");
        }
    }
}
