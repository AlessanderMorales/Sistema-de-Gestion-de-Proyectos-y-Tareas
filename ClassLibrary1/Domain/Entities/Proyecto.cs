using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ServiceTarea.Domain.Entities;

namespace ServiceProyecto.Domain.Entities
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

            if (ContainsInjection(Nombre))
            {
                yield return new ValidationResult("El nombre contiene intentos explícitos de inyección SQL o contenido HTML/JS peligroso.", new[] { nameof(Nombre) });
            }

            if (!string.IsNullOrEmpty(Descripcion) && ContainsInjection(Descripcion))
            {
                yield return new ValidationResult("La descripción contiene intentos explícitos de inyección SQL o contenido HTML/JS peligroso.", new[] { nameof(Descripcion) });
            }
        }

        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}