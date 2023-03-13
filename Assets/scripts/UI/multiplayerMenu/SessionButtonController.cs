using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SessionButtonController : MonoBehaviour
{
    private GameObject ownerTextObject;
    private GameObject participantListObject;
    private TextMeshProUGUI ownerText;
    private TextMeshProUGUI participantList;
    private int id;
    void Awake()
    {
        ownerTextObject = transform.Find("owner").gameObject;
        participantListObject = transform.Find("participants").gameObject;
        ownerText = ownerTextObject.GetComponent<TextMeshProUGUI>();
        participantList = participantListObject.GetComponent<TextMeshProUGUI>();
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log(new Golf2Api().joinSession(id, out string token));
            Debug.Log(token);
        });
    }

    public void SetData(string owner, List<string> participants, int btnId)
    {
        string participantsText = "";

        for (int i = 0; i < participants.Count; i++)
        {
            string text = participants[i] + (i != participants.Count - 1 ? ", " : "");
            participantsText += text;
        }
        
        ownerText.text = owner;
        participantList.text = participantsText;
        id = btnId;
    }
}
