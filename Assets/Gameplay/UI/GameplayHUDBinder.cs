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
        }

        private void Start()
        {
            if (hud != null)
                hud.Bind(economy, baseHealth, waveManager, towerSelectionController);

            if (towerSelectionController != null)
                towerSelectionController.SetHud(hud);
        }
    }
}
