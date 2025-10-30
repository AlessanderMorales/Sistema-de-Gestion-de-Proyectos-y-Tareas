using System.Collections.Generic;
using ServiceProyecto.Domain.Entities;

namespace ServiceProyecto.Application.Service.Reportes
{
    public interface IExcelReporteBuilder
    {
        byte[] GenerarReporteGeneralProyectosExcel(string usuarioNombre = "Sistema");
        byte[] GenerarReporteGeneralProyectosExcel(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema");
    }
}