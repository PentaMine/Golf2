using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuttCounterController : MonoBehaviour
{
    private TextMeshProUGUI txtComp;
    private int putts = 0;
    public string EngPrefix, EngSuffix;
    public string CroPrefix, CroSuffix;

    void Start()
    {
        PlayerController.onPlayerShoot += OnPlayerShoot;
        txtComp = GetComponent<TextMeshProUGUI>();
        UpdateCounter();
    }

    void OnPlayerShoot(Vector3 force)
    {
        UpdateCounter();
    }

    void UpdateCounter()
    {
        txtComp.text = (SettingManager.settings.lang == SettingManager.Language.ENGLISH ? EngPrefix : CroPrefix)
                       + putts.ToString()
                       + (SettingManager.settings.lang == SettingManager.Language.ENGLISH ? EngSuffix : CroSuffix);
        putts++;
    }
}