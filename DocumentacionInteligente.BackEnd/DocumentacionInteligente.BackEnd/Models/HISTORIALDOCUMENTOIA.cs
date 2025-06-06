using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("HISTORIAL_DOCUMENTOS_IA")]
public class HISTORIALDOCUMENTOSIA
{
    [Key]
    public int ID { get; set; }

    [Required]
    public int USUARIO_ID { get; set; }

    [Required]
    public DateTime FECHA_GENERACION { get; set; } = DateTime.Now;

    public string? PROMPT { get; set; }

    [Required]
    public int CANTIDAD_PALABRAS_PROMPT { get; set; }

    [Required]
    public int TOKENS_ENTRADA { get; set; }

    [Required]
    public int TOKENS_SALIDA { get; set; }


    public string? RESULTADO { get; set; }
}
