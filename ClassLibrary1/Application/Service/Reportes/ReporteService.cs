using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceUsuario.Application.Service;
using System.Collections.Generic;

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ReporteService
    {
        private readonly IPdfReporteBuilder _pdfBuilder;
        private readonly IExcelReporteBuilder _excelBuilder;

        public ReporteService(IPdfReporteBuilder pdfBuilder, IExcelReporteBuilder excelBuilder)
        {
            _pdfBuilder = pdfBuilder;
            _excelBuilder = excelBuilder;
        }

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