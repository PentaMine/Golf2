using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine.UIElements;

//using WebSocketSharp;

public class Golf2Socket
{
    public WebSocket websocket;
    private string socketArg;
    public bool isAuthorisedToSession;
    public delegate void SyncEvent(SyncData data);

    public static event SyncEvent OnSync;

    public class SyncData
    {
        public string owner;
        public List<string> participants;

        public SyncData(string owner, string[] participants)
        {
            this.owner = owner;
            this.participants = new List<string>(participants);
        }
    }

    // outgoing events
    private enum OutEventType
    {
        HANDSHAKE = 0,
        DISCONNECT = 1
    }

    // incoming events
    private enum InEventType
    {
        HANDSHAKE_ACK = 0,
        HANDSHAKE_NAK = 1,
        SYNCHRONISE = 2
    }
    
    private class Message
    {
        public int type;
        public object content;
    }
    
    private class SyncMessage
    {
        public int type;
        public Content content;
        public class Content
        {
            public string owner;
            public string[] participants;
        }
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
        var messageText = System.Text.Encoding.UTF8.GetString(bytes);
        Debug.Log("Received: " + messageText);

        Message message = JsonConvert.DeserializeObject<Message>(messageText);

        switch (message.type)
        {
            case (int) InEventType.HANDSHAKE_ACK:
                Debug.Log("auth to session");
                isAuthorisedToSession = true;
                break;
            case (int) InEventType.HANDSHAKE_NAK:
                Debug.LogError("unauth to session");
                isAuthorisedToSession = false;
                break;
            case (int) InEventType.SYNCHRONISE:
                HandleSync(messageText);
                break;
        }
    }

    private void HandleSync(string messageText)
    {
        SyncMessage message = JsonConvert.DeserializeObject<SyncMessage>(messageText);
        // invoke sync event.
        OnSync.Invoke(new SyncData(message.content.owner, message.content.participants));
    }
    
    private string ComposeMessage(OutEventType type, object content = null)
    {
        return JsonConvert.SerializeObject(new { type, content });
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

    public void Disconnect()
    {
        websocket.SendText(ComposeMessage(OutEventType.DISCONNECT));
    }
}