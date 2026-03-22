using UnityEngine;

/// <summary>
/// CrystalCollector: Handles crystal collision and collection.
/// When player's hand touches a crystal, it's collected for points.
/// Attach this script to each crystal prefab or instance.
/// </summary>
public class CrystalCollector : MonoBehaviour
{
    [SerializeField] private int pointsValue = 5;
    [SerializeField] private float destructionDelay = 0.3f;

    private ParticleSystem crystalParticles;
    private AudioSource crystalAudio;
    private bool isCollected = false;

    void Start()
    {
        crystalParticles = GetComponent<ParticleSystem>();
        crystalAudio = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected)
            return;

        if (other.CompareTag("PlayerHand") 
            || other.name.Contains("Hand") 
            || other.name.Contains("Controller"))
        {
            CollectCrystal();
        }
    }

    void CollectCrystal()
    {
        isCollected = true;
        Debug.Log($"[CrystalCollector] Crystal collected! +{pointsValue} points");

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(pointsValue);

        if (crystalParticles != null)
            crystalParticles.Play();

        if (crystalAudio != null && crystalAudio.clip != null)
            crystalAudio.PlayOneShot(crystalAudio.clip);

        Destroy(gameObject, destructionDelay);
    }

    public void ForceCollect()
    {
        if (!isCollected)
            CollectCrystal();
    }

    public bool IsCollected()
    {
        return isCollected;
    }
}
