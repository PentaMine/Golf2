using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionCountdownController : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        Golf2Socket.OnSessionCountdown += UpdateCountdown;
        Golf2Socket.OnSessionBegin += OnSessionBegin;
    }

    private void UpdateCountdown(int remaining)
    {
        textComponent.text = remaining.ToString();
    }

    private void OnSessionBegin()
    {
        SessionData.socketManager = SocketConnection.instance.socketManager;
        SceneManager.LoadScene("MultiplayerLevel");
    }
    
}
