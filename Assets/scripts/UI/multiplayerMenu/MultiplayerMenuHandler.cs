using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool auth;
    private Golf2Api api;
    private List<Golf2Api.Session> sessions = new();
    public GameObject loadingCanvasObject;
    private Canvas loadingCanvas;
    private bool isConnectionAttempted = false;
    private bool areSessionsLoaded = false;
    public GameObject sessionInfoContent;
    public GameObject sessionButtonPrefab;
    private bool isNoInternet = false;

    void Awake()
    {
        api = new Golf2Api();
        usernameInput = usernameInputObject.GetComponent<TextMeshProUGUI>();
        errorMessageComponent = errorMessageObject.GetComponent<TextMeshProUGUI>();
        noAuthCanvas = noAuthCanvasObject.GetComponent<Canvas>();
        loadingCanvas = loadingCanvasObject.GetComponent<Canvas>();
        noInternetCanvas = noInternetCanvasObject.GetComponent<Canvas>();
        errorMessageComponent.text = "";

        new Thread(() => api.getAvailableSessions(out sessions)).Start();
        new Thread(() => Authenticate()).Start();
    }

    public void SubmitUsername()
    {
        Golf2Api.ApiResponse response = api.authenticateClient(usernameInput.text);
        switch (response)
        {
            case Golf2Api.ApiResponse.USERNAME_TAKEN:
                errorMessageComponent.text = "USERNAME ALREADY TAKEN";
                break;
            case Golf2Api.ApiResponse.NO_INTERNET:
                errorMessageComponent.text = "NO INTERNET CONNECTION";
                break;
        }

        noAuthCanvas.enabled = !auth;
    }

    private void Authenticate()
    {
        Golf2Api.ApiResponse verification = api.verifyAuth();
        auth = verification == Golf2Api.ApiResponse.OK;
        isNoInternet = verification == Golf2Api.ApiResponse.NO_INTERNET;
        isConnectionAttempted = true;
        Debug.Log(isNoInternet);
        Debug.Log(api.verifyAuth());
    }

    private void FixedUpdate()
    {
        loadingCanvas.enabled = !isConnectionAttempted;
        noAuthCanvas.enabled = !auth;
        noInternetCanvas.enabled = isNoInternet;
        if (sessions.Count > 0 && !areSessionsLoaded)
        {
            Debug.Log(sessions.Count);
            foreach (Golf2Api.Session session in sessions)
            {
                Instantiate(sessionButtonPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), sessionInfoContent.transform).GetComponent<SessionButtonController>().SetData(session.owner, session.participants, session.id);
            }
            areSessionsLoaded = true;
        }
    }

    public void CreateSessionButton()
    {
        api.createSession(out string socketArg);
        SocketData.socketArg = socketArg;
        SocketData.isSessionOwner = true;
        Debug.Log(socketArg);
        SceneManager.LoadScene("SessionMenu");
    }
}