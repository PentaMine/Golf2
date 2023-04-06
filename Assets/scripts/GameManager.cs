using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState state;
    public float finalDuration;
    public float gameDuration;
    public bool isPaused;
    public Canvas pauseScreen;
    public static GameManager instance;
    private bool isOnline;

    public delegate void PausedChange(bool isPaused);

    public event PausedChange OnPausedChange;

    public enum GameState
    {
        START, // the game started, but the player did not do anything
        ONGOING, // the player is doing something
        END // the player reached the hole
    }

    public GameManager(bool isOnline)
    {
        HoleController.onPlayerFinish += OnPlayerFinish;
        PlayerController.onPlayerShoot += OnPlayerShoot;
        state = GameState.START;
        instance = this;
        this.isOnline = isOnline;
    }

    public float GetGameDuration()
    {
        if (state == GameState.START && !isOnline)
        {
            return 0;
        }

        if (state == GameState.END)
        {
            return finalDuration;
        }

        return gameDuration;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (OnPausedChange != null && state != GameState.END) OnPausedChange(isPaused);
        }

        if ((!isPaused && state != GameState.START) || isOnline)
        {
            gameDuration += Time.deltaTime;
        }
    }

    void OnPlayerFinish(string name)
    {
        if (isOnline)
        {
            return;
        }
        finalDuration = GetGameDuration();
        // display the end screen
        GameObject.FindGameObjectWithTag("Overlay").GetComponent<Canvas>().enabled = true;
        state = GameState.END;
    }

    void OnPlayerShoot(Vector3 force)
    {
        state = GameState.ONGOING;
    }

    void OnDisable()
    {
        HoleController.onPlayerFinish -= OnPlayerFinish;
        PlayerController.onPlayerShoot -= OnPlayerShoot;
    }
}