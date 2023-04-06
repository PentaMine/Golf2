using UnityEngine;
using UnityEngine.SceneManagement;

public class SPFinishButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public void GenerateNext()
    {
        SceneManager.LoadScene("GeneratedLevel");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}