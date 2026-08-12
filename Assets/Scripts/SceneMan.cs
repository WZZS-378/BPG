using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMan : MonoBehaviour
{
    public static SceneMan instance;
    public bool isTransitioning = false;
    private float savedTimer = 0f;
    private bool savedTimerIsRunning = false;
    private bool shouldRestoreTimer;
    public int playersAtExit = 0;
    private int totalPlayers = 3; // Blue, Pink, Green
    public int deathCount;
    public GameObject gameWonScreenPrefab;
    private string playerName = "Player"; // NEW
    public int selectedLevelIndex = 0;
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        deathCount = 0;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        isTransitioning = false;

        // NEW: Set player name on GameWonScreen when it's loaded
        if (scene.name != "MainMenu")
        {
            GameWonScreen winScreen = FindFirstObjectByType<GameWonScreen>();
            if (winScreen != null)
            {
                winScreen.SetPlayerName(playerName);
            }
        }
        if (scene.name == "MainMenu")
        {
            StartCoroutine(RefreshLeaderboardNextFrame());
        }
    }

    // NEW: Set the player name
    public void SetPlayerName(string name)
    {
        playerName = name;
        Debug.Log("Player name set to: " + playerName);
    }

    public void reloadLevel()
    {
        if (isTransitioning) return;
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        TimeUpdater timeUpdater = Object.FindAnyObjectByType<TimeUpdater>();
        if (timeUpdater != null)
        {
            savedTimer = timeUpdater.timer;
            savedTimerIsRunning = timeUpdater.timerIsRunning;
            shouldRestoreTimer = true;
        }
        isTransitioning = true;
        ResetPlayersAtExit();

        Debug.Log("Reloading level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMM()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        Debug.Log("Loading Main Menu");
        SceneManager.LoadScene("MainMenu");
        deathCount = 0;
    }

    public void LoadTut()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        SceneManager.LoadScene(1);
        deathCount = 0;
    }

    public void LoadSceneOne()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        SceneManager.LoadScene(2);
        deathCount = 0;
    }

    public void LoadSceneTwo()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        SceneManager.LoadScene(3);
        deathCount = 0;
    }

    public void LoadSceneThree()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        SceneManager.LoadScene(4);
        deathCount = 0;
    }

    public void LoadSceneFour()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        SceneManager.LoadScene(5);
        deathCount = 0;
    }
    public void LoadSceneFive()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        SceneManager.LoadScene(6);
        deathCount = 0;
    }
    public void LoadSceneSix()
    {
        if (isTransitioning) return;
        ResetPlayersAtExit();
        ClearSavedTimer();
        isTransitioning = true;

        SceneManager.LoadScene(7);
        deathCount = 0;
    }


    public bool TryGetSavedTimer(out float timerValue, out bool timerWasRunning)
    {
        timerValue = savedTimer;
        timerWasRunning = savedTimerIsRunning;

        if (!shouldRestoreTimer)
        {
            return false;
        }

        shouldRestoreTimer = false;
        return true;
    }

    private void ClearSavedTimer()
    {
        savedTimer = 0f;
        savedTimerIsRunning = false;
        shouldRestoreTimer = false;
    }

    public void RegisterPlayerAtExit()
    {
        playersAtExit++;
        if (playersAtExit >= totalPlayers)
        {
            ShowGameWonScreen();
        }
    }
    private void ShowGameWonScreen()
    {
        TimeUpdater timeUpdater = FindFirstObjectByType<TimeUpdater>();
        if (timeUpdater == null)
        {
            Debug.LogError("TimeUpdater not found in scene!");
            return;
        }
        timeUpdater.timerPaused = true;
        float timeTaken = timeUpdater.timer;
        GameObject winScreenObj = Instantiate(gameWonScreenPrefab);
        GameWonScreen winScreen = winScreenObj.GetComponent<GameWonScreen>();

        if (winScreen != null)
        {
            // NEW: Set player name before showing
            winScreen.SetPlayerName(playerName);
            winScreen.Show(timeTaken, deathCount);
        }
        else
        {
            Debug.LogError("No GameWonScreen found in scene");
        }
    }
    
    public void ResetPlayersAtExit()
    {
        playersAtExit = 0;
    }
    
    public void AddDeath()
    {
        deathCount++;
        Debug.Log("Deaths: " + deathCount);
    }
    public void SetSelectedLevel(int index)
    {
        selectedLevelIndex = index;
    }
    private IEnumerator RefreshLeaderboardNextFrame()
    {
        yield return null; // wait 1 frame so UI exists

        LeaderboardUI lb = FindFirstObjectByType<LeaderboardUI>();
        if (lb != null)
        {
            lb.RefreshLeaderboard();
        }
    }
}