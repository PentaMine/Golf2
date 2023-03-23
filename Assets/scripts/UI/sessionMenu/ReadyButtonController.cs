using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        GetComponent<Button>().onClick.AddListener(() =>
        {
            isReady = !isReady;
            ChangeState(isReady);
        });

        Golf2Socket.OnPlayerSync += ChangeText;
    }

    void ChangeState(bool state)
    {
        if (state)
        {
            //textComponent.text = "UNREADY";
            // send to server asynchronously
            new Thread(SocketConnection.instance.socketManager.SetReady).Start();
        }
        else
        {
            //textComponent.text = "READY";
            // send to server asynchronously
            new Thread(SocketConnection.instance.socketManager.SetUnready).Start();
        }
    }

    void ChangeText(Golf2Socket.SyncData data)
    {
        if (data.ready.Contains(SettingManager.settings.name))
        {
            textComponent.text = "UNREADY";
        }
        else
        {
            textComponent.text = "READY";
        }
    }
}