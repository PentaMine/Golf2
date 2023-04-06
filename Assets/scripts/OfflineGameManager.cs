using UnityEngine;

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

    public void OnDestroy()
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