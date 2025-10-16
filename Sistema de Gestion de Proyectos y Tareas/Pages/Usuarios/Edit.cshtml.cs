using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    [Authorize(Policy = "SoloAdmin")]
    public class EditModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;
        public EditModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var usuario = _usuarioService.ObtenerUsuarioPorId(id.Value);

            if (usuario == null)
            {
                return NotFound();
            }
            Usuario = usuario;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _usuarioService.ActualizarUsuario(Usuario);

            return RedirectToPage("./Index");
        }
    }
}