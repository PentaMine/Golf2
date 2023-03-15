using NativeWebSocket;
using UnityEngine;

public class SocketConnection : MonoBehaviour
{
    // Start is called before the first frame update
    private Golf2Socket webSocket;
    private bool initAttempted = false;
    void Start()
    {
        webSocket = new Golf2Socket("Main.socketArg");
    }

    void Update()
    {
        if (webSocket.websocket.State == WebSocketState.Open && !initAttempted)
        {
            initAttempted = true;
            Debug.Log("rewr");
            webSocket.SendSocketArg();
        }
        
        
#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await webSocket.websocket.Close();
    }
}