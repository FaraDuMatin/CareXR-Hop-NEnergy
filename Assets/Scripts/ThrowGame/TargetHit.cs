using UnityEngine;

/// <summary>
/// Detects when a ball hits this target and notifies the ThrowGameManager.
/// The manager handles hit effects (flash, sound, particles) and destruction.
/// Attach to the TargetPrefab. Works with both trigger and non-trigger colliders.
/// </summary>
public class TargetHit : MonoBehaviour
{
    [Tooltip("Reference to the ThrowGameManager in the scene (set automatically by manager)")]
    public ThrowGameManager gameManager;

    private bool hasBeenHit = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasBeenHit && IsBall(other.gameObject))
        {
            HandleHit(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenHit && IsBall(collision.gameObject))
        {
            HandleHit(collision.gameObject);
        }
    }

    bool IsBall(GameObject obj)
    {
        return obj.name.Contains("BallPrefab");
    }

    void HandleHit(GameObject ball)
    {
        hasBeenHit = true;

        Debug.Log("[TargetHit] Target hit!");

        // Destroy the ball immediately
        Destroy(ball);

        // Notify the manager — it handles effects and target destruction
        if (gameManager != null)
        {
            gameManager.OnTargetHit(transform.position);
        }
        else
        {
            // Fallback: destroy self if no manager
            Destroy(gameObject);
        }
    }
}
