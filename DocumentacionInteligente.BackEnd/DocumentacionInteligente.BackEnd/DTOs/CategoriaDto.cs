public class CategoriaDTO
{
    public int ID { get; set; }
    public string NOMBRE { get; set; } = string.Empty;
    public string DESCRIPCION { get; set; } = string.Empty;
    public DateTime CREATE_DATE { get; set; }
    public ICollection<DocumentoDto> DOCUMENTOS { get; set; } = new List<DocumentoDto>();
}
