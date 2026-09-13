using UnityEngine;

public class LevelProgressionController : MonoBehaviour
{
    [SerializeField] private LevelProgression progression;
    [SerializeField] private LevelProgressManager progressManager;
    [SerializeField] private LevelUnlockSystem unlockSystem;

    private LevelProgressionSaveService saveService;

    public int CurrentLevel => progression.CurrentLevel;

    private void Awake()
    {
        saveService = new LevelProgressionSaveService();

        LoadProgress();
    }

    public bool CanStartLevel(int levelNumber)
    {
        if (unlockSystem == null)
            return false;

        return unlockSystem.IsLevelUnlocked(levelNumber);
    }

    public bool StartLevel(int levelNumber)
    {
        if (!CanStartLevel(levelNumber))
        {
            Debug.Log("No se puede iniciar el nivel " + levelNumber +
                      " porque está bloqueado.");

            return false;
        }

        Debug.Log("Nivel " + levelNumber + " iniciado.");
        return true;
    }

    public bool CompleteCurrentLevel()
    {
        int currentLevel = progression.CurrentLevel;

        if (!CanStartLevel(currentLevel))
        {
            Debug.LogWarning("El nivel actual está bloqueado.");
            return false;
        }

        if (progressManager.IsLevelCompleted(currentLevel))
        {
            Debug.Log("El nivel " + currentLevel + " ya estaba completado.");
            return false;
        }

        progressManager.CompleteLevel(currentLevel);

        Debug.Log("Nivel " + currentLevel + " completado.");

        if (progression.CanAdvance())
        {
            unlockSystem.UnlockNextLevel();
            progression.AdvanceLevel();

            Debug.Log("Nivel " + progression.CurrentLevel +
                      " desbloqueado y establecido como nivel actual.");
        }
        else
        {
            Debug.Log("Se completó el último nivel.");
        }

        SaveProgress();

        return true;
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        return progressManager.IsLevelCompleted(levelNumber);
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        return unlockSystem.IsLevelUnlocked(levelNumber);
    }

    private void SaveProgress()
    {
        saveService.Save(
            progression.CurrentLevel,
            GetHighestUnlockedLevel(),
            progressManager
        );
    }

    private void LoadProgress()
    {
        if (!saveService.HasSavedProgress())
        {
            Debug.Log("No existe progreso guardado. Se utilizará el progreso inicial.");
            return;
        }

        int savedCurrentLevel = saveService.LoadCurrentLevel();
        int savedHighestUnlockedLevel =
            saveService.LoadHighestUnlockedLevel();

        progression.SetCurrentLevel(savedCurrentLevel);
        unlockSystem.SetHighestUnlockedLevel(savedHighestUnlockedLevel);

        for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
        {
            bool completed =
                saveService.LoadLevelCompleted(levelNumber);

            progressManager.SetLevelCompleted(
                levelNumber,
                completed
            );
        }

        Debug.Log(
            "Progreso cargado. Nivel actual: " +
            progression.CurrentLevel
        );
    }

    private int GetHighestUnlockedLevel()
    {
        int highestUnlockedLevel = 1;

        for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
        {
            if (unlockSystem.IsLevelUnlocked(levelNumber))
            {
                highestUnlockedLevel = levelNumber;
            }
        }

        return highestUnlockedLevel;
    }


// Este codigo es provicional y sirve para reiniciar el progreso de niveles en el juego. Se puede eliminar una vez que se haya implementado un sistema de depuración más robusto.
    [ContextMenu("Reset Saved Progress")]
public void ResetSavedProgress()
{
    PlayerPrefs.DeleteKey("EternalTowers_CurrentLevel");
    PlayerPrefs.DeleteKey("EternalTowers_HighestUnlockedLevel");

    for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
    {
        PlayerPrefs.DeleteKey(
            "EternalTowers_Level_" + levelNumber + "_Completed"
        );
    }

    PlayerPrefs.Save();

    progression.ResetProgression();
    unlockSystem.SetHighestUnlockedLevel(1);

    for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
    {
        progressManager.SetLevelCompleted(levelNumber, false);
    }

    Debug.Log("Progreso de niveles reiniciado.");
}
}