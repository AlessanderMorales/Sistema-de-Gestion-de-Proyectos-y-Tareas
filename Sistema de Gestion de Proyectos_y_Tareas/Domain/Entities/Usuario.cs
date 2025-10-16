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

        [Required(ErrorMessage = "La contrase�a es obligatoria.")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "La contrase�a debe tener entre 8 y 15 caracteres.")]
        [DataType(DataType.Password)]
        public string Contrase�a { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email no v�lido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, seleccione un rol (empleado o jefe de proyecto).")]
        public string Rol { get; set; } = string.Empty;

        public int Estado { get; set; } = 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string nombrePattern = @"^(?! )[A-Za-z������������]+(?: [A-Za-z������������]+)*$";
            string sinEspaciosMultiples = @" {2,}";

            if (string.IsNullOrWhiteSpace(PrimerNombre) || !Regex.IsMatch(PrimerNombre.Trim(), nombrePattern) || Regex.IsMatch(PrimerNombre, sinEspaciosMultiples))
                yield return new ValidationResult("El primer nombre solo puede contener letras y un espacio entre palabras, sin espacios al inicio/final ni m�ltiples espacios.", new[] { nameof(PrimerNombre) });

            if (!string.IsNullOrWhiteSpace(SegundoNombre))
            {
                if (!Regex.IsMatch(SegundoNombre.Trim(), nombrePattern) || Regex.IsMatch(SegundoNombre, sinEspaciosMultiples))
                    yield return new ValidationResult("El segundo nombre solo puede contener letras y un espacio entre palabras, sin espacios al inicio/final ni m�ltiples espacios.", new[] { nameof(SegundoNombre) });
            }

            if (string.IsNullOrWhiteSpace(Apellidos) || !Regex.IsMatch(Apellidos.Trim(), nombrePattern) || Regex.IsMatch(Apellidos, sinEspaciosMultiples))
                yield return new ValidationResult("El apellido solo puede contener letras y un espacio entre palabras, sin espacios al inicio/final ni m�ltiples espacios.", new[] { nameof(Apellidos) });

            if (string.IsNullOrWhiteSpace(Contrase�a) ||
                Contrase�a.Length < 8 || Contrase�a.Length > 15 ||
                !Regex.IsMatch(Contrase�a, @"[A-Z]") ||
                !Regex.IsMatch(Contrase�a, @"[a-z]") ||
                !Regex.IsMatch(Contrase�a, @"\d") ||
                !Regex.IsMatch(Contrase�a, @"[\W_]"))
            {
                yield return new ValidationResult("La contrase�a debe tener entre 8 y 15 caracteres, incluir al menos una may�scula, una min�scula, un n�mero y un car�cter especial.", new[] { nameof(Contrase�a) });
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

                // Patr�nes expl�citos de SQLi comunes y tautolog�as
                string[] explicitSqlPatterns = new[]
                {
                    @"(--|;--)",                        // comentarios SQL
                    @"\bunion\s+select\b",              // UNION SELECT
                    @"\bdrop\s+table\b",                // DROP TABLE
                    @"\binsert\s+into\b",               // INSERT INTO
                    @"\btruncate\s+table\b",            // TRUNCATE TABLE
                    @"\bdelete\s+from\b",               // DELETE FROM
                    @"\bupdate\s+\w+\s+set\b",          // UPDATE ... SET
                    @"\bexec\s*\(",                     // EXEC(
                    @"\bxp_cmdshell\b",                 // procedimientos peligrosos
                    @"\bbenchmark\s*\(",                // MySQL benchmark attempts
                    @"\bwaitfor\s+delay\b",             // tiempo de espera inyecci�n
                    @"(['""]\s*or\s+['""]?1['""]?\s*=\s*['""]?1['""]?)", // ' OR '1'='1'
                    @"\bor\s+1\s*=\s*1\b"               // or 1=1
                };

                foreach (var p in explicitSqlPatterns)
                {
                    if (Regex.IsMatch(lowerInput, p, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        return true;
                }

                // Patr�nes XSS / HTML peligrosos expl�citos
                string[] xssPatterns = new[]
                {
                    @"<\s*script\b",                    // <script>
                    @"<\s*iframe\b",                    // <iframe>
                    @"javascript\s*:",                  // javascript:
                    @"on\w+\s*=",                       // atributos on*
                    @"<\s*img\b.*\bsrc\s*=",            // img con src
                    @"<\s*a\b.*\bhref\s*="              // a con href
                };

                foreach (var p in xssPatterns)
                {
                    if (Regex.IsMatch(lowerInput, p, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        return true;
                }

                return false;
            }

            if (ContainsInjection(PrimerNombre) ||
                !string.IsNullOrEmpty(SegundoNombre) && ContainsInjection(SegundoNombre) ||
                !string.IsNullOrEmpty(Apellidos) && ContainsInjection(Apellidos) ||
                ContainsInjection(Email))
            {
                yield return new ValidationResult("No se permiten intentos expl�citos de inyecci�n SQL o contenido HTML/JS peligroso.", new[] { nameof(PrimerNombre), nameof(SegundoNombre), nameof(Apellidos), nameof(Email) });
            }

            var rolesValidos = new[] { "empleado", "jefe de proyecto" };
            if (string.IsNullOrWhiteSpace(Rol) || !rolesValidos.Contains(Rol.Trim().ToLowerInvariant()))
            {
                yield return new ValidationResult(
                    "Seleccione un rol v�lido: 'empleado' o 'jefe de proyecto'.",
                    new[] { nameof(Rol) });
            }
        }
    }
}