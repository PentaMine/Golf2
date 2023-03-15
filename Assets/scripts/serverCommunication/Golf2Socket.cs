using System;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//using WebSocketSharp;

public class Golf2Socket
{
    public WebSocket websocket;
    private string socketArg;
    public bool isInSession;

    public enum WebSocketResponse
    {
        OK,
        NO_INTERNET,
    }

    // outgoing events
    private enum OutEventType
    {
        HANDSHAKE = 0
    }

    // incoming events
    private enum InEventType
    {
    }

    public Golf2Socket(string socketArg)
    {
        InitConnection();
        this.socketArg = socketArg;
    }

    private void InitConnection()
    {
        websocket = new WebSocket(SettingManager.settings.getWebSocketUri());

        websocket.OnOpen += OnSocketOpen;
        websocket.OnError += OnSocketError;
        websocket.OnClose += OnSocketClose;
        websocket.OnMessage += OnSocketMessage;

        websocket.Connect();
    }

    private void OnSocketOpen()
    {
        Debug.Log("Connection open");
    }

    private void OnSocketError(string e)
    {
        Debug.Log("WebSocket error: " + e);
    }

    private void OnSocketClose(WebSocketCloseCode e)
    {
        Debug.Log("Connection closed");
    }

    private void OnSocketMessage(byte[] bytes)
    {
        var message = System.Text.Encoding.UTF8.GetString(bytes);
        Debug.Log("Received: " + message);
    }

    private string ComposeMessage(OutEventType type, object content)
    {
        return JsonConvert.SerializeObject(new {type = type, content });
    }

    public void SendSocketArg()
    {
        if (websocket.State != WebSocketState.Open)
        {
            throw new Exception("Socket not connected!");
        }

        string message = ComposeMessage(OutEventType.HANDSHAKE, new { socketArg });
        Debug.Log(message);
        websocket.SendText(message);
    }
}