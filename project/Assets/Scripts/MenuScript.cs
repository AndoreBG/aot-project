using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Fase");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
