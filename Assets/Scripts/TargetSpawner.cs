using UnityEngine;

/// <summary>
/// TargetSpawner: Spawns target prefabs in the scene based on game mode.
/// Attach to an empty GameObject in your scene.
/// </summary>
public class TargetSpawner : MonoBehaviour
{
    // =========== References ===========
    public GameObject targetPrefab;  // Drag your Target prefab here

    // =========== Spawn Settings ===========
    public int targetCountInclusive = 5;      // Arc mode: 5 targets in a circle
    public float arcRadius = 1.5f;            // How far from player
    public float arcHeight = 1.2f;            // Height of targets (eye level)

    private float horizontalSpawnDistance = 2f; // How far in front for Toss mode

    void Start()
    {
        // Optionally spawn on start (or wait for GameManager)
    }

    /// <summary>
    /// Spawn targets in arc around player (Inclusive Mode - wheelchair friendly).
    /// Targets arranged in semi-circle at arm's reach height.
    /// </summary>
    public void SpawnTargetsInclusive()
    {
        if (targetPrefab == null)
        {
            Debug.LogError("[TargetSpawner] Target prefab not assigned!");
            return;
        }

        // Get player camera position
        Transform cameraTransform = VRReferences.Instance.headTransform;

        // Spawn N targets in a semi-circle
        for (int i = 0; i < targetCountInclusive; i++)
        {
            // Calculate angle (180° arc, from -90 to +90)
            float angle = Mathf.Lerp(-90f, 90f, (float)i / (targetCountInclusive - 1));
            float angleRad = angle * Mathf.Deg2Rad;

            // Calculate position (semi-circle at arm's reach)
            Vector3 spawnPos = cameraTransform.position + new Vector3(
                Mathf.Sin(angleRad) * arcRadius,
                arcHeight,
                Mathf.Cos(angleRad) * arcRadius
            );

            // Instantiate target
            Instantiate(targetPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log($"[TargetSpawner] Spawned {targetCountInclusive} targets in arc (Inclusive Mode)");
    }

    /// <summary>
    /// Spawn targets in front of player (Toss Mode - room exploration).
    /// Targets at various distances to encourage throwing.
    /// </summary>
    public void SpawnTargetsToss(int count = 8)
    {
        if (targetPrefab == null)
        {
            Debug.LogError("[TargetSpawner] ERROR: Target prefab not assigned!");
            return;
        }

        Transform cameraTransform = VRReferences.Instance.headTransform;
        Debug.Log($"[TargetSpawner] Camera position: {cameraTransform.position}");

        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(-2f, 2f);      // Wider left/right
            float randomY = Random.Range(0.5f, 1.5f);   // Eye level
            float randomZ = Random.Range(2f, 5f);       // Further away

            Vector3 spawnPos = cameraTransform.position + new Vector3(randomX, randomY, randomZ);
            
            GameObject spawnedTarget = Instantiate(targetPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[TargetSpawner] Spawned target {i+1} at: {spawnPos}");
        }

        Debug.Log($"[TargetSpawner] Spawned {count} targets in front (Toss Mode)");
    }

    /// <summary>
    /// Clear all active targets from the scene.
    /// Useful when transitioning between states.
    /// </summary>
    public void ClearAllTargets()
    {
        TargetCollider[] allTargets = FindObjectsOfType<TargetCollider>();
        foreach (TargetCollider target in allTargets)
        {
            Destroy(target.gameObject);
        }
        Debug.Log("[TargetSpawner] Cleared all targets");
    }
}
