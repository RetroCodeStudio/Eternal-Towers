using UnityEngine;

public class SettingsManager
{
    public GameSettings Current { get; private set; }

    public SettingsManager(GameSettings initialSettings = null)
    {
        Current = initialSettings ?? new GameSettings();
        Current.volumen = Mathf.Clamp01(Current.volumen);
    }

    public void SetGraphics(GraphicsQuality graphics)
    {
        Current.graficos = graphics;
    }

    public void SetVolume(float volume)
    {
        Current.volumen = Mathf.Clamp01(volume);
    }
}