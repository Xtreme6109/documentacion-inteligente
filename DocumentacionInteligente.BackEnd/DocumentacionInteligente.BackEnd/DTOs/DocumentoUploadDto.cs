public class DocumentoSubidaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool CreadoIA { get; set; }
    public IFormFile Archivo { get; set; } = default!;
}
