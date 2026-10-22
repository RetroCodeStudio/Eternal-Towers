using System;
using System.Collections.Generic;
using EternalTowers.Gameplay.Core;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class TowerBuildMenu : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform optionContainer;
    [SerializeField] private TowerBuildOption optionPrefab;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 90f);

    private readonly List<TowerBuildOption> options = new List<TowerBuildOption>();
    private RectTransform menuRect;
    private CanvasGroup canvasGroup;
    private Vector2 worldPosition;
    private Camera targetCamera;
    private bool isOpen;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        menuRect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        Close();
    }

    private void LateUpdate()
    {
        if (isOpen)
            UpdateScreenPosition();
    }

    public bool Open(
        Vector2 targetWorldPosition,
        Camera cameraToFollow,
        IReadOnlyList<TowerData> availableTowers,
        GameEconomy economy,
        Action<TowerData> onTowerSelected)
    {
        if (canvas == null || optionContainer == null || optionPrefab == null)
            return false;

        int validTowerCount = 0;
        if (availableTowers != null)
        {
            for (int towerIndex = 0; towerIndex < availableTowers.Count; towerIndex++)
            {
                TowerData towerData = availableTowers[towerIndex];
                if (towerData != null && towerData.TowerPrefab != null)
                    validTowerCount++;
            }
        }

        if (validTowerCount == 0)
        {
            Close();
            return false;
        }

        worldPosition = targetWorldPosition;
        targetCamera = cameraToFollow != null ? cameraToFollow : worldCamera;
        if (targetCamera == null)
            targetCamera = Camera.main;

        gameObject.SetActive(true);
        isOpen = true;
        SetVisible(true);
        PopulateOptions(availableTowers, validTowerCount, economy, onTowerSelected);
        UpdateScreenPosition();
        return true;
    }

    public void Close()
    {
        isOpen = false;
        SetVisible(false);
    }

    public bool ContainsScreenPoint(Vector2 screenPoint)
    {
        if (!isOpen || canvas == null || menuRect == null)
            return false;

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(menuRect, screenPoint, uiCamera);
    }

    private void PopulateOptions(
        IReadOnlyList<TowerData> availableTowers,
        int validTowerCount,
        GameEconomy economy,
        Action<TowerData> onTowerSelected)
    {
        while (options.Count < validTowerCount)
            options.Add(Instantiate(optionPrefab, optionContainer));

        int optionIndex = 0;
        for (int towerIndex = 0; towerIndex < availableTowers.Count; towerIndex++)
        {
            TowerData towerData = availableTowers[towerIndex];
            if (towerData == null || towerData.TowerPrefab == null)
                continue;

            TowerBuildOption option = options[optionIndex];
            option.gameObject.SetActive(true);
            option.Bind(towerData, economy, onTowerSelected);
            optionIndex++;
        }

        for (int index = optionIndex; index < options.Count; index++)
            options[index].gameObject.SetActive(false);
    }

    private void UpdateScreenPosition()
    {
        RectTransform parentRect = menuRect.parent as RectTransform;
        if (targetCamera == null || canvas == null || menuRect == null || parentRect == null)
            return;

        Vector3 screenPoint = targetCamera.WorldToScreenPoint(worldPosition);
        if (screenPoint.z <= 0f)
        {
            Close();
            return;
        }

        screenPoint += (Vector3)screenOffset;
        Rect safeArea = Screen.safeArea;
        Vector2 scaledSize = menuRect.rect.size * canvas.scaleFactor;
        float minX = safeArea.xMin + scaledSize.x * menuRect.pivot.x;
        float maxX = safeArea.xMax - scaledSize.x * (1f - menuRect.pivot.x);
        float minY = safeArea.yMin + scaledSize.y * menuRect.pivot.y;
        float maxY = safeArea.yMax - scaledSize.y * (1f - menuRect.pivot.y);
        screenPoint.x = Mathf.Clamp(screenPoint.x, minX, Mathf.Max(minX, maxX));
        screenPoint.y = Mathf.Clamp(screenPoint.y, minY, Mathf.Max(minY, maxY));

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, uiCamera, out Vector2 localPoint))
            menuRect.anchoredPosition = localPoint;
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