using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ServiceTarea.Domain.Entities
{
    public class Tarea : IValidatableObject
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

        [Required(ErrorMessage = "El proyecto es obligatorio.")]
        [Display(Name = "Proyecto")]
        public int IdProyecto { get; set; }   // 🔹 Solo guardas el ID

        public int? IdUsuarioAsignado { get; set; }
        public string Status { get; set; } = "SinIniciar";

        public int Estado { get; set; } = 1;

        // 🔹 Lista de IDs de comentarios (no la entidad completa)
        public ICollection<int> IdComentarios { get; set; } = new List<int>();

        // (Validaciones permanecen igual)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string pattern = @"^(?! )[A-Za-zÁÉÍÓÚáéíóúÑñ0-9]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñ0-9]+)*$";
            string multipleSpaces = @" {2,}";

            if (string.IsNullOrWhiteSpace(Titulo) || !Regex.IsMatch(Titulo.TrimEnd(), pattern) || Regex.IsMatch(Titulo, multipleSpaces))
                yield return new ValidationResult("El título solo puede contener letras, números y un espacio entre palabras.", new[] { nameof(Titulo) });

            if (!string.IsNullOrWhiteSpace(Descripcion))
            {
                if (!Regex.IsMatch(Descripcion.TrimEnd(), pattern) || Regex.IsMatch(Descripcion, multipleSpaces))
                    yield return new ValidationResult("La descripción solo puede contener letras y números.", new[] { nameof(Descripcion) });
            }

            if (!string.IsNullOrWhiteSpace(Prioridad))
            {
                if (!Regex.IsMatch(Prioridad.TrimEnd(), pattern) || Regex.IsMatch(Prioridad, multipleSpaces))
                    yield return new ValidationResult("La prioridad solo puede contener letras y números.", new[] { nameof(Prioridad) });
            }
        }
    }
}
