namespace DocumentacionInteligente.BackEnd.Models
{
    public class DocumentoDTO2
    {
        public class HistorialCambio2
        {
            public int? Number { get; set; }
            public DateTime? Date { get; set; }
            public string Description { get; set; }
        }

        public string TítuloDelDocumento { get; set; }
        public DateTime? FechaDeEdición { get; set; }
        public string Version { get; set; }
        public string CodigoDelDocumento { get; set; }
        public string ElaboradoPor { get; set; }
        public string RevisadoPor { get; set; }
        public string IObjetivo { get; set; }
        public string IIAlcance { get; set; }

        // Ahora es string (puede ser plano) o un JSON serializado.
        public object IIIResponsabilidades { get; set; }
        public Dictionary<string, Dictionary<string, string>> IVDesarrollo { get; set; }

        public string VVigencia { get; set; }
        public string VIReferenciasBibliográficas { get; set; }
        public List<HistorialCambio2> VIIHistorialDeCambioDeDocumentos { get; set; }
        public string VIIIFirmas { get; set; }

    }
}
