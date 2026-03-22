using UnityEngine;

/// <summary>
/// PylonSpawner: Spawns 4 pylons at cardinal points (N/E/W/S) for Spinning challenge.
/// Player must select each pylon with ray interaction in a fixed demo order:
/// North (front) → East (right) → West (left) → South (behind).
/// Only the active pylon is highlighted and plays sound.
/// Selected pylons turn green then disappear.
/// </summary>
public class PylonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pylonPrefab;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private float pylonHeight = 1.5f;

    [Header("Beacon Audio")]
    [Tooltip("Looping audio clip so the player can find the active pylon by listening")]
    [SerializeField] private AudioClip beaconClip;

    [Tooltip("Max distance the beacon can be heard (meters)")]
    [SerializeField] private float maxAudioDistance = 10f;

    [Tooltip("Distance at which beacon is at full volume (meters)")]
    [SerializeField] private float minAudioDistance = 0.5f;

    [Header("Visual Feedback")]
    [Tooltip("Color for a validated (completed) pylon before it disappears")]
    [SerializeField] private Color validatedPylonColor = Color.green;

    [Tooltip("Seconds the pylon stays green before disappearing")]
    [SerializeField] private float destroyDelay = 0.5f;

    private GameObject[] spawnedPylons = new GameObject[4];

    // Fixed demo order: North(0) → East(1) → West(3) → South(2)
    // Spawn indices: [0]=North, [1]=East, [2]=South, [3]=West
    private int[] demoOrder = { 0, 1, 3, 2 };
    private int currentStep = 0;
    private bool challengeActive = false;

    void Start()
    {
        if (pylonPrefab == null)
        {
            Debug.LogError("[PylonSpawner] pylonPrefab not assigned!");
            return;
        }

        Debug.Log("[PylonSpawner] Ready to spawn pylons at cardinal points");
    }

    /// <summary>
    /// Spawns 4 pylons at N/E/S/W cardinal points and starts the selection challenge.
    /// </summary>
    public void SpawnCardinalPylons()
    {
        Debug.Log("[PylonSpawner] Spawning 4 pylons for Spinning challenge");

        if (VRReferences.Instance == null || VRReferences.Instance.headTransform == null)
        {
            Debug.LogError("[PylonSpawner] VRReferences.headTransform not found!");
            return;
        }
        Transform playerHead = VRReferences.Instance.headTransform;

        // Define cardinal directions: North, East, South, West
        Vector3[] directions = new Vector3[]
        {
            playerHead.forward,          // North (forward)  → index 0
            playerHead.right,            // East (right)     → index 1
            -playerHead.forward,         // South (backward) → index 2
            -playerHead.right            // West (left)      → index 3
        };

        string[] directionNames = { "North", "East", "South", "West" };

        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPos = playerHead.position + directions[i] * spawnDistance;
            spawnPos.y = pylonHeight;

            GameObject pylon = Instantiate(pylonPrefab, spawnPos, Quaternion.identity);
            pylon.name = "Pylon_" + directionNames[i];
            pylon.tag = "Pylon";

            // Wire up PylonInteractable for ray selection
            PylonInteractable interactable = pylon.GetComponent<PylonInteractable>();
            if (interactable == null)
            {
                interactable = pylon.AddComponent<PylonInteractable>();
            }
            interactable.pylonIndex = i;
            interactable.spawner = this;

            // Add AudioSource component (but don't play yet — only the active pylon plays)
            if (beaconClip != null)
            {
                AudioSource beacon = pylon.AddComponent<AudioSource>();
                beacon.clip = beaconClip;
                beacon.loop = true;
                beacon.playOnAwake = false;
                beacon.spatialBlend = 1f;
                beacon.minDistance = minAudioDistance;
                beacon.maxDistance = maxAudioDistance;
                beacon.rolloffMode = AudioRolloffMode.Logarithmic;
                // Do NOT play — we'll start audio only on the active pylon
            }

            spawnedPylons[i] = pylon;

            Debug.Log($"[PylonSpawner] Spawned {directionNames[i]} pylon at {spawnPos}");
        }

        // Start the selection challenge
        currentStep = 0;
        challengeActive = true;

        // Start audio only on the first active pylon
        ActivateCurrentPylonAudio();

        Debug.Log("[PylonSpawner] Spinning challenge started — select North pylon first!");
    }

    /// <summary>
    /// Called by PylonInteractable when a pylon is selected by ray.
    /// Only accepts the pylon matching the current step in the demo order.
    /// </summary>
    public void OnPylonValidated(int pylonIndex)
    {
        if (!challengeActive) return;

        int expectedIndex = demoOrder[currentStep];

        if (pylonIndex != expectedIndex)
        {
            Debug.Log($"[PylonSpawner] Wrong pylon! Expected index {expectedIndex}, got {pylonIndex}. Ignoring.");
            return;
        }

        Debug.Log($"[PylonSpawner] Pylon {pylonIndex} validated! Step {currentStep + 1}/4");

        // Stop audio on this pylon
        StopPylonAudio(spawnedPylons[pylonIndex]);

        // Flash green then destroy
        SetPylonColor(spawnedPylons[pylonIndex], validatedPylonColor);
        Destroy(spawnedPylons[pylonIndex], destroyDelay);
        spawnedPylons[pylonIndex] = null;

        // Score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1);
        }

        // Advance to next step
        currentStep++;

        if (currentStep >= demoOrder.Length)
        {
            // All pylons validated!
            challengeActive = false;
            Debug.Log("[PylonSpawner] All 4 pylons validated! Spinning challenge complete!");
        }
        else
        {
            // Start audio on next active pylon
            ActivateCurrentPylonAudio();
        }
    }

    /// <summary>
    /// Starts beacon audio only on the current active pylon.
    /// </summary>
    private void ActivateCurrentPylonAudio()
    {
        if (currentStep >= demoOrder.Length) return;

        int activeIndex = demoOrder[currentStep];
        GameObject activePylon = spawnedPylons[activeIndex];

        if (activePylon != null)
        {
            AudioSource audio = activePylon.GetComponent<AudioSource>();
            if (audio != null && !audio.isPlaying)
            {
                audio.Play();
                Debug.Log($"[PylonSpawner] Beacon audio started on {activePylon.name}");
            }
        }
    }

    /// <summary>
    /// Stops beacon audio on a specific pylon.
    /// </summary>
    private void StopPylonAudio(GameObject pylon)
    {
        if (pylon == null) return;
        AudioSource audio = pylon.GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Stop();
        }
    }

    /// <summary>
    /// Sets the color of a pylon's material.
    /// </summary>
    private void SetPylonColor(GameObject pylon, Color color)
    {
        if (pylon == null) return;
        Renderer renderer = pylon.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    /// <summary>
    /// Clears all spawned pylons.
    /// </summary>
    public void ClearAllPylons()
    {
        Debug.Log("[PylonSpawner] Clearing all pylons");
        challengeActive = false;

        foreach (GameObject pylon in spawnedPylons)
        {
            if (pylon != null)
            {
                Destroy(pylon);
            }
        }

        spawnedPylons = new GameObject[4];
    }

    /// <summary>
    /// Resets a specific pylon color.
    /// </summary>
    public void ResetPylonColor(GameObject pylon)
    {
        SetPylonColor(pylon, Color.white);
    }
}
