using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El primer nombre es obligatorio.")]
        [StringLength(50)]
        public string PrimerNombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string? SegundoNombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100)]
        public string? Apellidos { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(20)]
        public string? Rol { get; set; }
    }
}
