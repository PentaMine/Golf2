using NativeWebSocket;
using UnityEngine;

public class SocketConnection : MonoBehaviour
{
    public Golf2Socket socketManager;
    private bool initAttempted = false;
    private float timePassed;
    public static SocketConnection instance;
    private bool didReturn;

    void Start()
    {
        
        instance = this;
        if (SessionData.isReturning)
        {
            socketManager = SessionData.socketManager;
            socketManager.Refresh();
            didReturn = true;
            SessionData.isReturning = false;
        }
        else
        {
            socketManager = new Golf2Socket(SocketData.socketArg);
        }

        Golf2Socket.OnError += reason => Debug.LogError(reason);
    }

    void Update()
    {
        if (socketManager.websocket.State == WebSocketState.Open && !initAttempted && !didReturn)
        {
            initAttempted = true;
            socketManager.SendSocketArg();
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        socketManager.websocket.DispatchMessageQueue();
#endif
    }

    private void OnApplicationQuit()
    {
        socketManager.Disconnect();
        new Golf2Api().leaveSession();
    }
}