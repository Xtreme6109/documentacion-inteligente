using System.ComponentModel.DataAnnotations;

public class PALABRAS_CLAVE
{
    [Key]
    public int ID { get; set; }

    public int DOCUMENTO_ID { get; set; }

    public string PALABRA { get; set; } = string.Empty;
}
