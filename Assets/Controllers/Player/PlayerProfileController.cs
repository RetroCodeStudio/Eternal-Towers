using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileController : MonoBehaviour
{
    [Header("Profile UI")]
    [SerializeField] private TMP_Text playerIdText;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField genderInput;
    [SerializeField] private TMP_InputField ageInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Text statusText;

    private PlayerProfileService playerProfileService;

    private void Awake()
    {
        IPlayerProfileRepository repository =
            new SQLitePlayerProfileRepository();

        playerProfileService =
            new PlayerProfileService(repository);
    }

    private void Start()
    {
        LoadProfileIntoUI();

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveProfile);
        }
    }

    private void LoadProfileIntoUI()
    {
        PlayerProfile profile = playerProfileService.GetProfile();

        if (profile == null)
        {
            SetStatus("No se pudo cargar el perfil.");
            return;
        }

        if (playerIdText != null)
        {
            playerIdText.text = $"ID: {profile.playerId}";
        }

        if (nameInput != null)
        {
            nameInput.text = profile.playerName;
        }

        if (genderInput != null)
        {
            genderInput.text = profile.gender;
        }

        if (ageInput != null)
        {
            ageInput.text = profile.age > 0
                ? profile.age.ToString()
                : string.Empty;
        }

        if (passwordInput != null)
        {
            passwordInput.text = profile.password;
        }

        SetStatus("Perfil cargado desde SQLite.");
    }

    private void SaveProfile()
    {
        string playerName = nameInput != null
            ? nameInput.text.Trim()
            : string.Empty;

        string gender = genderInput != null
            ? genderInput.text.Trim()
            : string.Empty;

        string password = passwordInput != null
            ? passwordInput.text
            : string.Empty;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            SetStatus("El nombre del jugador no puede estar vacío.");
            return;
        }

        if (!int.TryParse(ageInput.text, out int age))
        {
            SetStatus("La edad debe ser un número.");
            return;
        }

        if (age < 1 || age > 120)
        {
            SetStatus("La edad debe estar entre 1 y 120 años.");
            return;
        }

        playerProfileService.UpdateProfile(
            playerName,
            gender,
            age,
            password
        );

        SetStatus("Perfil guardado correctamente en SQLite.");
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }

        Debug.Log($"[PlayerProfileController] {message}");
    }

    private void OnDestroy()
    {
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(SaveProfile);
        }

        playerProfileService?.Dispose();
    }
}