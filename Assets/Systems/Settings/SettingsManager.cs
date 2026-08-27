using System;
using UnityEngine;

/// <summary>
/// Gestiona la configuración global durante la ejecución del juego.
/// </summary>
public class SettingsManager
{
    private readonly ISettingsRepository repository;

    public GameSettings Current { get; private set; }

    public SettingsManager(ISettingsRepository repository)
    {
        this.repository = repository
            ?? throw new ArgumentNullException(nameof(repository));

        Current = repository.Load() ?? new GameSettings();
        Current.volumen = Mathf.Clamp01(Current.volumen);

        SettingsApplier.Apply(Current);
    }

    public void SetGraphics(GraphicsQuality graphics)
    {
        Current.graficos = graphics;

        SettingsApplier.ApplyGraphics(graphics);
    }

    public void SetVolume(float volume)
    {
        Current.volumen = Mathf.Clamp01(volume);

        SettingsApplier.ApplyVolume(Current.volumen);
    }

    public void Save()
    {
        Current.volumen = Mathf.Clamp01(Current.volumen);

        SettingsApplier.Apply(Current);
        repository.Save(Current);
    }
}