using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonHandler : MonoBehaviour
{
    public void ToAboutUs()
    {
        SceneManager.LoadScene("AboutUs");
    }

    public void ToGeneratedLevels()
    {
        SceneManager.LoadScene("GeneratedLevel");
    }

    public void Exit()
    {
        SettingManager.SaveSettings();
        Application.Quit();
    }

    public void ToMultiplayer()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }

    public void ChangeLanguage()
    {
        // change the language from cro to eng or from eng to cro depending on the current language
        SettingManager.settings.lang = SettingManager.settings.lang == SettingManager.Language.ENGLISH ? SettingManager.Language.CROATIAN : SettingManager.Language.ENGLISH;
        SettingManager.SaveSettings();
    }
}