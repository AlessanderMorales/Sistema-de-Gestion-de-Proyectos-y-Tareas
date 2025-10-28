using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace ServiceUsuario.Domain.Entities
{
    public class Usuario : IValidatableObject
    {
     public int Id { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100)]
   [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Primer Apellido")]
    public string PrimerApellido { get; set; } = string.Empty;

        [StringLength(50)]
   [Display(Name = "Segundo Apellido")]
     public string? SegundoApellido { get; set; }

        [StringLength(50)]
        [Display(Name = "Nombre de Usuario")]
  public string NombreUsuario { get; set; } = string.Empty;

    [StringLength(15, MinimumLength = 8, ErrorMessage = "La contrasena debe tener entre 8 y 15 caracteres.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; } = string.Empty;

      [Required(ErrorMessage = "El email es obligatorio.")]
      [EmailAddress(ErrorMessage = "Formato de email no valido.")]
  public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un rol.")]
     public string Rol { get; set; } = string.Empty;

        public int Estado { get; set; } = 1;

     public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
            string nombrePattern = @"^(?! )[A-Za-zAEIOUaeiouNn'-]+(?: [A-Za-zAEIOUaeiouNn'-]+)*$";
    string sinEspaciosMultiples = @" {2,}";
    string contieneNumerosPattern = @"\d";
        string caracteresEspecialesPattern = @"[$%#&@!*()+=\[\]{};:""<>,.?/\\|`~^]";

            if (string.IsNullOrWhiteSpace(Nombres))
 {
        yield return new ValidationResult("Los nombres son obligatorios.", new[] { nameof(Nombres) });
      }
 else
   {
            if (Regex.IsMatch(Nombres, contieneNumerosPattern))
                {
      yield return new ValidationResult("Los nombres no pueden contener numeros. Solo se permiten letras, espacios, guiones y apostrofes.", new[] { nameof(Nombres) });
  }
 else if (Regex.IsMatch(Nombres, caracteresEspecialesPattern))
    {
 yield return new ValidationResult("Los nombres no pueden contener caracteres especiales. Solo se permiten letras, espacios, guiones y apostrofes.", new[] { nameof(Nombres) });
         }
     else if (!Regex.IsMatch(Nombres.Trim(), nombrePattern))
                {
  yield return new ValidationResult("Los nombres solo pueden contener letras, espacios, guiones y apostrofes.", new[] { nameof(Nombres) });
       }
     else if (Regex.IsMatch(Nombres, sinEspaciosMultiples))
                {
         yield return new ValidationResult("Los nombres no pueden tener multiples espacios consecutivos.", new[] { nameof(Nombres) });
 }
            else if (Nombres != Nombres.Trim())
           {
              yield return new ValidationResult("Los nombres no deben empezar ni terminar con espacios.", new[] { nameof(Nombres) });
  }
   }

        if (string.IsNullOrWhiteSpace(PrimerApellido))
         {
    yield return new ValidationResult("El primer apellido es obligatorio.", new[] { nameof(PrimerApellido) });
            }
else
    {
       if (Regex.IsMatch(PrimerApellido, contieneNumerosPattern))
          {
                  yield return new ValidationResult("El primer apellido no puede contener numeros.", new[] { nameof(PrimerApellido) });
           }
         else if (Regex.IsMatch(PrimerApellido, caracteresEspecialesPattern))
      {
        yield return new ValidationResult("El primer apellido no puede contener caracteres especiales.", new[] { nameof(PrimerApellido) });
  }
    else if (!Regex.IsMatch(PrimerApellido.Trim(), nombrePattern))
                {
    yield return new ValidationResult("El primer apellido solo puede contener letras, guiones y apostrofes.", new[] { nameof(PrimerApellido) });
    }
     else if (Regex.IsMatch(PrimerApellido, sinEspaciosMultiples))
             {
  yield return new ValidationResult("El primer apellido no puede tener multiples espacios consecutivos.", new[] { nameof(PrimerApellido) });
     }
else if (PrimerApellido != PrimerApellido.Trim())
           {
          yield return new ValidationResult("El primer apellido no debe empezar ni terminar con espacios.", new[] { nameof(PrimerApellido) });
                }
            }

          if (!string.IsNullOrWhiteSpace(SegundoApellido))
   {
          if (Regex.IsMatch(SegundoApellido, contieneNumerosPattern))
            {
   yield return new ValidationResult("El segundo apellido no puede contener numeros.", new[] { nameof(SegundoApellido) });
}
    else if (Regex.IsMatch(SegundoApellido, caracteresEspecialesPattern))
         {
      yield return new ValidationResult("El segundo apellido no puede contener caracteres especiales.", new[] { nameof(SegundoApellido) });
         }
      else if (!Regex.IsMatch(SegundoApellido.Trim(), nombrePattern))
      {
          yield return new ValidationResult("El segundo apellido solo puede contener letras, guiones y apostrofes.", new[] { nameof(SegundoApellido) });
      }
      else if (Regex.IsMatch(SegundoApellido, sinEspaciosMultiples))
        {
     yield return new ValidationResult("El segundo apellido no puede tener multiples espacios consecutivos.", new[] { nameof(SegundoApellido) });
 }
                else if (SegundoApellido != SegundoApellido.Trim())
        {
     yield return new ValidationResult("El segundo apellido no debe empezar ni terminar con espacios.", new[] { nameof(SegundoApellido) });
         }
            }

            if (!string.IsNullOrWhiteSpace(Contraseña) && !Contraseña.StartsWith("PBKDF2:"))
    {
      if (Contraseña.Length < 8 || Contraseña.Length > 15 ||
    !Regex.IsMatch(Contraseña, @"[A-Z]") ||
  !Regex.IsMatch(Contraseña, @"[a-z]") ||
     !Regex.IsMatch(Contraseña, @"\d") ||
        !Regex.IsMatch(Contraseña, @"[\W_]"))
                {
            yield return new ValidationResult("La contrasena debe tener entre 8 y 15 caracteres, incluir al menos una mayuscula, una minuscula, un numero y un caracter especial.", new[] { nameof(Contraseña) });
    }
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

            if (ContainsInjection(Nombres))
    yield return new ValidationResult("No se permiten intentos de inyeccion SQL o contenido HTML peligroso en los nombres.", new[] { nameof(Nombres) });

            if (!string.IsNullOrWhiteSpace(PrimerApellido) && ContainsInjection(PrimerApellido))
 yield return new ValidationResult("No se permiten intentos de inyeccion SQL o contenido HTML peligroso en el primer apellido.", new[] { nameof(PrimerApellido) });

        if (!string.IsNullOrWhiteSpace(SegundoApellido) && ContainsInjection(SegundoApellido))
                yield return new ValidationResult("No se permiten intentos de inyeccion SQL o contenido HTML peligroso en el segundo apellido.", new[] { nameof(SegundoApellido) });
    }
    }
}