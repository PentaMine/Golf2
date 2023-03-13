using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SessionMenuHandler : MonoBehaviour
{
    public GameObject ownerNameObject;
    private TextMeshProUGUI ownerName;
    public GameObject participantListObject;
    private TextMeshProUGUI participantList;
    void Start()
    {
        ownerName = ownerNameObject.GetComponent<TextMeshProUGUI>();
        participantList = participantListObject.GetComponent<TextMeshProUGUI>();
    }

    public void SetData()
    {
        
    }
}
