using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class OnlineGameManager : GameManager
{
    public GameObject playerBodyPrefab;
    public Material player1Material;
    public Material player2Material;
    public Material player3Material;
    public Material player4Material;

    private List<Material> materials;
    private Golf2Socket socketManager;
    private GameObject player;
    private Rigidbody playerRb;
    private Dictionary<string, Player> foreignPlayers = new Dictionary<string, Player>();
    private List<string> participants;
    private Canvas endCanvas;
    private Canvas scoreboard;
    private int playersFinished;

    private class Player
    {
        public GameObject playerObject;
        public Rigidbody playerRb;

        public Player(GameObject playerObject)
        {
            this.playerObject = playerObject;
            this.playerRb = playerObject.GetComponent<Rigidbody>();
        }
    }

    public OnlineGameManager() : base(true)
    {
        OnPausedChange += UpdateCanvas;
    }

    void Awake()
    {
        socketManager = SessionData.socketManager;

        player = GameObject.FindGameObjectWithTag("PlayerBody");
        playerRb = player.GetComponent<Rigidbody>();
        pauseScreen = GameObject.FindGameObjectWithTag("PauseScreen").GetComponent<Canvas>();
        pauseScreen.enabled = false;
        endCanvas = GameObject.FindGameObjectWithTag("EndScreen").GetComponent<Canvas>();
        scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<Canvas>();
        materials = new List<Material> { player1Material, player2Material, player3Material, player4Material };

        Golf2Socket.OnPosSync += OnPosSync;
        Golf2Socket.OnPlayerSync += HandlePlayerLeaving;
        Golf2Socket.OnSessionEnd += HandleSessionEnd;
        HoleController.onPlayerFinish += HandlePlayerFinish;
        Golf2Socket.OnGameEnd += HandleGameEnd;

        participants = new List<string>(socketManager.participants);
        CreateForeignPlayers();
    }

    void FixedUpdate()
    {
        socketManager.UpdatePos(player.transform.position, playerRb.velocity);
    }

    /*
     use LateUpdate instead of Update to avoid overriding the Update method in GameManager because
     it is responsible for game duration calculations and overriding it would not run them
     */
    private void LateUpdate()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        socketManager.websocket.DispatchMessageQueue();
#endif
    }

    void OnPosSync(string user, Vector3 pos, Vector3 velocity)
    {
        Player foreignPlayer = foreignPlayers[user];
        foreignPlayer.playerObject.transform.position = pos;
        foreignPlayer.playerRb.velocity = velocity;
    }

    void CreateForeignPlayers()
    {
        float distanceBetweenPlayers = 1.5f;
        float fullDistance = (socketManager.participants.Count - 1) * distanceBetweenPlayers;
        Vector3 startPos = player.transform.position - new Vector3(0, 0, fullDistance / 2);
        for (int i = 0; i < socketManager.participants.Count; i++)
        {
            Vector3 pos = startPos + new Vector3(0, 0, distanceBetweenPlayers * i);
            Debug.DrawRay(pos, Vector3.up, Color.red, 50000);
            if (!socketManager.participants[i].Equals(SettingManager.settings.name))
            {
                GameObject foreignPlayer = Instantiate(playerBodyPrefab, pos + new Vector3(0, .501f, 0), Quaternion.Euler(Vector3.zero));
                foreignPlayer.GetComponent<MeshRenderer>().material = materials[i];
                foreignPlayer.name = socketManager.participants[i];
                foreignPlayers.Add(socketManager.participants[i], new Player(foreignPlayer));
            }
            else
            {
                player.transform.position = pos;
                player.GetComponent<PlayerController>().material = materials[i];
            }
        }
    }

    private void UpdateCanvas(bool isPaused)
    {
        pauseScreen.enabled = isPaused;
    }

    private void HandlePlayerLeaving(Golf2Socket.SyncData data)
    {
        foreach (var participant in participants)
        {
            if (!socketManager.participants.Contains(participant))
            {
                Destroy(foreignPlayers[participant].playerObject);
                break;
            }
        }

        participants = new List<string>(socketManager.participants);
    }

    private void HandleSessionEnd()
    {
        Debug.Log("session died");
        endCanvas.enabled = true;
        socketManager.Disconnect();
        Invoke("Exit", 1);
    }

    private void Exit()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }

    private void HandlePlayerFinish(string playerName)
    {
        playersFinished++;
        if (playerName == "PlayerBody")
        {
            finalDuration = GetGameDuration();
            GameObject.FindGameObjectWithTag("Overlay").GetComponent<Canvas>().enabled = true;
            state = GameState.END;
            socketManager.SendFinishPacket();
            return;
        }

        foreignPlayers[playerName].playerObject.GetComponent<SphereCollider>().enabled = false;
        foreignPlayers[playerName].playerObject.GetComponent<MeshRenderer>().enabled = false;
        foreignPlayers[playerName].playerObject.SetActive(false);
    }

    private void HandleGameEnd(List<Golf2Socket.PlayerScore> scores)
    {
        Invoke("DisplayScoreboard", 1.5f);
        Invoke("LoadSessionMenu", 4.5f);
        SessionData.isReturning = true;

        socketManager.ResetSession();
    }

    private void DisplayScoreboard()
    {
        scoreboard.enabled = true;
    }

    private void LoadSessionMenu()
    {
        SceneManager.LoadScene("SessionMenu");
    }

    private void OnDestroy()
    {
        Golf2Socket.OnPosSync -= OnPosSync;
        Golf2Socket.OnPlayerSync -= HandlePlayerLeaving;
        Golf2Socket.OnSessionEnd -= HandleSessionEnd;
        HoleController.onPlayerFinish -= HandlePlayerFinish;
        Golf2Socket.OnGameEnd -= HandleGameEnd;
        OnPausedChange -= UpdateCanvas;
    }

    private void OnDisable()
    {
        Debug.Log("dsa");
    }
}