using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Supabase")]
    [SerializeField] private string supabaseUrl;
    [SerializeField] private string apiKey;

    [Header("UI")]
    [SerializeField] private TMP_Text[] levelTexts;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Submit Settings")]
    [SerializeField] private string playerName = "Player";

    private static LeaderboardUI instance;

    void Awake()
    {
        Debug.Log($"LeaderboardUI Awake: {gameObject.name}");
        Debug.Log($"levelTexts length = {(levelTexts == null ? -1 : levelTexts.Length)}");

        if (instance != null && instance != this)
        {
            Debug.Log("Destroying duplicate LeaderboardUI");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RefreshLeaderboard()
    {
        StartCoroutine(GetLeaderboard());
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    private IEnumerator GetLeaderboard()
    {
        string url =
            $"{supabaseUrl}/rest/v1/leaderboard" +
            "?select=level,player_name,time_ms" +
            "&order=level.asc";

        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("apikey", apiKey);
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Leaderboard fetch failed: " + request.error);
            ShowFeedback("Failed to load leaderboard", true);
            yield break;
        }

        Debug.Log("RAW RESPONSE: " + request.downloadHandler.text);

        List<LeaderboardEntry> entries =
            JsonConvert.DeserializeObject<List<LeaderboardEntry>>(
                request.downloadHandler.text
            );

        if (entries == null)
        {
            Debug.LogError("Entries is NULL");
            yield break;
        }

        UpdateUI(entries);
    }

    private void UpdateUI(List<LeaderboardEntry> entries)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            return;

        if (levelTexts == null || levelTexts.Length == 0)
        {
            Debug.LogError("levelTexts not bound");
            return;
        }

        for (int i = 0; i < levelTexts.Length; i++)
        {
            if (levelTexts[i] != null)
                levelTexts[i].text = "-";
        }

        foreach (var entry in entries)
        {
            if (entry == null) continue;

            int index = entry.level;

            if (index < 0 || index >= levelTexts.Length)
                continue;

            if (levelTexts[index] != null)
            {
                levelTexts[index].text =
                    $"Name: {entry.player_name}\nTime: {FormatTime(entry.time_ms)}";
            }
        }
    }

    private string FormatTime(int ms)
    {
        int minutes = ms / 60000;
        int seconds = (ms % 60000) / 1000;
        int millis = ms % 1000;

        return $"{minutes:00}:{seconds:00}.{millis:000}";
    }

    public void SubmitScore(int level, int timeMs)
    {
        StartCoroutine(SubmitScoreCoroutine(level, timeMs));
    }

    private IEnumerator SubmitScoreCoroutine(int level, int timeMs)
    {
        string url = $"{supabaseUrl}/rest/v1/rpc/submit_score";

        SubmitScoreRequest requestBody = new SubmitScoreRequest
        {
            p_level = (short)level,
            p_player_name = playerName,
            p_time_ms = timeMs
        };

        string json = JsonUtility.ToJson(requestBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", apiKey);
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowFeedback("Score submitted!", false);
            yield return GetLeaderboard();
        }
        else
        {
            Debug.LogError(request.downloadHandler.text);
            ShowFeedback("Failed to submit score", true);
        }
    }

    private void ShowFeedback(string message, bool isError)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = isError ? Color.red : Color.green;

        StartCoroutine(FadeFeedback());
    }

    private IEnumerator FadeFeedback()
    {
        yield return new WaitForSeconds(3f);

        if (feedbackText != null)
            feedbackText.text = "";
    }
    public void BindTexts(TMP_Text[] texts)
    {
        levelTexts = texts;
        RefreshLeaderboard();
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public int level;
    public string player_name;
    public int time_ms;
}

[System.Serializable]
public class SubmitScoreRequest
{
    public short p_level;
    public string p_player_name;
    public int p_time_ms;
}