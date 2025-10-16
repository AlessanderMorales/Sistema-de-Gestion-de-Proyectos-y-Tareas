

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public Usuario Usuario { get; set; } = new();
        public CreateModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _usuarioService.CrearNuevoUsuario(Usuario);

            return RedirectToPage("./Index");
        }
    }
}