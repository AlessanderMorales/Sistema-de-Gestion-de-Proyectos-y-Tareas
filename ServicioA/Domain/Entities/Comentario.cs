using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ServiceUsuario.Domain.Entities;
using ServiceTarea.Domain.Entities;

namespace ServiceComentario.Domain.Entities
{
    public class Comentario : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El contenido del comentario es obligatorio.")]
      [StringLength(500, ErrorMessage = "El contenido no puede exceder los 500 caracteres.")]
 public string Contenido { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }
        public int Estado { get; set; }

        [Required]
     public int IdTarea { get; set; }

   [Required]
        public int IdUsuario { get; set; }

     [ValidateNever]
        public Usuario Usuario { get; set; }

        [ValidateNever]
        public Tarea Tarea { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string pattern = @"^(?! )[A-Za-zAEIOUaeiouNn0-9.,?!;:'""()\s-]+(?: [A-Za-zAEIOUaeiouNn0-9.,?!;:'""()\s-]+)*$";
 string multipleSpaces = @" {2,}";

            if (string.IsNullOrWhiteSpace(Contenido) || !Regex.IsMatch(Contenido.TrimEnd(), pattern) || Regex.IsMatch(Contenido, multipleSpaces))
            {
         yield return new ValidationResult(
     "El contenido del comentario solo puede contener letras, numeros, signos de puntuacion y espacios.",
        new[] { nameof(Contenido) });
}

            if (Contenido != Contenido.TrimStart())
      {
      yield return new ValidationResult(
      "El contenido del comentario no debe empezar con espacios.",
  new[] { nameof(Contenido) });
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

            if (ContainsInjection(Contenido))
            {
                yield return new ValidationResult(
        "El contenido del comentario contiene intentos de inyeccion SQL o XSS.",
        new[] { nameof(Contenido) });
            }
        }
  }
}