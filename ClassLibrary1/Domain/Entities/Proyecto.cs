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

        [StringLength(30, ErrorMessage = "La descripcion no puede exceder los 30 caracteres.")]
        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalizacion es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalizacion")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Estado")]
        public byte Estado { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string nombrePattern = @"^(?! )[A-Za-z0-9AEIOUaeiouNn\s'-]+(?: [A-Za-z0-9AEIOUaeiouNn\s'-]+)*$";
            string sinEspaciosMultiples = @" {2,}";
            string caracteresEspecialesPattern = @"[$%#&@!*()+=\[\]{};:""<>,.?/\\|`~^]";

            if (!string.IsNullOrWhiteSpace(Nombre))
            {
                if (Regex.IsMatch(Nombre, caracteresEspecialesPattern))
                {
                    yield return new ValidationResult("El nombre no puede contener caracteres especiales. Solo se permiten letras, numeros, espacios, guiones y apostrofes.", new[] { nameof(Nombre) });
                }
                else if (!Regex.IsMatch(Nombre.Trim(), nombrePattern))
                {
                    yield return new ValidationResult("El nombre solo puede contener letras, numeros, espacios, guiones y apostrofes.", new[] { nameof(Nombre) });
                }
                else if (Regex.IsMatch(Nombre, sinEspaciosMultiples))
                {
                    yield return new ValidationResult("El nombre no puede tener multiples espacios consecutivos.", new[] { nameof(Nombre) });
                }
                else if (Nombre != Nombre.Trim())
                {
                    yield return new ValidationResult("El nombre no debe empezar ni terminar con espacios.", new[] { nameof(Nombre) });
                }
            }

            if (!string.IsNullOrWhiteSpace(Descripcion))
            {
                if (Regex.IsMatch(Descripcion, caracteresEspecialesPattern))
                {
                    yield return new ValidationResult("La descripcion no puede contener caracteres especiales peligrosos.", new[] { nameof(Descripcion) });
                }
                else if (Regex.IsMatch(Descripcion, sinEspaciosMultiples))
                {
                    yield return new ValidationResult("La descripcion no puede tener multiples espacios consecutivos.", new[] { nameof(Descripcion) });
                }
                else if (Descripcion != Descripcion.Trim())
                {
                    yield return new ValidationResult("La descripcion no debe empezar ni terminar con espacios.", new[] { nameof(Descripcion) });
                }
            }

            if (FechaFin < FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalizacion no puede ser anterior a la fecha de inicio.",
                    new[] { nameof(FechaFin) });
            }

            if (FechaFin == FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalizacion no puede ser igual a la fecha de inicio.",
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
                    @"\bxp_cmdshell\b",
                    @"\bbenchmark\s*\(",
                    @"\bwaitfor\s+delay\b",
                    @"(['""]\s*or\s+['""]?1['""]?\s*=\s*['""]?1['""]?)",
                    @"\bor\s+1\s*=\s*1\b",
                    @"\bselect\s+.*\s+from\b",
                    @"\balter\s+table\b"
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
                    @"on\w+\s*=",
                    @"<\s*object\b",
                    @"<\s*embed\b"
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
                yield return new ValidationResult("El nombre contiene intentos de inyeccion SQL o contenido HTML peligroso.", new[] { nameof(Nombre) });
            }

            if (!string.IsNullOrEmpty(Descripcion) && ContainsInjection(Descripcion))
            {
                yield return new ValidationResult("La descripcion contiene intentos de inyeccion SQL o contenido HTML peligroso.", new[] { nameof(Descripcion) });
            }
        }

        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}