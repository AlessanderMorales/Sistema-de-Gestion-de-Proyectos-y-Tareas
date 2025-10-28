using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly TareaService _tareaService;
        public IEnumerable<Tarea> Tareas { get; private set; } = new List<Tarea>();

        public IndexModel(TareaService tareaService)
        {
            _tareaService = tareaService;
        }

        public void OnGet()
        {
            if (User.IsInRole("Empleado"))
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var usuarioId))
                {
                    Tareas = _tareaService.ObtenerTareasPorUsuarioAsignado(usuarioId);
                    return;
                }
            }

            Tareas = _tareaService.ObtenerTodasLasTareas();
        }

        public IActionResult OnPost(int id)
        {
            if (User.IsInRole("Empleado"))
            {
                TempData["ErrorMessage"] = "No estás autorizado para eliminar tareas.";
                return RedirectToPage("./Index");
            }

            var tarea = _tareaService.ObtenerTareaPorId(id);
            if (tarea != null)
            {
                _tareaService.EliminarTarea(tarea);
            }
            return RedirectToPage("./Index");
        }
    }
}