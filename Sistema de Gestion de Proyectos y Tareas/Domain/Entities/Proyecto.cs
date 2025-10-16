using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities
{
    public class Proyecto : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio.")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder los 30 caracteres.")]
        [Display(Name = "Nombre del Proyecto")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "La descripción no puede exceder los 30 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Estado")]
        public byte Estado { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaFin < FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalización no puede ser anterior a la fecha de inicio.",
                    new[] { nameof(FechaFin) });
            }

            if (FechaFin == FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalización no puede ser igual a la fecha de inicio.",
                    new[] { nameof(FechaFin) });
            }

            string[] sqlKeywords = { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "UNION", "ALTER", "CREATE", "EXEC", "TRUNCATE", "MERGE", "CALL", "GRANT", "REVOKE", "WHERE", "FROM" };
            string[] sqlOperators = { "--", ";--", ";", "/*", "*/", "@@", "@", "char", "nchar", "varchar", "nvarchar", "alter", "begin", "cast", "create", "cursor", "declare", "end", "exec", "execute", "fetch", "kill", "open", "sys", "sysobjects", "syscolumns", "table", "update", "or", "and", "=", "%", "'", "\"", "(", ")", "<script>", "</script>", "javascript:", "data:text/html" };

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

            if (ContainsInjection(Nombre) || (!string.IsNullOrEmpty(Descripcion) && ContainsInjection(Descripcion)))
            {
                yield return new ValidationResult("Los campos de texto contienen palabras clave o caracteres peligrosos que no están permitidos.", new[] { nameof(Nombre), nameof(Descripcion) });
            }
        }

        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}