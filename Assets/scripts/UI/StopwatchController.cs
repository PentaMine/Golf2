using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StopwatchController : MonoBehaviour
{
    private TextMeshProUGUI txtComp;
    public string EngPrefix, EngSuffix;
    public string CroPrefix, CroSuffix;
    private GameManager gameManager;

    private void Start()
    {
        txtComp = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        txtComp.text = (SettingManager.settings.lang == SettingManager.Language.ENGLISH ? EngPrefix : CroPrefix)
                       + GameManager.instance.GetGameDuration().ToString("0.00")
                       + (SettingManager.settings.lang == SettingManager.Language.ENGLISH ? EngSuffix : CroSuffix);
    }
}