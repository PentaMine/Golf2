using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;



public class Main
{
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