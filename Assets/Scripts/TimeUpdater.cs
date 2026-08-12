using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUpdater : MonoBehaviour
{
    public bool timerIsRunning = false;
    public bool timerPaused = false;
    public TMP_Text timeText;
    public GameObject[] players;
    public float timer = 0.0f;
    public int seconds = 0;
    public int minutes = 0;
    public float milliSeconds = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneMan.instance != null && SceneMan.instance.TryGetSavedTimer(out float savedTime, out bool wasRunning))
        {
            timer = savedTime;
            timerIsRunning = wasRunning;
        }

        UpdateTimerDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerPaused)
        {
            return;
        }
        if (!timerIsRunning)
        {
            foreach(GameObject player in players)
            {
                bool checker = player.GetComponent<PlayerController>().firstMove;
                if(checker) {timerIsRunning = true;}
            }
        }
        
        if (timerIsRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        minutes = Mathf.FloorToInt(timer / 60);
        seconds = Mathf.FloorToInt(timer % 60);
        milliSeconds = (timer % 1) * 1000;

        timeText.text = string.Format("Timer: {0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
    }
}
