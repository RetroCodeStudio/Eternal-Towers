using SQLite;

/// <summary>
/// Representa la configuración global almacenada en la tabla SQLite.
/// </summary>
[Table("configuracion")]
public class SettingsRecord
{
    [PrimaryKey]
    [Column("id_config")]
    public int IdConfig { get; set; }

    [Column("graficos")]
    public int Graficos { get; set; }

    [Column("volumen")]
    public float Volumen { get; set; }
}
