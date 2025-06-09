using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DocumentacionInteligente.BackEnd.Models;

namespace DocumentacionInteligente.BackEnd.Models
{
    public class CATEGORIAS
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string NOMBRE { get; set; } = string.Empty;

        public string DESCRIPCION { get; set; } = string.Empty;

        public DateTime CREATE_DATE { get; set; }

        // Relación de navegación: una categoría tiene muchos documentos
        [JsonIgnore]
        public virtual ICollection<DOCUMENTOS> DOCUMENTOS { get; set; } = new List<DOCUMENTOS>();

        [JsonIgnore]
        public virtual ICollection<USUARIOS> USUARIOS { get; set; } = new List<USUARIOS>();
    }
}
