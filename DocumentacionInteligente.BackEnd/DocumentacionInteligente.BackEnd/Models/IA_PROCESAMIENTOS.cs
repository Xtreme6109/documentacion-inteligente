using System.ComponentModel.DataAnnotations;

public class IA_PROCESAMIENTOS
{
    [Key]
    public int ID { get; set; }

    public int DOCUMENTO_ID { get; set; }

    public string TIPO { get; set; } = string.Empty;

    public string RESULTADO { get; set; } = string.Empty;

    public DateTime FECHA { get; set; }
}
