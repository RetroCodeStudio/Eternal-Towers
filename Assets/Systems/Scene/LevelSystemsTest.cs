using UnityEngine;

public class LevelSystemsTest : MonoBehaviour
{
    [SerializeField] private LevelProgressManager progressManager;
    [SerializeField] private LevelProgression progression;
    [SerializeField] private LevelUnlockSystem unlockSystem;

    void Start()
    {
        Debug.Log("=== PRUEBA COMPLETA DEL SISTEMA DE NIVELES ===");

        // 1. Comprobar estado inicial
        Debug.Log("Nivel actual: " + progression.CurrentLevel);
        Debug.Log("Nivel 1 completado inicialmente: " +
                  progressManager.IsLevelCompleted(1));

        // 2. Completar nivel 1
        progressManager.CompleteLevel(1);

        Debug.Log("Nivel 1 completado después de completar: " +
                  progressManager.IsLevelCompleted(1));

        // 3. Desbloquear siguiente nivel
        unlockSystem.UnlockNextLevel();

        Debug.Log("¿Nivel 2 está desbloqueado?: " +
                  unlockSystem.IsLevelUnlocked(2));

        // 4. Avanzar al siguiente nivel
        if (progression.CanAdvance())
        {
            progression.AdvanceLevel();
        }

        Debug.Log("Nivel actual después de avanzar: " +
                  progression.CurrentLevel);

        Debug.Log("=== FIN DE LA PRUEBA ===");
    }
}