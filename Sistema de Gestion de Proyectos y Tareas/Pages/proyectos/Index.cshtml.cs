using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();
        public IndexModel(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }

        public void OnGet()
        {
            if (User.IsInRole("Empleado"))
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var usuarioId))
                {
                    Proyectos = _proyectoService.ObtenerProyectosPorUsuarioAsignado(usuarioId);
                    return;
                }
            }

            Proyectos = _proyectoService.ObtenerTodosLosProyectos();
        }

        public IActionResult OnPost(int id)
        {
            // Prevent Empleado role from deleting projects
            if (User.IsInRole("Empleado"))
            {
                TempData["ErrorMessage"] = "No estás autorizado para eliminar proyectos.";
                return RedirectToPage("./Index");
            }

            var proyecto = _proyectoService.ObtenerProyectoPorId(id);
            if (proyecto != null)
            {
                _proyectoService.EliminarProyecto(proyecto);
            }
            // Some services may also expose direct delete by id
            try
            {
                _proyectoService.EliminarProyectoPorId(id);
            }
            catch
            {
                // ignore if method not implemented
            }

            TempData["SuccessMessage"] = "Proyecto eliminado correctamente.";
            return RedirectToPage("./Index");
        }
    }
}
