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
        Current.masterVolume = Current.volumen;

        SettingsApplier.ApplyVolume(Current.masterVolume);
    }

    public void SetMasterVolume(float volume)
    {
        Current.masterVolume = Mathf.Clamp01(volume);
        Current.volumen = Current.masterVolume;
        SettingsApplier.ApplyVolume(Current.masterVolume);
    }

    public void SetMusicVolume(float volume)
    {
        Current.musicVolume = Mathf.Clamp01(volume);
    }

    public void SetSfxVolume(float volume)
    {
        Current.sfxVolume = Mathf.Clamp01(volume);
    }

    public void Save()
    {
        Current.volumen = Mathf.Clamp01(Current.volumen);
        Current.masterVolume = Mathf.Clamp01(Current.masterVolume);
        Current.musicVolume = Mathf.Clamp01(Current.musicVolume);
        Current.sfxVolume = Mathf.Clamp01(Current.sfxVolume);

        SettingsApplier.Apply(Current);
        repository.Save(Current);
    }
}