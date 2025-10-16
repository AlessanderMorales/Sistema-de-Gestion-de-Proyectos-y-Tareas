using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using System;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Common;

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

            // No permitir editar a otro SuperAdmin
            if (!string.IsNullOrWhiteSpace(usuario.Rol) &&
                usuario.Rol.Equals(Roles.SuperAdmin, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "No puedes modificar a otro administrador.";
                return RedirectToPage("./Index");
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

            // Recuperar usuario actual a actualizar (por si el rol se envía mal)
            var existing = _usuarioService.ObtenerUsuarioPorId(Usuario.Id);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToPage("./Index");
            }

            if (!string.IsNullOrWhiteSpace(existing.Rol) &&
                existing.Rol.Equals(Roles.SuperAdmin, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "No puedes modificar a otro administrador.";
                return RedirectToPage("./Index");
            }

            // Normal update
            _usuarioService.ActualizarUsuario(Usuario);

            // Mensaje de éxito — será mostrado como modal por el layout
            TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            return RedirectToPage("./Index");
        }
    }
}