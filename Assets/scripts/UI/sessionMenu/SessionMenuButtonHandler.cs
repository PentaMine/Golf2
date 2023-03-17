using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionMenuButtonHandler : MonoBehaviour
{
    public void Leave()
    {
        SocketConnection.instance.webSocket.Disconnect();
        new Golf2Api().leaveSession();
        SceneManager.LoadScene("MultiplayerMenu");
    }
}
