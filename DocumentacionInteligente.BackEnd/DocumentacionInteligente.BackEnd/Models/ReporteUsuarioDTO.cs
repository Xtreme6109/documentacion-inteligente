using System;
using System.ComponentModel.DataAnnotations;

public class ReporteUsuarioDTO
{
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }
}
