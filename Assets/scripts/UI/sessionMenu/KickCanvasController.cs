using UnityEngine;
using UnityEngine.SceneManagement;

public class KickCanvasController : MonoBehaviour
{
    void Start()
    {
        Golf2Socket.OnKick += HandleKick;
    }

    private void HandleKick()
    {
        //Canvas endCanvas = GameObject.FindGameObjectWithTag("KickScreen").GetComponent<Canvas>();
        Canvas kickCanvas = GetComponent<Canvas>();
        kickCanvas.enabled = true;
        Invoke("Exit", 1);
    }

    private void Exit()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }

    private void OnDestroy()
    {
        Golf2Socket.OnKick -= HandleKick;
    }
}
