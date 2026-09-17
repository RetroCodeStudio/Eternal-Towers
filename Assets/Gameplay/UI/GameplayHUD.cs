using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EternalTowers.Gameplay.UI
{
    public class GameplayHUD : MonoBehaviour
    {
        [Header("Primary HUD")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI baseHealthText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI enemiesText;
        [SerializeField] private GameObject startWaveButton;
        [SerializeField] private Button startButton;

        [Header("Selected Tower")]
        [SerializeField] private GameObject selectedTowerPanel;
        [SerializeField] private Image selectedTowerIcon;
        [SerializeField] private TextMeshProUGUI selectedTowerTitle;
        [SerializeField] private TextMeshProUGUI selectedTowerDamageText;
        [SerializeField] private TextMeshProUGUI selectedTowerRangeText;
        [SerializeField] private TextMeshProUGUI selectedTowerCadenceText;
        [SerializeField] private TextMeshProUGUI selectedTowerUpgradeText;
        [SerializeField] private TextMeshProUGUI selectedTowerLevelText;
        [SerializeField] private Button upgradeTowerButton;

        private EternalTowers.Gameplay.Core.GameEconomy economy;
        private EternalTowers.Gameplay.Core.BaseHealth baseHealth;
        private EternalTowers.Gameplay.Waves.WaveManager waveManager;
        private TowerSelectionController towerSelectionController;
        private TowerController selectedTower;

        public void Bind(
            EternalTowers.Gameplay.Core.GameEconomy economyRef,
            EternalTowers.Gameplay.Core.BaseHealth baseHealthRef,
            EternalTowers.Gameplay.Waves.WaveManager waveManagerRef,
            TowerSelectionController towerSelectionRef)
        {
            economy = economyRef;
            baseHealth = baseHealthRef;
            waveManager = waveManagerRef;
            towerSelectionController = towerSelectionRef;

            if (waveManager != null)
            {
                waveManager.WaveStarted += HandleWaveStarted;
                waveManager.WaveCompleted += HandleWaveCompleted;
                waveManager.EnemySpawned += HandleEnemyChanged;
                waveManager.EnemyDefeated += HandleEnemyChanged;
                waveManager.EnemyReachedBase += HandleEnemyChanged;
            }

            if (startButton != null)
            {
                startButton.onClick.RemoveListener(StartWave);
                startButton.onClick.AddListener(StartWave);
            }

            if (upgradeTowerButton != null)
            {
                upgradeTowerButton.onClick.RemoveListener(UpgradeSelectedTower);
                upgradeTowerButton.onClick.AddListener(UpgradeSelectedTower);
            }

            RefreshAll();
        }

        private void OnDestroy()
        {
            if (waveManager != null)
            {
                waveManager.WaveStarted -= HandleWaveStarted;
                waveManager.WaveCompleted -= HandleWaveCompleted;
                waveManager.EnemySpawned -= HandleEnemyChanged;
                waveManager.EnemyDefeated -= HandleEnemyChanged;
                waveManager.EnemyReachedBase -= HandleEnemyChanged;
            }

            if (startButton != null)
                startButton.onClick.RemoveListener(StartWave);

            if (upgradeTowerButton != null)
                upgradeTowerButton.onClick.RemoveListener(UpgradeSelectedTower);
        }

        public void SetSelectedTower(TowerController tower)
        {
            selectedTower = tower;
            RefreshSelectedTowerInfo();
        }

        public void ClearSelectedTower()
        {
            selectedTower = null;
            RefreshSelectedTowerInfo();
        }

        public void StartWave()
        {
            if (waveManager == null)
                return;

            if (waveManager.State == EternalTowers.Gameplay.Waves.WaveManager.WaveState.Spawning ||
                waveManager.State == EternalTowers.Gameplay.Waves.WaveManager.WaveState.Active)
                return;

            waveManager.StartNextWave();
        }

        private void UpgradeSelectedTower()
        {
            if (selectedTower == null || economy == null)
                return;

            if (TowerUpgradeSystem.TryUpgrade(selectedTower, economy))
            {
                RefreshAll();
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

        private void HandleEnemyChanged(EternalTowers.Gameplay.Enemies.Enemy enemy)
        {
            RefreshEnemyCount();
            RefreshWaveText();
        }

        private void RefreshEnemyCount()
        {
            if (enemiesText == null)
                return;

            int count = waveManager != null ? waveManager.ActiveEnemies : 0;
            enemiesText.text = "Enemies: " + count;
        }

        private void RefreshWaveText()
        {
            if (waveText == null || waveManager == null)
                return;

            int waveNumber = waveManager.WaveCount > 0 ? Mathf.Max(0, waveManager.CurrentWaveIndex + 1) : 0;
            waveText.text = "Wave: " + waveNumber + "/" + waveManager.WaveCount;
        }

        private void RefreshSelectedTowerInfo()
        {
            if (selectedTowerPanel == null)
                return;

            if (selectedTower == null || selectedTower.TowerData == null)
            {
                selectedTowerPanel.SetActive(false);
                return;
            }

            selectedTowerPanel.SetActive(true);

            if (selectedTowerIcon != null)
            {
                selectedTowerIcon.sprite = selectedTower.CurrentVisualSprite;
                selectedTowerIcon.gameObject.SetActive(selectedTower.CurrentVisualSprite != null);
            }

            if (selectedTowerTitle != null)
                selectedTowerTitle.text = selectedTower.TowerName;

            EternalTowers.Gameplay.Towers.TowerLevelDefinition currentLevel = selectedTower.CurrentLevel;
            if (currentLevel == null)
            {
                selectedTowerPanel.SetActive(false);
                return;
            }

            if (selectedTowerDamageText != null)
                selectedTowerDamageText.text = "Damage: " + currentLevel.damage.ToString("0.##");

            if (selectedTowerRangeText != null)
                selectedTowerRangeText.text = "Range: " + currentLevel.range.ToString("0.##");

            if (selectedTowerCadenceText != null)
                selectedTowerCadenceText.text = "Cadence: " + currentLevel.attackSpeed.ToString("0.##") + "/s";

            if (selectedTowerUpgradeText != null)
                selectedTowerUpgradeText.text = selectedTower.CanUpgrade ? "Upgrade: " + selectedTower.UpgradeCost : "Max level";

            if (selectedTowerLevelText != null)
                selectedTowerLevelText.text = "Level: " + selectedTower.UpgradeLevel + " / " + selectedTower.MaxUpgradeLevel;

            if (upgradeTowerButton != null)
                upgradeTowerButton.interactable = selectedTower.CanUpgrade && economy != null && economy.CanAfford(selectedTower.UpgradeCost);
        }

        private void RefreshAll()
        {
            if (goldText != null)
                goldText.text = economy != null ? "Gold: " + economy.CurrentGold : "Gold: 0";

            if (baseHealthText != null)
                baseHealthText.text = baseHealth != null ? "Base HP: " + baseHealth.CurrentHealth + "/" + baseHealth.MaxHealth : "Base HP: 0/0";

            RefreshWaveText();
            RefreshEnemyCount();
            RefreshSelectedTowerInfo();

            bool canStartWave = waveManager != null &&
                waveManager.State != EternalTowers.Gameplay.Waves.WaveManager.WaveState.Spawning &&
                waveManager.State != EternalTowers.Gameplay.Waves.WaveManager.WaveState.Active &&
                waveManager.State != EternalTowers.Gameplay.Waves.WaveManager.WaveState.Starting;

            if (startWaveButton != null)
                startWaveButton.SetActive(canStartWave);

            if (startButton != null)
                startButton.interactable = canStartWave;
        }
    }
}
