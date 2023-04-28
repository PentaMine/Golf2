using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionMenuButtonHandler : MonoBehaviour
{
    public void Leave()
    {
        SceneManager.LoadScene("MultiplayerMenu");
        SocketConnection.instance.socketManager.Disconnect();
        new Golf2Api().leaveSession();
    }

    public void Ready()
    {
        SocketConnection.instance.socketManager.SetReady();
    }
    
    public void Unready()
    {
        SocketConnection.instance.socketManager.SetReady();
    }
}
