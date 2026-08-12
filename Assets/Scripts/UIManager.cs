using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject sceneMan;

    [Header("UI")]
    public TMP_InputField nameInput;
    [SerializeField] private TMP_Text[] leaderboardTexts;

    void Start()
    {
        sceneMan = GameObject.FindWithTag("SceneManager");
        LeaderboardUI leaderboard = FindFirstObjectByType<LeaderboardUI>();

        if (leaderboard != null)
        {
            leaderboard.BindTexts(leaderboardTexts);
        }
    }

    private void SetPlayerName()
    {
        string playerName = nameInput != null ? nameInput.text : "Player";

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        sceneMan.GetComponent<SceneMan>().SetPlayerName(playerName);
    }

    public void MainMenuButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().LoadMM();
    }

    public void LevelOneButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().SetSelectedLevel(1);
        sceneMan.GetComponent<SceneMan>().LoadSceneOne();
    }

    public void LevelTwoButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().SetSelectedLevel(2);
        sceneMan.GetComponent<SceneMan>().LoadSceneTwo();
    }

    public void LevelThreeButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().SetSelectedLevel(3);
        sceneMan.GetComponent<SceneMan>().LoadSceneThree();
    }

    public void LevelFourButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().SetSelectedLevel(4);
        sceneMan.GetComponent<SceneMan>().LoadSceneFour();
    }
    public void LevelFiveButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().SetSelectedLevel(5);
        sceneMan.GetComponent<SceneMan>().LoadSceneFive();
    }
    public void LevelSixButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().SetSelectedLevel(6);
        sceneMan.GetComponent<SceneMan>().LoadSceneSix();
    }
    public void TutButton()
    {
        SetPlayerName();
        sceneMan.GetComponent<SceneMan>().SetSelectedLevel(0);
        sceneMan.GetComponent<SceneMan>().LoadTut();
    }
}