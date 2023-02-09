using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class Startup
{
    [InitializeOnLoadMethod]
    public static void OnStartup()
    {
        SettingManager.LoadSettings();
    }
}