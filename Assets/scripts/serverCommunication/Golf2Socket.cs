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
    private bool isMapSent;

    public delegate void SyncEvent(SyncData data);

    public static event SyncEvent OnPlayerSync;

    public delegate void MapSyncEvent(ProceduralMapGenerator.MapData data);

    public static event MapSyncEvent OnMapSync;

    public class SyncData
    {
        public string owner;
        public List<string> participants;
        public List<string> ready;

        public SyncData(string owner, string[] participants, string[] ready)
        {
            this.owner = owner;
            this.participants = new List<string>(participants);
            this.ready = ready == null ? new List<string>() : new List<string>(ready);
        }
    }

    // outgoing events
    private enum OutEventType
    {
        HANDSHAKE = 0,
        DISCONNECT = 1,
        SET_READY = 2,
        SET_UNREADY = 3
    }

    // incoming events
    private enum InEventType
    {
        HANDSHAKE_ACK = 0,
        HANDSHAKE_NAK = 1,
        SYNCHRONISE = 2,
        MAP_SYNC = 3
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
            public string[] ready;
        }
    }

    private class MapDataMessage
    {
        public List<Vec2> path;
        public List<Vec2> boostPads;
        public Vec2 start;
        public Vec2 end;

        public MapDataMessage(List<Vec2> path, List<Vec2> boostPads, Vec2 start, Vec2 end)
        {
            this.path = path;
            this.boostPads = boostPads;
            this.start = start;
            this.end = end;
        }

        public class Vec2
        {
            public float x, y;

            public Vec2(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public Vector2 ToVector2()
            {
                return new Vector2(this.x, this.y);
            }
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
            case (int)InEventType.HANDSHAKE_ACK:
                Debug.Log("auth to session");
                isAuthorisedToSession = true;
                break;
            case (int)InEventType.HANDSHAKE_NAK:
                Debug.LogError("unauth to session");
                isAuthorisedToSession = false;
                break;
            case (int)InEventType.SYNCHRONISE:
                HandleSync(messageText);
                break;
            case (int)InEventType.MAP_SYNC:
                HandleMapSync(messageText);
                break;
        }
    }

    private void HandleMapSync(string messageText)
    {
        MapDataMessage message = JsonConvert.DeserializeObject<MapDataMessage>(messageText);

        if (OnMapSync == null)
        {
            return;
        }

        // invoke map sync event
        OnMapSync(new ProceduralMapGenerator.MapData(
            DesimplifyVectors(message.path),
            DesimplifyVectors(message.boostPads)
        ));
    }

    private void HandleSync(string messageText)
    {
        SyncMessage message = JsonConvert.DeserializeObject<SyncMessage>(messageText);
        if (OnPlayerSync == null)
        {
            return;
        }

        // invoke sync event.
        OnPlayerSync.Invoke(new SyncData(message.content.owner, message.content.participants, message.content.ready));
    }

    private string ComposeMessage(OutEventType type, object content = null)
    {
        return JsonConvert.SerializeObject(new { type, content });
    }

    private List<MapDataMessage.Vec2> SimplifyVectors(List<Vector2> vectors)
    {
        List<MapDataMessage.Vec2> simpleVectors = new List<MapDataMessage.Vec2>();
        foreach (var vector in vectors)
        {
            simpleVectors.Add(SimplifyVector(vector));
        }

        return simpleVectors;
    }

    private MapDataMessage.Vec2 SimplifyVector(Vector2 vector)
    {
        return new MapDataMessage.Vec2(vector.x, vector.y);
    }

    private List<Vector2> DesimplifyVectors(List<MapDataMessage.Vec2> vectors)
    {
        List<Vector2> simpleVectors = new List<Vector2>();
        foreach (var vector in vectors)
        {
            simpleVectors.Add(vector.ToVector2());
        }

        return simpleVectors;
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


    public void SetReady()
    {
        Debug.Log("ready");
        if (!Main.isSessionOwner)
        {
            websocket.SendText(ComposeMessage(OutEventType.SET_READY));
            return;
        }
        
        // don't waste resources if the map has already been generated
        if (isMapSent)
        {
            websocket.SendText(ComposeMessage(OutEventType.SET_READY));
            return;
        }

        // simplification of vectors is necessary because JsonConvert does not play well with the builtin Vector2 class
        // with lots of attributes, so we need to reduce the vectors to only x and y values  
        ProceduralMapGenerator.MapData mapData = ProceduralMapGenerator.GetMapData();
        MapDataMessage message = new MapDataMessage(
            SimplifyVectors(mapData.path),
            SimplifyVectors(mapData.boostPads),
            SimplifyVector(mapData.start),
            SimplifyVector(mapData.end)
        );

        websocket.SendText(ComposeMessage(OutEventType.SET_READY, JsonConvert.SerializeObject(new { mapData = message })));
        isMapSent = true;
    }

    public void SetUnready()
    {
        websocket.SendText(ComposeMessage(OutEventType.SET_UNREADY));
    }
}