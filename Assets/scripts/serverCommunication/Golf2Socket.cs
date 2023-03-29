using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

//using WebSocketSharp;

public class Golf2Socket
{
    public WebSocket websocket;
    private string socketArg;
    public bool isAuthorisedToSession;
    public List<string> participants;
    private bool isMapSent;

    public delegate void SocketConnectEvent();

    public static event SocketConnectEvent OnSocketConnect;

    public delegate void SyncEvent(SyncData data);

    public static event SyncEvent OnPlayerSync;

    public delegate void MapSyncEvent(ProceduralMapGenerator.MapData data);

    public static event MapSyncEvent OnMapSync;

    public delegate void SessionCountdownEvent(int timeRemaining);

    public static event SessionCountdownEvent OnSessionCountdown;

    public delegate void SessionBeginEvent();

    public static event SessionBeginEvent OnSessionBegin;

    public delegate void PosSyncEvent(string user, Vector3 pos, Vector3 velocity);

    public static event PosSyncEvent OnPosSync;
    
    public delegate void EndOfSessionEvent();

    public static event EndOfSessionEvent OnSessionEnd;

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
        SET_UNREADY = 3,
        POS_SYNC = 4
    }

    // incoming events
    private enum InEventType
    {
        HANDSHAKE_ACK = 0,
        HANDSHAKE_NAK = 1,
        SYNCHRONISE = 2,
        MAP_SYNC = 3,
        SESSION_COUNTDOWN = 4,
        POS_SYNC = 5,
        END_OF_SESSION = 6
    }

    private class Message
    {
        public int type;
        public object content;
    }

    private class PosSyncMessage
    {
        public int type;
        public Content content;

        public class Content
        {
            public string user;
            public float px, py, pz; // position
            public float vx, vz; // velocity
        }
    }

    private class SessionCountdownMessage
    {
        public int type;
        public Content content;

        public class Content
        {
            public int time;
        }
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

    private class MapData
    {
        public List<Vec2> path;
        public List<Vec2> boostPads;
        public Vec2 start;
        public Vec2 end;

        public MapData(List<Vec2> path, List<Vec2> boostPads, Vec2 start, Vec2 end)
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
                return new Vector2(x, y);
            }
        }
    }

    private class MapDataMessage
    {
        public int type;
        public Content content;

        public class Content
        {
            public MapData mapData;
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
        if (OnSocketConnect != null) OnSocketConnect();
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
            case (int)InEventType.SESSION_COUNTDOWN:
                HandleSessionCountdown(messageText);
                break;
            case (int)InEventType.POS_SYNC:
                HandlePosSync(messageText);
                break;
            case (int)InEventType.END_OF_SESSION:
                HandleEndOfSession();
                break;
        }
    }

    private void HandleEndOfSession()
    {
        if (OnSessionEnd != null) OnSessionEnd();
    }

    private void HandlePosSync(string messageText)
    {
        PosSyncMessage message = JsonConvert.DeserializeObject<PosSyncMessage>(messageText);

        if (OnPosSync != null)
            OnPosSync(
                message.content.user,
                new Vector3(
                    message.content.px,
                    message.content.py,
                    message.content.pz
                ),
                new Vector3(
                    message.content.vx,
                    0,
                    message.content.vz
                ));
    }

    private void HandleSessionCountdown(string messageText)
    {
        SessionCountdownMessage message = JsonConvert.DeserializeObject<SessionCountdownMessage>(messageText);

        if (OnSessionCountdown != null) OnSessionCountdown(message.content.time);
        if (OnSessionBegin != null && message.content.time == 0) OnSessionBegin();
    }

    private void HandleMapSync(string messageText)
    {
        MapDataMessage message = JsonConvert.DeserializeObject<MapDataMessage>(messageText);
        // invoke map sync event
        if (OnMapSync != null)
        {
            OnMapSync(new ProceduralMapGenerator.MapData(
                DesimplifyVectors(message.content.mapData.path),
                DesimplifyVectors(message.content.mapData.boostPads)
            ));
        }
    }

    private void HandleSync(string messageText)
    {
        SyncMessage message = JsonConvert.DeserializeObject<SyncMessage>(messageText);
        // invoke sync event.
        if (OnPlayerSync != null) OnPlayerSync.Invoke(new SyncData(message.content.owner, message.content.participants, message.content.ready));

        List<string> newParticipants = new List<string>(message.content.participants);
        newParticipants.Add(message.content.owner);

        participants = newParticipants;
    }

    private string ComposeMessage(OutEventType type, object content = null)
    {
        return JsonConvert.SerializeObject(new { type, content });
    }

    private List<MapData.Vec2> SimplifyVectors(List<Vector2> vectors)
    {
        List<MapData.Vec2> simpleVectors = new List<MapData.Vec2>();
        foreach (var vector in vectors)
        {
            simpleVectors.Add(SimplifyVector(vector));
        }

        return simpleVectors;
    }

    private MapData.Vec2 SimplifyVector(Vector2 vector)
    {
        return new MapData.Vec2(vector.x, vector.y);
    }

    private List<Vector2> DesimplifyVectors(List<MapData.Vec2> vectors)
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
        if (!SocketData.isSessionOwner)
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
        MapData message = new MapData(
            SimplifyVectors(mapData.path),
            SimplifyVectors(mapData.boostPads),
            SimplifyVector(mapData.start),
            SimplifyVector(mapData.end)
        );

        websocket.SendText(ComposeMessage(OutEventType.SET_READY, new { mapData = message }));
        isMapSent = true;
    }

    public void SetUnready()
    {
        websocket.SendText(ComposeMessage(OutEventType.SET_UNREADY));
    }

    public void UpdatePos(Vector3 pos, Vector3 velocity)
    {
        double px = Math.Round(pos.x, 2);
        double py = Math.Round(pos.y, 2);
        double pz = Math.Round(pos.z, 2);        
        
        double vx = Math.Round(velocity.x, 3);
        double vy = Math.Round(velocity.y, 3);
        double vz = Math.Round(velocity.z, 3);
        websocket.SendText(ComposeMessage(OutEventType.POS_SYNC, new { px, py, pz, vx, vy, vz }));
    }
}