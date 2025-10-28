using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceUsuario.Application.Service;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Configuracion
{
    [Authorize]
    public class CambiarContraseñaModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public CambiarContraseñaInput Input { get; set; } = new();

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public CambiarContraseñaModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public class CambiarContraseñaInput
        {
            [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña Actual")]
            public string ContraseñaActual { get; set; } = string.Empty;

            [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
            [StringLength(15, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 15 caracteres.")]
            [DataType(DataType.Password)]
            [Display(Name = "Nueva Contraseña")]
            public string NuevaContraseña { get; set; } = string.Empty;

            [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
            [DataType(DataType.Password)]
            [Compare("NuevaContraseña", ErrorMessage = "Las contraseñas no coinciden.")]
            [Display(Name = "Confirmar Nueva Contraseña")]
            public string ConfirmarContraseña { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null)
            {
                MensajeError = "No se pudo identificar al usuario.";
                return Page();
            }

            int usuarioId = int.Parse(usuarioIdClaim.Value);

            if (!ValidarContraseña(Input.NuevaContraseña))
            {
                ModelState.AddModelError("Input.NuevaContraseña", 
                    "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.");
                return Page();
            }

            bool cambioExitoso = _usuarioService.CambiarContraseña(usuarioId, Input.ContraseñaActual, Input.NuevaContraseña);

            if (cambioExitoso)
            {
                MensajeExito = "Contraseña cambiada exitosamente.";
                return RedirectToPage("/Configuracion/CambiarContraseña");
            }
            else
            {
                ModelState.AddModelError("Input.ContraseñaActual", "La contraseña actual no es correcta.");
                return Page();
            }
        }

        private bool ValidarContraseña(string contraseña)
        {
            if (string.IsNullOrWhiteSpace(contraseña)) return false;
            if (contraseña.Length < 8 || contraseña.Length > 15) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contraseña, @"[A-Z]")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contraseña, @"[a-z]")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contraseña, @"\d")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contraseña, @"[\W_]")) return false;
            return true;
        }
    }
}
