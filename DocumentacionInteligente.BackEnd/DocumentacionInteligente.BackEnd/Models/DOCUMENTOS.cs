using DocumentacionInteligente.BackEnd.Models;
using System.ComponentModel.DataAnnotations;

public class DOCUMENTOS
{
    [Key]
    public int ID { get; set; }

    public string TITULO { get; set; } = string.Empty;

    public string RUTA_ARCHIVO { get; set; } = string.Empty;

    public string DESCRIPCION { get; set; } = string.Empty;

    public int USUARIO_ID { get; set; }

    public int? CATEGORIA_ID { get; set; }
    public virtual CATEGORIAS? CATEGORIA { get; set; }

    public DateTime CREATE_DATE { get; set; }

    public bool? CREADO_IA { get; set; }

    public string ESTADO { get; set; } = string.Empty;

    public int? VERSION_ACTUAL { get; set; }
    public virtual VERSIONES? VERSION { get; set; }
}
