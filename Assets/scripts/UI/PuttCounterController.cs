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
    }

    void OnPlayerShoot(Vector3 force)
    {
        putts++;
        txtComp.text = (Settings.language == Settings.Language.ENGLISH ? EngPrefix : CroPrefix)
                       + putts.ToString()
                       + (Settings.language == Settings.Language.ENGLISH ? EngSuffix : CroSuffix);
    }
}