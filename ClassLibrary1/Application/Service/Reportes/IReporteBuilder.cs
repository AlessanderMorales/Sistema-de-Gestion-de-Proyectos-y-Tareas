using System.Collections.Generic;
using ServiceProyecto.Domain.Entities;

namespace ServiceProyecto.Application.Service.Reportes
{
    public interface IReporteBuilder
    {
        byte[] GenerarReporteGeneralProyectosPdf(IEnumerable<Proyecto> proyectos, string usuarioNombre);
        byte[] GenerarReporteGeneralProyectosPdf(string usuarioNombre);
        byte[] GenerarReporteProyectoPdf(int idProyecto, string usuarioNombre);
        byte[] GenerarReporteGeneralProyectosExcel(IEnumerable<Proyecto> proyectos, string usuarioNombre);
        byte[] GenerarReporteGeneralProyectosExcel(string usuarioNombre);
    }
}