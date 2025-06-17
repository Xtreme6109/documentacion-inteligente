namespace DocumentacionInteligente.BackEnd.Models
{
    public class ROL
    {
        public int ID { get; set; }
        public string NOMBRE { get; set; }
        public string DESCRIPCION { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public bool ESTADO { get; set; }

        public ICollection<PermisoMenuRol> PermisosMenu { get; set; }
    }
}