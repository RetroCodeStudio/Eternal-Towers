using System;

public enum GraphicsQuality
{
    Bajo,
    Medio,
    Alto
}

[Serializable]
public class GameSettings
{
    public int id_config = 1;
    public GraphicsQuality graficos = GraphicsQuality.Medio;
    public float volumen = 1f;
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}