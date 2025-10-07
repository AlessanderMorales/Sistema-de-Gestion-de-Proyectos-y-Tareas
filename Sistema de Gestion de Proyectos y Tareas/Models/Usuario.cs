using System.ComponentModel.DataAnnotations;
namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El primer nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El primer nombre no puede exceder los 50 caracteres.")]
        [Display(Name = "Primer Nombre")]
        public string PrimerNombre { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El segundo nombre no puede exceder los 50 caracteres.")]
        [Display(Name = "Segundo Nombre")]
        public string? SegundoNombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres.")]
        [Display(Name = "Apellidos")]
        public string? Apellidos { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña no puede exceder los 100 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(20, ErrorMessage = "El rol no puede exceder los 20 caracteres.")]
        [Display(Name = "Rol")]
        public string? Rol { get; set; }
    }
}
