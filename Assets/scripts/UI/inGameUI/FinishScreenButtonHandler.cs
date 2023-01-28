using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScreenButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public static void GenerateNext()
    {
        SceneManager.LoadScene("GeneratedLevel");
    }

    public static void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
