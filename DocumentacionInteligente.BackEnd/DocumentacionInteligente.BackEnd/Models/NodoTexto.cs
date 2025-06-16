using DocumentacionInteligente.BackEnd.Models;

public class NodoTexto
{
    public string Texto { get; set; }

    public List<NodoTexto> Lista { get; set; }

    public Dictionary<string, NodoTexto> Objeto { get; set; }

    public bool EsTexto => Texto != null;
    public bool EsLista => Lista != null;
    public bool EsObjeto => Objeto != null;
}
