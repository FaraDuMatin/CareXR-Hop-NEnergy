using UnityEngine;

/// <summary>
/// HeadHeightChecker: Monitors camera height during Bear Walk challenge.
/// If head goes above 0.8m, triggers alarm sound.
/// Attach to GameManager or main Camera.
/// </summary>
public class HeadHeightChecker : MonoBehaviour
{
    private const float HEIGHT_LIMIT = 0.8f;  // Max height in meters (standing height)
    private Transform cameraTransform;
    private AudioSource alarmAudio;
    private bool alarmActive = false;

    void Start()
    {
        // Get VR head camera
        if (VRReferences.Instance != null && VRReferences.Instance.headTransform != null)
            cameraTransform = VRReferences.Instance.headTransform;
        else
            Debug.LogError("[HeadHeightChecker] VRReferences.headTransform not found!");

        // Get or create AudioSource for alarm
        alarmAudio = GetComponent<AudioSource>();
        if (alarmAudio == null)
        {
            alarmAudio = gameObject.AddComponent<AudioSource>();
        }

        // Create a placeholder beep sound (no external file needed)
        CreatePlaceholderBeep();

        Debug.Log("[HeadHeightChecker] Height limit set to: " + HEIGHT_LIMIT + "m");
        Debug.Log("[HeadHeightChecker] Using procedural beep sound (no audio file required)");
    }

    /// <summary>
    /// Creates a simple beep sound procedurally for placeholder audio.
    /// </summary>
    void CreatePlaceholderBeep()
    {
        // Generate a 1-second beep at 1000 Hz
        int sampleRate = 44100;
        float frequency = 1000f;  // 1000 Hz beep
        float duration = 0.2f;    // 0.2 seconds
        int numSamples = (int)(sampleRate * duration);

        AudioClip beep = AudioClip.Create("BeepPlaceholder", numSamples, 1, sampleRate, false);
        float[] samples = new float[numSamples];

        for (int i = 0; i < numSamples; i++)
        {
            // Generate sine wave
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * i / sampleRate);
        }

        beep.SetData(samples, 0);
        alarmAudio.clip = beep;
    }

    void Update()
    {
        if (cameraTransform == null)
            return;

        float headHeight = cameraTransform.position.y;

        if (headHeight > HEIGHT_LIMIT)
        {
            // Head is too high!
            if (!alarmActive)
            {
                TriggerAlarm();
            }
        }
        else
        {
            // Head is at safe height
            if (alarmActive)
            {
                StopAlarm();
            }
        }

        // Debug visualization
        Debug.DrawRay(cameraTransform.position, Vector3.down * 0.5f, 
            (headHeight > HEIGHT_LIMIT) ? Color.red : Color.green);
    }

    void TriggerAlarm()
    {
        alarmActive = true;
        Debug.Log($"[HeadHeightChecker] ALARM! Head height: {cameraTransform.position.y:F2}m (limit: {HEIGHT_LIMIT}m)");

        if (alarmAudio != null && alarmAudio.clip != null)
        {
            alarmAudio.PlayOneShot(alarmAudio.clip);
        }
        else
        {
            Debug.LogWarning("[HeadHeightChecker] No alarm sound assigned!");
        }
    }

    void StopAlarm()
    {
        alarmActive = false;
        Debug.Log("[HeadHeightChecker] Head height OK");

        if (alarmAudio != null)
        {
            alarmAudio.Stop();
        }
    }

    /// <summary>
    /// Returns current head height for debugging.
    /// </summary>
    public float GetHeadHeight()
    {
        return cameraTransform != null ? cameraTransform.position.y : 0f;
    }
}
