using UnityEngine;



public class Main
{   
    // major, minor and patch are all two digits
    public const int clientVersion = 020001; // 02 00 01, 2.0.1 
    
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