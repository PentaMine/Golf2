using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using NativeWebSocket;

public class Connection : MonoBehaviour
{
    

    // Start is called before the first frame update
    private Golf2Socket webSocket;
    void Start()
    {
        webSocket = new Golf2Socket();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await webSocket.websocket.Close();
    }
}