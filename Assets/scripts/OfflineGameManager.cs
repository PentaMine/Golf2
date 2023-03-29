using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class OfflineGameManager : GameManager
{
    public OfflineGameManager() : base(false)
    {
        OnPausedChange += UpdateCanvas;
    }

    void Awake()
    {
        pauseScreen = GameObject.FindGameObjectWithTag("PauseScreen").GetComponent<Canvas>();
        pauseScreen.enabled = false;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        OnPausedChange -= UpdateCanvas;
    }

    private void UpdateCanvas(bool isPaused)
    {
        pauseScreen.enabled = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
}