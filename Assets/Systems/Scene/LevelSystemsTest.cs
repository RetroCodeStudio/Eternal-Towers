using UnityEngine;

public class LevelSystemsTest : MonoBehaviour
{
    [SerializeField] private LevelProgressionController controller;

    void Start()
    {
        Debug.Log("=== PRUEBA DE INTEGRACIÓN ===");

        Debug.Log("Nivel actual: " + controller.CurrentLevel);

        Debug.Log("¿Nivel 1 desbloqueado?: " +
                  controller.IsLevelUnlocked(1));

        Debug.Log("¿Nivel 2 desbloqueado?: " +
                  controller.IsLevelUnlocked(2));

        Debug.Log("--- Intentando iniciar nivel 2 ---");
        controller.StartLevel(2);

        Debug.Log("--- Iniciando nivel 1 ---");
        controller.StartLevel(1);

        Debug.Log("--- Completando nivel actual ---");
        controller.CompleteCurrentLevel();

        Debug.Log("Nivel actual después de completar: " +
                  controller.CurrentLevel);

        Debug.Log("¿Nivel 2 desbloqueado ahora?: " +
                  controller.IsLevelUnlocked(2));

        Debug.Log("¿Nivel 1 completado?: " +
                  controller.IsLevelCompleted(1));

        Debug.Log("=== FIN DE LA PRUEBA ===");
    }
}