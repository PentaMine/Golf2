using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class MultiplayerMenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject usernameInputObject;
    private TextMeshProUGUI usernameInput;
    public GameObject errorMessageObject;
    private TextMeshProUGUI errorMessageComponent;
    public GameObject noAuthCanvasObject;
    private Canvas noAuthCanvas;
    private bool auth;
    private Golf2ApiWrapper api;
    private List<Golf2ApiWrapper.Session> sessions = new();
    void Awake()
    {
        api = new Golf2ApiWrapper();
        usernameInput = usernameInputObject.GetComponent<TextMeshProUGUI>();
        errorMessageComponent = errorMessageObject.GetComponent<TextMeshProUGUI>();
        noAuthCanvas = noAuthCanvasObject.GetComponent<Canvas>();
        auth = api.verifyAuth() == Golf2ApiWrapper.ApiResponse.OK;
        noAuthCanvas.enabled = !auth;
        Debug.Log(auth);
        errorMessageComponent.text = "";
        new Thread(() => api.getAvailableSessions(out sessions)).Start();
    }
    
    public void SubmitUsername()
    {
        Golf2ApiWrapper.ApiResponse response = api.authenticateClient(usernameInput.text);
        switch (response)
        {
            case Golf2ApiWrapper.ApiResponse.USERNAME_TAKEN:
                errorMessageComponent.text = "USERNAME ALREADY TAKEN";
                break;
            case Golf2ApiWrapper.ApiResponse.NO_INTERNET:
                errorMessageComponent.text = "NO INTERNET CONNECTION";
                break;
        }
        noAuthCanvas.enabled = !auth;
    }

    private void FixedUpdate()
    {
        Debug.Log(sessions.Count);
    }
}