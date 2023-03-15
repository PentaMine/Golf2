using UnityEngine;
using UnityEngine.SceneManagement;
public class AboutUsButtonHandler : MonoBehaviour
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
