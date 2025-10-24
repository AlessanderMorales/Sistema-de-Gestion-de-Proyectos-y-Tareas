using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities
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

        [StringLength(15, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 15 caracteres.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email no válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, seleccione un rol (empleado o jefe de proyecto).")]
        public string Rol { get; set; } = string.Empty;

        public int Estado { get; set; } = 1;

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

            // Validar contraseña solo si se proporciona (para actualizaciones o creaciones manuales)
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

            if (PrimerNombre != PrimerNombre.Trim())
                yield return new ValidationResult("El primer nombre no debe empezar ni terminar con espacios.", new[] { nameof(PrimerNombre) });
            if (!string.IsNullOrEmpty(SegundoNombre) && SegundoNombre != SegundoNombre.Trim())
                yield return new ValidationResult("El segundo nombre no debe empezar ni terminar con espacios.", new[] { nameof(SegundoNombre) });
            if (!string.IsNullOrEmpty(Apellidos) && Apellidos != Apellidos.Trim())
                yield return new ValidationResult("El apellido no debe empezar ni terminar con espacios.", new[] { nameof(Apellidos) });

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

            if (ContainsInjection(PrimerNombre))
                yield return new ValidationResult("No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso en el primer nombre.", new[] { nameof(PrimerNombre) });

            if (!string.IsNullOrWhiteSpace(SegundoNombre) && ContainsInjection(SegundoNombre))
                yield return new ValidationResult("No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso en el segundo nombre.", new[] { nameof(SegundoNombre) });

            if (!string.IsNullOrWhiteSpace(Apellidos) && ContainsInjection(Apellidos))
                yield return new ValidationResult("No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso en los apellidos.", new[] { nameof(Apellidos) });

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