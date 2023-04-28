using UnityEngine;



public class Main
{   
    // major, minor and patch are all two digits
    public const int clientVersion = 020000; // 2.0.0
    
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