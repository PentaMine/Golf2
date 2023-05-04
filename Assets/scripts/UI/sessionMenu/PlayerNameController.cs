using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerNameController : MonoBehaviour
{
    private TextMeshProUGUI textComp;
    private Button buttonComp;
    private RawImage buttonImage;
    private string playerName;
    private HorizontalLayoutGroup layoutGroup;
    private bool isUpdateRequired;
    private float lastDelta;

    void Awake()
    {
        GameObject textObject = transform.Find("text").gameObject;
        textComp = textObject.GetComponent<TextMeshProUGUI>();

        GameObject buttonObject = transform.Find("button").gameObject;
        buttonComp = buttonObject.GetComponent<Button>();
        buttonImage = buttonObject.GetComponent<RawImage>();
        buttonComp.onClick.AddListener(KickPlayer);
    }

    public void SetData(string name, bool isOwner)
    {
        playerName = name;
        textComp.text = name;
        buttonComp.enabled = isOwner;
        buttonImage.enabled = isOwner;
        isUpdateRequired = true;
        
        // add layout group 
        layoutGroup = gameObject.AddComponent(typeof(HorizontalLayoutGroup)) as HorizontalLayoutGroup;
        layoutGroup.spacing = 6f;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
    }

    void KickPlayer()
    {
        SocketConnection.instance.socketManager.KickPlayer(playerName);
    }

    private void Update()
    {
        lastDelta = Time.deltaTime;
        // update the 
        layoutGroup.childForceExpandHeight = false;
    }
}