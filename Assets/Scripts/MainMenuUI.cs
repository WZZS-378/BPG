using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
     [SerializeField] private TMP_InputField nameInput;

    public void OnPlayButton()
    {
        string playerName = nameInput.text;

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        SceneMan.instance.SetPlayerName(playerName);

        // load selected level
        int level = SceneMan.instance.selectedLevelIndex;

        LoadLevel(level);
    }

    public void SetLevel(int index)
    {
        SceneMan.instance.SetSelectedLevel(index);
    }

    private void LoadLevel(int index)
    {
        switch (index)
        {
            case 0: SceneMan.instance.LoadTut(); break;
            case 1: SceneMan.instance.LoadSceneOne(); break;
            case 2: SceneMan.instance.LoadSceneTwo(); break;
            case 3: SceneMan.instance.LoadSceneThree(); break;
            case 4: SceneMan.instance.LoadSceneFour(); break;
            case 5: SceneMan.instance.LoadSceneFive(); break;
            case 6: SceneMan.instance.LoadSceneSix(); break;
            default: SceneMan.instance.LoadTut(); break;
        }
    }
}