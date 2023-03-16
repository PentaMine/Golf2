using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListController : MonoBehaviour
{
    public GameObject ownerObject;
    public GameObject participantsObject;
    private TextMeshProUGUI owner;
    private TextMeshProUGUI participants;
    void Awake()
    {
        owner = ownerObject.GetComponent<TextMeshProUGUI>();
        participants = participantsObject.GetComponent<TextMeshProUGUI>();
        Golf2Socket.OnSync += UpdateText;
    }

    // Update is called once per frame
    void UpdateText(Golf2Socket.SyncData data)
    {
        owner.text = data.owner;
        string participantsText = "";
        foreach (var participant in data.participants)
        {
            participantsText += participant + "\n";
        }

        participants.text = participantsText;
    }
}
