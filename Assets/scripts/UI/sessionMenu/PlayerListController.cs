using System;
using TMPro;
using UnityEngine;

public class PlayerListController : MonoBehaviour
{
    public GameObject playerNameObject;
    public GameObject ownerObject;

    private const string croText = " (spreman)";
    private const string engText = " (ready)";
    private TextMeshProUGUI ownerTextComp;
    
    
    void Awake()
    {
        Golf2Socket.OnPlayerSync += UpdateText;
        ownerTextComp = ownerObject.GetComponent<TextMeshProUGUI>();
    }

    private void UpdateText(Golf2Socket.SyncData data)
    {
        ResetList();
        
        string readyText = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? engText : croText;
        ownerTextComp.text = data.owner + (data.ready.Contains(data.owner) ? readyText : "");

        foreach (var participant in data.participants)
        {
            string txt = participant + (data.ready.Contains(participant) ? readyText : "");
            Debug.Log(txt);
            Instantiate(playerNameObject, transform).GetComponent<PlayerNameController>().SetData(txt, SocketConnection.instance.socketManager.isOwner);
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
