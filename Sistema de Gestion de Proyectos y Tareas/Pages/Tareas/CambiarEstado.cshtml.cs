using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class CambiarEstadoModel : PageModel
    {
        private readonly TareaService _tareaService;

        [BindProperty]
      public int TareaId { get; set; }

        [BindProperty]
        public string NuevoStatus { get; set; }

        public Tarea Tarea { get; set; }
        public SelectList StatusDisponibles { get; set; }

   public CambiarEstadoModel(TareaService tareaService)
        {
            _tareaService = tareaService;
     }

      public IActionResult OnGet(int id)
        {
            TareaId = id;
            Tarea = _tareaService.ObtenerTareaPorId(id);

  if (Tarea == null)
         {
    TempData["ErrorMessage"] = "Tarea no encontrada.";
           return RedirectToPage("Index");
            }

            // Verificar permisos
    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
       if (int.TryParse(idClaim, out var usuarioId))
      {
       // Si es empleado, verificar que la tarea le pertenece
          if (User.IsInRole("Empleado") && Tarea.IdUsuarioAsignado != usuarioId)
             {
          TempData["ErrorMessage"] = "No tienes permiso para cambiar el estado de esta tarea.";
            return RedirectToPage("Index");
        }
         }

     var estados = new List<SelectListItem>
  {
                new SelectListItem { Value = "SinIniciar", Text = "Sin Iniciar" },
     new SelectListItem { Value = "EnProgreso", Text = "En Progreso" },
        new SelectListItem { Value = "Completada", Text = "Completada" }
        };

      StatusDisponibles = new SelectList(estados, "Value", "Text", Tarea.Status);

  return Page();
}

        public IActionResult OnPost()
        {
     var tarea = _tareaService.ObtenerTareaPorId(TareaId);
            
   if (tarea == null)
  {
              TempData["ErrorMessage"] = "Tarea no encontrada.";
 return RedirectToPage("Index");
            }

          // Verificar permisos
     var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  if (int.TryParse(idClaim, out var usuarioId))
       {
    if (User.IsInRole("Empleado") && tarea.IdUsuarioAsignado != usuarioId)
    {
        TempData["ErrorMessage"] = "No tienes permiso para cambiar el estado de esta tarea.";
   return RedirectToPage("Index");
     }
            }

            try
            {
       tarea.Status = NuevoStatus;
    _tareaService.ActualizarTarea(tarea);
                TempData["SuccessMessage"] = "Estado de la tarea actualizado exitosamente.";
            }
      catch (Exception ex)
 {
   TempData["ErrorMessage"] = $"Error al actualizar el estado: {ex.Message}";
    }

      return RedirectToPage("Index");
        }
    }
}
