using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using ServiceCommon.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
          var validationContext = new ValidationContext(Usuario, serviceProvider: null, items: null);
      var validationResults = Usuario.Validate(validationContext).ToList();
     
            foreach (var validationResult in validationResults)
    {
       foreach (var memberName in validationResult.MemberNames)
      {
         ModelState.AddModelError($"Usuario.{memberName}", validationResult.ErrorMessage ?? "Error de validación");
        }
            }

            if (!ModelState.IsValid)
            {
    return Page();
     }

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

      _usuarioService.ActualizarUsuario(Usuario);

        TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
      return RedirectToPage("./Index");
        }
    }
}