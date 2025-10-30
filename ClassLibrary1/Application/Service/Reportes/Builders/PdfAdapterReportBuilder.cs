using ServiceProyecto.Domain.Entities;
using System.Collections.Generic;
using ServiceProyecto.Application.Service.Reportes;
using ServiceProyecto.Application.Service.Reportes.Builders;

namespace ServiceProyecto.Application.Service.Reportes.Builders
{
    // Adapter: reutiliza tu ReporteService actual para cumplir IReportBuilder
    public class PdfAdapterReportBuilder : IReportBuilder
    {
        private readonly ReporteService _reporteService;
        private readonly List<Proyecto> _proyectos = new();
        private string _usuarioNombre = "Sistema";
        private byte[] _result;

        public PdfAdapterReportBuilder(ReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        public void Start(string usuarioNombre)
        {
            _usuarioNombre = usuarioNombre ?? "Sistema";
            _proyectos.Clear();
            _result = null;
        }

        public void AddProject(Proyecto proyecto)
        {
            if (proyecto != null) _proyectos.Add(proyecto);
        }

        public void Finish()
        {
            // Llama al servicio existente que genera PDF para lista de proyectos
            _result = _reporteService.GenerarReporteGeneralProyectosPdf(_proyectos, _usuarioNombre);
        }

        public byte[] GetReport() => _result;
    }
}