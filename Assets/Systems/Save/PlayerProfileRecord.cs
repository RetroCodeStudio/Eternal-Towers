using SQLite;

[Table("Usuario")]
public class PlayerProfileRecord
{
    [PrimaryKey]
    [Column("id_usuario")]
    public string PlayerId { get; set; }

    [Column("nombre_usuario")]
    public string PlayerName { get; set; }

    [Column("genero")]
    public string Gender { get; set; }

    [Column("edad")]
    public int Age { get; set; }

    [Column("contraseña")]
    public string Password { get; set; }
}