using UnityEngine;

/// <summary>
/// Inicializa la configuración global al abrir el juego y la guarda al cerrarlo.
/// </summary>
public class SettingsBootstrap : MonoBehaviour
{
    public static SettingsManager Settings { get; private set; }

    private SQLiteSettingsRepository repository;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        GameObject bootstrapObject = new GameObject(nameof(SettingsBootstrap));
        bootstrapObject.AddComponent<SettingsBootstrap>();
    }

    private void Awake()
    {
        if (Settings != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        repository = new SQLiteSettingsRepository();
        Settings = new SettingsManager(repository);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            SaveSettings();
        }
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void OnDestroy()
    {
        repository?.Dispose();
    }

    private void SaveSettings()
    {
        Settings?.Save();
    }
}