using System;
using EternalTowers.Gameplay.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TowerBuildOption : MonoBehaviour
{
    [SerializeField] private Image towerIcon;
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button button;

    private TowerData towerData;
    private GameEconomy economy;
    private Action<TowerData> selectionCallback;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        Unbind();
    }

    public void Bind(TowerData data, GameEconomy gameEconomy, Action<TowerData> onSelected)
    {
        Unbind();
        towerData = data;
        economy = gameEconomy;
        selectionCallback = onSelected;

        if (button == null)
            button = GetComponent<Button>();

        if (towerIcon != null)
        {
            towerIcon.sprite = towerData != null
                ? (towerData.TowerIcon != null ? towerData.TowerIcon : towerData.GetLevelSprite(0))
                : null;
            towerIcon.enabled = towerIcon.sprite != null;
        }

        if (towerNameText != null)
            towerNameText.text = towerData != null ? towerData.TowerName : string.Empty;

        if (costText != null)
            costText.text = towerData != null ? towerData.Cost.ToString() : string.Empty;

        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
            button.interactable = CanAffordTower();
        }

        if (economy != null)
            economy.GoldChanged += HandleGoldChanged;
    }

    private void Unbind()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClick);

        if (economy != null)
            economy.GoldChanged -= HandleGoldChanged;

        economy = null;
        selectionCallback = null;
    }

    private bool CanAffordTower()
    {
        return towerData != null && economy != null && economy.CanAfford(towerData.Cost);
    }

    private void HandleGoldChanged(int currentGold)
    {
        if (button != null)
            button.interactable = CanAffordTower();
    }

    private void HandleClick()
    {
        if (CanAffordTower())
            selectionCallback?.Invoke(towerData);
    }
}