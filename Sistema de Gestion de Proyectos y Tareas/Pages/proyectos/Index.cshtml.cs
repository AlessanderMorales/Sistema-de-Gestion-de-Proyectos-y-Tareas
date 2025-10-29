using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Application.Service.Reportes;
using ServiceProyecto.Domain.Entities;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        private readonly ReporteService _reporteService;

        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

        public IndexModel(ProyectoService proyectoService, ReporteService reporteService)
        {
            _proyectoService = proyectoService;
            _reporteService = reporteService;
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
            try
            {
                _proyectoService.EliminarProyectoPorId(id);
            }
            catch
            {
                // ignore if not implemented
            }

            TempData["SuccessMessage"] = "Proyecto eliminado correctamente.";
            return RedirectToPage("./Index");
        }

        public IActionResult OnPostGenerarPdfGeneral()
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos();
            var pdfBytes = _reporteService.GenerarReporteGeneralProyectosPdf(proyectos);
            return File(pdfBytes, "application/pdf", "Reporte_General_Proyectos.pdf");
        }
    }
}
