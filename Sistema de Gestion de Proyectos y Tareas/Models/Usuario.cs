using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{
    public class Usuario : IValidatableObject
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
        [StringLength(15, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 15 caracteres.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string nombrePattern = @"^(?! )[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñ]+)*$";
            string sinEspaciosMultiples = @" {2,}";

            if (string.IsNullOrWhiteSpace(PrimerNombre) || !Regex.IsMatch(PrimerNombre.Trim(), nombrePattern) || Regex.IsMatch(PrimerNombre, sinEspaciosMultiples))
                yield return new ValidationResult("El primer nombre solo puede contener letras y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(PrimerNombre) });

            if (!string.IsNullOrWhiteSpace(SegundoNombre))
            {
                if (!Regex.IsMatch(SegundoNombre.Trim(), nombrePattern) || Regex.IsMatch(SegundoNombre, sinEspaciosMultiples))
                    yield return new ValidationResult("El segundo nombre solo puede contener letras y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(SegundoNombre) });
            }

            if (string.IsNullOrWhiteSpace(Apellidos) || !Regex.IsMatch(Apellidos.Trim(), nombrePattern) || Regex.IsMatch(Apellidos, sinEspaciosMultiples))
                yield return new ValidationResult("El apellido solo puede contener letras y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(Apellidos) });

            if (string.IsNullOrWhiteSpace(Contraseña) ||
                Contraseña.Length < 8 || Contraseña.Length > 15 ||
                !Regex.IsMatch(Contraseña, @"[A-Z]") ||
                !Regex.IsMatch(Contraseña, @"[a-z]") ||
                !Regex.IsMatch(Contraseña, @"\d") ||
                !Regex.IsMatch(Contraseña, @"[\W_]"))
            {
                yield return new ValidationResult("La contraseña debe tener entre 8 y 15 caracteres, incluir al menos una mayúscula, una minúscula, un número y un carácter especial.", new[] { nameof(Contraseña) });
            }

            if (PrimerNombre != PrimerNombre.Trim())
                yield return new ValidationResult("El primer nombre no debe empezar ni terminar con espacios.", new[] { nameof(PrimerNombre) });
            if (!string.IsNullOrEmpty(SegundoNombre) && SegundoNombre != SegundoNombre.Trim())
                yield return new ValidationResult("El segundo nombre no debe empezar ni terminar con espacios.", new[] { nameof(SegundoNombre) });
            if (!string.IsNullOrEmpty(Apellidos) && Apellidos != Apellidos.Trim())
                yield return new ValidationResult("El apellido no debe empezar ni terminar con espacios.", new[] { nameof(Apellidos) });

            string[] sqlKeywords = { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "UNION", "ALTER", "CREATE", "EXEC", "TRUNCATE", "MERGE", "CALL", "GRANT", "REVOKE", "WHERE", "FROM" };
            string[] sqlOperators = { "--", ";--", ";", "/*", "*/", "@@", "@", "char", "nchar", "varchar", "nvarchar", "alter", "begin", "cast", "create", "cursor", "declare", "end", "exec", "execute", "fetch", "kill", "open", "sys", "sysobjects", "syscolumns", "table", "update", "or", "and", "=", "%", "'", "\"", "(", ")" };

            bool ContainsSqlInjection(string input)
            {
                if (string.IsNullOrEmpty(input)) return false;
                string lowerInput = input.ToLowerInvariant();
                foreach (var keyword in sqlKeywords)
                {
                    if (Regex.IsMatch(lowerInput, $@"\b{keyword.ToLowerInvariant()}\b")) return true;
                }
                foreach (var op in sqlOperators)
                {
                    if (lowerInput.Contains(op)) return true;
                }
                return false;
            }

            if (ContainsSqlInjection(PrimerNombre) ||
                (!string.IsNullOrEmpty(SegundoNombre) && ContainsSqlInjection(SegundoNombre)) ||
                (!string.IsNullOrEmpty(Apellidos) && ContainsSqlInjection(Apellidos)))
            {
                yield return new ValidationResult("No se permiten palabras clave ni caracteres peligrosos en los nombres o apellidos.", new[] { nameof(PrimerNombre), nameof(SegundoNombre), nameof(Apellidos) });
            }
        }
    }
}
