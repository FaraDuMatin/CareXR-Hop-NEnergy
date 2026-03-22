using UnityEngine;

/// <summary>
/// Manages the throw game: spawns ONE target at a random grid position,
/// tracks score, and plays hit effects (color flash, sound, particles).
/// Attach to an empty GameObject in the scene.
/// </summary>
public class ThrowGameManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The TargetPrefab to spawn")]
    public GameObject targetPrefab;

    [Tooltip("Transform marking the center of the target wall")]
    public Transform spawnPoint;

    [Header("Hit Effects")]
    [Tooltip("AudioClip to play when a target is hit")]
    public AudioClip hitSound;

    [Tooltip("Volume for the hit sound (0-1)")]
    [Range(0f, 1f)]
    public float hitSoundVolume = 1f;

    [Tooltip("Particle system prefab to spawn on hit (e.g. explosion/confetti)")]
    public GameObject hitParticlePrefab;

    [Tooltip("Material to briefly flash the target with on hit (e.g. bright green/white)")]
    public Material hitFlashMaterial;

    [Header("Grid Layout")]
    [Tooltip("Horizontal spacing between the two columns (meters)")]
    public float columnSpacing = 0.5f;

    [Tooltip("Heights for the 3 rows (bottom to top)")]
    public float rowHeight1 = 1.0f;
    public float rowHeight2 = 1.5f;
    public float rowHeight3 = 2.0f;

    [Header("Settings")]
    [Tooltip("Delay before spawning the next target after a hit (seconds)")]
    public float respawnDelay = 1.5f;

    [Tooltip("How long the hit flash + particles last before the target is destroyed")]
    public float hitEffectDuration = 0.5f;

    private int score = 0;
    private Vector3[] gridPositions;
    private GameObject currentTarget;
    private int lastSpawnIndex = -1;

    void Start()
    {
        BuildGrid();
        SpawnRandomTarget();
    }

    void BuildGrid()
    {
        float centerX = spawnPoint != null ? spawnPoint.position.x : 0f;
        float z = spawnPoint != null ? spawnPoint.position.z : 0f;

        float leftX = centerX - columnSpacing / 2f;
        float rightX = centerX + columnSpacing / 2f;

        gridPositions = new Vector3[]
        {
            new Vector3(leftX,  rowHeight1, z),
            new Vector3(rightX, rowHeight1, z),
            new Vector3(leftX,  rowHeight2, z),
            new Vector3(rightX, rowHeight2, z),
            new Vector3(leftX,  rowHeight3, z),
            new Vector3(rightX, rowHeight3, z),
        };
    }

    /// <summary>
    /// Spawns one target at a random grid position (different from the last one).
    /// </summary>
    void SpawnRandomTarget()
    {
        if (targetPrefab == null)
        {
            Debug.LogWarning("[ThrowGameManager] Missing targetPrefab reference!");
            return;
        }

        // Pick a random position different from the last one
        int index;
        do
        {
            index = Random.Range(0, gridPositions.Length);
        } while (index == lastSpawnIndex && gridPositions.Length > 1);

        lastSpawnIndex = index;
        Vector3 pos = gridPositions[index];

        currentTarget = Instantiate(targetPrefab, pos, Quaternion.identity);

        // Wire up the manager reference on TargetHit
        TargetHit hitScript = currentTarget.GetComponent<TargetHit>();
        if (hitScript == null)
        {
            hitScript = currentTarget.AddComponent<TargetHit>();
        }
        hitScript.gameManager = this;

        Debug.Log($"[ThrowGameManager] Target spawned at slot {index} ({pos})");
    }

    /// <summary>
    /// Called by TargetHit when a ball hits the target.
    /// Plays effects, then destroys and respawns.
    /// </summary>
    public void OnTargetHit(Vector3 hitPosition)
    {
        score++;
        Debug.Log($"[ThrowGameManager] Target hit! Score: {score}");

        // --- Hit effects on the target ---
        if (currentTarget != null)
        {
            // 1. Flash color: swap material to hitFlashMaterial
            if (hitFlashMaterial != null)
            {
                Renderer renderer = currentTarget.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = hitFlashMaterial;
                }
            }

            // 2. Play hit sound at the target's position
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, hitPosition, hitSoundVolume);
            }

            // 3. Spawn particle effect at hit position
            if (hitParticlePrefab != null)
            {
                GameObject particles = Instantiate(hitParticlePrefab, hitPosition, Quaternion.identity);
                // Auto-destroy particles after they finish (default 3 seconds)
                Destroy(particles, 3f);
            }

            // Destroy the target after a short delay so the flash is visible
            Destroy(currentTarget, hitEffectDuration);
        }

        // Spawn next target after respawn delay
        Invoke(nameof(SpawnRandomTarget), respawnDelay);
    }
}
