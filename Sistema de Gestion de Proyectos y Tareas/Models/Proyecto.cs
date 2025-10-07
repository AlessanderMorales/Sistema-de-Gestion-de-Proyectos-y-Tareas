using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
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
        }
    }
}