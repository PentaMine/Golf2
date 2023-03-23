using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerMenuButtonHandler : MonoBehaviour
{
    public void CreateSession()
    {
        GameObject.FindGameObjectWithTag("ConfirmBtn").GetComponent<MultiplayerMenuHandler>().CreateSessionButton();
    }

    public void Refresh()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }
}

