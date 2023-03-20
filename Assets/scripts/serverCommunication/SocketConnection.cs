using NativeWebSocket;
using UnityEngine;

public class SocketConnection : MonoBehaviour
{
    public Golf2Socket webSocket;
    private bool initAttempted = false;
    private float timePassed;
    public static SocketConnection instance;

    void Start()
    {
        instance = this;
        webSocket = new Golf2Socket(Main.socketArg);
    }

    void Update()
    {
        if (webSocket.websocket.State == WebSocketState.Open && !initAttempted)
        {
            initAttempted = true;
            webSocket.SendSocketArg();
        }

        if ((!webSocket.isAuthorisedToSession || webSocket.websocket.State != WebSocketState.Open) && timePassed > 5)
        {
            Debug.Log("failed to connect");
        }


#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.websocket.DispatchMessageQueue();
#endif
    }

    private void OnApplicationQuit()
    {
        webSocket.Disconnect();
        new Golf2Api().leaveSession();
    }
}