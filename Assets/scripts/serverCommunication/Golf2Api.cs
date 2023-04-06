using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;



public class Golf2Api
{
    public enum ApiResponse
    {
        OK,
        NO_INTERNET,
        USERNAME_TAKEN,
        UNAUTHORISED,
        NOT_IN_SESSION
    }

    private class Response
    {
        public HttpStatusCode code;
        public string body;

        public Response(HttpStatusCode code, string body)
        {
            this.code = code;
            this.body = body;
        }
    }

    private class ClientAuthResponse
    {
        public int code;
        public ResponseTag response;

        public class ResponseTag
        {
            public string token;
        };
    }

    private class SessionBrowseResponse
    {
        public int code;
        public ResponseTag response;

        public class ResponseTag
        {
            public SessionTag[] sessions;

            public class SessionTag
            {
                public int id;
                public string owner;
                public string[] participant_list;
            }
        };
    }
    
    private class SessionJoinResponse
    {
        public int code;
        public ResponseTag response;

        public class ResponseTag
        {
            public string socketArg;
        };
    }

    public class Session
    {
        public Session(int id, string owner, string[] participants)
        {
            this.id = id;
            this.owner = owner;
            this.participants = new List<string>(participants);
        }

        public int id;
        public string owner;
        public List<string> participants;
    }

    private Response makeRequest(string path, HttpMethod method, string authToken = "", string body = "")
    {
        //Debug.Log(SettingManager.settings.getFullHTTPUri(path));
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(SettingManager.settings.getFullHTTPUri(path)),
            Content = body.Length > 0 ? new StringContent(body, Encoding.UTF8, "application/json") : null,
            Headers =
            {
                { "Authorization", "Bearer " + authToken },
            }
        };
        using (var response = client.SendAsync(request).GetAwaiter().GetResult())
        {
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return new Response(response.StatusCode, responseBody);
        }
    }

    public ApiResponse verifyAuth()
    {
        try
        {
            Response response = makeRequest("/verifyauth", HttpMethod.Get, SettingManager.settings.authToken);
            return response.code == HttpStatusCode.OK ? ApiResponse.OK : ApiResponse.UNAUTHORISED;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return ApiResponse.NO_INTERNET;
        }
    }

    public ApiResponse authenticateClient(string name)
    {   
        try
        {
            name = name.Trim(trimChars: new[] { ' ', '\u200b' });
            Response response = makeRequest("/clientauth", HttpMethod.Post, body: JsonConvert.SerializeObject(new { name = name }));

            if (response.code == HttpStatusCode.OK)
            {
                SettingManager.settings.authToken = JsonConvert.DeserializeObject<ClientAuthResponse>(response.body).response.token;
                SettingManager.settings.name = name;
                SettingManager.SaveSettings();
                return ApiResponse.OK;
            }

            return ApiResponse.USERNAME_TAKEN;
        }
        catch (Exception)
        {
            return ApiResponse.NO_INTERNET;
        }
    }


    public ApiResponse getAvailableSessions(out List<Session> sessions)
    {
        List<Session> ConvertToSessions(SessionBrowseResponse responses)
        {
            List<Session> sessions = new List<Session>();
            foreach (SessionBrowseResponse.ResponseTag.SessionTag res in responses.response.sessions)
            {
                sessions.Add(new Session(res.id, res.owner, res.participant_list));
            }

            return sessions;
        }

        sessions = new List<Session>();

        try
        {
            Response response = makeRequest("/browsesessions", HttpMethod.Post, body: JsonConvert.SerializeObject(new { count = 5 }));

            if (response.code == HttpStatusCode.OK)
            {
                SessionBrowseResponse sessionBrowse = JsonConvert.DeserializeObject<SessionBrowseResponse>(response.body);
                sessions = ConvertToSessions(sessionBrowse);
                return ApiResponse.OK;
            }

            return ApiResponse.USERNAME_TAKEN;
        }
        catch (Exception)
        {
            return ApiResponse.NO_INTERNET;
        }
    }

    public ApiResponse joinSession(int id, out string socketToken, bool didRetry = false)
    {
        socketToken = "";
        try
        {
            Response response = makeRequest("/joinsession", HttpMethod.Post, authToken: SettingManager.settings.authToken, body: JsonConvert.SerializeObject(new { sessionId = id }));

            if (response.code == HttpStatusCode.Conflict && !didRetry)
            {
                leaveSession();
                return joinSession(id, out socketToken, didRetry: true);
            }
            else if (response.code == HttpStatusCode.Unauthorized)
            {
                return ApiResponse.UNAUTHORISED;
            }

            socketToken = JsonConvert.DeserializeObject<SessionJoinResponse>(response.body).response.socketArg;
            return ApiResponse.OK;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return ApiResponse.NO_INTERNET;
        }
    }
    
    public ApiResponse leaveSession()
    {
        try
        {
            Response response = makeRequest("/leavesession", HttpMethod.Post, authToken: SettingManager.settings.authToken);
            
            if (response.code == HttpStatusCode.BadRequest)
            {
                return ApiResponse.NOT_IN_SESSION;
            }

            return ApiResponse.OK;
        }
        catch (Exception)
        {
            return ApiResponse.NO_INTERNET;
        }
    }
    
    public ApiResponse createSession(out string socketToken, bool didRetry = false)
    {
        socketToken = "";
        try
        {
            Response response = makeRequest("/newsession", HttpMethod.Post, authToken: SettingManager.settings.authToken);

            if (response.code == HttpStatusCode.Conflict && !didRetry)
            {
                leaveSession();
                return createSession(out socketToken, didRetry: true);
            }
            else if (response.code == HttpStatusCode.Unauthorized)
            {
                return ApiResponse.UNAUTHORISED;
            }

            socketToken = JsonConvert.DeserializeObject<SessionJoinResponse>(response.body).response.socketArg;
            return ApiResponse.OK;
        }
        catch (Exception)
        {
            return ApiResponse.NO_INTERNET;
        }
        
    }
}