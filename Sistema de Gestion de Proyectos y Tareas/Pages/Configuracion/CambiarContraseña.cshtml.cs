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
    public class CambiarContrase�aModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public CambiarContrase�aInput Input { get; set; } = new();

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public CambiarContrase�aModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public class CambiarContrase�aInput
        {
            [Required(ErrorMessage = "La contrase�a actual es obligatoria.")]
            [DataType(DataType.Password)]
            [Display(Name = "Contrase�a Actual")]
            public string Contrase�aActual { get; set; } = string.Empty;

            [Required(ErrorMessage = "La nueva contrase�a es obligatoria.")]
            [StringLength(15, MinimumLength = 8, ErrorMessage = "La contrase�a debe tener entre 8 y 15 caracteres.")]
            [DataType(DataType.Password)]
            [Display(Name = "Nueva Contrase�a")]
            public string NuevaContrase�a { get; set; } = string.Empty;

            [Required(ErrorMessage = "Debe confirmar la nueva contrase�a.")]
            [DataType(DataType.Password)]
            [Compare("NuevaContrase�a", ErrorMessage = "Las contrase�as no coinciden.")]
            [Display(Name = "Confirmar Nueva Contrase�a")]
            public string ConfirmarContrase�a { get; set; } = string.Empty;
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

            if (!ValidarContrase�a(Input.NuevaContrase�a))
            {
                ModelState.AddModelError("Input.NuevaContrase�a", 
                    "La contrase�a debe contener al menos una may�scula, una min�scula, un n�mero y un car�cter especial.");
                return Page();
            }

            bool cambioExitoso = _usuarioService.CambiarContrase�a(usuarioId, Input.Contrase�aActual, Input.NuevaContrase�a);

            if (cambioExitoso)
            {
                MensajeExito = "Contrase�a cambiada exitosamente.";
                return RedirectToPage("/Configuracion/CambiarContrase�a");
            }
            else
            {
                ModelState.AddModelError("Input.Contrase�aActual", "La contrase�a actual no es correcta.");
                return Page();
            }
        }

        private bool ValidarContrase�a(string contrase�a)
        {
            if (string.IsNullOrWhiteSpace(contrase�a)) return false;
            if (contrase�a.Length < 8 || contrase�a.Length > 15) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contrase�a, @"[A-Z]")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contrase�a, @"[a-z]")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contrase�a, @"\d")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(contrase�a, @"[\W_]")) return false;
            return true;
        }
    }
}
