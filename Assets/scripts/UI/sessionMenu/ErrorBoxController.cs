using TMPro;
using UnityEngine;

public class ErrorBoxController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Golf2Socket.OnError += HandleError;
    }

    // Update is called once per frame
    void HandleError(string e)
    {
        GetComponent<TextMeshProUGUI>().text = "ERROR: " + e.ToUpper();
    }

    private void OnDestroy()
    {
        Golf2Socket.OnError -= HandleError;
    }
}
