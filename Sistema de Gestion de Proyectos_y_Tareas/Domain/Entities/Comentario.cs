using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities
{
    public class Comentario : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El contenido del comentario es obligatorio.")]
        [StringLength(500, ErrorMessage = "El contenido no puede exceder los 500 caracteres.")]
        public string Contenido { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }
        public int Estado { get; set; }

        public int IdTarea { get; set; }
        public int IdUsuario { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }

        [ValidateNever]
        public Tarea Tarea { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string pattern = @"^(?! )[A-Za-z¡…Õ”⁄·ÈÌÛ˙—Ò0-9.,?!;:'""()\s-]+(?: [A-Za-z¡…Õ”⁄·ÈÌÛ˙—Ò0-9.,?!;:'""()\s-]+)*$"; // Permite m·s caracteres para comentarios
            string multipleSpaces = @" {2,}";

            if (string.IsNullOrWhiteSpace(Contenido) || !Regex.IsMatch(Contenido.TrimEnd(), pattern) || Regex.IsMatch(Contenido, multipleSpaces))
                yield return new ValidationResult("El contenido del comentario solo puede contener letras, n˙meros, signos de puntuaciÛn y un espacio entre palabras, sin espacios al inicio/final ni m˙ltiples espacios.", new[] { nameof(Contenido) });

            if (Contenido != Contenido.TrimStart())
                yield return new ValidationResult("El contenido del comentario no debe empezar con espacios.", new[] { nameof(Contenido) });

            bool ContainsInjection(string input)
            {
                if (string.IsNullOrEmpty(input)) return false;
                string lowerInput = input.ToLowerInvariant();

                string[] explicitSqlPatterns = new[]
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

                foreach (var p in explicitSqlPatterns)
                {
                    if (Regex.IsMatch(lowerInput, p, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        return true;
                }

                string[] xssPatterns = new[]
                {
                    @"<\s*script\b",
                    @"<\s*iframe\b",
                    @"javascript\s*:",
                    @"on\w+\s*=",
                };

                foreach (var p in xssPatterns)
                {
                    if (Regex.IsMatch(lowerInput, p, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        return true;
                }

                return false;
            }

            if (ContainsInjection(Contenido))
            {
                yield return new ValidationResult("El contenido del comentario contiene intentos explÌcitos de inyecciÛn SQL o contenido HTML/JS peligroso.", new[] { nameof(Contenido) });
            }
        }
    }
}