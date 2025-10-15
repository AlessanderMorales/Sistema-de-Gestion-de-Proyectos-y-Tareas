// Archivo: Pages/Usuarios/CreateModel.cshtml.cs (Versi�n Corregida)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public Usuario Usuario { get; set; } = new();

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public CreateModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Usuario == null) return Page();

            if (string.IsNullOrEmpty(Usuario.Contrase�a))
            {
                // allow empty contrase�a: service will generate default
            }
            else
            {
                if (Usuario.Contrase�a != ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Las contrase�as no coinciden.");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var generated = _usuarioService.CrearNuevoUsuario(Usuario);
                if (!string.IsNullOrEmpty(generated))
                {
                    // show generated password via TempData so we can display with modal in UI
                    TempData["GeneratedPassword"] = generated;
                    TempData["GeneratedForUser"] = Usuario.Username ?? string.Empty;
                }
                return RedirectToPage("./Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}