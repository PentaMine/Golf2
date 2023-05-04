using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isReady = false;
    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        textComponent.text = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? "READY" : "SPREMAN";
        
        GetComponent<Button>().onClick.AddListener(() =>
        {
            isReady = !isReady;
            ChangeState(isReady);
        });

        Golf2Socket.OnPlayerSync += ChangeText;
    }

    void ChangeState(bool state)
    {
        // send to server asynchronously
        if (state)
        {
            new Thread(SocketConnection.instance.socketManager.SetReady).Start();
        }
        else
        {
            new Thread(SocketConnection.instance.socketManager.SetUnready).Start();
        }
    }

    void ChangeText(Golf2Socket.SyncData data)
    {
        if (data.ready.Contains(SettingManager.settings.name))
        {
            textComponent.text = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? "UNREADY" : "NESPREMAN";
        }
        else
        {
            textComponent.text = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? "READY" : "SPREMAN";
        }
    }
}