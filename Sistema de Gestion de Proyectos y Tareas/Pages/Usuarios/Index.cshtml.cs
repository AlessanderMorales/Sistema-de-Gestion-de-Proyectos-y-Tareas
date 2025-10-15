using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    [Authorize]
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
            var user = User;
            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "jefe de proyecto" || role.ToLowerInvariant() == "supervisor")
            {
                Usuarios = _usuarioService.ObtenerTodosLosUsuarios();
            }
            else
            {
                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var userId))
                {
                    var u = _usuarioService.ObtenerUsuarioPorId(userId);
                    if (u != null) Usuarios = new List<Usuario> { u };
                }
            }
            return Page();
        }

        public IActionResult OnPostDelete(int? id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role.ToLowerInvariant() != "admin")
            {
                return Forbid();
            }

            if (id.HasValue)
            {
                _usuarioService.EliminarUsuario(id.Value);
            }
            return RedirectToPage("Index");
        }
    }
}