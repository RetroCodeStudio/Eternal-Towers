using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelProgressionTestUI : MonoBehaviour
{
    [Header("Sistema")]
    [SerializeField] private LevelProgressionController controller;

    [Header("Interfaz")]
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private Transform levelList;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Button completeButton;
    [SerializeField] private TMP_Text messageText;

    private void Start()
    {
        RefreshUI();

        completeButton.onClick.AddListener(CompleteCurrentLevel);
    }

    private void RefreshUI()
    {
        currentLevelText.text =
            "Nivel actual: " + controller.CurrentLevel;

        CreateLevelButtons();

        UpdateMessage("Selecciona un nivel.");
    }

    private void CreateLevelButtons()
    {
        for (int i = levelList.childCount - 1; i >= 0; i--)
        {
            Destroy(levelList.GetChild(i).gameObject);
        }

        int totalLevels = 10;

        for (int levelNumber = 1; levelNumber <= totalLevels; levelNumber++)
        {
            CreateLevelButton(levelNumber);
        }
    }

    private void CreateLevelButton(int levelNumber)
    {
        Button button = Instantiate(levelButtonPrefab, levelList);

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        bool unlocked = controller.IsLevelUnlocked(levelNumber);
        bool completed = controller.IsLevelCompleted(levelNumber);

        if (completed)
        {
            buttonText.text = "Nivel " + levelNumber + " - COMPLETADO";
        }
        else if (unlocked)
        {
            buttonText.text = "Nivel " + levelNumber + " - DESBLOQUEADO";
        }
        else
        {
            buttonText.text = "Nivel " + levelNumber + " - BLOQUEADO";
        }

        button.interactable = unlocked;

        button.onClick.AddListener(() => SelectLevel(levelNumber));
    }

    private void SelectLevel(int levelNumber)
    {
        if (controller.StartLevel(levelNumber))
        {
            UpdateMessage("Nivel " + levelNumber + " iniciado.");
        }
        else
        {
            UpdateMessage("El nivel " + levelNumber + " está bloqueado.");
        }
    }

    private void CompleteCurrentLevel()
    {
        if (controller.CompleteCurrentLevel())
        {
            RefreshUI();

            UpdateMessage(
                "Nivel completado. Progreso actualizado."
            );
        }
        else
        {
            UpdateMessage(
                "No se pudo completar el nivel actual."
            );
        }
    }

    private void UpdateMessage(string message)
    {
        messageText.text = message;
    }
}