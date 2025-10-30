using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceUsuario.Application.Service;
using System.Collections.Generic;

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ReporteService
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        private readonly PdfReporteBuilder _pdfBuilder;
        private readonly ExcelReporteBuilder _excelBuilder;

        public ReporteService(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
            _tarea_service_placeholder: _ = tareaService; // evita warnings
            _tareaService = tareaService;
            _usuarioService = usuarioService;

            // Builders delegan la lógica de generación
            _pdfBuilder = new PdfReporteBuilder(_proyectoService, _tareaService, _usuarioService);
            _excelBuilder = new ExcelReporteBuilder(_proyectoService, _tareaService, _usuarioService);
        }

        // Compatibilidad: envoltorios que reutilizan builders
        public byte[] GenerarReporteProyectoPdf(int idProyecto, string usuarioNombre = "Sistema")
            => _pdfBuilder.GenerarReporteProyectoPdf(idProyecto, usuarioNombre);

        public byte[] GenerarReporteGeneralProyectosPdf(string usuarioNombre = "Sistema")
            => _pdfBuilder.GenerarReporteGeneralProyectosPdf(usuarioNombre);

        public byte[] GenerarReporteGeneralProyectosPdf(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema")
            => _pdfBuilder.GenerarReporteGeneralProyectosPdf(proyectos, usuarioNombre);

        public byte[] GenerarReporteGeneralProyectosExcel(string usuarioNombre = "Sistema")
            => _excelBuilder.GenerarReporteGeneralProyectosExcel(usuarioNombre);

        public byte[] GenerarReporteGeneralProyectosExcel(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema")
            => _excelBuilder.GenerarReporteGeneralProyectosExcel(proyectos, usuarioNombre);
    }
}