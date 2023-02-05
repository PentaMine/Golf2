using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Startup
{
    // run startup routine
    static Startup()
    {
        SettingManager.LoadSettings();
    }
}
