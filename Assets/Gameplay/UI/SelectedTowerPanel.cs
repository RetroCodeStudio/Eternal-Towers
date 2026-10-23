using EternalTowers.Gameplay.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EternalTowers.Gameplay.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SelectedTowerPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image towerIcon;
        [SerializeField] private TextMeshProUGUI towerTitle;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI rangeText;
        [SerializeField] private TextMeshProUGUI cadenceText;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button upgradeButton;

        private GameEconomy economy;
        private TowerController selectedTower;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            SetVisible(false);
        }

        private void OnDestroy()
        {
            if (economy != null)
                economy.GoldChanged -= HandleGoldChanged;

            if (upgradeButton != null)
                upgradeButton.onClick.RemoveListener(UpgradeSelectedTower);
        }

        public void Bind(GameEconomy gameEconomy)
        {
            if (economy != null)
                economy.GoldChanged -= HandleGoldChanged;

            economy = gameEconomy;
            if (economy != null)
                economy.GoldChanged += HandleGoldChanged;

            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveListener(UpgradeSelectedTower);
                upgradeButton.onClick.AddListener(UpgradeSelectedTower);
            }

            Refresh();
        }

        public void SetSelectedTower(TowerController tower)
        {
            selectedTower = tower;
            Refresh();
        }

        public void ClearSelection()
        {
            selectedTower = null;
            Refresh();
        }

        private void UpgradeSelectedTower()
        {
            if (selectedTower != null && TowerUpgradeSystem.TryUpgrade(selectedTower, economy))
                Refresh();
        }

        private void HandleGoldChanged(int currentGold)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (selectedTower == null || selectedTower.TowerData == null || selectedTower.CurrentLevel == null)
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);

            if (towerIcon != null)
            {
                towerIcon.sprite = selectedTower.CurrentVisualSprite != null
                    ? selectedTower.CurrentVisualSprite
                    : selectedTower.TowerData.TowerIcon;
                towerIcon.enabled = towerIcon.sprite != null;
            }

            if (towerTitle != null)
                towerTitle.text = selectedTower.TowerName;

            if (damageText != null)
                damageText.text = "Damage: " + selectedTower.CurrentLevel.damage.ToString("0.##");

            if (rangeText != null)
                rangeText.text = "Range: " + selectedTower.CurrentLevel.range.ToString("0.##");

            if (cadenceText != null)
                cadenceText.text = "Cadence: " + selectedTower.CurrentLevel.attackSpeed.ToString("0.##") + "/s";

            if (upgradeText != null)
                upgradeText.text = selectedTower.CanUpgrade ? "Upgrade: " + selectedTower.UpgradeCost : "Max level";

            if (levelText != null)
                levelText.text = "Level: " + selectedTower.UpgradeLevel + " / " + selectedTower.MaxUpgradeLevel;

            if (upgradeButton != null)
                upgradeButton.interactable = selectedTower.CanUpgrade && economy != null && economy.CanAfford(selectedTower.UpgradeCost);
        }

        private void SetVisible(bool visible)
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}