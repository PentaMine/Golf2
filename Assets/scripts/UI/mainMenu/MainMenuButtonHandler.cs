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
        Settings.SaveSettings();
        Application.Quit();
    }

    public void ChangeLanguage()
    {
        // change the language from cro to eng or from eng to cro depending on the current language
        Settings.language = Settings.language == Settings.Language.ENGLISH ? Settings.Language.CROATIAN : Settings.Language.ENGLISH;
        Settings.SaveSettings();
    }
}