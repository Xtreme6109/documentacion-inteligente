using DocumentacionInteligente.BackEnd.Models;

public class DocumentoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Categoria { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool CreadoIA { get; set; }
    public DateTime CreateDate { get; set; }
    public int VersionActual { get; set; }
    public string? RutaArchivo { get; set; } // Si quieres mostrar un botón de descarga


    public int UsuarioCreadorId { get; set; } 
    public int UsuarioId { get; set; }
    public string NombreCategoria { get; set; } = string.Empty; 
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public string NombreUsuarioCreador { get; set; }


    
}

