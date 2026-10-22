using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EternalTowers.Gameplay.UI
{
    public class GameplayHUD : MonoBehaviour
    {
        [Header("Primary HUD")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private Image goldIcon;
        [SerializeField] private TextMeshProUGUI baseHealthText;
        [SerializeField] private Image baseHealthIcon;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private Image waveIcon;

        [Header("Legacy Wave Controls")]
        [SerializeField] private GameObject startWaveButton;
        [SerializeField] private Button startButton;

        private EternalTowers.Gameplay.Core.GameEconomy economy;
        private EternalTowers.Gameplay.Core.BaseHealth baseHealth;
        private EternalTowers.Gameplay.Waves.WaveManager waveManager;

        public void Bind(
            EternalTowers.Gameplay.Core.GameEconomy economyRef,
            EternalTowers.Gameplay.Core.BaseHealth baseHealthRef,
            EternalTowers.Gameplay.Waves.WaveManager waveManagerRef)
        {
            UnbindSources();

            economy = economyRef;
            baseHealth = baseHealthRef;
            waveManager = waveManagerRef;

            if (economy != null)
                economy.GoldChanged += HandleGoldChanged;

            if (baseHealth != null)
                baseHealth.HealthChanged += HandleBaseHealthChanged;

            if (waveManager != null)
            {
                waveManager.WaveStarted += HandleWaveStarted;
                waveManager.WaveCompleted += HandleWaveCompleted;
            }

            if (startWaveButton != null)
                startWaveButton.SetActive(false);

            if (startButton != null)
                startButton.gameObject.SetActive(false);

            RefreshAll();
        }

        private void OnDestroy()
        {
            UnbindSources();

        }

        private void UnbindSources()
        {
            if (economy != null)
                economy.GoldChanged -= HandleGoldChanged;

            if (baseHealth != null)
                baseHealth.HealthChanged -= HandleBaseHealthChanged;

            if (waveManager != null)
            {
                waveManager.WaveStarted -= HandleWaveStarted;
                waveManager.WaveCompleted -= HandleWaveCompleted;
            }
        }

        private void HandleWaveStarted(int waveNumber)
        {
            RefreshAll();
        }

        private void HandleWaveCompleted(int waveNumber)
        {
            RefreshAll();
        }

        private void HandleGoldChanged(int currentGold)
        {
            RefreshAll();
        }

        private void HandleBaseHealthChanged(int currentHealth, int maxHealth)
        {
            RefreshAll();
        }

        private void RefreshWaveText()
        {
            if (waveText == null)
                return;

            int waveNumber = waveManager != null && waveManager.HasWave ? waveManager.CurrentWaveIndex + 1 : 0;
            waveText.text = "OLEADA " + waveNumber;
        }

        private void RefreshAll()
        {
            if (goldText != null)
                goldText.text = economy != null ? economy.CurrentGold.ToString() : "0";

            if (baseHealthText != null)
                baseHealthText.text = baseHealth != null ? baseHealth.CurrentHealth + "/" + baseHealth.MaxHealth : "0/0";

            RefreshWaveText();
        }
    }
}
