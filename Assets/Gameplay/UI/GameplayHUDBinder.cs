using EternalTowers.Gameplay.Core;
using EternalTowers.Gameplay.Waves;
using UnityEngine;

namespace EternalTowers.Gameplay.UI
{
    public class GameplayHUDBinder : MonoBehaviour
    {
        [SerializeField] private GameplayHUD hud;
        [SerializeField] private GameEconomy economy;
        [SerializeField] private BaseHealth baseHealth;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private TowerSelectionController towerSelectionController;
        [SerializeField] private TowerPlacementSystem towerPlacementSystem;
        [SerializeField] private TowerBuildMenu buildMenu;
        [SerializeField] private SelectedTowerPanel selectedTowerPanel;

        private void Awake()
        {
            if (economy == null)
                economy = FindAnyObjectByType<GameEconomy>();

            if (baseHealth == null)
                baseHealth = FindAnyObjectByType<BaseHealth>();

            if (waveManager == null)
                waveManager = FindAnyObjectByType<WaveManager>();

            if (towerSelectionController == null)
                towerSelectionController = FindAnyObjectByType<TowerSelectionController>();

            if (towerPlacementSystem == null)
                towerPlacementSystem = FindAnyObjectByType<TowerPlacementSystem>();

            if (buildMenu == null)
                buildMenu = FindAnyObjectByType<TowerBuildMenu>();

            if (selectedTowerPanel == null)
                selectedTowerPanel = FindAnyObjectByType<SelectedTowerPanel>();
        }

        private void Start()
        {
            if (hud != null)
                hud.Bind(economy, baseHealth, waveManager);

            if (towerPlacementSystem != null)
                towerPlacementSystem.SetEconomy(economy);

            if (selectedTowerPanel != null)
                selectedTowerPanel.Bind(economy);

            if (towerSelectionController != null)
                towerSelectionController.SetInteractionReferences(
                    towerPlacementSystem,
                    buildMenu,
                    selectedTowerPanel);
        }
    }
}
