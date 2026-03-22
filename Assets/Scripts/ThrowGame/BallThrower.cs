using UnityEngine;

/// <summary>
/// Spawns a ball attached to the right controller.
/// The player throws it by pressing the right index trigger —
/// the ball inherits the controller's tracked velocity for a natural throw.
/// </summary>
public class BallThrower : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The BallPrefab to instantiate. Needs Rigidbody + Collider (NO Grabbable).")]
    public GameObject ballPrefab;

    [Tooltip("The right controller transform (e.g. RightControllerAnchor)")]
    public Transform controllerTransform;

    [Header("Spawn Settings")]
    [Tooltip("Local offset from the controller where the ball sits (tweak so it looks like it's in the palm)")]
    public Vector3 spawnLocalOffset = new Vector3(0f, 0f, 0.08f);

    [Tooltip("Seconds to wait before spawning a new ball after the previous one is thrown/destroyed")]
    public float respawnDelay = 2f;

    [Tooltip("Ball auto-destroys after this many seconds (0 = never)")]
    public float ballLifetime = 10f;

    [Header("Throw Settings")]
    [Tooltip("Multiplier applied to the controller velocity on release")]
    public float throwForceMultiplier = 2f;

    private GameObject currentBall;
    private bool waitingToRespawn = false;
    private bool ballIsHeld = false;

    void Start()
    {
        SpawnBall();
    }

    void Update()
    {
        // If the ball was destroyed externally (target hit, lifetime, out of bounds),
        // start the respawn timer
        if (currentBall == null && !waitingToRespawn)
        {
            ballIsHeld = false;
            waitingToRespawn = true;
            Invoke(nameof(SpawnBall), respawnDelay);
        }

        // Throw on right index trigger press
        if (ballIsHeld && currentBall != null && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            ThrowBall();
        }
    }

    void SpawnBall()
    {
        waitingToRespawn = false;

        if (ballPrefab == null || controllerTransform == null)
        {
            Debug.LogWarning("[BallThrower] Missing ballPrefab or controllerTransform reference!");
            return;
        }

        // Spawn the ball as a child of the controller so it follows the hand
        currentBall = Instantiate(ballPrefab, controllerTransform);
        currentBall.transform.localPosition = spawnLocalOffset;
        currentBall.transform.localRotation = Quaternion.identity;

        // Make kinematic while held — no physics needed yet
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        ballIsHeld = true;
        Debug.Log("[BallThrower] Ball spawned in right hand.");
    }

    void ThrowBall()
    {
        if (currentBall == null) return;

        ballIsHeld = false;

        // Detach from controller
        currentBall.transform.SetParent(null);

        // Enable physics
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // Apply the controller's tracked velocity for a natural throw
            Vector3 velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Vector3 angularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);

            rb.linearVelocity = velocity * throwForceMultiplier;
            rb.angularVelocity = angularVelocity;
        }

        Debug.Log("[BallThrower] Ball thrown!");

        // Auto-destroy after lifetime
        if (ballLifetime > 0f)
        {
            Destroy(currentBall, ballLifetime);
        }

        // Clear reference so respawn timer kicks in
        currentBall = null;
    }
}
