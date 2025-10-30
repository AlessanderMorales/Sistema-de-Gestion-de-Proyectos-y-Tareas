using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Application.Service.Reportes;
using ServiceProyecto.Application.Service.Reportes.Builders;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceUsuario.Application.Service;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        private readonly ReporteService _reporteService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

        public IndexModel(
            ProyectoService proyectoService,
            ReporteService reporteService,
            TareaService tareaService,
            UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
            _reporteService = reporteService;
            _tareaService = tareaService;
            _usuarioService = usuarioService;
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

            var usuarioNombre = User.FindFirst(ClaimTypes.Email)?.Value
                                ?? User.Identity?.Name
                                ?? User.FindFirst(ClaimTypes.Name)?.Value
                                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? "Sistema";

            var pdfBytes = _reporteService.GenerarReporteGeneralProyectosPdf(proyectos, usuarioNombre);
            return File(pdfBytes, "application/pdf", "Reporte_General_Proyectos.pdf");
        }

        // Asegúrate de que este método exista para descargar Excel
        public IActionResult OnPostGenerarExcelGeneral()
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos();

            var usuarioNombre = User.FindFirst(ClaimTypes.Email)?.Value
                                ?? User.Identity?.Name
                                ?? User.FindFirst(ClaimTypes.Name)?.Value
                                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? "Sistema";

            // Usa ExcelReportBuilder (ya incluido en tu proyecto)
            var builder = new ServiceProyecto.Application.Service.Reportes.Builders.ExcelReportBuilder(_tareaService, _usuarioService);
            builder.Start(usuarioNombre);
            foreach (var p in proyectos)
            {
                builder.AddProject(p);
            }
            builder.Finish();
            var excelBytes = builder.GetReport();

            return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Reporte_General_Proyectos.xlsx");
        }
    }
}