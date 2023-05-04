using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCanvasController : MonoBehaviour
{
    void Start()
    {
        Golf2Socket.OnSessionEnd += HandleSessionEnd;
    }

    private void HandleSessionEnd()
    {
        //Canvas endCanvas = GameObject.FindGameObjectWithTag("EndScreen").GetComponent<Canvas>();
        Canvas endCanvas = GetComponent<Canvas>();
        endCanvas.enabled = true;
        Invoke("Exit", 1);
    }

    private void Exit()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }

    private void OnDestroy()
    {
        Golf2Socket.OnSessionEnd -= HandleSessionEnd;
    }
}
