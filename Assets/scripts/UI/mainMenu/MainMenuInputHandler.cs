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
        GameObject.FindGameObjectWithTag("Loading").GetComponent<Canvas>().enabled = true;
        SceneManager.LoadScene("GeneratedLevel");
    }

    public void Exit()
    {
        Main.OnShutdown();
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