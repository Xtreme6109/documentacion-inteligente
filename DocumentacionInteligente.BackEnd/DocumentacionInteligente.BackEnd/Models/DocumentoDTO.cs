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
        public string IIIResponsabilidades { get; set; }
        public string IVDesarrollo { get; set; }
        public string VVigencia { get; set; }
        public string VIReferenciasBibliográficas { get; set; }
        public List<HistorialCambio> VIIHistorialDeCambioDeDocumentos { get; set; }
        public string VIIIFirmas { get; set; }

    }
}
