using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class Startup
{
    // run startup routine
    [RuntimeInitializeOnLoadMethod]    
    public static void OnStartup()
    {
        SettingManager.LoadSettings();
    }
}