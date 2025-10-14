
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities
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
        public int id_proyecto { get; set; }
        public Proyecto? Proyecto { get; set; }
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
                    if (Regex.IsMatch(op, @"^[a-zA-Z]+$"))
                    {
                        if (Regex.IsMatch(lowerInput, $@"\b{op.ToLowerInvariant()}\b")) return true;
                    }
                    else
                    {
                        if (lowerInput.Contains(op)) return true;
                    }
                }
                return false;
            }

            if (ContainsSqlInjection(Titulo) ||
                !string.IsNullOrEmpty(Descripcion) && ContainsSqlInjection(Descripcion) ||
                !string.IsNullOrEmpty(Prioridad) && ContainsSqlInjection(Prioridad))
            {
                yield return new ValidationResult("No se permiten palabras clave ni caracteres peligrosos en los campos de texto.", new[] { nameof(Titulo), nameof(Descripcion), nameof(Prioridad) });
            }
        }
    }
}