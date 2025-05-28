using System.ComponentModel.DataAnnotations;

public class USUARIOS
{
    [Key]
    public int ID { get; set; }

    [Required, MaxLength(100)]
    public string NOMBRE { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string CORREO { get; set; } = string.Empty;

    public string PASSWORD_HASH { get; set; } = string.Empty;

    public string ROL { get; set; } = string.Empty;

    public DateTime CREATE_DATE { get; set; }
}

