using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ServiceProyecto.Domain.Entities;
using ServiceComentario.Domain.Entities;

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
        public int id_proyecto { get; set; }

        public Proyecto? Proyecto { get; set; }

        public int? IdUsuarioAsignado { get; set; }
        public string Status { get; set; } = "SinIniciar";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string pattern = @"^(?! )[A-Za-zÁÉÍÓÚáéíóúÑñ0-9]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñ0-9]+)*$";
            string multipleSpaces = @" {2,}";

            if (string.IsNullOrWhiteSpace(Titulo) || !Regex.IsMatch(Titulo.TrimEnd(), pattern) || Regex.IsMatch(Titulo, multipleSpaces))
                yield return new ValidationResult("El título solo puede contener letras, números y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(Titulo) });

            if (Titulo != Titulo.TrimStart())
                yield return new ValidationResult("El título no debe empezar con espacios.", new[] { nameof(Titulo) });

            if (!string.IsNullOrWhiteSpace(Descripcion))
            {
                if (!Regex.IsMatch(Descripcion.TrimEnd(), pattern) || Regex.IsMatch(Descripcion, multipleSpaces))
                    yield return new ValidationResult("La descripción solo puede contener letras, números y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(Descripcion) });

                if (Descripcion != Descripcion.TrimStart())
                    yield return new ValidationResult("La descripción no debe empezar con espacios.", new[] { nameof(Descripcion) });
            }

            if (!string.IsNullOrWhiteSpace(Prioridad))
            {
                if (!Regex.IsMatch(Prioridad.TrimEnd(), pattern) || Regex.IsMatch(Prioridad, multipleSpaces))
                    yield return new ValidationResult("La prioridad solo puede contener letras, números y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(Prioridad) });

                if (Prioridad != Prioridad.TrimStart())
                    yield return new ValidationResult("La prioridad no debe empezar con espacios.", new[] { nameof(Prioridad) });
            }

            bool ContainsInjection(string input)
            {
                if (string.IsNullOrEmpty(input)) return false;
                string lowerInput = input.ToLowerInvariant();

                var sqlPatterns = new[]
                {
                    @"(--|;--)",
                    @"\bunion\s+select\b",
                    @"\bdrop\s+table\b",
                    @"\binsert\s+into\b",
                    @"\btruncate\s+table\b",
                    @"\bdelete\s+from\b",
                    @"\bupdate\s+\w+\s+set\b",
                    @"\bexec\s*\(",
                    @"(['""]\s*or\s+['""]?1['""]?\s*=\s*['""]?1['""]?)",
                    @"\bor\s+1\s*=\s*1\b"
                };

                foreach (var p in sqlPatterns)
                {
                    if (Regex.IsMatch(lowerInput, p, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        return true;
                }

                var xssPatterns = new[]
                {
                    @"<\s*script\b",
                    @"<\s*iframe\b",
                    @"javascript\s*:",
                    @"on\w+\s*="
                };

                foreach (var p in xssPatterns)
                {
                    if (Regex.IsMatch(lowerInput, p, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        return true;
                }

                return false;
            }

            if (ContainsInjection(Titulo))
                yield return new ValidationResult("El título contiene intentos explícitos de inyección SQL o contenido HTML/JS peligroso.", new[] { nameof(Titulo) });

            if (!string.IsNullOrEmpty(Descripcion) && ContainsInjection(Descripcion))
                yield return new ValidationResult("La descripción contiene intentos explícitos de inyección SQL o contenido HTML/JS peligroso.", new[] { nameof(Descripcion) });

            if (!string.IsNullOrEmpty(Prioridad) && ContainsInjection(Prioridad))
                yield return new ValidationResult("La prioridad contiene intentos explícitos de inyección SQL o contenido HTML/JS peligroso.", new[] { nameof(Prioridad) });
        }
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}