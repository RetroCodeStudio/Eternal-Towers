using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Conecta los controles del menú con la configuración global del juego.
/// </summary>
public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private Dropdown graphicsDropdown;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (graphicsDropdown == null || volumeSlider == null)
        {
            Debug.LogError("Asigna el Dropdown y el Slider en el inspector.");
            enabled = false;
            return;
        }

        if (SettingsBootstrap.Settings == null)
        {
            Debug.LogError("La configuración global no fue inicializada.");
            enabled = false;
            return;
        }

        ConfigureGraphicsDropdown();
        LoadSettingsIntoUI();

        graphicsDropdown.onValueChanged.AddListener(OnGraphicsChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnDestroy()
    {
        if (graphicsDropdown != null)
        {
            graphicsDropdown.onValueChanged.RemoveListener(OnGraphicsChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }

    private void ConfigureGraphicsDropdown()
    {
        graphicsDropdown.ClearOptions();

        graphicsDropdown.AddOptions(
            new List<string>(Enum.GetNames(typeof(GraphicsQuality)))
        );
    }

    private void LoadSettingsIntoUI()
    {
        GameSettings settings = SettingsBootstrap.Settings.Current;

        graphicsDropdown.value = (int)settings.graficos;
        graphicsDropdown.RefreshShownValue();

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = settings.volumen;
    }

    private void OnGraphicsChanged(int value)
    {
        if (!Enum.IsDefined(typeof(GraphicsQuality), value))
        {
            return;
        }

        SettingsBootstrap.Settings.SetGraphics((GraphicsQuality)value);
        SettingsBootstrap.Settings.Save();
    }

    private void OnVolumeChanged(float value)
    {
        SettingsBootstrap.Settings.SetVolume(value);
        SettingsBootstrap.Settings.Save();
    }
}