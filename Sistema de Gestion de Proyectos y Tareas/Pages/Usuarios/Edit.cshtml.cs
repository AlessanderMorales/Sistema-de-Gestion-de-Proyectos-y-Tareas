using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;
        public bool CanEditRole { get; private set; }

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

            // Only admin or the user themself can edit
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            var currentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = currentRole.ToLowerInvariant() == "admin";
            var isSelf = int.TryParse(currentIdClaim, out var cid) && cid == usuario.Id;
            if (!isAdmin && !isSelf)
            {
                return Forbid();
            }

            Usuario = usuario;

            CanEditRole = isAdmin;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var currentRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                CanEditRole = currentRole.ToLowerInvariant() == "admin";
                return Page();
            }

            // Only admin or the user themself can post changes
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = role.ToLowerInvariant() == "admin";
            var isSelf = int.TryParse(idClaim, out var cid) && cid == Usuario.Id;
            if (!isAdmin && !isSelf)
            {
                return Forbid();
            }

            // If current user is not admin, prevent role changes
            if (!isAdmin)
            {
                var existing = _usuarioService.ObtenerUsuarioPorId(Usuario.Id);
                if (existing != null)
                {
                    Usuario.Rol = existing.Rol;
                }
            }

            _usuarioService.ActualizarUsuario(Usuario);

            return RedirectToPage("./Index");
        }
    }
}