using System.ComponentModel.DataAnnotations;

public class VERSIONES
{
    [Key]
    public int ID { get; set; }

    public int DOCUMENTO_ID { get; set; }

    public int NUMERO_VERSION { get; set; }

    public string RUTA_ARCHIVO { get; set; } = string.Empty;

    public DateTime FECHA_CREACION { get; set; }

    public string NOTAS { get; set; } = string.Empty;
}

