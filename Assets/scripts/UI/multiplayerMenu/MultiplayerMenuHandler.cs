using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MultiplayerMenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject usernameInputObject;
    private TextMeshProUGUI usernameInput;

    public GameObject errorMessageObject;
    private TextMeshProUGUI errorMessageComponent;

    public GameObject noAuthCanvasObject;
    private Canvas noAuthCanvas;

    public GameObject noInternetCanvasObject;
    private Canvas noInternetCanvas;

    public GameObject loadingCanvasObject;
    private Canvas loadingCanvas;

    public GameObject oldVersionCanvasObject;
    private Canvas oldVersionCanvas;

    [FormerlySerializedAs("sessionInfoContent")]
    public GameObject sessionInfoList;

    public GameObject sessionButtonPrefab;

    public GameObject loadingIconObject;
    private RawImage loadingIcon;

    private Golf2Api api;

    private delegate void SessionFetchEvent(List<Golf2Api.Session> sessions);

    private event SessionFetchEvent OnSessionFetch;

    private delegate void VerifyAuthEvent(Golf2Api.ApiResponse response);

    private event VerifyAuthEvent OnVerifyAuth;

    private delegate void UsernameConfirmationEvent(Golf2Api.ApiResponse response, string message);

    private event UsernameConfirmationEvent OnUsernameConfirmation;

    void Awake()
    {
        api = new Golf2Api();
        usernameInput = usernameInputObject.GetComponent<TextMeshProUGUI>();
        errorMessageComponent = errorMessageObject.GetComponent<TextMeshProUGUI>();
        noAuthCanvas = noAuthCanvasObject.GetComponent<Canvas>();
        loadingCanvas = loadingCanvasObject.GetComponent<Canvas>();
        noInternetCanvas = noInternetCanvasObject.GetComponent<Canvas>();
        oldVersionCanvas = oldVersionCanvasObject.GetComponent<Canvas>();
        errorMessageComponent.text = "";
        loadingIcon = loadingIconObject.GetComponent<RawImage>();
        loadingIcon.enabled = false;
        loadingCanvas.enabled = true;

        // verify auth and fetch sessions asynchronously
        new Thread(VerifyAuth).Start();
        new Thread(FetchSessions).Start();

        OnVerifyAuth += OnAuth;
        OnUsernameConfirmation += OnUsernameCheck;
        OnSessionFetch += LoadSessionList;
    }

    void FetchSessions()
    {
        api.getAvailableSessions(out List<Golf2Api.Session> sessions);
        if (OnSessionFetch != null) OnSessionFetch(sessions);
    }

    void VerifyAuth()
    {
        Golf2Api.ApiResponse response = api.verifyAuth();
        if (OnVerifyAuth != null) OnVerifyAuth(response);
    }

    public void SubmitUsername(string name)
    {
        Tuple<Golf2Api.ApiResponse, string> response = api.authenticateClient(name);
        if (OnUsernameConfirmation != null) OnUsernameConfirmation(response.Item1, response.Item2);
    }

    public void SubmitUsernameBtn()
    {
        loadingIcon.enabled = true;
        new Thread(() => SubmitUsername(usernameInput.text)).Start();
    }


    private void OnAuth(Golf2Api.ApiResponse response)
    {
        // run on main thread because ui components can only be modified on the main thread
        MainThreadWorker.mainThread.AddJob(() =>
        {
            loadingCanvas.enabled = false;
            noAuthCanvas.enabled = response != Golf2Api.ApiResponse.OK;
            noInternetCanvas.enabled = response == Golf2Api.ApiResponse.NO_INTERNET;
            oldVersionCanvas.enabled = response == Golf2Api.ApiResponse.OLD_VERSION;
        });
    }

    void LoadSessionList(List<Golf2Api.Session> sessions)
    {
        // run on main thread because gameobjects can only be instantiated on the main thread
        MainThreadWorker.mainThread.AddJob(() =>
        {
            foreach (Golf2Api.Session session in sessions)
            {
                Instantiate(sessionButtonPrefab,/* Vector3.zero, Quaternion.Euler(Vector3.zero), */sessionInfoList.transform)
                    .GetComponent<SessionButtonController>().SetData(session.owner, session.participants, session.id);
            }
        });
    }

    void OnUsernameCheck(Golf2Api.ApiResponse response, string message)
    {
        // run on main thread because ui components can only be modified on the main thread
        MainThreadWorker.mainThread.AddJob(() =>
        {
            errorMessageComponent.text = message;
            noAuthCanvas.enabled = response != Golf2Api.ApiResponse.OK;
            loadingIcon.enabled = false;
        });
    }

    public void CreateSessionButton()
    {
        api.createSession(out string socketArg);
        SocketData.socketArg = socketArg;
        SocketData.isSessionOwner = true;
        SceneManager.LoadScene("SessionMenu");
    }
}