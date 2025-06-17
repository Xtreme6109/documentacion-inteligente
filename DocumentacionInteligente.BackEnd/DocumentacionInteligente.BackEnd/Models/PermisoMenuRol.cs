namespace DocumentacionInteligente.BackEnd.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class PermisoMenuRol
    {
        public int ID { get; set; }
        
        public string ROL { get; set; }

        public int MENU_ID { get; set; }

        public bool CAN_VIEW { get; set; }
        public bool CAN_EDIT { get; set; }
        public DateTime CREATE_DATE { get; set; }

        public Menu Menu { get; set; }

        [JsonIgnore]
        public ROL Rol { get; set; } 
    }
}
