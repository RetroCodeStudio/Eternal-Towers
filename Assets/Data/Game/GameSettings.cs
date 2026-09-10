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
}