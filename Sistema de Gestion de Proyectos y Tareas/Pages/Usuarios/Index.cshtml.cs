using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly UsuarioService _usuarioService;
        public IEnumerable<Usuario> Usuarios { get; private set; } = new List<Usuario>();

        public IndexModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult OnGet()
        {
            Usuarios = _usuarioService.ObtenerTodosLosUsuarios();
            return Page();
        }

        public IActionResult OnPostDelete(int? id)
        {
            if (id.HasValue)
            {
                _usuarioService.EliminarUsuario(id.Value);
            }
            return RedirectToPage("Index");
        }
    }
}