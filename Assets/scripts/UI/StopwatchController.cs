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
        txtComp.text = (Settings.language == Settings.Language.ENGLISH ? EngPrefix : CroPrefix)
                       + GameManager.instance.GetGameDuration().ToString("0.00")
                       + (Settings.language == Settings.Language.ENGLISH ? EngSuffix : CroSuffix);
    }
}