using System.ComponentModel.DataAnnotations;

public class LOGS_ACCESO
{
    [Key]
    public int ID { get; set; }

    public int USUARIO_ID { get; set; }

    public string ACCION { get; set; } = string.Empty;

    public int? DOCUMENTO_ID { get; set; }

    public DateTime FECHA { get; set; }
}
