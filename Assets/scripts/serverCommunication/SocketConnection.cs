using System.Threading;
using NativeWebSocket;
using UnityEngine;

public class SocketConnection : MonoBehaviour
{
    public Golf2Socket socketManager;
    private bool initAttempted = false;
    private float timePassed;
    public static SocketConnection instance;

    void Start()
    {
        instance = this;
        socketManager = new Golf2Socket(SocketData.socketArg);
    }

    void Update()
    {
        if (socketManager.websocket.State == WebSocketState.Open && !initAttempted)
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