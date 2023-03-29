using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class OnlineGameManager : GameManager
{
    public GameObject playerBodyPrefab;
    private Golf2Socket socketManager;
    private GameObject player;
    private Rigidbody playerRb;
    private Dictionary<string, Player> foreignPlayers = new Dictionary<string, Player>();
    private List<string> participants;
    private const string foreignPlayerPrefix = "FP";
    private Canvas endCanvas;

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

        Golf2Socket.OnPosSync += OnPosSync;
        Golf2Socket.OnPlayerSync += HandlePlayerLeaving;
        Golf2Socket.OnSessionEnd += HandleSessionEnd;

        CreateForeignPlayers();

        participants = new List<string>(socketManager.participants);
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
        socketManager.participants.ForEach(s => { Debug.LogAssertion(s); });
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
                //foreignPlayer.tag = foreignPlayerPrefix + socketManager.participants[i];
                foreignPlayers.Add(socketManager.participants[i], new Player(foreignPlayer));
            }
            else
            {
                player.transform.position = pos;
            }
        }
    }

    private void UpdateCanvas(bool isPaused)
    {
        pauseScreen.enabled = isPaused;
    }

    private void HandlePlayerLeaving(Golf2Socket.SyncData data)
    {
        Debug.LogWarning("fsdfsdfsdfdsd");
        foreach (var participant in participants)
        {
            if (!socketManager.participants.Contains(participant))
            {
                Destroy(foreignPlayers[participant].playerObject);
            }
        }
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
}