using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UIManager: Handles HUD interactions, button events, and UI state.
/// Attach to the Canvas GameObject with World Space rendering.
/// </summary>
public class UIManager : MonoBehaviour
{
    // =========== UI References ===========
    public TextMeshProUGUI scoreText;          // Display: "SCORE: 0"
    public TextMeshProUGUI timerText;          // Display: "TEMPS: 30"
    public Button modeExplorationButton;       // "Mode Exploration" button
    public Button modeInclusiveButton;         // "Mode Inclusif (Fauteuil)" button
    public GameObject menuPanel;               // Panel containing menu buttons (enable/disable)

    [Header("Ray Interaction")]
    [Tooltip("Drag the ray interaction GameObjects here (e.g. [BuildingBlock] ISDK_RayInteraction) to disable them during gameplay")]
    public GameObject[] rayInteractors;        // Cubes or ray interactors to disable

    // =========== Game Mode State ===========
    private bool isInclusiveMode = false;

    void Start()
    {
        // Assign button click listeners
        if (modeExplorationButton != null)
            modeExplorationButton.onClick.AddListener(OnExplorationModeClicked);

        if (modeInclusiveButton != null)
            modeInclusiveButton.onClick.AddListener(OnInclusiveModeClicked);

        // Initialize UI display
        UpdateScoreDisplay(0);
        UpdateTimerDisplay(30f);

        // Ensure menu is visible at start
        if (menuPanel != null)
            menuPanel.SetActive(true);
    }

    void Update()
    {
        // Sync timer display with GameManager
        if (GameManager.Instance != null)
        {
            UpdateTimerDisplay(GameManager.Instance.timer);
            UpdateScoreDisplay(GameManager.Instance.currentScore);
        }
    }

    /// <summary>
    /// Called when "Mode Exploration" button is clicked.
    /// Disables inclusive mode (normal MR with room detection).
    /// </summary>
    public void OnExplorationModeClicked()
    {
        Debug.Log("[UIManager] Mode Exploration selected");
        isInclusiveMode = false;

        // Hide menu
        if (menuPanel != null)
            menuPanel.SetActive(false);

        SetRayInteractorsActive(false);

        // Notify GameManager to use normal MR mode
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetInclusiveMode(false);
            GameManager.Instance.StartGame();
        }

        // TODO: Trigger OVRSceneManager surface detection here
        // Example: OVRSceneManager.Instance.RequestSceneCapture();
    }

    /// <summary>
    /// Called when "Mode Inclusif (Fauteuil)" button is clicked.
    /// Enables inclusive mode (arc-based targets, no room scan).
    /// </summary>
    public void OnInclusiveModeClicked()
    {
        Debug.Log("[UIManager] Mode Inclusif selected");
        isInclusiveMode = true;

        // Hide menu
        if (menuPanel != null)
            menuPanel.SetActive(false);

        SetRayInteractorsActive(false);

        // Notify GameManager to use inclusive mode
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetInclusiveMode(true);
            GameManager.Instance.StartGame();
        }

        // TODO: Disable OVRSceneManager surface detection
        // TODO: Activate arc-based target spawner
    }

    /// <summary>
    /// Update score display on HUD
    /// </summary>
    public void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
            scoreText.text = $"SCORE: {score}";
    }

    /// <summary>
    /// Update timer display on HUD
    /// </summary>
    private void UpdateTimerDisplay(float time)
    {
        if (timerText != null)
        {
            int seconds = Mathf.Max(0, Mathf.RoundToInt(time));
            timerText.text = $"TEMPS: {seconds}";
        }
    }

    /// <summary>
    /// Show game over screen and menu options
    /// </summary>
    public void ShowGameOverScreen()
    {
        Debug.Log("[UIManager] Showing Game Over screen");
        if (menuPanel != null)
            menuPanel.SetActive(true);

        SetRayInteractorsActive(true);

        // TODO: Display final score, "Play Again" button, etc.
    }

    /// <summary>
    /// Return to main menu
    /// </summary>
    public void ReturnToMenu()
    {
        Debug.Log("[UIManager] Returning to menu");
        if (GameManager.Instance != null)
            GameManager.Instance.EndGame();

        if (menuPanel != null)
            menuPanel.SetActive(true);

        SetRayInteractorsActive(true);

        // TODO: Reset HUD, disable active challenges
    }

    /// <summary>
    /// Shows or hides the visual cube meshes for ray interactors.
    /// Only toggles MeshRenderers — keeps the GameObjects and child
    /// ISDK_RayInteraction active so rays still work during gameplay.
    /// </summary>
    private void SetRayInteractorsActive(bool isActive)
    {
        if (rayInteractors != null)
        {
            foreach (GameObject interactor in rayInteractors)
            {
                if (interactor != null)
                {
                    // Hide/show the cube visual only (not the entire GameObject)
                    MeshRenderer mr = interactor.GetComponent<MeshRenderer>();
                    if (mr != null) mr.enabled = isActive;

                    // Also toggle any child MeshRenderers (visual elements)
                    foreach (MeshRenderer childMr in interactor.GetComponentsInChildren<MeshRenderer>())
                    {
                        childMr.enabled = isActive;
                    }
                }
            }
        }
    }
}
