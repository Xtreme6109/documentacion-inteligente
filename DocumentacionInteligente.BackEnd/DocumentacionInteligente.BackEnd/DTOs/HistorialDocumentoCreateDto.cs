public class HistorialDocumentoCreateDto
{
    public int UsuarioId { get; set; }
    public string? Prompt { get; set; }
    public int CantidadPalabrasPrompt { get; set; }
    public int TokensEntrada { get; set; }
    public int TokensSalida { get; set; }
    public string? Resultado { get; set; }
}
