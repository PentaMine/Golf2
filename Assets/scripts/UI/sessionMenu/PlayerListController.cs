using TMPro;
using UnityEngine;

public class PlayerListController : MonoBehaviour
{
    private const string croText = " (spreman)";
    private const string engText = " (ready)";
    public GameObject ownerObject;
    public GameObject participantsObject;
    private TextMeshProUGUI owner;
    private TextMeshProUGUI participants;
    void Awake()
    {
        owner = ownerObject.GetComponent<TextMeshProUGUI>();
        participants = participantsObject.GetComponent<TextMeshProUGUI>();
        Golf2Socket.OnPlayerSync += UpdateText;
    }

    // Update is called once per frame
    void UpdateText(Golf2Socket.SyncData data)
    {

        string readyText = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? engText : croText;
        
        owner.text = data.owner + (data.ready.Contains(data.owner) ? readyText : "");
        string participantsText = "";
        foreach (var participant in data.participants)
        {
            participantsText += participant + (data.ready.Contains(participant) ? readyText : "") + "\n";
        }

        participants.text = participantsText;
    }
}
