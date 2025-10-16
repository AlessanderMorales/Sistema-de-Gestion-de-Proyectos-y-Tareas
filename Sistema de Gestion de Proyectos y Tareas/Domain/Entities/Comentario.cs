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
            string pattern = @"^(?! )[A-Za-zÁÉÍÓÚáéíóúÑñ0-9.,?!;:'""()\s-]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñ0-9.,?!;:'""()\s-]+)*$"; // Permite más caracteres para comentarios
            string multipleSpaces = @" {2,}";

            if (string.IsNullOrWhiteSpace(Contenido) || !Regex.IsMatch(Contenido.TrimEnd(), pattern) || Regex.IsMatch(Contenido, multipleSpaces))
                yield return new ValidationResult("El contenido del comentario solo puede contener letras, números, signos de puntuación y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(Contenido) });

            if (Contenido != Contenido.TrimStart())
                yield return new ValidationResult("El contenido del comentario no debe empezar con espacios.", new[] { nameof(Contenido) });

            string[] sqlKeywords = { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "UNION", "ALTER", "CREATE", "EXEC", "TRUNCATE", "MERGE", "CALL", "GRANT", "REVOKE", "WHERE", "FROM" };
            string[] sqlOperators = { "--", ";--", ";", "/*", "*/", "@@", "@", "char", "nchar", "varchar", "nvarchar", "alter", "begin", "cast", "create", "cursor", "declare", "end", "exec", "execute", "fetch", "kill", "open", "sys", "sysobjects", "syscolumns", "table", "update", "or", "and", "=", "%", "'", "\"", "(", ")", "<script>", "</script>", "javascript:", "data:text/html" }; // Agregados para URL y XSS

            bool ContainsInjection(string input)
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
                        if (lowerInput.Contains(op.ToLowerInvariant())) return true;
                    }
                }
                if (Regex.IsMatch(lowerInput, @"<a\s+href\s*=\s*(['""]?)\s*javascript:", RegexOptions.IgnoreCase)) return true;
                if (Regex.IsMatch(lowerInput, @"<img\s+src\s*=\s*(['""]?)\s*javascript:", RegexOptions.IgnoreCase)) return true;
                if (Regex.IsMatch(lowerInput, @"<iframe\s+src\s*=\s*(['""]?)\s*javascript:", RegexOptions.IgnoreCase)) return true;
                if (Regex.IsMatch(lowerInput, @"<\s*script\b[^>]*>(.*?)</\s*script\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline)) return true;


                return false;
            }

            if (ContainsInjection(Contenido))
            {
                yield return new ValidationResult("El contenido del comentario contiene palabras clave o caracteres peligrosos que no están permitidos.", new[] { nameof(Contenido) });
            }
        }
    }
}