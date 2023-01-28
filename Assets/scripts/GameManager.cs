using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;
    private float finalDuration;
    private float gameDuration;
    private Canvas pauseScreen;
    public bool isPaused;

    public enum GameState
    {
        START, // the game started, but the player did not do anything
        ONGOING, // the player is doing something
        END // the player reached the hole
    }

    void Awake()
    {
        instance = this;
        state = GameState.START;
        pauseScreen = GameObject.FindGameObjectWithTag("PauseScreen").GetComponent<Canvas>();
        pauseScreen.enabled = false;

        // subscribe to game events
        HoleController.onPlayerFinish += OnPlayerFinish;
        PlayerController.onPlayerShoot += OnPlayerShoot;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseScreen.enabled = isPaused;
            if (isPaused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }

        if (!isPaused && state != GameState.START)
        {
            gameDuration += Time.deltaTime;
        }
    }

    void OnPlayerFinish()
    {
        finalDuration = GetGameDuration();
        state = GameState.END;
        // display the end screen
        GameObject.FindGameObjectWithTag("Overlay").GetComponent<Canvas>().enabled = true;
    }

    void OnPlayerShoot(Vector3 force)
    {
        state = GameState.ONGOING;
    }

    public float GetGameDuration()
    {
        if (state == GameState.START)
        {
            return 0;
        }

        if (state == GameState.END)
        {
            return finalDuration;
        }

        return gameDuration;
    }

    private void UnPause()
    {
        // let time flow at the normal rate
        Time.timeScale = 1f;
    }

    private void Pause()
    {
        // stop time
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        UnPause();
    }
}