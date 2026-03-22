using UnityEngine;
using UnityEngine.UI;
using TMPro ; 

/// <summary>
/// GameManager: Core game loop controller for CareXR Sprint Interval Training.
/// Manages 30s Action / 30s Rest cycles, score tracking, and state transitions.
/// Attach to empty GameObject named "GameManager" in your scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    // =========== Singleton ===========
    public static GameManager Instance { get; private set; }

    // =========== State Machine ===========
    public enum GameState { Menu, Spinning, Toss, BearWalk, Rest, GameOver }
    public GameState currentState = GameState.Menu;
    private GameState nextState;

    // =========== Game Variables ===========
    public int currentScore = 0;
    public float timer = 30f;
    private float maxTimeAction = 30f;    // Duration of action phase (e.g., Toss, BearWalk)
    private float maxTimeRest = 30f;      // Duration of rest phase

    // =========== UI References ===========
    public TextMeshProUGUI timerUI;     // Drag Timer Text here in Inspector
    public TextMeshProUGUI scoreUI;     // Drag Score Text here in Inspector

    // =========== Spawner References ===========
    public TargetSpawner targetSpawner;  // Drag TargetSpawner here in Inspector
    public PylonSpawner pylonSpawner;    // Drag PylonSpawner here in Inspector
    public CrystalSpawner crystalSpawner;  // Drag CrystalSpawner here in Inspector
    public WarpPad warpPad;              // Drag WarpPad here in Inspector (or leave null for auto-create)

    // =========== State Sequence ===========
    private GameState[] actionSequence = { GameState.Spinning, GameState.Toss, GameState.BearWalk };
    private int currentChallengeIndex = 0;
    private bool isInclusiveMode = false;

    void Awake()
    {
        // Singleton pattern (safe for standalone)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentState = GameState.Menu;
        
        // Initialize WarpPad if not assigned
        if (warpPad == null)
        {
            GameObject warpPadGO = new GameObject("WarpPad");
            warpPadGO.transform.SetParent(transform);
            warpPadGO.transform.localPosition = new Vector3(0, -0.5f, 0);  // Below player feet
            warpPad = warpPadGO.AddComponent<WarpPad>();
            Debug.Log("[GameManager] Auto-created WarpPad at Y=-0.5 (below player)");
        }
        else
        {
            // If manually assigned, also position it correctly
            warpPad.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        
        UpdateUIDisplay();
    }

    void Update()
    {
        // Only countdown timer if game is active
        if (currentState != GameState.Menu && currentState != GameState.GameOver)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                HandleStateTransition();
            }
        }

        // Update UI every frame
        UpdateUIDisplay();
    }

    /// <summary>
    /// Transition to next state when timer expires.
    /// Action phases (Spinning, Toss, etc.) -> Rest
    /// Rest -> Next Action or GameOver
    /// </summary>
    private void HandleStateTransition()
    {
        if (currentState == GameState.Spinning || currentState == GameState.Toss || currentState == GameState.BearWalk)
        {
            // Transition from Action to Rest
            nextState = GameState.Rest;
            timer = maxTimeRest;
        }
        else if (currentState == GameState.Rest)
        {
            // Transition from Rest to next Action
            currentChallengeIndex++;
            
            if (currentChallengeIndex < actionSequence.Length)
            {
                nextState = actionSequence[currentChallengeIndex];
                timer = maxTimeAction;
            }
            else
            {
                // All challenges completed
                nextState = GameState.GameOver;
                timer = 0f;
            }
        }

        currentState = nextState;
        OnStateChanged();
    }

    /// <summary>
    /// Called whenever state changes. Use this to trigger challenge-specific logic.
    /// </summary>
    private void OnStateChanged()
    {
        Debug.Log($"[GameManager] State changed to: {currentState}");
        Debug.Log($"[GameManager] TargetSpawner is null: {targetSpawner == null}");
        Debug.Log($"[GameManager] PylonSpawner is null: {pylonSpawner == null}");
        Debug.Log($"[GameManager] CrystalSpawner is null: {crystalSpawner == null}");

        // Clear old targets, pylons, and crystals
        if (targetSpawner != null)
            targetSpawner.ClearAllTargets();
        if (pylonSpawner != null)
            pylonSpawner.ClearAllPylons();
        if (crystalSpawner != null)
            crystalSpawner.ClearAllCrystals();

        switch (currentState)
        {
            case GameState.Spinning:
                Debug.Log($"[GameManager] Spinning state - calling SpawnCardinalPylons");
                // Spawn 4 pylons at cardinal points (N/S/E/W) for gaze validation
                if (pylonSpawner != null)
                {
                    pylonSpawner.SpawnCardinalPylons();
                }
                else
                {
                    Debug.LogError("[GameManager] PylonSpawner is NULL - assign it in Inspector!");
                }
                break;
            case GameState.Toss:
                Debug.Log($"[GameManager] Toss state - spawning targets and crystals");
                // Spawn targets for throwing challenge
                if (targetSpawner != null)
                {
                    if (isInclusiveMode)
                        targetSpawner.SpawnTargetsInclusive();
                    else
                        targetSpawner.SpawnTargetsToss(8);
                }
                else
                {
                    Debug.LogError("[GameManager] TargetSpawner is NULL - assign it in Inspector!");
                }
                
                // Spawn crystals as additional collectibles/visual elements
                if (crystalSpawner != null)
                {
                    crystalSpawner.SpawnCrystalsScattered(5);
                }
                else
                {
                    Debug.LogWarning("[GameManager] CrystalSpawner is NULL - no crystals will spawn");
                }
                break;
            case GameState.BearWalk:
                Debug.Log("[GameManager] BearWalk state - head height monitoring active (0.8m limit)");
                // BearWalk challenge: player must crawl under 0.8m
                // HeadHeightChecker will handle the altitude monitoring
                break;
            case GameState.Rest:
                // TODO: Show rest UI, fade particles, show score gained
                break;
            case GameState.GameOver:
                // TODO: Save score to Supabase, show final UI
                break;
        }
    }

    /// <summary>
    /// Call this when player hits a target (collision detected).
    /// Increments score and updates UI.
    /// </summary>
    public void AddScore(int points = 1)
    {
        currentScore += points;
        Debug.Log($"[GameManager] Score: {currentScore}");
        // TODO: Play "ting" sound effect here
    }

    /// <summary>
    /// Update timer and score displays.
    /// </summary>
    private void UpdateUIDisplay()
    {
        if (timerUI != null)
        {
            timerUI.text = Mathf.Max(0, timer).ToString("F1"); // Show 1 decimal (e.g., "12.5")
        }

        if (scoreUI != null)
        {
            scoreUI.text = currentScore.ToString();
        }
    }

    /// <summary>
    /// Public methods for UI buttons
    /// </summary>
    public void StartGame()
    {
        currentChallengeIndex = 0;
        currentScore = 0;
        currentState = actionSequence[0];
        timer = maxTimeAction;
        OnStateChanged();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        currentState = GameState.GameOver;
        timer = 0f;
    }

    /// <summary>
    /// Toggle inclusive mode (epicenter: arc of targets around player instead of room-mapped)
    /// </summary>
    public void SetInclusiveMode(bool enabled)
    {
        isInclusiveMode = enabled;
        Debug.Log($"[GameManager] Inclusive Mode: {enabled}");
    }
}
