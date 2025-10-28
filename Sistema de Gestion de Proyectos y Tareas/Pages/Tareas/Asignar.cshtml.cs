using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceTarea.Application.Service;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize(Policy = "OnlyJefe")]
    public class AsignarModel : PageModel
    {
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

      [BindProperty]
        public int TareaId { get; set; }

        [BindProperty]
   public List<int> UsuariosIds { get; set; } = new List<int>();

 public MultiSelectList UsuariosDisponibles { get; set; }
        public IEnumerable<Usuario> UsuariosActualmenteAsignados { get; set; }
        public string NombreTarea { get; set; }

        public AsignarModel(TareaService tareaService, UsuarioService usuarioService)
  {
            _tareaService = tareaService;
            _usuarioService = usuarioService;
        }

  public void OnGet(int id)
     {
   TareaId = id;
      
 // Obtener nombre de la tarea
         var tarea = _tareaService.ObtenerTareaPorId(id);
   NombreTarea = tarea?.Titulo ?? "Tarea desconocida";
            
      // Obtener usuarios disponibles
            var usuarios = _usuarioService.ObtenerTodosLosUsuarios()
 .Where(u => u.Rol != "SuperAdmin")
    .Select(u => new {
    u.Id,
         NombreCompleto = $"{u.Nombres} {u.PrimerApellido}" + 
            (string.IsNullOrEmpty(u.SegundoApellido) ? "" : $" {u.SegundoApellido}")
        })
     .ToList();
  
         // Obtener IDs de usuarios actualmente asignados
            var idsAsignados = _tareaService.ObtenerIdsUsuariosAsignados(id).ToList();
   
            // Obtener objetos Usuario completos para mostrar
    UsuariosActualmenteAsignados = _usuarioService.ObtenerTodosLosUsuarios()
                .Where(u => idsAsignados.Contains(u.Id))
 .ToList();

            UsuariosDisponibles = new MultiSelectList(usuarios, "Id", "NombreCompleto", idsAsignados);
 }

        public IActionResult OnPost()
        {
            if (TareaId <= 0)
   {
          TempData["ErrorMessage"] = "ID de tarea inválido.";
        return RedirectToPage("Index");
  }

      if (UsuariosIds == null || !UsuariosIds.Any())
          {
      TempData["ErrorMessage"] = "Debe seleccionar al menos un usuario.";
    return RedirectToPage(new { id = TareaId });
         }

   try
            {
  _tareaService.AsignarMultiplesUsuarios(TareaId, UsuariosIds);
        TempData["SuccessMessage"] = $"Tarea asignada exitosamente a {UsuariosIds.Count} usuario(s).";
           return RedirectToPage("Index");
  }
      catch (Exception ex)
   {
        TempData["ErrorMessage"] = $"Error al asignar la tarea: {ex.Message}";
     return RedirectToPage(new { id = TareaId });
            }
        }
    }
}
