using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListController : MonoBehaviour
{
    public GameObject playerNameObject;
    public GameObject ownerObject;

    private const string croText = " (spreman)";
    private const string engText = " (ready)";
    private TextMeshProUGUI ownerTextComp;
    private bool isInitialised;
    private Dictionary<string, GameObject> foreignNames = new();
    
    
    void Awake()
    {
        Golf2Socket.OnPlayerSync += UpdateText;
        ownerTextComp = ownerObject.GetComponent<TextMeshProUGUI>();
    }

    private void UpdateText(Golf2Socket.SyncData data)
    {
        string readyText = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? engText : croText;
        ownerTextComp.text = data.owner + (data.ready.Contains(data.owner) ? readyText : "");

        foreach (var participant in data.participants)
        {
            string txt = participant + (data.ready.Contains(participant) ? readyText : "");
            Debug.Log(txt);

            foreignNames.TryGetValue(participant, out GameObject existingName);
                
            if (existingName == null)
            {
                GameObject playerNameObj = Instantiate(playerNameObject, transform);
                playerNameObj.GetComponent<PlayerNameController>().SetData(txt, SocketConnection.instance.socketManager.isOwner);
                foreignNames.Add(participant, playerNameObj);
                return;
            }
            
            existingName.GetComponent<PlayerNameController>().SetData(txt, SocketConnection.instance.socketManager.isOwner);
        }

        foreach (var key in foreignNames.Keys)
        {
            if (!SocketConnection.instance.socketManager.participants.Contains(key))
            {
                Destroy(foreignNames[key]);
            }
        }
    }

    private void ResetList()
    {
        foreach (Transform tf in transform.GetComponentsInChildren<Transform>())
        {
            if (tf.name != gameObject.name)
            {
                Destroy(tf.gameObject);
            }
        }
    }
    
    private void OnDestroy()
    {
        Golf2Socket.OnPlayerSync -= UpdateText;
    }
}
