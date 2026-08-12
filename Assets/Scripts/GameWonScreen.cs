using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameWonScreen : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timeText;
    public TMP_Text deathsText;
    public TMP_Text submissionStatus; // NEW: Show submission feedback
    public Button nextLevelButton;
    public Button mainMenuButton;
    
    [Header("Level Progression")]
    public string[] levelOrder = { "TutorialLevel", "Level1", "Level2", "Level 3", "Level4", "Level5", "Level6" };
    private int currentLevelIndex = -1;
    private int currentLevelNumber = -1; // For leaderboard (1-based)

    [Header("Leaderboard")]
    private LeaderboardUI leaderboardUI;
    public string playerName = "Player"; // Can be set from a settings menu

    private float timeTaken;
    private int deathCount;
    GameObject sceneMan;
    
    void Awake()
    {
        sceneMan = GameObject.FindWithTag("SceneManager");
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Find current level
        for (int i = 0; i < levelOrder.Length; i++)
        {
            Debug.Log($"levelOrder[{i}] = '{levelOrder[i]}'");
            if (levelOrder[i] == currentScene)
            {
                Debug.Log($"levelOrder[{i}] = '{levelOrder[i]}'");
                currentLevelIndex = i;
                currentLevelNumber = i;
                break;
            }
        }
        Debug.Log($"Current scene: {currentScene}, Found index: {currentLevelIndex}");

        // Find LeaderboardUI in scene
        leaderboardUI = FindFirstObjectByType<LeaderboardUI>();
        if (leaderboardUI == null)
        {
            Debug.LogWarning("LeaderboardUI not found in scene!");
        }
    }
    
    public void Show(float timeTaken, int deathCount)
    {
        this.timeTaken = timeTaken;
        this.deathCount = deathCount;

        // Format the time the same way TimeUpdater does
        int minutes = Mathf.FloorToInt(timeTaken / 60);
        int seconds = Mathf.FloorToInt(timeTaken % 60);
        float milli = (timeTaken % 1) * 1000;
        string timeString = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, (int)milli);
        
        if (timeText != null)
            timeText.text = $"Time: {timeString}";
        if (deathsText != null)
            deathsText.text = $"Deaths: {deathCount}";
        
        // Clear submission status
        if (submissionStatus != null)
            submissionStatus.text = "Submitting score...";
        
        gameObject.SetActive(true);

        // AUTO-SUBMIT score to leaderboard
        SubmitScore();
    }

    private void SubmitScore()
    {
        if (leaderboardUI == null)
        {
            if (submissionStatus != null)
                submissionStatus.text = "Leaderboard unavailable";
            Debug.LogWarning("LeaderboardUI not found - cannot submit score");
            return;
        }

        // Convert time to milliseconds
        int timeMs = Mathf.RoundToInt(timeTaken * 1000);

        // Set player name (you can customize this)
        leaderboardUI.SetPlayerName(playerName);

        // Submit the score
        leaderboardUI.SubmitScore(currentLevelNumber, timeMs);

        // Show feedback
        if (submissionStatus != null)
            submissionStatus.text = $"Score submitted!";
    }

    public void OnNextLevelButton()
    {
        SceneMan sm = sceneMan.GetComponent<SceneMan>();

        int nextIndex = currentLevelIndex + 1;

        if (nextIndex >= levelOrder.Length)
        {
            sm.LoadMM();
            return;
        }

        switch (nextIndex)
        {
            case 1: sm.LoadSceneOne(); break;
            case 2: sm.LoadSceneTwo(); break;
            case 3: sm.LoadSceneThree(); break;
            case 4: sm.LoadSceneFour(); break;
            case 5: sm.LoadSceneFive(); break;
            case 6: sm.LoadSceneSix(); break;
            default: sm.LoadMM(); break;
        }
    }
    
    public void OnMainMenuButton()
    {
        SceneMan sm = sceneMan.GetComponent<SceneMan>();
        sm.LoadMM();
    }

    // Optional: Allow player to set their name before playing
    public void SetPlayerName(string name)
    {
        playerName = name;
    }
}