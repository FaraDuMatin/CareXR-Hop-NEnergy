using UnityEngine;

/// <summary>
/// TargetCollider: Handles collision detection between player hands and targets.
/// Triggers particle effects, sound, score increase, and target destruction on hit.
/// Attach to each target prefab.
/// </summary>
public class TargetCollider : MonoBehaviour
{
    // =========== References ===========
    private ParticleSystem particleSystem;
    private AudioSource audioSource;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        if (particleSystem == null)
            Debug.LogWarning($"[TargetCollider] No ParticleSystem found on {gameObject.name}");
        
        if (audioSource == null)
            Debug.LogWarning($"[TargetCollider] No AudioSource found on {gameObject.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        // Accept collision from hands, controllers, or any tagged "PlayerHand"
        if (other.CompareTag("PlayerHand") 
            || other.name.Contains("Hand") 
            || other.name.Contains("Controller"))
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        Debug.Log($"[TargetCollider] Target hit: {gameObject.name}");

        if (particleSystem != null)
            particleSystem.Play();

        if (audioSource != null)
            audioSource.Play();

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(1);
        else
            Debug.LogError("[TargetCollider] GameManager.Instance is null!");

        Destroy(gameObject, 0.5f);
    }
}
