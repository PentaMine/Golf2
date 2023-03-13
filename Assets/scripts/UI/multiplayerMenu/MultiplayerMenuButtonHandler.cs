using UnityEngine;

public class MultiplayerMenuButtonHandler : MonoBehaviour
{
    public void CreateSession()
    {
        GameObject.FindGameObjectWithTag("ConfirmBtn").GetComponent<MultiplayerMenuHandler>().CreateSessionButton();
    }
}

