using UnityEngine;



public class Main
{
    public static string socketArg;
    
    [RuntimeInitializeOnLoadMethod]
    public static void OnStartup()
    {
        SettingManager.LoadSettings();
    }

    public static void OnShutdown()
    {
        new Golf2Api().leaveSession();
        SettingManager.SaveSettings();
    }
}