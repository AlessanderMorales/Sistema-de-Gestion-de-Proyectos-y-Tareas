using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    [Authorize(Policy = "SoloAdmin")]
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