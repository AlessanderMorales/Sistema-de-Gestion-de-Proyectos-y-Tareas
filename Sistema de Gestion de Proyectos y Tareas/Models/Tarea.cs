using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{//agregar validaciones
    public class Tarea
    {
        [Key] // Clave Primaria, al igual que en Proyecto.cs
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El título de la tarea es obligatorio.")] // Validación como en Proyecto.cs
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        [Display(Name = "Prioridad")]
        public string? Prioridad { get; set; } // Ejemplo: Alta, Media, Baja

        // --- Relación con Proyecto (Clave Foránea) ---
        [Required(ErrorMessage = "Debes asignar un proyecto.")]
        [Display(Name = "Proyecto Asignado")]
        public int ProyectoId { get; set; }

        // Propiedad de navegación (necesita 'using System.ComponentModel.DataAnnotations.Schema;' si no está)
        [ForeignKey("ProyectoId")]
        public Proyecto? Proyecto { get; set; } 
    }
}