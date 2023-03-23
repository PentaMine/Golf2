using UnityEngine;



public class Main
{
    [RuntimeInitializeOnLoadMethod]
    public static void OnStartup()
    {
        SettingManager.LoadSettings();
        SessionData.Init();
    }

    public static void OnShutdown()
    {
        new Golf2Api().leaveSession();
        SettingManager.SaveSettings();
    }
}