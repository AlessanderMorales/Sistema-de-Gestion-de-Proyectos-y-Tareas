using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{//agregar validaciones
    public class Tarea
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El título de la tarea es obligatorio.")] 
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        [Display(Name = "Prioridad")]
        public string? Prioridad { get; set; } 

        [Required(ErrorMessage = "Debes asignar un proyecto.")]
        [Display(Name = "Proyecto Asignado")]
        public int ProyectoId { get; set; }
        [ForeignKey("ProyectoId")]
        public Proyecto? Proyecto { get; set; } 
    }
}