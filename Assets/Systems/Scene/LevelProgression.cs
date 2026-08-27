using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int totalLevels = 10;

    public int CurrentLevel => currentLevel;

    public bool CanAdvance()
    {
        return currentLevel < totalLevels;
    }

    public void AdvanceLevel()
    {
        if (!CanAdvance())
        {
            Debug.Log("Ya se alcanzó el último nivel.");
            return;
        }

        currentLevel++;
        Debug.Log("Avanzando al nivel " + currentLevel);
    }

    public void ResetProgression()
    {
        currentLevel = 1;
        Debug.Log("Progresión reiniciada.");
    }
}