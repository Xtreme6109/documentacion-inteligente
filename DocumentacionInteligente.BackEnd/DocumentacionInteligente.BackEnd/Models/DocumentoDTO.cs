namespace DocumentacionInteligente.BackEnd.Models
{
    public class DocumentoDTO
    {
        public class HistorialCambio
        {
            public int? Number { get; set; }
            public DateTime? Date { get; set; }
            public string Description { get; set; }
        }


        public string TítuloDelDocumento { get; set; }
        public DateTime? FechaDeEdición { get; set; }
        public string Version { get; set; }
        public string CódigoDelDocumento { get; set; }
        public string ElaboradoPor { get; set; }
        public string RevisadoPor { get; set; }
        public string IObjetivo { get; set; }
        public string IIAlcance { get; set; }
        public Dictionary<string, string> IIIResponsabilidades { get; set; }
        public Dictionary<string, string> IVDesarrollo { get; set; }
        public string VVigencia { get; set; }
        public string VIReferenciasBibliográficas { get; set; }
        public List<HistorialCambio> VIIHistorialDeCambioDeDocumentos { get; set; }
        public string VIIIFirmas { get; set; }
        public string Titulo { get; set; }
        public int Hoja { get; set; }
        public int TotalHojas { get; set; }
        public string AutorizadoPor { get; set; }
        public DateTime? FechaDivulgacion { get; set; }
        public int Categoria { get; set; }  // Id de categoría
        public string NombreCategoria { get; set; } = string.Empty;  // <-- Agrega esta propiedad

        public int UsuarioCreadorId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string NombreUsuarioCreador { get; set; }

    }
}
