﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;

public class Golf2Socket
{
    public WebSocket websocket;
    private string socketArg;
    public List<string> participants;
    private bool isMapSent;
    private Vector3 lastPosSent;
    private bool isFinished;
    public bool isOwner;

    // events

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

    public delegate void EndOfGameEvent(List<PlayerScore> scores);

    public static event EndOfGameEvent OnGameEnd;

    public delegate void ErrorEvent(string reason);

    public static event ErrorEvent OnError;
    
    public delegate void KickEvent();

    public static event KickEvent OnKick;

    // event classes

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

    public class PlayerScore
    {
        public string name;
        public float score;

        public PlayerScore(string name, float score)
        {
            this.name = name;
            this.score = score;
        }
    }

    // outgoing events
    private enum OutEventType
    {
        HANDSHAKE = 0,
        DISCONNECT = 1,
        SET_READY = 2,
        SET_UNREADY = 3,
        POS_SYNC = 4,
        FINISH = 5,
        REFRESH = 6,
        KICK = 7
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
        SESSION_CLOSED = 6,
        GAME_FINISHED = 7,
        KICK = 8
    }

    // message classes

    private class Message
    {
        public int type;
        public object content;
    }

    private class NakMessage
    {
        public int type;
        public Content content;

        public class Content
        {
            public string reason;
        }
    }

    private class GameEndMessage
    {
        public int type;
        public Content content;

        public class Content
        {
            public string[] participants;
            public float[] scores;
        }
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

    public Golf2Socket(string socketArg, bool isOwner)
    {
        InitConnection();
        this.socketArg = socketArg;
        this.isOwner = isOwner;
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
        //Debug.Log("Connection open");
    }

    private void OnSocketError(string e)
    {
        if (OnError != null) OnError(e);
        Debug.LogError(e);
        //Debug.Log("WebSocket error: " + e);
    }

    private void OnSocketClose(WebSocketCloseCode e)
    {
        //Debug.Log("Connection closed");
    }

    private void OnSocketMessage(byte[] bytes)
    {
        var messageText = System.Text.Encoding.UTF8.GetString(bytes);

        Message message = JsonConvert.DeserializeObject<Message>(messageText);

        /*if (message.type != (int)InEventType.POS_SYNC)
        {*/
            Debug.Log(messageText);
        //}
            
        switch (message.type)
        {
            case (int)InEventType.HANDSHAKE_ACK:
                break;
            case (int)InEventType.HANDSHAKE_NAK:
                HandleNak(messageText);
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
            case (int)InEventType.SESSION_CLOSED:
                HandleEndOfSession();
                break;
            case (int)InEventType.GAME_FINISHED:
                HandleGameEnd(messageText);
                break;
            case (int)InEventType.KICK:
                HandleKick();
                break;
        }
    }

    private void HandleNak(string messageText)
    {
        NakMessage message = JsonConvert.DeserializeObject<NakMessage>(messageText);
        if (OnError != null) OnError(message.content.reason);
    }

    private void HandleGameEnd(string messageText)
    {
        List<PlayerScore> scores = new List<PlayerScore>();
        GameEndMessage message = JsonConvert.DeserializeObject<GameEndMessage>(messageText);

        for (int i = 0; i < message.content.participants.Length; i++)
        {
            scores.Add(new PlayerScore(
                message.content.participants[i],
                message.content.scores[i]
            ));
        }

        scores = scores.OrderBy(s => s.score).ToList();

        if (OnGameEnd != null) OnGameEnd(scores);
    }

    private void HandleKick()
    {
        if (OnKick != null) OnKick();
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
        websocket.SendText(message);
    }

    public void Disconnect()
    {
        websocket.SendText(ComposeMessage(OutEventType.DISCONNECT));
    }


    public void SetReady()
    {
        if (!isOwner)
        {
            websocket.SendText(ComposeMessage(OutEventType.SET_READY));
            return;
        }

        // don't waste resources if the map has already been generated or will not be accepted
        if (isMapSent || participants.Count == 1)
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
        if (isFinished)
        {
            return;
        }
        
        float px = (float)Math.Round(pos.x, 3);
        float py = (float)Math.Round(pos.y, 3);
        float pz = (float)Math.Round(pos.z, 3);
        
        // don't waste bandwidth if the pos has not changed 
        pos = new Vector3(px, py, pz);
        if (pos == lastPosSent)
        {
            return;
        }

        double vx = Math.Round(velocity.x, 3);
        double vy = Math.Round(velocity.y, 3);
        double vz = Math.Round(velocity.z, 3);
        
        websocket.SendText(ComposeMessage(OutEventType.POS_SYNC, new { px, py, pz, vx, vy, vz }));
        lastPosSent = pos;
    }

    public void SendFinishPacket()
    {
        isFinished = true;
        double time = Math.Round(GameManager.instance.GetGameDuration(), 3);

        websocket.SendText(ComposeMessage(
            OutEventType.FINISH,
            new { time }
        ));
    }

    public void Refresh()
    {
        websocket.SendText(ComposeMessage(OutEventType.REFRESH));
    }
    
    public void KickPlayer(string playerName)
    {
        websocket.SendText(ComposeMessage(OutEventType.KICK, new {name = playerName}));
    }

    public void ResetSession()
    {
        isMapSent = false;
        isFinished = false;
    }
}