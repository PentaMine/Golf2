using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public string engTxt, croTxt;
    public bool isAllUpper;
    private TextMeshProUGUI txtComp;

    private void Start()
    {
        txtComp = GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        txtComp.text = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? engTxt : croTxt;
        if (isAllUpper)
        {
            txtComp.text = txtComp.text.ToUpper();
        }
    }
}