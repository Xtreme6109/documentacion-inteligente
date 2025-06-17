namespace DocumentacionInteligente.BackEnd.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; } 
        public string Icon { get; set; }    
        public string Link { get; set; }

        public ICollection<PermisoMenuRol> PermisosMenuRoles { get; set; }
    }
}
