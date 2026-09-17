using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatMenuController : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuScene))
            SceneManager.LoadScene(mainMenuScene);
    }
}
