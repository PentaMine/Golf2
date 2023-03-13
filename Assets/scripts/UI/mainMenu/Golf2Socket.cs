using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using NativeWebSocket;
using Unity.VisualScripting;

//using WebSocketSharp;

public class Golf2Socket
{
    public WebSocket websocket;
    public enum WebSocketResponse
    {
        OK,
        NO_INTERNET,
    }
    public Golf2Socket()
    {
        InitConnection();
    }

    public void InitConnection()
    {
        websocket = new WebSocket(SettingManager.settings.getWebSocketUri());
        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("WebSocket error: " + e);
        };

        websocket.OnClose += (e) => { Debug.Log("Connection closed"); };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);
        };

        websocket.Connect();
    }
}