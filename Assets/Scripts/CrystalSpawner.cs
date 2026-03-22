using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// CrystalSpawner: Spawns crystal/gem props as interactive game elements.
/// Crystals can be collectibles, targets, or visual decorations.
/// Attach to GameManager or spawner controller.
/// </summary>
public class CrystalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] crystalPrefabs;  // Drag crystal prefabs here
    [SerializeField] private Material gemMaterial;  // Assign Gems.mat here
    [SerializeField] private float spawnDistance = 2.5f;
    [SerializeField] private float crystalHeight = 1.2f;
    [SerializeField] private bool randomRotation = true;
    [SerializeField] private bool randomScale = false;

    private List<GameObject> spawnedCrystals = new List<GameObject>();

    void Start()
    {
        if (crystalPrefabs == null || crystalPrefabs.Length == 0)
        {
            Debug.LogWarning("[CrystalSpawner] No crystal prefabs assigned!");
        }

        // Load Gems.mat if not assigned
        if (gemMaterial == null)
        {
            #if UNITY_EDITOR
            gemMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Aztech Games/GemsAndCrystals/Materials/Gems.mat");
            #else
            gemMaterial = Resources.Load<Material>("Gems");
            #endif
            
            if (gemMaterial != null)
                Debug.Log("[CrystalSpawner] Loaded Gems.mat automatically");
            else
                Debug.LogWarning("[CrystalSpawner] Could not load Gems.mat - crystals will be magenta");
        }
    }

    /// <summary>
    /// Spawns N crystals in a scattered formation around the player.
    /// </summary>
    public void SpawnCrystalsScattered(int count = 5)
    {
        if (crystalPrefabs.Length == 0)
        {
            Debug.LogError("[CrystalSpawner] No crystals to spawn!");
            return;
        }

        if (VRReferences.Instance == null || VRReferences.Instance.headTransform == null)
        {
            Debug.LogError("[CrystalSpawner] VRReferences.headTransform not found!");
            return;
        }
        Transform playerHead = VRReferences.Instance.headTransform;

        Debug.Log($"[CrystalSpawner] Spawning {count} crystals in scattered formation");

        for (int i = 0; i < count; i++)
        {
            // Random position around player
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(1.5f, 3f);
            
            Vector3 spawnPos = playerHead.position + 
                new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * distance, 
                           crystalHeight, 
                           Mathf.Sin(angle * Mathf.Deg2Rad) * distance);

            // Pick random crystal prefab
            int prefabIndex = Random.Range(0, crystalPrefabs.Length);
            GameObject crystalInstance = Instantiate(
                crystalPrefabs[prefabIndex],
                spawnPos,
                Quaternion.identity
            );

            // Apply material
            Renderer renderer = crystalInstance.GetComponent<Renderer>();
            if (renderer != null && gemMaterial != null)
            {
                renderer.material = gemMaterial;
            }

            // Random rotation for variety
            if (randomRotation)
            {
                crystalInstance.transform.Rotate(
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f)
                );
            }

            // Optional: Random scale
            if (randomScale)
            {
                float scale = Random.Range(0.8f, 1.2f);
                crystalInstance.transform.localScale = Vector3.one * scale;
            }

            crystalInstance.name = $"Crystal_{i}";
            spawnedCrystals.Add(crystalInstance);

            Debug.Log($"[CrystalSpawner] Spawned crystal {i} at {spawnPos}");
        }
    }

    /// <summary>
    /// Spawns crystals in a specific pattern (e.g., for Spinning challenge).
    /// </summary>
    public void SpawnCrystalsCircle(int count = 8, float radius = 2f)
    {
        if (crystalPrefabs.Length == 0)
        {
            Debug.LogError("[CrystalSpawner] No crystals to spawn!");
            return;
        }

        if (VRReferences.Instance == null || VRReferences.Instance.headTransform == null)
        {
            Debug.LogError("[CrystalSpawner] VRReferences.headTransform not found!");
            return;
        }
        Transform playerHead = VRReferences.Instance.headTransform;

        Debug.Log($"[CrystalSpawner] Spawning {count} crystals in circle formation");

        for (int i = 0; i < count; i++)
        {
            float angle = (i / (float)count) * 360f;
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector3 spawnPos = playerHead.position + direction * radius;
            spawnPos.y = crystalHeight;

            // Pick random crystal prefab
            int prefabIndex = Random.Range(0, crystalPrefabs.Length);
            GameObject crystalInstance = Instantiate(
                crystalPrefabs[prefabIndex],
                spawnPos,
                Quaternion.identity
            );

            // Apply material
            Renderer renderer = crystalInstance.GetComponent<Renderer>();
            if (renderer != null && gemMaterial != null)
            {
                renderer.material = gemMaterial;
            }

            if (randomRotation)
            {
                crystalInstance.transform.Rotate(0, Random.Range(0f, 360f), 0);
            }

            crystalInstance.name = $"Crystal_Circle_{i}";
            spawnedCrystals.Add(crystalInstance);

            Debug.Log($"[CrystalSpawner] Spawned circle crystal {i} at {spawnPos}");
        }
    }

    /// <summary>
    /// Clears all spawned crystals.
    /// </summary>
    public void ClearAllCrystals()
    {
        Debug.Log($"[CrystalSpawner] Clearing {spawnedCrystals.Count} crystals");

        foreach (GameObject crystal in spawnedCrystals)
        {
            if (crystal != null)
                Destroy(crystal);
        }

        spawnedCrystals.Clear();
    }

    /// <summary>
    /// Returns count of active crystals.
    /// </summary>
    public int GetCrystalCount()
    {
        return spawnedCrystals.Count;
    }

    /// <summary>
    /// Destroy a specific crystal (e.g., when collected).
    /// </summary>
    public void DestroyCrystal(GameObject crystal)
    {
        if (spawnedCrystals.Contains(crystal))
        {
            spawnedCrystals.Remove(crystal);
            Destroy(crystal);
            Debug.Log("[CrystalSpawner] Crystal destroyed");
        }
    }
}
