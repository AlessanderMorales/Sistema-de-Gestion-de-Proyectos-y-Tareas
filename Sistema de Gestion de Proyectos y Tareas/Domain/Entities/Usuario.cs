using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities
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

        [StringLength(15, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 15 caracteres.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email no válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, seleccione un rol (empleado o jefe de proyecto).")]
        public string Rol { get; set; } = string.Empty;

        public int Estado { get; set; } = 1;

        [Obsolete("Use Nombres instead")]
        public string PrimerNombre
        {
            get => Nombres?.Split(' ').FirstOrDefault() ?? string.Empty;
            set => Nombres = value;
        }

        [Obsolete("Use PrimerApellido and SegundoApellido instead")]
        public string Apellidos
        {
            get => $"{PrimerApellido} {SegundoApellido}".Trim();
            set
            {
                var partes = value?.Split(' ', 2);
                if (partes?.Length > 0) PrimerApellido = partes[0];
                if (partes?.Length > 1) SegundoApellido = partes[1];
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string nombrePattern = @"^(?! )[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñ]+)*$";
            string sinEspaciosMultiples = @" {2,}";

            if (string.IsNullOrWhiteSpace(Nombres) || !Regex.IsMatch(Nombres.Trim(), nombrePattern) || Regex.IsMatch(Nombres, sinEspaciosMultiples))
                yield return new ValidationResult("Los nombres solo pueden contener letras y un espacio entre palabras, sin espacios al inicio/final ni múltiples espacios.", new[] { nameof(Nombres) });

            if (string.IsNullOrWhiteSpace(PrimerApellido) || !Regex.IsMatch(PrimerApellido.Trim(), nombrePattern) || Regex.IsMatch(PrimerApellido, sinEspaciosMultiples))
                yield return new ValidationResult("El primer apellido solo puede contener letras, sin espacios al inicio/final.", new[] { nameof(PrimerApellido) });

            if (!string.IsNullOrWhiteSpace(SegundoApellido))
            {
                if (!Regex.IsMatch(SegundoApellido.Trim(), nombrePattern) || Regex.IsMatch(SegundoApellido, sinEspaciosMultiples))
                    yield return new ValidationResult("El segundo apellido solo puede contener letras, sin espacios al inicio/final.", new[] { nameof(SegundoApellido) });
            }

            if (!string.IsNullOrWhiteSpace(Contraseña) && !Contraseña.StartsWith("PBKDF2:"))
            {
                if (Contraseña.Length < 8 || Contraseña.Length > 15 ||
                    !Regex.IsMatch(Contraseña, @"[A-Z]") ||
                    !Regex.IsMatch(Contraseña, @"[a-z]") ||
                    !Regex.IsMatch(Contraseña, @"\d") ||
                    !Regex.IsMatch(Contraseña, @"[\W_]"))
                {
                    yield return new ValidationResult("La contraseña debe tener entre 8 y 15 caracteres, incluir al menos una mayúscula, una minúscula, un número y un carácter especial.", new[] { nameof(Contraseña) });
                }
            }

            if (Nombres != Nombres?.Trim())
                yield return new ValidationResult("Los nombres no deben empezar ni terminar con espacios.", new[] { nameof(Nombres) });
            if (!string.IsNullOrEmpty(PrimerApellido) && PrimerApellido != PrimerApellido.Trim())
                yield return new ValidationResult("El primer apellido no debe empezar ni terminar con espacios.", new[] { nameof(PrimerApellido) });
            if (!string.IsNullOrEmpty(SegundoApellido) && SegundoApellido != SegundoApellido.Trim())
                yield return new ValidationResult("El segundo apellido no debe empezar ni terminar con espacios.", new[] { nameof(SegundoApellido) });

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

            if (ContainsInjection(Nombres))
                yield return new ValidationResult("No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso en los nombres.", new[] { nameof(Nombres) });

            if (!string.IsNullOrWhiteSpace(PrimerApellido) && ContainsInjection(PrimerApellido))
                yield return new ValidationResult("No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso en el primer apellido.", new[] { nameof(PrimerApellido) });

            if (!string.IsNullOrWhiteSpace(SegundoApellido) && ContainsInjection(SegundoApellido))
                yield return new ValidationResult("No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso en el segundo apellido.", new[] { nameof(SegundoApellido) });

            var rolesValidos = new[] { "empleado", "jefe de proyecto", "superadmin", "jefedeproyecto" };
            if (string.IsNullOrWhiteSpace(Rol) || !rolesValidos.Contains(Rol.Trim().ToLowerInvariant()))
            {
                yield return new ValidationResult(
                    "Seleccione un rol válido: 'empleado' o 'jefe de proyecto'.",
                    new[] { nameof(Rol) });
            }
        }
    }
}