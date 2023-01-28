using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    // run startup routine
    private void Start()
    {
        Settings.LoadSettings();
    }
}
