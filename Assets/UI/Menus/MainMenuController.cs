using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string levelSelectScene = "LevelSelect";
    [SerializeField] private string profileScene = "Profile";
    [SerializeField] private string settingsScene = "Settings";

    public void Play()
    {
        if (!string.IsNullOrEmpty(levelSelectScene))
            SceneManager.LoadScene(levelSelectScene);
    }

    public void OpenProfile()
    {
        if (!string.IsNullOrEmpty(profileScene))
            SceneManager.LoadScene(profileScene);
    }

    public void OpenSettings()
    {
        if (!string.IsNullOrEmpty(settingsScene))
            SceneManager.LoadScene(settingsScene);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
