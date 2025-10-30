using System.Collections.Generic;
using ServiceProyecto.Domain.Entities;

namespace ServiceProyecto.Application.Service.Reportes
{
    public interface IPdfReporteBuilder
    {
        byte[] GenerarReporteProyectoPdf(int idProyecto, string usuarioNombre = "Sistema");
        byte[] GenerarReporteGeneralProyectosPdf(string usuarioNombre = "Sistema");
        byte[] GenerarReporteGeneralProyectosPdf(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema");
    }
}