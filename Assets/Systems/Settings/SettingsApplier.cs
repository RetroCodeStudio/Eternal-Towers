using UnityEngine;

/// <summary>
/// Aplica los valores de configuración a los sistemas globales de Unity.
/// </summary>
public static class SettingsApplier
{
    public static void Apply(GameSettings settings)
    {
        if (settings == null)
        {
            return;
        }

        ApplyGraphics(settings.graficos);
        ApplyVolume(settings.volumen);
    }

    public static void ApplyGraphics(GraphicsQuality graphics)
    {
        if (QualitySettings.names.Length == 0)
        {
            return;
        }

        int qualityIndex = graphics switch
        {
            GraphicsQuality.Bajo => 0,
            GraphicsQuality.Medio => QualitySettings.names.Length / 2,
            GraphicsQuality.Alto => QualitySettings.names.Length - 1,
            _ => QualitySettings.names.Length / 2
        };

        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    public static void ApplyVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }
}