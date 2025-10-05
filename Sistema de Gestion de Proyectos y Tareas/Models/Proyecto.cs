using System;
using System.ComponentModel.DataAnnotations; // ¡NECESITAS esta importación!

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{
    public class Proyecto
    {
        [Key] // Indica que esta propiedad es la clave primaria
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre del Proyecto")] // Etiqueta amigable para la interfaz de usuario
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; } // Puede ser nullable si tu base de datos lo permite

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)] // Ayuda a renderizar un control de fecha en HTML
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Estado")]
        public byte Estado { get; set; } // O int, dependiendo de cómo lo uses (ej. 0/1)

        // @Nombre, @Descripcion, @FechaInicio, @FechaFin
    }
}