using UnityEngine;
using UnityEngine.SceneManagement;

public class MPFinishButtonHandler : MonoBehaviour
{
    public void LeaveSession()
    {
        SessionData.socketManager.Disconnect();
        SceneManager.LoadScene("MultiplayerMenu");
    }
}
